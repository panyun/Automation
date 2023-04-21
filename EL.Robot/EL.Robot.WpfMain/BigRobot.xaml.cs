using ControlzEx.Standard;
using EL;
using EL.Robot.Component.PIP;
using EL.Robot.Core;
using EL.Robot.Win;
using EL.Robot.WpfMain.Model;
using EL.WindowsAPI;
using Robot.ViewModel;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Input;

namespace Robot
{
    /// <summary>
    /// BigRobot.xaml 的交互逻辑
    /// </summary>
    public partial class BigRobot : Window
    {
        BigRobotViewModel _vm;
        Screen primaryScreen = Screen.PrimaryScreen;
        public BigRobot()
        {
            InitializeComponent();
            Left = 100;
            Top = 100;
            Loaded += BigRobot_Loaded;
        }

        private void BigRobot_Loaded(object sender, RoutedEventArgs e)
        {
            _vm = DataContext as BigRobotViewModel;
            _vm.Percent = 76;

            //ShowFloat();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void Run_Click(object sender, RoutedEventArgs e)
        {
            //启动或暂停
        }

        private void Right_Click(object sender, RoutedEventArgs e)
        {
            //浮动到右边
            /*new FloatRightWindow().Show();
            this.Hide();*/
            WindowManager.Hide<FloatRightWindow>();
        }

        /*/// <summary>
        /// 展示信息
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="error"></param>
        public void ShowMessage(string msg, bool error = false)
        {
            var toast = new ToastOptions()
            {
                Icon = ToastIcons.Information,
                ToastMargin = new Thickness(0, 120, 0, 0),
                Background = (System.Windows.Media.Brush)new BrushConverter().ConvertFromString("#43A047"),
                Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFromString("#FFFFFF"),
                IconForeground = (System.Windows.Media.Brush)new BrushConverter().ConvertFromString("#FFFFFF"),
                FontSize = 16,
                IconSize = 16,
                Location = ToastLocation.ScreenTopCenter,
                Time = 3000
            };
            if (error)
            {
                toast.Icon = ToastIcons.Warning;
                toast.Background = (System.Windows.Media.Brush)new BrushConverter().ConvertFromString("#FF5722");
            }
            Toast.Show(msg, toast);
        }
*/

        /// <summary>
        /// 显示浮动框
        /// </summary>
        public void ShowFloat(string msg, int type, int time = 10000)
        {
            this.Dispatcher.Invoke((Delegate)(() =>
            {
                ///<summary>
                /// 对接Actio判定气泡框是否显示
                /// </summary>
                _vm.ShowMsg = true;
                Task.Run(() =>
                {
                    Thread.Sleep(time);
                    Dispatcher.Invoke(() =>
                    {
                        _vm.ShowMsg = false;
                    });
                });
                btn_Msg.Visibility = Visibility.Visible;
                if (type == 1)
                {
                    _vm.MsgButtonTxt = "定位";
                    _vm.MsgTitle = "异常信息";
                    _vm.MsgContent = msg;
                    _vm.MsgType = 1;
                    _vm.ChangeImg(false);
                    ShowWindow();
                }
                else if (type == 2)
                {
                    _vm.MsgType = 0;
                    _vm.MsgButtonTxt = "确定";
                    _vm.MsgTitle = "提示信息";
                    _vm.MsgContent = msg;
                    _vm.MsgType = 0;
                    _vm.ChangeImg(true);
                    ShowWindow();
                }
                else if (type == 3)
                {
                    btn_Msg.Visibility = Visibility.Hidden;
                    _vm.MsgType = 0;
                    _vm.MsgButtonTxt = "";
                    _vm.MsgTitle = "提示信息";
                    _vm.MsgContent = msg;
                    _vm.MsgType = 0;
                    _vm.ChangeImg(true);
                    ShowWindow();
                }
            }));
        }

        private void HideFloat_Click(object sender, RoutedEventArgs e)
        {
            _vm.ShowMsg = false;
            _vm.ChangeImg(true);
            WindowManager.MaximizedWindow();
            //todo :刷新图
        }

        private void bigImg_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                /*new BlueWindow().Show();*/
                WindowManager.Show<BlueWindow>();

            }
        }

        private void Grid_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            bool isMouseOver = this.IsMouseOver;
            if (isMouseOver || (Mouse.DirectlyOver != null && ((Mouse.DirectlyOver as FrameworkElement)?.Name == "taskMenu1") || (Mouse.DirectlyOver as FrameworkElement)?.Name == "ContextMenuBorder"))
                return;
            if (Left + Width > primaryScreen.Bounds.Width - 10)
            {
                Task.Run(() =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        while (Left < primaryScreen.Bounds.Width - 23)
                        {
                            Left += 5;
                            Thread.Sleep(10);
                        }
                        IconBorder.Visibility = Visibility.Visible;
                    });
                });
            }
            else if (Left < 5)
            {
                Task.Run(() =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        while (Left > -ActualWidth + 23)
                        {
                            Left -= 5;
                            Thread.Sleep(10);
                        }
                        IconBorderLeft.Visibility = Visibility.Visible;
                    });
                });
            }
            else if (Top < 5)
            {
                Task.Run(() =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        while (Top > (-ActualHeight + 43))
                        {
                            Top -= 5;
                            Thread.Sleep(10);
                        }
                        IconBorderTop.Visibility = Visibility.Visible;
                    });
                });
            }
        }

        void ShowWindow()
        {
            PopupLable.Placement = PlacementMode.Top;
            PopupLable.HorizontalOffset = -23;
            if (IconBorder.Visibility == Visibility.Visible)
            {
                IconBorder_MouseEnter(null, null);
            }
            else if (IconBorderLeft.Visibility == Visibility.Visible)
            {
                PopupLable.HorizontalOffset = 0;
                IconBorderLeft_MouseEnter(null, null);
            }
            else if (IconBorderTop.Visibility == Visibility.Visible)
            {
                PopupLable.Placement = PlacementMode.Right;
                PopupLable.HorizontalOffset = 0;
                IconBorderTop_MouseEnter(null, null);
            }
        }
        private void IconBorder_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (IconBorder.Visibility == Visibility.Visible)
            {
                IconBorder.Visibility = Visibility.Collapsed;
            }
            Task.Run(() =>
            {
                Thread.Sleep(200);
                Dispatcher.Invoke(() =>
                {
                    while (Left + ActualWidth > primaryScreen.Bounds.Width + 23)
                    {
                        Left -= 5;
                        Thread.Sleep(5);
                    }
                });
            });
        }

        private void IconBorderTop_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (IconBorderTop.Visibility == Visibility.Visible)
            {
                IconBorderTop.Visibility = Visibility.Collapsed;
            }
            Task.Run(() =>
            {
                Thread.Sleep(200);
                Dispatcher.Invoke(() =>
                {
                    while (Top < 0)
                    {
                        Top += 5;
                        Thread.Sleep(5);
                    }
                });
            });
        }

        private void IconBorderLeft_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (IconBorderLeft.Visibility == Visibility.Visible)
            {
                IconBorderLeft.Visibility = Visibility.Collapsed;
            }
            Task.Run(() =>
            {
                Thread.Sleep(200);
                Dispatcher.Invoke(() =>
                {
                    while (Left < -23)
                    {
                        Left += 5;
                        Thread.Sleep(10);
                    }
                });
            });
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.MenuItem menuItem = sender as System.Windows.Controls.MenuItem;
            if (menuItem?.Header.ToString() == "登录")
            {
                WindowManager.Show<WxLoginWindow>();
            }
            else if (menuItem?.Header.ToString().StartsWith("注销") == true)
            {
                WxLoginWindow.Logout();
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var contextMenu = FindResource("taskMenu") as System.Windows.Controls.ContextMenu;
            var contextMenu1 = FindResource("taskMenu1") as System.Windows.Controls.ContextMenu;

            var menu = sender as System.Windows.Controls.MenuItem;
            var menu1 = contextMenu.Items[1] as System.Windows.Controls.MenuItem;
            var menu2 = contextMenu1.Items[1] as System.Windows.Controls.MenuItem;

            if (menu.Header.ToString() == "显示流程图")
            {
                menu1.Header = "隐藏流程图";
                menu2.Header = "隐藏流程图";
                var win = WindowManager.ShowForm<ChromiumWebBrowserFlow>();
                win.ShowFlowCallBack = () =>
                {
                    menu1.Header = "显示流程图";
                    menu2.Header = "显示流程图";
                };
            }
            else
            {
                menu1.Header = "显示流程图";
                menu2.Header = "显示流程图";
                WindowManager.CloseForm<ChromiumWebBrowserFlow>();
            }
        }
        private void MenuItem_Click3(object sender, RoutedEventArgs e)
        {
            var rtn = System.Windows.Forms.MessageBox.Show($"您确定要退出机器人吗？", "退出机器人", System.Windows.Forms.MessageBoxButtons.OKCancel, System.Windows.Forms.MessageBoxIcon.Information, System.Windows.Forms.MessageBoxDefaultButton.Button1, System.Windows.Forms.MessageBoxOptions.DefaultDesktopOnly);
            if (rtn == System.Windows.Forms.DialogResult.OK)
            {
                WindowManager.Close<BigRobot>();
                System.Windows.Application.Current.Shutdown();
            }
        }

        private void MenuItem_Click_Show_Console(object sender, RoutedEventArgs e)
        {
            var contextMenu = FindResource("taskMenu") as System.Windows.Controls.ContextMenu;
            var contextMenu1 = FindResource("taskMenu1") as System.Windows.Controls.ContextMenu;

            var menu = sender as System.Windows.Controls.MenuItem;
            var menu1 = contextMenu.Items[0] as System.Windows.Controls.MenuItem;
            var menu2 = contextMenu1.Items[0] as System.Windows.Controls.MenuItem;

            if (menu.Header.ToString() == "显示控制台")
            {
                var consoleWindow = WindowManager.Show<ConsoleWindow>();
                consoleWindow.CallBack = () =>
                {
                    menu1.Header = "显示控制台";
                    menu2.Header = "显示控制台";
                };
                menu1.Header = "隐藏控制台";
                menu2.Header = "隐藏控制台";
            }
            else
            {
                menu1.Header = "显示控制台";
                menu2.Header = "显示控制台";
                WindowManager.Hide<ConsoleWindow>();

            }
        }

        private void taskMenu1_Closed(object sender, RoutedEventArgs e)
        {
            Grid_MouseLeave(null, null);
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(localinfo.Instance?.headImgUrl))
            {
                WindowManager.Show<BlueWindow>();
            }
        }
        //打开画中画
        private  void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(localinfo.Instance?.headImgUrl))
            {
                return;
            }
            Task.Run(async () => {
                var pip = Boot.GetComponent<RobotComponent>().GetComponent<PIPServerComponent>();
                var result = await pip.StartAsync();
                if (!result.Item1)
                {
                    System.Windows.MessageBox.Show(result.Item2);
                }
            });
        }
    }
}
