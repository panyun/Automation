using EL.Robot.Component;
using EL.Robot.Core;
using Robot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EL.Robot.WpfMain
{
    /// <summary>
    /// StepFloatRightWindow.xaml 的交互逻辑
    /// </summary>
    public partial class StepFloatRightWindow : Window
    {
        public StepFloatRightWindow()
        {
            InitializeComponent();
            var primaryScreen = Screen.PrimaryScreen;
            this.Left = primaryScreen.Bounds.Width - this.Width; ;
            this.Top = primaryScreen.Bounds.Height / 3 - this.Height;
            this.Topmost = true;
            Task.Run(() =>
            {
                int totle = 20;
                int needUnit = 10;
                while (true)
                {
                    Thread.Sleep(500);
                    Dispatcher.Invoke(() =>
                    {
                        if (needUnit > totle)
                        {
                            needUnit = 0;
                            MainCanvas.Children.Clear();
                        }
                        DoDraw(totle, needUnit);
                    });
                    needUnit++;
                }
            });
        }
        public void SwitchState()
        {
            var robot = Boot.GetComponent<RobotComponent>();
            //启动或暂停
            if (robot.ExecState == ExecState.None)
            {
                robot.ExecState = ExecState.IsPaused;
            }
            else if (robot.ExecState != ExecState.None)
            {
                Task.Run(() =>
                {
                    robot.ExecState = ExecState.None;
                    if (robot.ELTaskPaused != null && !robot.ELTaskPaused.IsCompleted)
                        robot.ELTaskPaused.SetResult(true);
                });
            }
        }
        /// <summary>
        /// 画进度
        /// </summary>
        /// <param name="totle">总任务数</param>
        /// <param name="needUnit">已完成数量</param>
        //public void DoDraw(int totle = 40, int needUnit = 20)
        //{
        //    MainCanvas.Children.Clear();
        //    double step = 360.0 / totle;
        //    int baseNumber = totle * 3 / 4;
        //    needUnit += baseNumber;
        //    totle += baseNumber;
        //    int length = 2;
        //    double radius = MainCanvas.Width / 2;
        //    for (int i = baseNumber; i < needUnit; i++)
        //    {
        //        var X1 = ((radius - length) * Math.Cos(i * step * Math.PI / 180)) + radius;
        //        var Y1 = ((radius - length) * Math.Sin(i * step * Math.PI / 180)) + radius;
        //        var X2 = (radius * Math.Cos(i * step * Math.PI / 180)) + radius;
        //        var Y2 = (radius * Math.Sin(i * step * Math.PI / 180)) + radius;

        //        Line lineScale = new Line
        //        {
        //            X1 = X1,
        //            Y1 = Y1,
        //            X2 = X2,
        //            Y2 = Y2,
        //            Stroke = (Brush)new BrushConverter().ConvertFromString("#006400"),
        //            //Stroke = (Brush)new BrushConverter().ConvertFromString("#EBEBEB"),
        //            StrokeThickness = 1
        //        };

        //        MainCanvas.Children.Add(lineScale);
        //    }
        //}

        /// <summary>
        /// 画进度
        /// </summary>
        /// <param name="totle">总任务数</param>
        /// <param name="needUnit">已完成数量</param>
        public void DoDraw(int totle = 40, int needUnit = 20)
        {
            MainCanvas.Children.Clear();
            double step = 360.0 / totle;
            int length = 2;
            double radius = MainCanvas.Width / 2;
            for (int i = 0; i < totle; i++)
            {
                var X1 = ((radius - length) * Math.Cos((i * step - 90) * Math.PI / 180)) + radius;
                var Y1 = ((radius - length) * Math.Sin((i * step - 90) * Math.PI / 180)) + radius;
                var X2 = (radius * Math.Cos((i * step - 90) * Math.PI / 180)) + radius;
                var Y2 = (radius * Math.Sin((i * step - 90) * Math.PI / 180)) + radius;

                Line lineScale = new Line
                {
                    X1 = X1,
                    Y1 = Y1,
                    X2 = X2,
                    Y2 = Y2,
                    Stroke = (Brush)new BrushConverter().ConvertFromString("#006400"),
                    StrokeThickness = 1
                };
                if (i > needUnit)
                    lineScale.Stroke = (Brush)new BrushConverter().ConvertFromString("#EBEBEB");
                MainCanvas.Children.Add(lineScale);
            }
        }

        private void Button_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var button = (System.Windows.Controls.Button)sender;
            lable.Content = button.Tag;
            popup.IsOpen = true;
            popup.PlacementTarget = button;
            popup.HorizontalOffset = -(button.Tag.ToString().Length * 12);
            popup.VerticalOffset = -27;
        }

        private void button_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            popup.IsOpen = false;
        }

        public void UpdateStateImg()
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                var robot = Boot.GetComponent<RobotComponent>();
                if (robot.ExecState == ExecState.IsPaused)
                {
                    //换成继续运行图标
                    Recogimg1.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/继续1.png"));
                }
                if (robot.ExecState == ExecState.None)
                {
                    //换成暂停图标
                    Recogimg1.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/暂停.png"));
                }
            }));
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            var robot = Boot.GetComponent<RobotComponent>();
            if (!(robot.ExecState == ExecState.IsPaused || robot.ExecState == ExecState.IsStep))
                return;
            Task.Run(() =>
            {

                if (robot.ELTaskPaused != null && !robot.ELTaskPaused.IsCompleted)
                {
                    robot.ELTaskPaused.SetResult(true);
                    robot.ExecState = ExecState.None;
                }

            });
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var robot = Boot.GetComponent<RobotComponent>();
            var flowComponent = robot.GetComponent<FlowComponent>();
            flowComponent.WriteNodeLog(robot.CurrentNode, $"程序手动暂停！");
            if (!(robot.ExecState == ExecState.IsPaused || robot.ExecState == ExecState.IsStep))
                return;
            Task.Run(() =>
            {
                if (robot.ELTaskPaused != null && !robot.ELTaskPaused.IsCompleted)
                {
                    robot.ExecState = ExecState.IsStep;
                    robot.ELTaskPaused.SetResult(true);
                }
            });
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var robot = Boot.GetComponent<RobotComponent>();
            robot.StopFlow();
        }

        private void button11_Click(object sender, RoutedEventArgs e)
        {
            var button = (System.Windows.Controls.Button)sender;
            var consoleWindow = WindowManager.GetWindow<ConsoleWindow>();
            if (consoleWindow != null && consoleWindow.Visibility == Visibility.Visible)
            {
                WindowManager.Hide<ConsoleWindow>();
                button.Tag = "打开日志";
                return;
            }
            WindowManager.Show<ConsoleWindow>();
            button.Tag = "隐藏日志!";
        }
    }
}
