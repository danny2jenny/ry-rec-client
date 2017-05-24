using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ry.rec
{
    public partial class FormPlayBack : Form, PlayBackEvent
    {
        public int nvr;                 // nvr
        public int channel;             // channel
        public int playBackSession;     // 回放的 Session
        public bool isPlaying = false, isPause = false;

        public IntPtr playerHandle;
        PlayerManager playerManager;

        bool draging = false;           // 是否在拖动

        DateTime playStart, playEnd, currentPlayTime = new DateTime();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="mgr"></param>
        public FormPlayBack(PlayerManager mgr)
        {
            InitializeComponent();
            this.playerManager = mgr;
            this.playerHandle = this.player.Handle;
        }

        /// <summary>
        /// 窗口显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormPlayBack_Shown(object sender, EventArgs e)
        {
            this.calendar.SelectDate(DateTime.Now);
        }

        private void FormPlayBack_Resize(object sender, EventArgs e)
        {
            labelCurrentTime.Left = ctlPanel.Width / 2 - labelCurrentTime.Width / 2;
        }

        /// <summary>
        /// 播放的回调接口实现
        /// </summary>
        /// <param name="parm">秒</param>
        void PlayBackEvent.onPlayBackTime(int parm)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((Action)delegate
                {

                    _playBackTime(parm);
                });
            }
            else
            {
                _playBackTime(parm);
            }
        }

        void _playBackTime(int parm)
        {
            if (!draging)
            {
                if (parm < 0)
                {
                    return;
                }
                timeSlider.Value = parm;                
            }
        }

        /// <summary>
        /// 日期选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void calendar_DaySelected(object sender, Pabo.Calendar.DaySelectedEventArgs e)
        {
            if (calendar.SelectedDates.Count > 0)
            {
                searchByDay(calendar.SelectedDates[0]);
            }
        }

        /// <summary>
        /// 双击播放对应的历史视频
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void historyList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            stopPlay();
            if (historyList.SelectedItems.Count > 0)
            {
                playStart = Convert.ToDateTime(historyList.SelectedItems[0].SubItems[0].Text);
                playEnd = Convert.ToDateTime(historyList.SelectedItems[0].SubItems[1].Text);
                playBackSession = playerManager.nvrAdapterMgr.playBackByTime(nvr, channel, playStart, playEnd, this.playerHandle, this);
                if (playBackSession > 0)
                {
                    isPlaying = true;
                    isPause = false;
                    setTimeBar(playStart, playEnd);
                    playerManager.nvrAdapterMgr.playBackCtl(nvr, playBackSession, PLAY_BACK_CMD.START, 0);
                }
            }
        }

        private void setTimeBar(DateTime dStart, DateTime dEnd)
        {
            timeSlider.Minimum = 0;
            timeSlider.Maximum = (int)(dEnd - dStart).TotalSeconds;
            labelStart.Text = historyList.SelectedItems[0].SubItems[0].Text;
            labelEnd.Text = historyList.SelectedItems[0].SubItems[1].Text;
        }

        /// <summary>
        /// 停止播放
        /// </summary>
        private void stopPlay()
        {
            if (isPlaying)
            {
                playerManager.nvrAdapterMgr.playBackClose(nvr, playBackSession);
                isPlaying = false;
                this.Refresh();
            }
        }

        /// <summary>
        /// 窗口关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormPlayBack_FormClosed(object sender, FormClosedEventArgs e)
        {
            stopPlay();
            this.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 拖动定位
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timeSlider_MouseUp(object sender, MouseEventArgs e)
        {
            if (draging)
            {
                draging = false;
                if (isPlaying)
                {
                    currentPlayTime = playStart.AddSeconds(timeSlider.Value);
                    playerManager.nvrAdapterMgr.playBackCtl(nvr, playBackSession, PLAY_BACK_CMD.POSITION, currentPlayTime);
                }                
            }
            
        }

        private void timeSlider_ValueChanged(object sender, EventArgs e)
        {
            currentPlayTime = playStart.AddSeconds(timeSlider.Value);
            labelCurrentTime.Text = currentPlayTime.ToString("HH:mm:ss");
        }

        private void timeSlider_MouseDown(object sender, MouseEventArgs e)
        {
            draging = true;
        }


        /// <summary>
        /// 单击，暂停或者是继续
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void player_MouseClick(object sender, MouseEventArgs e)
        {
            if (!isPlaying)
            {
                return;
            }
            if (isPause)
            {
                isPause = false;
                playerManager.nvrAdapterMgr.playBackCtl(nvr, playBackSession, PLAY_BACK_CMD.RESUME, 0);                
            } else
            {
                isPause = true;
                playerManager.nvrAdapterMgr.playBackCtl(nvr, playBackSession, PLAY_BACK_CMD.PAUSE, 0);
            }
            
        }

        /// <summary>
        /// 按天进行视频的搜索
        /// </summary>
        /// <param name="day"></param>
        public void searchByDay(DateTime today)
        {

            int year = today.Year;
            int month = today.Month;
            int day = today.Day;
            DateTime start = new DateTime(year, month, day, 0, 0, 0);
            DateTime end = start.AddDays(1);

            List<DATA_PAIR> result = playerManager.nvrAdapterMgr.searchHistory(this.nvr, this.channel, start, end);
            if (result == null)
            {
                return;
            }

            historyList.Items.Clear();
            // 返回的查询结果加载到list
            foreach(DATA_PAIR rec in result)
            {
                //将查找的录像文件添加到列表中
                historyList.Items.Add(new ListViewItem(new string[] {
                    rec.start.ToString("yyyy-MM-dd HH:mm:ss"),
                    rec.end.ToString("yyyy-MM-dd HH:mm:ss") }));
            }
        }
    }
}
