using Automation;
using Automation.Inspect;
using EL;
using EL.Overlay;
using EL.Robot.Core;
using EL.Robot.WpfMain.Model;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Object = System.Object;

namespace Robot
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private static DispatcherOperationCallback exitFrameCallback = new DispatcherOperationCallback(ExitFrame);
        public static void DoEvents()
        {
            DispatcherFrame nestedFrame = new DispatcherFrame();
            DispatcherOperation exitOperation = Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, exitFrameCallback, nestedFrame);
            Dispatcher.PushFrame(nestedFrame);
            if (exitOperation.Status !=
            DispatcherOperationStatus.Completed)
            {
                exitOperation.Abort();
            }
        }

        private static Object ExitFrame(Object state)
        {
            DispatcherFrame frame = state as
            DispatcherFrame;
            frame.Continue = false;
            return null;
        }
        protected override async void OnStartup(StartupEventArgs e)
        {
            DispatcherHelper.dispatcher = Dispatcher;
            Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            EL.Robot.Core.RequestManager.CreateBoot();
            EL.Robot.Core.RequestManager.Init();
            WindowManager.RegisterEvenet(this);
            WindowManager.Show<StartUp>();
            var robot = Boot.GetComponent<RobotComponent>();
            _ = Task.Run(() =>
            {
                while (true)
                {
                    Thread.Sleep(1);
                    robot.Update();
                    //会等待所有方法执行完成才能继续运行，不是异步方法，流程执行队列
                }
            });
            _ = Task.Run(() =>
            {
                Thread.Sleep(3 * 1000);
                if (localinfo.IsDebug())
                {
                    var info = localinfo.Get();
                    WxLoginWindow.Login(info);
                }
            });
        }
        private void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Log.Error(e.Exception);
        }
    }

}

