using CefSharp;
using CefSharp.Wpf;
using EL.Robot.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EL.Robot.Browser
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ChromiumWebBrowserFlowWpf : Window
    {

        public ChromiumWebBrowser chromiumWebBrowser = default;
        public ChromiumWebBrowserFlowWpf()
        {
            InitializeComponent();
            //this.WindowState = WindowState.Maximized;
            chromiumWebBrowser = new ChromiumWebBrowser();
            //cwbFlow
            CefSharpSettings.WcfEnabled = true;
            var uri = System.Environment.CurrentDirectory + @"\dist\index.html";
            chromiumWebBrowser.LoadUrl(uri);
            JsEvent jsEvent = new JsEvent();
            chromiumWebBrowser.JavascriptObjectRepository.Settings.LegacyBindingEnabled = true;
            chromiumWebBrowser.JavascriptObjectRepository.Register("jsEvent", jsEvent, false);
            dpFlow.Children.Add(chromiumWebBrowser);
        }
        public void UpdateFlowAddr(bool isDebug)
        {
            this.Dispatcher.Invoke(() =>
            {
                int state = isDebug ? 1 : 0;
                var robot = Boot.GetComponent<RobotComponent>();
                this.Title = robot.GetComponent<FlowComponent>().Flow.Name;
                chromiumWebBrowser.ExecuteScriptAsyncWhenPageLoaded($"window.setJsonData('{robot.RpaJson}',{state},0)");
            });

        }
        /// <summary>
        /// id 节点id
        /// </summary>
        /// <param name="id">id 节点id</param>
        /// <param name="state"> state 未开始:0,执行中:1,执行成功:2,执行异常:3</param>
        /// <param name="msg">msg 异常消息</param>
        public void UpdateFlowInfo(string id, int state, string msg)
        {
            this.Dispatcher.Invoke(() =>
            {
                var robot = Boot.GetComponent<RobotComponent>();
                chromiumWebBrowser.ExecuteScriptAsyncWhenPageLoaded($"window.execChange('{id}',{state},'{msg}')");
            });
        }
    }
}
