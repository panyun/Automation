using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ControlzEx.Standard;
using EL;
using EL.Robot.WpfMain.Model;
using Robot.ViewModel;

namespace Robot
{

    /// <summary>
    /// BlueWindow.xaml 的交互逻辑
    /// </summary>
    public partial class BlueWindow : Window
    {

        private BlueViewModel blueViewModel;
        public BlueWindow()
        {
            InitializeComponent();
            /* ChangeHeaderNames headerNames = new ChangeHeaderNames();
             //hidenMenuItem.Name="testtest";*/
            var info = localinfo.Instance;
            blueViewModel = new BlueViewModel();
            this.DataContext = blueViewModel;
            userButton.ContextMenu = null;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            /*new ManageWindow().Show();*/
            WindowManager.Show<LogManageWindow>();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            /*new DispatchWindow() { Owner = this }.Show();*/

            WindowManager.Show<DispatchWindow>();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            WindowManager.Hide<BlueWindow>();
        }

        /// <summary>
        /// 关于我们
        /// </summary>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            WindowManager.Show<AboutWindow>();
        }

        //按钮点击
        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            this.menu.PlacementTarget = this.userButton;
            this.menu.Placement = PlacementMode.Bottom;
            this.menu.IsOpen = true;
        }

        private void guanwang_Click(object sender, RoutedEventArgs e)
        {
            var url = "http://rpaii.com/";
            Process.Start(url);
        }

        private void logout_Click(object sender, RoutedEventArgs e)
        {
            WxLoginWindow.Logout();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            WindowManager.Show<VersionWindow>();
        }
    }
}
