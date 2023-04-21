using EL.Robot.Core.SqliteEntity;
using EL.Robot.WpfMain.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Robot.Controls.Dispatch
{
    /// <summary>
    /// History.xaml 的交互逻辑
    /// </summary>
    public partial class History : UserControl
    {
        public static History Instance;
        public History()
        {
            Instance = this;
            InitializeComponent();
            //this.DataContext = new ScriptInformationViewModel();
            Update();
        }
        public void Update()
        {
            Count = RobotDataManagerService.GetFlowHistorysLength();
            FlowHistories = RobotDataManagerService.GetFlowHistorys(0, Count);
            if (FlowHistories == default) FlowHistories = new List<FlowHistory>();
            txt_Count.Text = $"共{Count}条";
            dg_FlowData.ItemsSource = FlowHistories;
        }
        public List<FlowHistory> FlowHistories { get; set; }
        public int Count { get; set; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var filePath = (string)btn.Tag;
            System.Diagnostics.Process.Start("notepad.exe", filePath);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var filePath = (string)btn.Tag;
            if (string.IsNullOrEmpty(filePath)) return;
            System.Diagnostics.Process.Start(filePath);
        }
    }
}
