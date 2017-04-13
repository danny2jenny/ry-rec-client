﻿using hk;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ry.rec
{
    // 云台方向定义
    public enum PTZ_DIRECTION
    {
        UP = 1,
        RIGHT =2,
        DOWN = 3,
        LEFT = 4,
        UP_RIGHT = 11,
        DOWN_RIGHT = 12,
        DONN_LEFT = 13,
        UP_LEFT = 14,
    }
    // NVR 的数据结构
    public class RyNvr
    {
        public int id;
        public String name;
        public String ip;
        public int port;
        public String login;
        public String pass;
        public int type;
        public int session;
    }

    class NvrManager
    {

        // 所有NVR的配置信息
        Hashtable ryNvrCfg = new Hashtable();

        // 管理的播放界面
        RealPlayerGrid realPlayerGrid;

        public NvrManager()
        {
            
        }

        public void setRealPlayerGrid(RealPlayerGrid rpg)
        {
            this.realPlayerGrid = rpg;
        }

        /**
         * 登出
         */
        private void nvrCleanup()
        {
            // 注销所有的登录
            foreach(RyNvr nvr in ryNvrCfg.Values)
            {
                if (nvr.session >= 0)
                {
                    CHCNetSDK.NET_DVR_Logout(nvr.session);
                }
            }

            ryNvrCfg.Clear();
            CHCNetSDK.NET_DVR_Cleanup();
        }

        /**
         * 初始化NVR配置
         */
        public void initConfig(String cfg)
        {
            nvrCleanup();

            // 解析Json
            List<RyNvr> nvrList = JsonConvert.DeserializeObject<List<RyNvr>>(cfg);

            // 添加到列表
            foreach (RyNvr nvr in nvrList)
            {
                ryNvrCfg.Add(nvr.id, nvr);
            }

            // 登陆到NVR
            Int32 nvrSessionId = -1;
            CHCNetSDK.NET_DVR_DEVICEINFO_V30 DeviceInfo = new CHCNetSDK.NET_DVR_DEVICEINFO_V30();
            CHCNetSDK.NET_DVR_Init();

            foreach (RyNvr nvr in ryNvrCfg.Values)
            {
                nvrSessionId = CHCNetSDK.NET_DVR_Login_V30(nvr.ip, nvr.port, nvr.login, nvr.pass, ref DeviceInfo);
                nvr.session = nvrSessionId;
            }
        }

        /**
         * 实时播放
         * 遍历相应的realplayergrid
         */
        public void realPlay(int nvr, int channel)
        {
            // 得到配置
            RyNvr nvrCfg = (RyNvr)ryNvrCfg[nvr];

            FormRealPlayer player= realPlayerGrid.getPlayer();

            if ((nvrCfg != null) && (player!=null))
            {
                player.isPlaying = true;
                player.previewInfo.lChannel = channel;
                IntPtr pUser = IntPtr.Zero;//用户数据 user data 
                player.realSession = CHCNetSDK.NET_DVR_RealPlay_V40(nvrCfg.session, ref player.previewInfo, null/*RealData*/, pUser);
                if (player.realSession != -1)
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
        }

        public void realPlayStop(int playSession)
        {
            CHCNetSDK.NET_DVR_StopRealPlay(playSession);
        }

        /**
         * PTZ 控制
         */
        public void ptzCtlStart(int nvr, int channel, PTZ_DIRECTION dir, int speed)
        {
            RyNvr nvrCfg = (RyNvr)ryNvrCfg[nvr];
            int d = 0,sp = (int)(speed * 0.7); 

            switch (dir)
            {
                case PTZ_DIRECTION.UP:
                    d = 21;
                    break;
                case PTZ_DIRECTION.RIGHT:
                    d = 24;
                    break;
                case PTZ_DIRECTION.DOWN:
                    d = 22;
                    break;
                case PTZ_DIRECTION.LEFT:
                    d = 23;
                    break;
                case PTZ_DIRECTION.UP_RIGHT:
                    d = 26;
                    break;
                case PTZ_DIRECTION.DOWN_RIGHT:
                    d = 28;
                    break;
                case PTZ_DIRECTION.DONN_LEFT:
                    d = 27;
                    break;
                case PTZ_DIRECTION.UP_LEFT:
                    d = 25;
                    break;
            }
            CHCNetSDK.NET_DVR_PTZControlWithSpeed_Other(nvrCfg.session, channel, d, 0, sp);

        }

        public void ptzStop(int nvr, int channel)
        {
            RyNvr nvrCfg = (RyNvr)ryNvrCfg[nvr];
            CHCNetSDK.NET_DVR_PTZControlWithSpeed_Other(nvrCfg.session, channel, 21, 1, 1);
            CHCNetSDK.NET_DVR_PTZControlWithSpeed_Other(nvrCfg.session, channel, 22, 1, 1);
            CHCNetSDK.NET_DVR_PTZControlWithSpeed_Other(nvrCfg.session, channel, 23, 1, 1);
            CHCNetSDK.NET_DVR_PTZControlWithSpeed_Other(nvrCfg.session, channel, 24, 1, 1);
            CHCNetSDK.NET_DVR_PTZControlWithSpeed_Other(nvrCfg.session, channel, 25, 1, 1);
            CHCNetSDK.NET_DVR_PTZControlWithSpeed_Other(nvrCfg.session, channel, 26, 1, 1);
            CHCNetSDK.NET_DVR_PTZControlWithSpeed_Other(nvrCfg.session, channel, 27, 1, 1);
            CHCNetSDK.NET_DVR_PTZControlWithSpeed_Other(nvrCfg.session, channel, 28, 1, 1);
        }

        /**
         * zoom 控制
         */
        public void zoomStart(int nvr, int channel, int dir)
        {
            RyNvr nvrCfg = (RyNvr)ryNvrCfg[nvr];
            CHCNetSDK.NET_DVR_PTZControlWithSpeed_Other(nvrCfg.session, channel, dir, 0, 7);

            CHCNetSDK.NET_DVR_PTZControlWithSpeed_Other(nvrCfg.session, channel, 11, 1, 7);
            CHCNetSDK.NET_DVR_PTZControlWithSpeed_Other(nvrCfg.session, channel, 12, 1, 7);
        }

        public void zoomStop(int nvr, int channel)
        {

        }

    }
}