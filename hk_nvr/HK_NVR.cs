using hk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ry.rec
{
    public class NVR_2001: NvrInterface
    {
        /// <summary>
        /// 登陆NVR
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        int NvrInterface.nvrLogin(String ip, int port, String username, String password)
        {
            Int32 nvrSessionId = 0;
            CHCNetSDK.NET_DVR_DEVICEINFO_V30 DeviceInfo = new CHCNetSDK.NET_DVR_DEVICEINFO_V30();
            CHCNetSDK.NET_DVR_Init();
            nvrSessionId = CHCNetSDK.NET_DVR_Login_V30(ip, port, username, password, ref DeviceInfo);

            if (nvrSessionId >= 0)
            {
                return nvrSessionId + 1;
            } else
            {
                return 0;
            }
        }

        /// <summary>
        /// 实时播放
        /// </summary>
        /// <param name="session"></param>
        /// <param name="cha"></param>
        /// <param name="handle"></param>
        /// <returns></returns>
        int NvrInterface.nvrRealPlay(int session, int cha, IntPtr handle) {

            CHCNetSDK.NET_DVR_PREVIEWINFO previewInfo = new CHCNetSDK.NET_DVR_PREVIEWINFO();

            previewInfo.hPlayWnd = handle;//预览窗口 live view window
            previewInfo.lChannel = cha;//预览的设备通道 the device channel number
            previewInfo.dwStreamType = 0;//码流类型：0-主码流，1-子码流，2-码流3，3-码流4，以此类推
            previewInfo.dwLinkMode = 0;//连接方式：0- TCP方式，1- UDP方式，2- 多播方式，3- RTP方式，4-RTP/RTSP，5-RSTP/HTTP 
            previewInfo.bBlocked = true; //0- 非阻塞取流，1- 阻塞取流
            previewInfo.dwDisplayBufNum = 3; //播放库显示缓冲区最大帧数

            int realSession = CHCNetSDK.NET_DVR_RealPlay_V40(session-1, ref previewInfo, null/*RealData*/, IntPtr.Zero);
            return realSession;
        }

        /// <summary>
        /// 关闭播放
        /// </summary>
        /// <param name="session"></param>
        void NvrInterface.nvrRealPlayStop(int session) {
            CHCNetSDK.NET_DVR_StopRealPlay(session);
        }

        /// <summary>
        /// PTZ 控制 开始
        /// </summary>
        /// <param name="nvr"></param>
        /// <param name="cha"></param>
        /// <param name="dir"></param>
        /// <param name="speed"></param>
        void NvrInterface.nvrPtzStart(int session, int cha, PTZ_DIR dir, int speed) {
            int d = 0;
            int sp = (int)(speed * 0.7);

            switch (dir)
            {
                case PTZ_DIR.UP:
                    d = 21;
                    break;
                case PTZ_DIR.RIGHT:
                    d = 24;
                    break;
                case PTZ_DIR.DOWN:
                    d = 22;
                    break;
                case PTZ_DIR.LEFT:
                    d = 23;
                    break;
                case PTZ_DIR.UP_RIGHT:
                    d = 26;
                    break;
                case PTZ_DIR.DOWN_RIGHT:
                    d = 28;
                    break;
                case PTZ_DIR.DONN_LEFT:
                    d = 27;
                    break;
                case PTZ_DIR.UP_LEFT:
                    d = 25;
                    break;
            }
            CHCNetSDK.NET_DVR_PTZControlWithSpeed_Other(session-1, cha, d, 0, sp);
        }

        /// <summary>
        /// PTZ 控制 停止
        /// </summary>
        /// <param name="nvr"></param>
        /// <param name="cha"></param>
        void NvrInterface.nvrPtzStop(int session, int cha) {
            CHCNetSDK.NET_DVR_PTZControlWithSpeed_Other(session - 1, cha, 21, 1, 1);
            CHCNetSDK.NET_DVR_PTZControlWithSpeed_Other(session - 1, cha, 22, 1, 1);
            CHCNetSDK.NET_DVR_PTZControlWithSpeed_Other(session - 1, cha, 23, 1, 1);
            CHCNetSDK.NET_DVR_PTZControlWithSpeed_Other(session - 1, cha, 24, 1, 1);
            CHCNetSDK.NET_DVR_PTZControlWithSpeed_Other(session - 1, cha, 25, 1, 1);
            CHCNetSDK.NET_DVR_PTZControlWithSpeed_Other(session - 1, cha, 26, 1, 1);
            CHCNetSDK.NET_DVR_PTZControlWithSpeed_Other(session - 1, cha, 27, 1, 1);
            CHCNetSDK.NET_DVR_PTZControlWithSpeed_Other(session - 1, cha, 28, 1, 1);
        }

        /// <summary>
        /// 变焦
        /// </summary>
        /// <param name="nvr"></param>
        /// <param name="cha"></param>
        /// <param name="dir"></param>
        void NvrInterface.nvrZoom(int session, int cha, ZOOM_DIR dir) {

            // 开始变焦
            if (dir == ZOOM_DIR.ZOOM_IN)
            {
                CHCNetSDK.NET_DVR_PTZControlWithSpeed_Other(session - 1, cha, 11, 0, 7);
            } else
            {
                CHCNetSDK.NET_DVR_PTZControlWithSpeed_Other(session - 1, cha, 12, 0, 7);
            }
            
            // 停止变焦
            CHCNetSDK.NET_DVR_PTZControlWithSpeed_Other(session - 1, cha, 11, 1, 7);
            CHCNetSDK.NET_DVR_PTZControlWithSpeed_Other(session - 1, cha, 12, 1, 7);
        }

        /// <summary>
        /// 释放SDK资源
        /// </summary>
        void NvrInterface.nvrFree() {
            CHCNetSDK.NET_DVR_Cleanup();
        }
    }
}
