using EL;
using EL.Robot;
using EL.Robot.Component;
using EL.Robot.Component.PIP;
using EL.Robot.Core;
using EL.Robot.Core.SqliteEntity;
using EL.Robot.WpfMain.DispatchWindows;
using EL.Robot.WpfMain.DispatchWindows.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace Robot.Controls.Dispatch
{
    /// <summary>
    /// MyScript.xaml 的交互逻辑
    /// </summary>
    public partial class MyScript : UserControl
    {
        public static MyScript Instance;
        public MyScript()
        {
            InitializeComponent();
            Instance = this;
            Update();
        }
        public void Update()
        {
            Children = RobotDataManagerService.GetFlowSummarys();
            if (Children == null || Children.Count == 0) return;
            Count = Children.Count;
            itemCtl_MyScript.ItemsSource = Children;
            txt_MyScript.Text = $"共{Count}个脚本";
        }
        public int CurrentId = default;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            popmenu.PlacementTarget = sender as Button;
            popmenu.IsOpen = false;
            popmenu.IsOpen = true;
            CurrentId = int.Parse((sender as Button).Tag + "");
        }

        private void btn_Runing_Click(object sender, RoutedEventArgs e)
        {
            var flow = RobotDataManagerService.GetFlowById(CurrentId);
            if (flow == null)
            {
                System.Windows.Forms.MessageBox.Show("未找到本地流程信息！");
                return;
            }
            var robot = Boot.GetComponent<RobotComponent>();
            robot.AddLocalFlowQueue(flow, false);
        }
        public int Count { get; set; }
        public List<FlowSummary> Children { get; set; }

        private void Plan_Click(object sender, RoutedEventArgs e)
        {
            WindowManager.Show<Plan>(new PlanViewModel(flowId: CurrentId));
        }

        private void pip_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(async () =>
            {
                var flow = new FlowScript() { Id = CurrentId }; 
                var pip = Boot.GetComponent<RobotComponent>().GetComponent<PIPServerComponent>();
                var result = await pip.StartAsync();
                if (result.Item1)
                {
                    if (!pip.Ready)
                    {
                        SpinWait.SpinUntil(() => pip.Ready, 60 * 1000);
                    }
                    if (pip.Ready)
                    {
                        pip.Send(flow);
                    }
                    else
                    {
                        System.Windows.MessageBox.Show($"服务连接超时!");
                    }
                }
                else
                {
                    System.Windows.MessageBox.Show(result.Item2);
                }
            });
        }
    }
}
