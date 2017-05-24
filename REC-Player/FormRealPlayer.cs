using hk;
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
        // Grid父容器
        RealPlayerGrid parentGrid;
        int column, row;

        // NVR Manager
        PlayerManager nvrManager;

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
        PTZ_DIR ptzDirection = PTZ_DIR.UP;

        // ZOOM 速度
        int ptzSpeed = 0;

        // 是否播放
        public bool isPlaying = false;

        // 是否选中
        public bool isSelected = false;

        // 播放数据
        public int realSession = -1;
        public IntPtr playerHandle;

        // NVR 和相应的通道
        public int nvr, channel;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="mgr"></param>
        public FormRealPlayer(PlayerManager mgr)
        {
            InitializeComponent();

            this.BackColor = Color.White;
            this.nvrManager = mgr;
            this.playerHandle = this.videoBox.Handle;

        }

        /// <summary>
        /// 设置播放信息 
        /// </summary>
        /// <param name="nvr"></param>
        /// <param name="channel"></param>
        public void setVideoId(int nvr, int channel)
        {
            this.nvr = nvr;
            this.channel = channel;
        }

        /// <summary>
        /// 停止播放
        /// </summary>
        public void stop()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((Action)delegate
                {
                    if (isPlaying)
                    {
                        nvrManager.nvrAdapterMgr.realPlayStop(this.nvr, this.realSession);
                        isPlaying = false;
                        this.Refresh();
                    }
                });
            }
            else
            {
                if (isPlaying)
                {
                    nvrManager.nvrAdapterMgr.realPlayStop(this.nvr,this.realSession);
                    isPlaying = false;
                    this.Refresh();
                }
            }
        }

        /// <summary>
        /// 设置颜色
        /// </summary>
        /// <param name="v"></param>
        public void formSelect(bool v)
        {
            if (v)
            {
                this.isSelected = true;
                this.BackColor = Color.Red;
            }
            else
            {
                this.isSelected = false;
                this.BackColor = Color.White;
            }
        }

        /// <summary>
        /// 设置显示的容器和其他的值
        /// </summary>
        /// <param name="pr"></param>
        /// <param name="column"></param>
        /// <param name="row"></param>
        public void setGrid(RealPlayerGrid pr, int column, int row)
        {
            this.parentGrid = pr;
            this.column = column;
            this.row = row;
        }

        /// <summary>
        /// 视频双击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void videoBox_DoubleClick(object sender, EventArgs e)
        {
            if (this.fullScreen)
            {
                // 缩小
                if (parentGrid != null)
                {

                    this.Dock = DockStyle.Fill;
                    this.TopLevel = false;
                    this.FormBorderStyle = FormBorderStyle.None;
                    this.fullScreen = false;
                    parentGrid.Controls.Add(this, column, row);
                }
                else
                {
                    this.TopLevel = true;
                    this.WindowState = FormWindowState.Normal;
                    this.fullScreen = false;
                }

            }
            else
            {
                // 全屏化
                if (parentGrid != null)
                {
                    this.Parent.Controls.Remove(this);
                    this.TopLevel = true;
                    this.WindowState = FormWindowState.Maximized;
                    this.fullScreen = true;
                }
                else
                {
                    this.TopLevel = true;
                    this.FormBorderStyle = FormBorderStyle.None;
                    this.fullScreen = true;
                }
            }
        }

        /// <summary>
        /// 视频鼠标按下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void videoBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (!isPlaying)
            {
                return;
            }

            if (e.Button == MouseButtons.Left)
            {
                Task.Run(() =>
                {
                    nvrManager.nvrAdapterMgr.ptzCtlStart(nvr, channel, ptzDirection, ptzSpeed);
                });

            }

        }

        /// <summary>
        /// 视频鼠标释放
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                    nvrManager.nvrAdapterMgr.ptzStop(nvr, channel);
                });
            }
        }

        /// <summary>
        /// 鼠标滚轮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                    nvrManager.nvrAdapterMgr.zoomStart(nvr, channel, ZOOM_DIR.ZOOM_IN);
                });

            }
            else
            {
                // 下滚
                Task.Run(() =>
                {
                    nvrManager.nvrAdapterMgr.zoomStart(nvr, channel, ZOOM_DIR.ZOOM_OUT);
                });
            }
        }

        /// <summary>
        /// 窗口关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormRealPlayer_FormClosed(object sender, FormClosedEventArgs e)
        {
            stop();
            if (parentGrid == null)
            {
                nvrManager.removeRealPlayer(this);
            }

            this.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 视频单击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void videoBox_MouseClick(object sender, MouseEventArgs e)
        {
            // 左键，设置焦点
            if ((e.Button == MouseButtons.Left) && (parentGrid != null))
            {
                parentGrid.unSelectAll();
                formSelect(true);
            }

            // 右键，关闭视频
            if (e.Button == MouseButtons.Right)
            {
                stop();
            }
        }

        /// <summary>
        /// 视频鼠移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                ptzDirection = PTZ_DIR.RIGHT;
            }
            else if (angle >= 22.5 && angle < 67.5)
            {
                this.Cursor = this.cur_up_right;
                ptzDirection = PTZ_DIR.UP_RIGHT;
            }
            else if (angle >= 67.5 && angle < 112.5)
            {
                this.Cursor = this.cur_up;
                ptzDirection = PTZ_DIR.UP;
            }
            else if (angle >= 112.5 && angle < 167.5)
            {
                this.Cursor = this.cur_up_left;
                ptzDirection = PTZ_DIR.UP_LEFT;
            }
            else if (angle >= 167.5 || angle <= -167.5)
            {
                this.Cursor = this.cur_left;
                ptzDirection = PTZ_DIR.LEFT;
            }
            else if (angle < -22.5 && angle >= -67.5)
            {
                this.Cursor = this.cur_down_right;
                ptzDirection = PTZ_DIR.DOWN_RIGHT;
            }
            else if (angle < -67.5 && angle >= -112.5)
            {
                this.Cursor = this.cur_down;
                ptzDirection = PTZ_DIR.DOWN;
            }
            else if (angle < -112.5 && angle >= -167.5)
            {
                this.Cursor = this.cur_down_left;
                ptzDirection = PTZ_DIR.DONN_LEFT;
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

        /// <summary>
        /// 多线程关闭方法
        /// </summary>
        public void closeMe()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((Action)delegate
                {
                    Close();
                });
            }
            else
            {
                Close();
            }
        }

    }
}
