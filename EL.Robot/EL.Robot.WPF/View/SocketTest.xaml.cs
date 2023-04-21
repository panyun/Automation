using EL.Robot.Core;
using EL.Robot.Core.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EL.Robot.WPF.View
{
    /// <summary>
    /// SocketTest.xaml 的交互逻辑
    /// </summary>
    public partial class SocketTest : Window
    {
        public SocketTest()
        {
            InitializeComponent();
            EL.Boot.App = new AppMananger();
            var log = new FileLogger();
            log.SetLevel(LogLevel.Trace);
            Boot.SetLog(log);
            Log.Trace($"————Main Start！——--");

            //加载程序集
            Boot.App.EventSystem.Add(typeof(RobotComponent).Assembly);
            Boot.App.EventSystem.Add(typeof(RobotComponent).Assembly);
            var inspect = Boot.AddComponent<RobotComponent>();
            System.Threading.Tasks.Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        System.Threading.Thread.Sleep(1);
                        Boot.Update();
                    }
                    catch (Exception e)
                    {
                        Log.Error(e.Message, e);
                    }
                }
            });
        }

        private async void btn_Login_Click(object sender, RoutedEventArgs e)
        {

            var robot = EL.Boot.GetComponent<RobotComponent>();
            await robot.Main(null);

        }

        private void btn_Login_Copy_Click(object sender, RoutedEventArgs e)
        {
            var robot = EL.Boot.GetComponent<RobotComponent>();
            var flow = robot.GetComponent<FlowComponent>();
            var node = flow.GetComponent<NodeComponent>();
            //if (!node.ELTaskPaused.IsCompleted)
            //    node.ELTaskPaused.SetResult(true);
        }

        private async void btn_Login_Copy1_ClickAsync(object sender, RoutedEventArgs e)
        {

            LoginRequest request = new LoginRequest();
            //request.Ip = "192.168.192.1";
            request.Ip = "43.136.132.80";
            request.Port = 10001;
            request.Type = 1;
            request.UserId = 2;
            var enter = await request.LoginAsync();
            var robot = EL.Boot.GetComponent<RobotComponent>();
            var flow = robot.GetComponent<FlowComponent>();
            //flow.Action = (x) =>
            //{
            //    this.Dispatcher.Invoke(() =>
            //    {
            //        lal_Text.Content = x;
            //    });
            //};
            var node = flow.GetComponent<NodeComponent>();
            //node.Action = (x) =>
            //{
            //    this.Dispatcher.Invoke(() =>
            //    {
            //        lal_Text.Content = x;
            //    });
            //};
        }
    }
}
