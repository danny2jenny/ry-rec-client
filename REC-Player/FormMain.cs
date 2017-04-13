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
        private readonly ChromiumWebBrowser browser;

        // 播放管理
        private NvrManager nvrManager;

        // 播放界面
        RealPlayerGrid realPlayerGrid = new RealPlayerGrid(5, 1);

        public FormMain()
        {
            InitializeComponent();

            String mc_url = ConfigurationManager.AppSettings["mc_url"];
            
            WindowState = FormWindowState.Maximized;
            browser = new ChromiumWebBrowser(mc_url)
            {
                Dock = DockStyle.Fill,
            };
            this.Controls.Add(browser);
            realPlayerGrid.Dock = DockStyle.Left;
            this.Controls.Add(realPlayerGrid);
            realPlayerGrid.Width = 300;

            // 视频播放管理对象
            nvrManager = new NvrManager();
            nvrManager.setRealPlayerGrid(realPlayerGrid);
            realPlayerGrid.setNvrManager(nvrManager);

            // 向浏览器注册对象
            browser.RegisterAsyncJsObject("videoPlayer",nvrManager);

        }

    }
}
