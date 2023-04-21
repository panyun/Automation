using CefSharp;
using CefSharp.WinForms;
using EL.Robot.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EL.Robot.Win
{

    public partial class ChromiumWebBrowserFlow : Form
    {
        ChromiumWebBrowser chromiumWebBrowser;
        public Action ShowFlowCallBack;
        public ChromiumWebBrowserFlow()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.Manual;
            this.Height = Screen.PrimaryScreen.Bounds.Height / 2 - 25;
            Left = Screen.PrimaryScreen.Bounds.Width - this.Width - 30;
            Top = Screen.PrimaryScreen.Bounds.Height - this.Height - 30 - this.Height;

            this.TopMost = true;
            chromiumWebBrowser = new ChromiumWebBrowser();
            chromiumWebBrowser.Dock = DockStyle.Fill;
            CefSharpSettings.WcfEnabled = true;
            var uri = Environment.CurrentDirectory + @"\dist\index.html";
            chromiumWebBrowser.LoadUrl(uri);
            chromiumWebBrowser.JavascriptObjectRepository.Settings.LegacyBindingEnabled = true;
            chromiumWebBrowser.JavascriptObjectRepository.Register("jsEvent", new jsEvent(), false, BindingOptions.DefaultBinder);
            this.Controls.Add(chromiumWebBrowser);
            this.FormClosing += (x, y) =>
            {
                ShowFlowCallBack?.Invoke();
                y.Cancel = true;
                this.Hide();
            };
        }
        public void UpdateFlowAddr(bool isDebug)
        {
            int state = isDebug ? 1 : 0;
            this.Invoke(new Action(() =>
            {
                var robot = Boot.GetComponent<RobotComponent>();
                this.Text = robot.GetComponent<FlowComponent>().MainFlow.Name;
                chromiumWebBrowser.ExecuteScriptAsyncWhenPageLoaded($"window.setJsonData('{robot.RpaJson}',{state},0)");
            }));
        }

        /// <summary>
        /// id 节点id
        /// </summary>
        /// <param name="id">id 节点id</param>
        /// <param name="state"> state 未开始:0,执行中:1,执行成功:2,执行异常:3</param>
        /// <param name="msg">msg 异常消息</param>
        public void UpdateFlowInfo(string id, int state, string msg)
        {
            this.Invoke(new Action(() =>
            {
                var robot = Boot.GetComponent<RobotComponent>();
                chromiumWebBrowser.ExecuteScriptAsyncWhenPageLoaded($"window.execChange('{id}',{state},'{msg}')");
            }));
        }
        /// <summary>
        /// 设置窗体的Region
        /// </summary>
        public void SetWindowRegion()
        {
            GraphicsPath FormPath;
            Rectangle rect = new Rectangle(0, 0, this.Width, this.Height);
            FormPath = GetRoundedRectPath(rect, 50);
            this.Region = new Region(FormPath);

        }
        /// <summary>
        /// 绘制圆角路径
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        private GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            int diameter = radius;
            Rectangle arcRect = new Rectangle(rect.Location, new Size(diameter, diameter));
            GraphicsPath path = new GraphicsPath();

            // 左上角
            path.AddArc(arcRect, 180, 90);

            // 右上角
            arcRect.X = rect.Right - diameter;
            path.AddArc(arcRect, 270, 90);

            // 右下角
            arcRect.Y = rect.Bottom - diameter;
            path.AddArc(arcRect, 0, 90);

            // 左下角
            arcRect.X = rect.Left;
            path.AddArc(arcRect, 90, 90);
            path.CloseFigure();//闭合曲线
            return path;
        }


        private void jttCornerRadiusLabel3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void jttCornerRadiusLabel1_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void jttCornerRadiusLabel2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
        }
    }
}
