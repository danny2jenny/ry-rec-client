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
    public class PlayerManager
    {
        // 日志
        private static Logger logger = LogManager.GetCurrentClassLogger();

        // 实时播放的Grid
        List<RealPlayerGrid> realPlayerGrids = new List<RealPlayerGrid>();

        // 实时播放的Form
        List<FormRealPlayer> realPlayerForms = new List<FormRealPlayer>();

        // 历史播放的Form
        List<FormPlayBack> playBackForms = new List<FormPlayBack>();


        // 管理的播放界面
        RealPlayerGrid currentRealPlayerGrid;

        // NVR接口管理
        public NvrAdapterMgr nvrAdapterMgr = new NvrAdapterMgr();

        int MAX_POP;

        /// <summary>
        /// 构造函数
        /// </summary>
        public PlayerManager()
        {
            // 配置，最多弹出的视频播放窗口
            MAX_POP = int.Parse(ConfigurationManager.AppSettings["MAX_POP"]);
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~PlayerManager()
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
        /// <param name="player"></param>
        public void removeRealPlayer(FormRealPlayer player)
        {
            realPlayerForms.Remove(player);
        }

        /// <summary>
        /// 初始化NVR配置
        /// </summary>
        /// <param name="cfg"></param>
        public void initConfig(String cfg)
        {
            closeAllVideo();
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
                player.stop();
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
            if (realPlayerForms.Count >= MAX_POP)
            {
                return;
            }
            
            FormMain.mainForm.BeginInvoke((Action)delegate
            {
                FormRealPlayer player = new FormRealPlayer(this);

                player.TopMost = true;
                realPlayerForms.Add(player);

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
        /// 历史回放窗口
        /// </summary>
        /// <param name="nvr"></param>
        /// <param name="channel"></param>
        public void playBack(int nvr, int channel, String name)
        {
            FormMain.mainForm.BeginInvoke((Action)delegate
            {
                FormPlayBack formPlayBack = new FormPlayBack(this);
                formPlayBack.TopMost = true;
                formPlayBack.nvr = nvr;
                formPlayBack.channel = channel;
                formPlayBack.Text = "历史回放：" + name;
                formPlayBack.Show();
                playBackForms.Add(formPlayBack);
            });
        }
        
        /// <summary>
        /// 从管理列表中删除历史播放窗口
        /// </summary>
        /// <param name="playBack"></param>
        public void removePlayBack(FormPlayBack playBack)
        {
            this.playBackForms.Remove(playBack);
        }

        /// <summary>
        /// 关闭所有的播放
        /// 包括历史和实时
        /// todo: 没有考虑多个RealGrid
        /// </summary>
        private void closeAllVideo()
        {
            FormMain.mainForm.BeginInvoke((Action)delegate
            {
                // 关闭所有的Grid播放
                currentRealPlayerGrid.stopAll();
                

                // 关闭实时播放窗口
                foreach (FormRealPlayer player in realPlayerForms)
                {
                    player.outClose = true;
                    player.Close();
                }
                realPlayerForms.Clear();
                // todo: 这个地方关闭的时候，不知道什么原因，NvrAdpMgr可能找不到对应的接口，
                // 通过NvrInterface.nvrFree中加入清除历史播放的Session可以解决部分问题。
                foreach (FormPlayBack playBack in playBackForms)
                {
                    playBack.outClose = true;
                    playBack.Close();
                }
                playBackForms.Clear();

                currentRealPlayerGrid.Refresh();
            });
            
        }        
    }
}
