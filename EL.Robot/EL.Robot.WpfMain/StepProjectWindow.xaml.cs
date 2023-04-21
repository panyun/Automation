using EL;
using EL.Robot.Core;
using EL.Robot.WpfMain.ViewModel;
using Robot.Controls;
using Robot.Test;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
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
using System.Windows.Shapes;

namespace Robot
{
    /// <summary>
    /// ProjectWindow.xaml 的交互逻辑
    /// </summary>
    public partial class StepProjectWindow : Window
    {
        //public static ObservableCollection<Msg> DataMsg { get; set; } = new ObservableCollection<Msg>();
        //public ObservableCollection<DataVertex> HistoryElements { get; set; } = new ObservableCollection<DataVertex>();
        int i = 0;
        public StepProjectWindow()
        {
            InitializeComponent();
            //this.Opacity = 0.8;
            this.WindowState = WindowState.Maximized;
            this.WindowStartupLocation = WindowStartupLocation.Manual;
            this.Left = 1000;
            this.Top = 1000;
            this.Left = SystemParameters.PrimaryScreenWidth - this.Width;
            this.Top = SystemParameters.PrimaryScreenHeight - this.Height;
            var uri = System.Environment.CurrentDirectory + @"\dist\index.html";
            this.wbrReport.Address = uri;
        }
        public void UpdateFlowAddr()
        {
            var robot = Boot.GetComponent<RobotComponent>();
            wbrReport.GetBrowser().MainFrame.ExecuteJavaScriptAsync($"window.setJsonData('{robot.RpaJson}',1,0)");
        }
        /// <summary>
        /// id 节点id
        /// </summary>
        /// <param name="id">id 节点id</param>
        /// <param name="state"> state 未开始:0,执行中:1,执行成功:2,执行异常:3</param>
        /// <param name="msg">msg 异常消息</param>
        public void UpdateFlowInfo(string id, int state, string msg)
        {
            /**
* 节点状态变动
* @param { string } id 节点id
* @param { number } state 未开始:0,执行中:1,执行成功:2,执行异常:3
* @param { string } msg 异常消息
*/
            wbrReport.GetBrowser().MainFrame.ExecuteJavaScriptAsync($"window.execChange('{id}',{state},'{msg}')");
        }
        //关闭
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            WindowManager.Close<StepProjectWindow>();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            this.WindowState = this.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
    }
}
