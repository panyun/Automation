using EL;
using EL.Async;
using EL.Robot.Core.Handler;
using EL.Robot.Core.Request;
using Robot.ViewModel;
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
using System.Windows.Shapes;

namespace Robot
{
    /// <summary>
    /// Manage.xaml 的交互逻辑
    /// </summary>
    public partial class ManageWindow
    {
        ManageViewModel vm = new ManageViewModel();
        public ManageWindow()
        {
            InitializeComponent();

            DataContext = vm;

            vm.WxLogined = false;
            vm.UserHeader = "pack://application:,,,/Resources/Images/robot.png";
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!vm.WxLogined)
            {
                var dlg = new WxLoginWindow() { Owner = this };
                if (dlg.ShowDialog() == true)
                {
                    //vm.UserHeader = dlg.LoginResult.data.headImgUrl;

                    vm.WxLogined = true;
                }
            }
            else
            {
                vm.UserHeader = "pack://application:,,,/Resources/Images/robot.png";
                vm.WxLogined = false;
            }

        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_MaxWindow(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
            else if (this.WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Maximized;
            }
        }

        private void Button_Click_MinWindow(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

    }


}
