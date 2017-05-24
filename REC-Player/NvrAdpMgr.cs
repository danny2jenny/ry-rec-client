using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ry.rec
{
    public enum NVR_ERROR
    {
        NO_NVR_CFG = -1,
        NO_NVR_ADP = -2,
        NO_LOGIN = -3,
    }

    // 回放命令
    public enum PLAY_BACK_CMD
    {
        START = 1,      // 开始
        PAUSE = 3,      // 暂停
        RESUME = 4,     // 恢复
        POSITION = 5    // 定位
    }
    public enum PTZ_DIR
    {
        UP = 1,
        RIGHT = 2,
        DOWN = 3,
        LEFT = 4,
        UP_RIGHT = 11,
        DOWN_RIGHT = 12,
        DONN_LEFT = 13,
        UP_LEFT = 14,
    }

    public enum ZOOM_DIR
    {
        ZOOM_IN = 1,
        ZOOM_OUT = 2,
    }

    public enum PTZ_ZOOM
    {
        ZOOM_IN = 1,
        ZOOM_OUT = 3,
    }

    /// <summary>
    /// 时间对的结构体
    /// </summary>
    public struct DATA_PAIR
    {
        public DateTime start;
        public DateTime end;
    }

    /// <summary>
    /// NVR 配置信息 
    /// 
    /// </summary>
    public class NVR_INFO
    {
        public int id;
        public String name;
        public String ip;
        public int port;
        public String login;
        public String pass;
        public int type;
        volatile public int session=0;

    }

    /// <summary>
    /// 历史视频回调的接口
    /// </summary>
    public interface PlayBackEvent
    {
        void onPlayBackTime(int parm);
    }

    /// <summary>
    /// 回访控制数据
    /// </summary>
    public class PlayBackCtlBlock
    {
        public int session;
        public DateTime start;
        public DateTime end;
        public PlayBackEvent even;
    }

    /// <summary>
    /// NVR Adp 的接口
    /// </summary>
    public interface NvrInterface
    {
        int nvrLogin(String ip, int port, String username, String password);
        int nvrRealPlay(int session, int cha, IntPtr handle);
        void nvrRealPlayStop(int session);
        void nvrPtzStart(int session, int cha, PTZ_DIR dir, int speed);
        void nvrPtzStop(int session, int cha);
        void nvrZoom(int session, int cha, ZOOM_DIR dir);
        List<DATA_PAIR> searchHistory(int session, int cha, DateTime start, DateTime end);
        int playBackByTime(int session, int cha, DateTime start, DateTime end, IntPtr handle, PlayBackEvent iPlayBack);
        int playBackCtl(int session, PLAY_BACK_CMD cmd, Object parm);
        void playBackClose(int session);
        void nvrFree();
    }

    /// <summary>
    /// NVR 管理类
    /// </summary>
    public class NvrAdapterMgr
    {
        // NVR 配置列表  nvrId -> NVR_INFO
        Hashtable nvrConfig = new Hashtable();

        // NVR Session 列表 nvrID -> NvrInterface
        Hashtable nvrAdapters = new Hashtable();

        // 当前程序的路径
        String bashPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

        // 定时器
        System.Timers.Timer timer = new System.Timers.Timer(5000);

        /// <summary>
        /// 构造函数
        /// </summary>
        public NvrAdapterMgr()
        {
            // 定时器初始化
            timer.Elapsed += new System.Timers.ElapsedEventHandler(onTimer);
            timer.AutoReset = true;
            timer.Enabled = false;
        }

        /// <summary>
        /// 定时执行的函数
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        public void onTimer(object source, System.Timers.ElapsedEventArgs e)
        {
            timer.Stop();
            nvrsLogin();
            timer.Start();
        }

        /// <summary>
        /// 清空nvrConfig
        /// 清理所有加载的Adapter
        /// </summary>
        public void clearAdpters()
        {
            nvrConfig.Clear();
            foreach (NvrInterface adp in nvrAdapters.Values)
            {
                adp.nvrFree();
            }
        }


        /// <summary>
        /// 动态加载各个NVR的接口
        /// </summary>
        public void loadLibs()
        {
            NvrInterface nvrAdp;

            foreach (NVR_INFO nvrInfo in nvrConfig.Values)
            {
                nvrAdp = (NvrInterface)nvrAdapters[nvrInfo.type];
                if (nvrAdp != null)
                {
                    // 接口存在，退出
                    continue;
                }

                // 不存在，加载接口
                Assembly ass = Assembly.LoadFile(bashPath+"nvr" +nvrInfo.type+".dll");
                if (ass != null)
                {
                    Type tp = ass.GetType("ry.rec.NVR_" + nvrInfo.type);
                    nvrAdp = (NvrInterface)Activator.CreateInstance(tp);
                    nvrAdapters.Add(nvrInfo.type, nvrAdp);
                }
            }

        }

        /// <summary>
        /// 登陆所有的NVR
        /// </summary>
        public void nvrsLogin()
        {
            NvrInterface nvrAdp;
            foreach (NVR_INFO nvrInfo in nvrConfig.Values)
            {
                if (nvrInfo.session > 0)
                {
                    continue;
                }
                nvrAdp = (NvrInterface)nvrAdapters[nvrInfo.type];

                if (nvrAdp != null)
                {
                    nvrInfo.session = nvrAdp.nvrLogin(nvrInfo.ip, nvrInfo.port, nvrInfo.login, nvrInfo.pass);
                }
            }
        }

        /// <summary>
        /// 初始化配置
        /// </summary>
        /// <param name="cfgString"></param>
        public void loadCfg(String cfgString)
        {
            clearAdpters();

            List<NVR_INFO> nvrList = JsonConvert.DeserializeObject<List<NVR_INFO>>(cfgString);

            foreach (NVR_INFO nvrInfo in nvrList)
            {
                nvrConfig.Add(nvrInfo.id, nvrInfo);
            }

            loadLibs();
            nvrsLogin();
            timer.Start();
        }

        /// <summary>
        /// 实时视频播放
        /// </summary>
        /// <param name="nvr"></param>
        /// <param name="cha"></param>
        /// <param name="handle"></param>
        /// <returns></returns>
        public int realPlay(int nvr, int cha, IntPtr handle)
        {
            NVR_INFO nvrInfo = (NVR_INFO)nvrConfig[nvr];
            if (nvrInfo == null)
            {
                return Convert.ToInt32(NVR_ERROR.NO_NVR_CFG);
            }

            if (nvrInfo.session < 1)
            {
                return Convert.ToInt32(NVR_ERROR.NO_LOGIN);
            }

            NvrInterface nvrAdp = (NvrInterface)nvrAdapters[nvrInfo.type];

            if (nvrAdp == null)
            {
                return Convert.ToInt32(NVR_ERROR.NO_NVR_ADP);
            }

            return nvrAdp.nvrRealPlay(nvrInfo.session, cha, handle);
        }

        /// <summary>
        /// 停止播放
        /// </summary>
        /// <param name="nvr"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        public int realPlayStop(int nvr, int session)
        {

            NVR_INFO nvrInfo = (NVR_INFO)nvrConfig[nvr];
            if (nvrInfo == null)
            {
                return Convert.ToInt32(NVR_ERROR.NO_NVR_CFG);
            }

            if (nvrInfo.session < 1)
            {
                return Convert.ToInt32(NVR_ERROR.NO_LOGIN);
            }

            NvrInterface nvrAdp = (NvrInterface)nvrAdapters[nvrInfo.type];

            if (nvrAdp == null)
            {
                return Convert.ToInt32(NVR_ERROR.NO_NVR_ADP);
            }

            nvrAdp.nvrRealPlayStop(session);

            return 0;

        }

        /// <summary>
        /// PTZ控制开始
        /// </summary>
        /// <param name="nvr"></param>
        /// <param name="channel"></param>
        /// <param name="dir"></param>
        /// <param name="speed"></param>
        public void ptzCtlStart(int nvr, int channel, PTZ_DIR dir, int speed)
        {
            NVR_INFO nvrInfo = (NVR_INFO)nvrConfig[nvr];
            if (nvrInfo == null)
            {
                return;
            }

            if (nvrInfo.session < 1)
            {
                return;
            }

            NvrInterface nvrAdp = (NvrInterface)nvrAdapters[nvrInfo.type];

            if (nvrAdp == null)
            {
                return;
            }

            nvrAdp.nvrPtzStart(nvrInfo.session,channel, dir, speed);

        }

        /// <summary>
        /// 停止PTZ
        /// </summary>
        /// <param name="nvr"></param>
        /// <param name="channel"></param>
        public void ptzStop(int nvr, int channel)
        {
            NVR_INFO nvrInfo = (NVR_INFO)nvrConfig[nvr];
            if (nvrInfo == null)
            {
                return;
            }

            if (nvrInfo.session < 1)
            {
                return;
            }

            NvrInterface nvrAdp = (NvrInterface)nvrAdapters[nvrInfo.type];

            if (nvrAdp == null)
            {
                return;
            }

            nvrAdp.nvrPtzStop(nvrInfo.session, channel);

        }

        /// <summary>
        /// 变焦控制
        /// </summary>
        /// <param name="nvr"></param>
        /// <param name="channel"></param>
        /// <param name="dir"></param>
        public void zoomStart(int nvr, int channel, ZOOM_DIR dir)
        {
            NVR_INFO nvrInfo = (NVR_INFO)nvrConfig[nvr];
            if (nvrInfo == null)
            {
                return;
            }

            if (nvrInfo.session < 1)
            {
                return;
            }

            NvrInterface nvrAdp = (NvrInterface)nvrAdapters[nvrInfo.type];

            if (nvrAdp == null)
            {
                return;
            }

            nvrAdp.nvrZoom(nvrInfo.session, channel, dir);

        }

        /// <summary>
        /// 查询一天内的历史视频
        /// </summary>
        /// <param name="nvr"></param>
        /// <param name="channel"></param>
        /// <param name="date"></param>
        public List<DATA_PAIR> searchHistory(int nvr, int channel, DateTime start, DateTime end)
        {

            NVR_INFO nvrInfo = (NVR_INFO)nvrConfig[nvr];
            if (nvrInfo == null)
            {
                return null;
            }

            if (nvrInfo.session < 1)
            {
                return null;
            }

            NvrInterface nvrAdp = (NvrInterface)nvrAdapters[nvrInfo.type];

            if (nvrAdp == null)
            {
                return null;
            }

            return nvrAdp.searchHistory(nvrInfo.session, channel, start, end);

        }

        /// <summary>
        /// 播放历史视频
        /// </summary>
        /// <param name="nvr"></param>
        /// <param name="channel"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="handle"></param>
        /// <param name="iPlayBack"></param>
        /// <returns></returns>
        public int playBackByTime(int nvr, int channel, DateTime start, DateTime end, IntPtr handle, PlayBackEvent iPlayBack)
        {
            NVR_INFO nvrInfo = (NVR_INFO)nvrConfig[nvr];
            if (nvrInfo == null)
            {
                return Convert.ToInt32(NVR_ERROR.NO_NVR_CFG);
            }

            if (nvrInfo.session < 1)
            {
                return Convert.ToInt32(NVR_ERROR.NO_LOGIN);
            }

            NvrInterface nvrAdp = (NvrInterface)nvrAdapters[nvrInfo.type];

            if (nvrAdp == null)
            {
                return Convert.ToInt32(NVR_ERROR.NO_NVR_ADP);
            }

            return nvrAdp.playBackByTime(nvrInfo.session, channel, start, end, handle, iPlayBack);
        }

        public int playBackCtl(int nvr, int session, PLAY_BACK_CMD cmd, Object parm)
        {
            NVR_INFO nvrInfo = (NVR_INFO)nvrConfig[nvr];
            if (nvrInfo == null)
            {
                return Convert.ToInt32(NVR_ERROR.NO_NVR_CFG);
            }

            if (nvrInfo.session < 1)
            {
                return Convert.ToInt32(NVR_ERROR.NO_LOGIN);
            }

            NvrInterface nvrAdp = (NvrInterface)nvrAdapters[nvrInfo.type];

            if (nvrAdp == null)
            {
                return Convert.ToInt32(NVR_ERROR.NO_NVR_ADP);
            }

            return nvrAdp.playBackCtl(session, cmd, parm);

        }

        /// <summary>
        /// 关闭实时播放
        /// </summary>
        /// <param name="nvr"></param>
        /// <param name="session"></param>
        public void playBackClose(int nvr, int session)
        {

            NVR_INFO nvrInfo = (NVR_INFO)nvrConfig[nvr];
            if (nvrInfo == null)
            {
                return;
            }

            if (nvrInfo.session < 1)
            {
                return;
            }

            NvrInterface nvrAdp = (NvrInterface)nvrAdapters[nvrInfo.type];

            if (nvrAdp == null)
            {
                return;
            }

            nvrAdp.playBackClose(session);

        }

    }
}
