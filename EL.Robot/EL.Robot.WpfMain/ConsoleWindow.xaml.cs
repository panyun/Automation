using EL;
using EL.Robot.Core;
using EL.Robot.WpfMain.ModelData;
using EL.Robot.WpfMain.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfInspect.ViewModels;

namespace Robot
{
    /// <summary>
    /// Console1Window.xaml 的交互逻辑
    /// </summary>
    public partial class ConsoleWindow : Window
    {
        public ObservableCollection<DataModelConsole> NodeFiltDetailSources { get; set; } = new ObservableCollection<DataModelConsole>();
        public Action CallBack;

        /* public ObservableCollection<TreeNodeViewModel> treeNodeViewModels { get; set; } = new ObservableCollection<TreeNodeViewModel>();*/
        public ConsoleWindow()
        {
            InitializeComponent();
            this.Topmost = true;
            WindowStartupLocation = WindowStartupLocation.Manual;
            this.Height = Screen.PrimaryScreen.Bounds.Height / 2 - 25;
            Left = Screen.PrimaryScreen.Bounds.Width - this.Width - 30;
            Top = Screen.PrimaryScreen.Bounds.Height - this.Height - 45;
            Listbox1.ItemsSource = NodeFiltDetailSources;
            var flowComponent = Boot.GetComponent<RobotComponent>().GetComponent<FlowComponent>();
            DataModelConsole obj = default;
            foreach (var item in flowComponent.LogMsgs)
            {
                obj = new DataModelConsole() { LogDataSource = item.ShowMsg };
                NodeFiltDetailSources.Add(obj);
            }
            Listbox1.ScrollIntoView(obj);
            var objs = VariablesViewModel.GetFlowVariableTree();
            if (objs != default)
            {
                tv_FlowParam.Items.Clear();
                objs.ForEach(x => tv_FlowParam.Items.Add(x));
            }
            var objsChild = VariablesViewModel.GetChildrenParamTree();
            if (objsChild != default)
            {
                tv_ChildrenFlowParam.Items.Clear();
                objsChild.ForEach(x => tv_ChildrenFlowParam.Items.Add(x));
            }
        }
        public void RefreshLogMsg()
        {
            DataModelConsole obj = default;
            this.Dispatcher.Invoke(() =>
            {
                var flow = Boot.GetComponent<RobotComponent>().GetComponent<FlowComponent>();
                NodeFiltDetailSources.Clear();
                foreach (var item in flow.LogMsgs)
                {
                    obj = new DataModelConsole() { LogDataSource = item.ShowMsg };
                    NodeFiltDetailSources.Add(obj);
                }
                Listbox1.ScrollIntoView(obj);
            });
            
        }
        public void RefreshVariables(string key)
        {
            this.Dispatcher.Invoke(() =>
            {
                var flowComponent = Boot.GetComponent<RobotComponent>().GetComponent<FlowComponent>();
                var objs = VariablesViewModel.GetFlowVariableTree();
                tv_FlowParam.Items.Clear();
                if (objs != default)
                {
                    objs.ForEach(x => tv_FlowParam.Items.Add(x));
                }
                var objsChild = VariablesViewModel.GetChildrenParamTree();
                if (objsChild != default)
                {
                    tv_ChildrenFlowParam.Items.Clear();
                    objsChild.ForEach(x => tv_ChildrenFlowParam.Items.Add(x));
                }
            });
        }
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (CallBack != null)
                CallBack();
            Hide();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void menu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void VarilableMessageShow(object sender, RoutedEventArgs e)
        {
            Listbox1.Visibility = Visibility.Collapsed;
            tv_FlowParam.Visibility = Visibility.Visible;


        }

        private void LogMessageShow(object sender, RoutedEventArgs e)
        {
            Listbox1.Visibility = Visibility.Visible;
            tv_FlowParam.Visibility = Visibility.Collapsed;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            this.WindowState = this.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }


    }
}
