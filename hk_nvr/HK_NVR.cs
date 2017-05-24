using hk;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ry.rec
{
    public class NVR_2001: NvrInterface
    {
        // 历史回放控制数据
        volatile private Hashtable playBackBlocks = new Hashtable();

        // 定时器
        System.Timers.Timer timer = new System.Timers.Timer(500);

        public NVR_2001()
        {
            // 定时器初始化
            timer.Elapsed += new System.Timers.ElapsedEventHandler(onTimer);
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        /// <summary>
        /// 定时执行的函数
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void onTimer(object source, System.Timers.ElapsedEventArgs e)
        {
            uint iOutValue = 0;  // 播放进度
            int pTime;
            IntPtr lpOutBuffer = Marshal.AllocHGlobal(4);

            timer.Stop();

            // 更新播放进度
            foreach(PlayBackCtlBlock playBackBlock in playBackBlocks.Values)
            {
                CHCNetSDK.NET_DVR_PlayBackControl_V40(playBackBlock.session-1, CHCNetSDK.NET_DVR_PLAYGETTIME, IntPtr.Zero, 0, lpOutBuffer, ref iOutValue);
                pTime = (int)Marshal.PtrToStructure(lpOutBuffer, typeof(int));
                playBackBlock.even.onPlayBackTime(pTime);
            }

            Marshal.FreeHGlobal(lpOutBuffer);

            timer.Start();
        }

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

        /// <summary>
        /// 录像查询
        /// </summary>
        /// <param name="session"></param>
        /// <param name="cha"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        List<DATA_PAIR> NvrInterface.searchHistory(int session, int cha, DateTime start, DateTime end)
        {
            // 查询结果句柄
            Int32 m_lFindHandle = -1;

            // 查询结果
            List<DATA_PAIR> resultArray = new List<DATA_PAIR>();

            // 查询出的一个时间
            DateTime oneStart, oneEnd;
            DATA_PAIR onePair;

            // 查询结构体
            CHCNetSDK.NET_DVR_FILECOND_V40 struFileCond_V40 = new CHCNetSDK.NET_DVR_FILECOND_V40();
            struFileCond_V40.lChannel = cha; //通道号 Channel number
            struFileCond_V40.dwFileType = 0xff; //0xff-全部，0-定时录像，1-移动侦测，2-报警触发，...
            struFileCond_V40.dwIsLocked = 0xff; //0-未锁定文件，1-锁定文件，0xff表示所有文件（包括锁定和未锁定）


            //设置录像查找的开始时间 Set the starting time to search video files
            struFileCond_V40.struStartTime.dwYear = (uint)start.Year;
            struFileCond_V40.struStartTime.dwMonth = (uint)start.Month;
            struFileCond_V40.struStartTime.dwDay = (uint)start.Day;
            struFileCond_V40.struStartTime.dwHour = (uint)start.Hour;
            struFileCond_V40.struStartTime.dwMinute = (uint)start.Minute;
            struFileCond_V40.struStartTime.dwSecond = (uint)start.Second;

            //设置录像查找的结束时间 Set the stopping time to search video files
            struFileCond_V40.struStopTime.dwYear = (uint)end.Year;
            struFileCond_V40.struStopTime.dwMonth = (uint)end.Month;
            struFileCond_V40.struStopTime.dwDay = (uint)end.Day;
            struFileCond_V40.struStopTime.dwHour = (uint)end.Hour;
            struFileCond_V40.struStopTime.dwMinute = (uint)end.Minute;
            struFileCond_V40.struStopTime.dwSecond = (uint)end.Second;

            //开始录像文件查找 Start to search video files 
            m_lFindHandle = CHCNetSDK.NET_DVR_FindFile_V40(session - 1, ref struFileCond_V40);

            if (m_lFindHandle < 0)
            {
                // 没有录像，或者是出错
                return null;
            }
            else
            {
                CHCNetSDK.NET_DVR_FINDDATA_V30 struFileData = new CHCNetSDK.NET_DVR_FINDDATA_V30(); ;
                while (true)
                {
                    //逐个获取查找到的文件信息 Get file information one by one.
                    int result = CHCNetSDK.NET_DVR_FindNextFile_V30(m_lFindHandle, ref struFileData);

                    if (result == CHCNetSDK.NET_DVR_ISFINDING)  //正在查找请等待 Searching, please wait
                    {
                        continue;
                    }
                    else if (result == CHCNetSDK.NET_DVR_FILE_SUCCESS) //获取文件信息成功 Get the file information successfully
                    {

                        oneStart = new DateTime(
                            (int)struFileData.struStartTime.dwYear,
                            (int)struFileData.struStartTime.dwMonth,
                            (int)struFileData.struStartTime.dwDay,
                            (int)struFileData.struStartTime.dwHour,
                            (int)struFileData.struStartTime.dwMinute,
                            (int)struFileData.struStartTime.dwSecond
                            );
                        oneEnd = new DateTime(
                            (int)struFileData.struStopTime.dwYear,
                            (int)struFileData.struStopTime.dwMonth,
                            (int)struFileData.struStopTime.dwDay,
                            (int)struFileData.struStopTime.dwHour,
                            (int)struFileData.struStopTime.dwMinute,
                            (int)struFileData.struStopTime.dwSecond
                            );
                        onePair = new DATA_PAIR();
                        onePair.start = oneStart;
                        onePair.end = oneEnd;

                        resultArray.Add(onePair);

                    }
                    else if (result == CHCNetSDK.NET_DVR_FILE_NOFIND || result == CHCNetSDK.NET_DVR_NOMOREFILE)
                    {
                        break; //未查找到文件或者查找结束，退出   No file found or no more file found, search is finished 
                    }
                    else
                    {
                        break;
                    }
                }

            }

            return resultArray;
        }

        /// <summary>
        /// 根据时间进行回放
        /// </summary>
        /// <param name="session"></param>
        /// <param name="cha"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="handle"></param>
        /// <returns></returns>
        int NvrInterface.playBackByTime(int session, int cha, DateTime start, DateTime end, IntPtr handle, PlayBackEvent iPlayBack)
        {
            int result;

            // 播放信息的对象
            CHCNetSDK.NET_DVR_VOD_PARA struVodPara = new CHCNetSDK.NET_DVR_VOD_PARA();
            struVodPara.dwSize = (uint)Marshal.SizeOf(struVodPara);

            struVodPara.struIDInfo.dwChannel = (uint)cha;
            struVodPara.hWnd = handle;

            // 开始时间
            struVodPara.struBeginTime.dwYear = (uint)start.Year;
            struVodPara.struBeginTime.dwMonth = (uint)start.Month;
            struVodPara.struBeginTime.dwDay = (uint)start.Day;
            struVodPara.struBeginTime.dwHour = (uint)start.Hour;
            struVodPara.struBeginTime.dwMinute = (uint)start.Minute;
            struVodPara.struBeginTime.dwSecond = (uint)start.Second;

            // 结束时间
            struVodPara.struEndTime.dwYear = (uint)end.Year;
            struVodPara.struEndTime.dwMonth = (uint)end.Month;
            struVodPara.struEndTime.dwDay = (uint)end.Day;
            struVodPara.struEndTime.dwHour = (uint)end.Hour;
            struVodPara.struEndTime.dwMinute = (uint)end.Minute;
            struVodPara.struEndTime.dwSecond = (uint)end.Second;

            result = CHCNetSDK.NET_DVR_PlayBackByTime_V40(session - 1, ref struVodPara);

            if (result == -1)
            {
                // 播放失败
                return 0;
            } else
            {
                // 播放成功
                PlayBackCtlBlock ctlBlack = new PlayBackCtlBlock();
                ctlBlack.session = result + 1;
                ctlBlack.start = start;
                ctlBlack.end = end;
                ctlBlack.even = iPlayBack;
                playBackBlocks.Add(result + 1, ctlBlack);
                return result + 1;
            }
        }

        /// <summary>
        /// 回放控制
        /// </summary>
        /// <param name="session"></param>
        /// <param name="cmd"></param>
        /// <param name="parm"></param>
        /// <returns></returns>
        int NvrInterface.playBackCtl(int session, PLAY_BACK_CMD cmd, Object parm)
        {
            uint iOutValue = 0;
            
            switch (cmd)
            {
                case PLAY_BACK_CMD.START:       // 播放
                    CHCNetSDK.NET_DVR_PlayBackControl_V40(session - 1, CHCNetSDK.NET_DVR_PLAYSTART, IntPtr.Zero, 0, IntPtr.Zero, ref iOutValue);
                    break;
                case PLAY_BACK_CMD.PAUSE:       // 暂停
                    CHCNetSDK.NET_DVR_PlayBackControl_V40(session - 1, CHCNetSDK.NET_DVR_PLAYPAUSE, IntPtr.Zero, 0, IntPtr.Zero, ref iOutValue);
                    break;
                case PLAY_BACK_CMD.RESUME:      // 恢复
                    CHCNetSDK.NET_DVR_PlayBackControl_V40(session - 1, CHCNetSDK.NET_DVR_PLAYRESTART, IntPtr.Zero, 0, IntPtr.Zero, ref iOutValue);
                    break;
                case PLAY_BACK_CMD.POSITION:    // 定位
                    DateTime pTime = (DateTime)parm;
                    CHCNetSDK.NET_DVR_TIME pTimeS = new CHCNetSDK.NET_DVR_TIME();
                    uint inSize = (uint)Marshal.SizeOf(pTimeS);

                    pTimeS.dwYear = (uint)pTime.Year;
                    pTimeS.dwMonth = (uint)pTime.Month;
                    pTimeS.dwDay = (uint)pTime.Day;
                    pTimeS.dwHour = (uint)pTime.Hour;
                    pTimeS.dwMinute = (uint)pTime.Minute;
                    pTimeS.dwSecond = (uint)pTime.Second;

                    IntPtr buffer = Marshal.AllocHGlobal(Marshal.SizeOf(pTimeS));
                    Marshal.StructureToPtr(pTimeS, buffer, false);
                    CHCNetSDK.NET_DVR_PlayBackControl_V40(session - 1, CHCNetSDK.NET_DVR_PLAYSETTIME, buffer, inSize, IntPtr.Zero, ref iOutValue);
                    Marshal.FreeHGlobal(buffer);
                    break;
            }
            return 0;
        }

        /// <summary>
        /// 回放停止
        /// </summary>
        /// <param name="session"></param>
        void NvrInterface.playBackClose(int session)
        {
            playBackBlocks.Remove(session);
            CHCNetSDK.NET_DVR_StopPlayBack(session-1);
        }
        
    }
}
