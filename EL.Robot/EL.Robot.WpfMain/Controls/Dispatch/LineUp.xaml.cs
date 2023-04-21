using EL.Robot.Core;
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
    /// LineUp.xaml 的交互逻辑
    /// </summary>
    public partial class LineUp : UserControl
    {
        public static LineUp Instance;
        public LineUp()
        {
            Instance = this;
            InitializeComponent();
            Update();
        }
        public void Update()
        {
           
                Children = RobotDataManagerService.GetFlowQueues();
                Count = Children?.Count ?? 0;
                lbl_QueueCount.Text = $"共{Count}个脚本";
                itemCtl_QueueData.ItemsSource = Children;
        }
        public long CurrentId = default;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            popmenu.PlacementTarget = sender as Button;
            popmenu.IsOpen = false;
            popmenu.IsOpen = true;
            CurrentId = long.Parse((sender as Button).Tag + "");
        }
        public int Count { get; set; }
        public List<FlowQueue> Children { get; set; }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            RobotDataManagerService.RemoveFlowQueue(CurrentId);
            Update();
        }
    }
}
