using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ry.rec
{
    public class NvrManager
    {
        // 日志
        private static Logger logger = LogManager.GetCurrentClassLogger();

        // 实时播放的Grid
        List<RealPlayerGrid> realPlayerGrids = new List<RealPlayerGrid>();

        // 实时播放的From
        List<FormRealPlayer> realPlayerFroms = new List<FormRealPlayer>();

        // 管理的播放界面
        RealPlayerGrid currentRealPlayerGrid;

        // NVR接口管理
        NvrAdapterMgr nvrAdapterMgr = new NvrAdapterMgr();

        int MAX_POP;

        /// <summary>
        /// 构造函数
        /// </summary>
        public NvrManager()
        {
            // 配置，最多弹出的视频播放窗口
            MAX_POP = int.Parse(ConfigurationManager.AppSettings["MAX_POP"]);
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~NvrManager()
        {
            nvrAdapterMgr.clearAdpters();
        }

        /// <summary>
        /// 添加一个Grid
        /// </summary>
        /// <param name="grid"></param>
        public void addRealPlayerGrid(RealPlayerGrid grid)
        {
            this.realPlayerGrids.Add(grid);
        }


        /// <summary>
        /// 设置当前的播放Grid
        /// </summary>
        /// <param name="rpg"></param>
        public void setCurrentRealPlayerGrid(RealPlayerGrid rpg)
        {
            this.currentRealPlayerGrid = rpg;
        }

        /// <summary>
        ///  
        /// </summary>
        public void removeRealPlayer(FormRealPlayer player)
        {
            realPlayerFroms.Remove(player);
        }



        /// <summary>
        /// 初始化NVR配置
        /// </summary>
        /// <param name="cfg"></param>
        public void initConfig(String cfg)
        {
            nvrAdapterMgr.loadCfg(cfg);
        }

        /// <summary>
        /// 实时播放，遍历相应的realplayergrid
        /// </summary>
        /// <param name="nvr"></param>
        /// <param name="channel"></param>
        public void realPlayInGrid(int nvr, int channel)
        {
            
            // 得到可用的播放窗口
            FormRealPlayer player = currentRealPlayerGrid.getPlayer();

            // 首先停止当前的播放
            if (player.isPlaying)
            {
                nvrAdapterMgr.realPlayStop(player.nvr, player.realSession);
            }

            player.isPlaying = true;
            player.nvr = nvr;
            player.channel = channel;

            player.realSession = nvrAdapterMgr.realPlay(nvr, channel, player.playerHandle);

            if (player.realSession >=0)

            {
                //播放成功
                player.isPlaying = true;
                player.setVideoId(nvr, channel);
            }
            else
            {
                player.isPlaying = false;
            }
            
        }

        /// <summary>
        /// 新窗体播放视频
        /// </summary>
        /// <param name="nvr"></param>
        /// <param name="channel"></param>

        public void realPlayInForm(int nvr, int channel)
        {
            if (realPlayerFroms.Count >= MAX_POP)
            {
                return;
            }
            
            FormMain.mainForm.BeginInvoke((Action)delegate
            {
                FormRealPlayer player = new FormRealPlayer(this);

                player.TopMost = true;
                realPlayerFroms.Add(player);

                player.isPlaying = true;
                player.realSession = nvrAdapterMgr.realPlay(nvr, channel, player.playerHandle);

                if (player.realSession >= 0)
                {
                    //播放成功
                    player.isPlaying = true;
                    player.setVideoId(nvr, channel);
                }
                else
                {
                    player.isPlaying = false;
                }

                player.Show();

            });
        }

        /// <summary>
        /// 停止实时播放
        /// </summary>
        /// <param name="playSession"></param>
        public void realPlayStop(int nvr, int playSession)
        {
            nvrAdapterMgr.realPlayStop(nvr, playSession);            
        }


        /// <summary>
        /// PTZ 控制
        /// </summary>
        /// <param name="nvr"></param>
        /// <param name="channel"></param>
        /// <param name="dir"></param>
        /// <param name="speed"></param>
        public void ptzCtlStart(int nvr, int channel, PTZ_DIR dir, int speed)
        {
            nvrAdapterMgr.ptzCtlStart(nvr, channel, dir, speed);
        }

        /// <summary>
        /// 停止PTZ
        /// </summary>
        /// <param name="nvr"></param>
        /// <param name="channel"></param>
        public void ptzStop(int nvr, int channel)
        {
            nvrAdapterMgr.ptzStop(nvr, channel);   
        }

        /// <summary>
        /// zoom 控制
        /// </summary>
        /// <param name="nvr"></param>
        /// <param name="channel"></param>
        /// <param name="dir"></param>
        public void zoomStart(int nvr, int channel, ZOOM_DIR dir)
        {
            nvrAdapterMgr.zoomStart(nvr, channel, dir);
        }
    }
}
