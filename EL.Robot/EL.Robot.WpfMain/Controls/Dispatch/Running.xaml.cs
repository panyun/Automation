using EL.Robot.Core.SqliteEntity;
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

namespace Robot.Controls.Dispatch
{
    /// <summary>
    /// MyScript.xaml 的交互逻辑
    /// </summary>
    public partial class Running : UserControl
    {
        public static Running Instance;
        public Running()
        {
            Instance = this;
            InitializeComponent();
            Update();
        }
        public void Update()
        {
            var runing = RobotDataManagerService.GetFlowRuning();

            if (runing != null)
            {
                RuningDatas = new List<FlowRuning>();
                lbl_RuningCount.Text = "共1个脚本";
                RuningDatas.Add(runing);
                itemCtl_RuningData.ItemsSource = RuningDatas;
                return;
            }
            lbl_RuningCount.Text = "共0个脚本";
        }
        public List<FlowRuning> RuningDatas { get; set; }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            popmenu.PlacementTarget = sender as Button;
            popmenu.IsOpen = false;
            popmenu.IsOpen = true;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            RobotDataManagerService.StopFlow();
            Update();
        }
    }
}
