using CefSharp.WinForms;
using hk;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ry.rec
{
    public partial class FormMain : Form
    {
        public static Form mainForm;

        private readonly ChromiumWebBrowser browser;

        // 播放管理
        private PlayerManager nvrManager;

        // 播放界面
        RealPlayerGrid realPlayerGrid;

        public FormMain()
        {
            InitializeComponent();

            mainForm = this;

            // 主窗口设置
            WindowState = FormWindowState.Maximized;

            // 浏览器设置
            String mc_url = ConfigurationManager.AppSettings["MC_URL"];
            browser = new ChromiumWebBrowser(mc_url)
            {
                Dock = DockStyle.Fill,
            };
            browser.KeyboardHandler = new KeyboardHandler();

            this.Controls.Add(browser);

            // 视频播放管理对象
            nvrManager = new PlayerManager();

            // 实时播放Grid
            realPlayerGrid = new RealPlayerGrid(nvrManager, 5, 1);
            realPlayerGrid.Dock = DockStyle.Left;
            realPlayerGrid.Width = 300;
            this.Controls.Add(realPlayerGrid);

            // 播放Grid与NvrManager关联
            nvrManager.setCurrentRealPlayerGrid(realPlayerGrid);
            nvrManager.addRealPlayerGrid(realPlayerGrid);

            // 向浏览器注册对象
            browser.RegisterAsyncJsObject("videoPlayer", nvrManager);            
        }
    }
}
