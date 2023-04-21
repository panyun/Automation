using EL;
using EL.Async;
using EL.Robot.Core;
using EL.Robot.Core.Handler;
using EL.Robot.Core.Request;
using EL.Robot.Core.SqliteEntity;
using EL.Video;
using Robot.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
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
    /// LogManageWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LogManageWindow
    {
        public LogManageWindow()
        {
            InitializeComponent();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            WindowManager.Close<LogManageWindow>();
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
       
        public static string fileName = string.Empty;
        //开始录制
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn.Content + "" == "开始录制")
            {
                var robot = Boot.GetComponent<RobotComponent>();
                fileName = System.IO.Path.Combine(ConfigItemsHelper.Log_BackupDirectory, IdGenerater.Instance.GenerateId() + DateTime.Now.ToString("yyyyMMdd_HHmmssFFF"));
                robot.StartVideo(fileName + ".avi");
                var audio = robot.GetComponent<AudioRecorderComponent>();
                audio.Start(fileName + ".wav");
                btn.Content = "停止录制";
            }
            else
            {
                var robot = Boot.GetComponent<RobotComponent>();
                robot.StopVideo();
                var audio = robot.GetComponent<AudioRecorderComponent>();
                audio?.Stop();
                btn.Content = "开始录制";
                Task.Factory.StartNew(async () =>
                {
                    await Task.Delay(1000);
                    var video = robot.GetComponent<VideoRecorderComponent>();
                    video.CombineAudio(fileName + ".wav", fileName + ".avi", fileName + ".mp4");
                });
            }

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var filename = fileName + ".mp4";
            if (File.Exists(filename))
            {
                Process.Start(fileName + ".mp4");
            }
        }
    }
}
