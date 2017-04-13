﻿using hk;
using ry.rec.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ry.rec
{
    public partial class FormRealPlayer : Form
    {
        // 父容器
        RealPlayerGrid layerPanel;
        int column, row;

        // 加载光标资源
        Cursor cur_up = new Cursor(Resources.up.GetHicon());
        Cursor cur_down = new Cursor(Resources.down.GetHicon());
        Cursor cur_right = new Cursor(Resources.right.GetHicon());
        Cursor cur_left = new Cursor(Resources.left.GetHicon());
        Cursor cur_up_left = new Cursor(Resources.up_left.GetHicon());
        Cursor cur_up_right = new Cursor(Resources.up_right.GetHicon());
        Cursor cur_down_left = new Cursor(Resources.down_left.GetHicon());
        Cursor cur_down_right = new Cursor(Resources.down_right.GetHicon());

        // 是否全屏
        bool fullScreen = false;

        // PTZ 方向
        PTZ_DIRECTION ptzDirection = PTZ_DIRECTION.UP;

        // ZOOM 速度
        int ptzSpeed = 0;

        // 是否播放
        public bool isPlaying = false;

        // 是否选中
        public bool isSelected = false;

        // 播放数据
        public CHCNetSDK.NET_DVR_PREVIEWINFO previewInfo = new CHCNetSDK.NET_DVR_PREVIEWINFO();
        public Int32 realSession = -1;

        int nvr, channel;

        public FormRealPlayer()
        {
            InitializeComponent();

            this.BackColor = Color.White;

            previewInfo.hPlayWnd = this.videoBox.Handle;//预览窗口 live view window
            previewInfo.lChannel = 33;//预览的设备通道 the device channel number
            previewInfo.dwStreamType = 0;//码流类型：0-主码流，1-子码流，2-码流3，3-码流4，以此类推
            previewInfo.dwLinkMode = 0;//连接方式：0- TCP方式，1- UDP方式，2- 多播方式，3- RTP方式，4-RTP/RTSP，5-RSTP/HTTP 
            previewInfo.bBlocked = true; //0- 非阻塞取流，1- 阻塞取流
            previewInfo.dwDisplayBufNum = 3; //播放库显示缓冲区最大帧数

        }

        public void setVideoId(int nvr, int channel)
        {
            this.nvr = nvr;
            this.channel = channel;
        }

        // 触发重新绘制窗体
        private void formRedraw()
        {
            Color color = this.BackColor;
            this.BackColor = Color.Yellow;
            this.BackColor = color;
        }

        // 设置颜色
        public void formSelect(bool v)
        {
            if (v)
            {
                this.isSelected = true;
                this.BackColor = Color.Red;
            } else
            {
                this.isSelected = false;
                this.BackColor = Color.White;
            }
        }

        // 设置显示的容器和其他的值
        public void SetPlayer(TableLayoutPanel pr, int column, int row)
        {
            this.layerPanel = (RealPlayerGrid)pr;
            this.column = column;
            this.row = row;
        }

        // 视频双击
        private void videoBox_DoubleClick(object sender, EventArgs e)
        {
            if (this.fullScreen)
            {
                // 缩小
                this.Dock = DockStyle.Fill;
                this.TopLevel = false;
                this.FormBorderStyle = FormBorderStyle.None;
                //this.WindowState = FormWindowState.Normal;
                this.fullScreen = false;
                layerPanel.Controls.Add(this, column, row);
            }
            else
            {
                // 全屏化
                this.Parent.Controls.Remove(this);
                this.TopLevel = true;
                //this.Dock = DockStyle.None;
                //this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                this.WindowState = FormWindowState.Maximized;
                this.fullScreen = true;
            }
        }

        // 视频鼠标按下
        private void videoBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (!isPlaying)
            {
                return;
            }

            if (e.Button == MouseButtons.Left)
            {
                Task.Run(() => {
                    layerPanel.nvrManager.ptzCtlStart(nvr, channel, ptzDirection, ptzSpeed);
                });
                
            }

        }

        // 视频鼠标释放
        private void videoBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (!isPlaying)
            {
                return;
            }
            if (e.Button == MouseButtons.Left)
            {
                Task.Run(() =>
                {
                    layerPanel.nvrManager.ptzStop(nvr, channel);
                });
            }
        }

        // 鼠标滚轮
        public void videoBox_MouseWheel(object sender, MouseEventArgs e)
        {
            if (!isPlaying)
            {
                return;
            }
            if (e.Delta > 0)
            {
                // 上滚
                Task.Run(() =>
                {
                    layerPanel.nvrManager.zoomStart(nvr, channel, 11);
                });
                
            }
            else
            {
                // 下滚
                Task.Run(() =>
                {
                    layerPanel.nvrManager.zoomStart(nvr, channel, 12);
                });
            }
        }


        // 视频单击
        private void videoBox_MouseClick(object sender, MouseEventArgs e)
        {
            // 左键，设置焦点
            if (e.Button == MouseButtons.Left)
            {
                layerPanel.unSelectAll();
                formSelect(true);
            }

            // 右键，关闭视频
            if (e.Button == MouseButtons.Right)
            {
                if (isPlaying)
                {
                    layerPanel.nvrManager.realPlayStop(this.realSession);
                    isPlaying = false;
                    formRedraw();
                }
            }
        }

        // 视频鼠移动
        private void videoBox_MouseMove(object sender, MouseEventArgs e)
        {
            // 中心点
            int m_x = this.Width / 2;
            int m_y = this.Height / 2;

            // 角度 -180~+180
            double angle = 180 * Math.Atan2(m_y - e.Y, e.X - m_x) / Math.PI;

            if (angle >= -22.5 && angle < 22.5)
            {
                this.Cursor = this.cur_right;
                ptzDirection = PTZ_DIRECTION.RIGHT;
            }
            else if (angle >= 22.5 && angle < 67.5)
            {
                this.Cursor = this.cur_up_right;
                ptzDirection = PTZ_DIRECTION.UP_RIGHT;
            }
            else if (angle >= 67.5 && angle < 112.5)
            {
                this.Cursor = this.cur_up;
                ptzDirection = PTZ_DIRECTION.UP;
            }
            else if (angle >= 112.5 && angle < 167.5)
            {
                this.Cursor = this.cur_up_left;
                ptzDirection = PTZ_DIRECTION.UP_LEFT;
            }
            else if (angle >= 167.5 || angle <= -167.5)
            {
                this.Cursor = this.cur_left;
                ptzDirection = PTZ_DIRECTION.LEFT;
            }
            else if (angle < -22.5 && angle >= -67.5)
            {
                this.Cursor = this.cur_down_right;
                ptzDirection = PTZ_DIRECTION.DOWN_RIGHT;
            }
            else if (angle < -67.5 && angle >= -112.5)
            {
                this.Cursor = this.cur_down;
                ptzDirection = PTZ_DIRECTION.DOWN;
            }
            else if (angle < -112.5 && angle >= -167.5)
            {
                this.Cursor = this.cur_down_left;
                ptzDirection = PTZ_DIRECTION.DONN_LEFT;
            }

            // 距离中心的距离（1~10）,表示PTZ速度
            double i = Math.Sqrt(Math.Pow(e.X - m_x, 2) + Math.Pow(e.Y - m_y, 2));
            int d = (int)((i / m_y) * 10);

            if (d <= 10)
            {
                ptzSpeed = d;
            }
            else
            {
                ptzSpeed = 10;
            }
        }

    }
}