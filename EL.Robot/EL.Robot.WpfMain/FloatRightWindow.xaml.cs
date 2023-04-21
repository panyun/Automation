using EL;
using EL.Hook;
using EL.Overlay;
using EL.Robot.Component;
using EL.Robot.Core;
using Robot.Utils;
using Robot.ViewModel;
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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Robot
{
    /// <summary>
    /// BigRobot.xaml 的交互逻辑
    /// </summary>
    public partial class FloatRightWindow : Window
    {

        public FloatRightWindow()
        {
            InitializeComponent();
            this.Topmost = true;
            var primaryScreen = Screen.PrimaryScreen;
            this.Left = primaryScreen.Bounds.Width - this.Width; ;
            this.Top = primaryScreen.Bounds.Height / 2 - this.Height;
            //KeyboardHookTemp keyboardHook = new KeyboardHookTemp();
            //keyboardHook.KeyUpEvent += (x, y) =>
            //{
            //    var data = y.KeyData;
            //    if (y.KeyData == (Keys.Control | Keys.Shift | Keys.Q))
            //        SwitchState();
            //    else if (y.KeyData == (Keys.Control | Keys.Shift | Keys.C))
            //    {
            //        var robot = Boot.GetComponent<RobotComponent>();
            //        robot.StopFlow();
            //    }
            //    return;
            //};
            //keyboardHook.Start();


            //Task.Run(() =>
            //{
            //    int totle = 50;
            //    int needUnit = 0;
            //    while (true)
            //    {
            //        Thread.Sleep(500);
            //        Dispatcher.Invoke(() =>
            //        {
            //            if (needUnit > totle)
            //            {
            //                needUnit = 0;
            //                MainCanvas.Children.Clear();
            //            }
            //            DoDraw(totle, needUnit);
            //        });
            //        needUnit++;
            //    }
            //});
            //var robot = Boot.GetComponent<RobotComponent>();
            //var hotkey = robot.AddComponent<HotkeyComponent>();
            //hotkey.Regist(this, HotkeyModifiers.MOD_CONTROL& HotkeyModifiers.MOD_SHIFT )
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

        public void UpdateStateImg()
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                var robot = Boot.GetComponent<RobotComponent>();
                if (robot.ExecState == ExecState.IsPaused)
                {
                    //换成继续运行图标
                    //btn_Start.Background = 
                    Recogimg1.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/继续.png"));
                }
                if (robot.ExecState == ExecState.None)
                {
                    //换成暂停图标
                    Recogimg1.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/暂停.png"));
                }
            }));
        }
        /// <summary>
        /// 画进度
        /// </summary>
        /// <param name="totle">总任务数</param>
        /// <param name="needUnit">已完成数量</param>
        public void DoDraw(int totle = 60, int needUnit = 20)
        {
            MainCanvas.Children.Clear();
            double step = 360.0 / totle;
            int baseNumber = totle * 3 / 4;
            needUnit += baseNumber;
            totle += baseNumber;
            int length = 2;
            double radius = MainCanvas.Width / 2;

            for (int i = baseNumber; i < needUnit; i++)
            {
                var X1 = ((radius - length) * Math.Cos(i * step * Math.PI / 180)) + radius;
                var Y1 = ((radius - length) * Math.Sin(i * step * Math.PI / 180)) + radius;
                var X2 = (radius * Math.Cos(i * step * Math.PI / 180)) + radius;
                var Y2 = (radius * Math.Sin(i * step * Math.PI / 180)) + radius;

                Line lineScale = new Line
                {
                    X1 = X1,
                    Y1 = Y1,
                    X2 = X2,
                    Y2 = Y2,
                    Stroke = (Brush)new BrushConverter().ConvertFromString("#006400"),
                    StrokeThickness = 5
                };

                MainCanvas.Children.Add(lineScale);
            }
        }
        private void Run_Click(object sender, RoutedEventArgs e)
        {
            SwitchState();
        }

        private void Right_Click(object sender, RoutedEventArgs e)
        {
            var robot = Boot.GetComponent<RobotComponent>();
            robot.StopFlow();
        }


    }


}
