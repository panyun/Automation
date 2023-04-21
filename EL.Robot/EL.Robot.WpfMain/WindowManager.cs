using EL;
using EL.Robot.Core;
using EL.Robot.WpfMain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using EL.Robot.Win;
using EL.Input;
using EL.WindowsAPI;
using System.Runtime.InteropServices;
using System.Diagnostics;
using EL.Robot.WpfMain.ViewModel;
using Robot.Controls.Dispatch;
using Automation;

namespace Robot
{
    public class WindowManager
    {
        private static Dictionary<Type, Window> Windows = new Dictionary<Type, Window>();
        private static Dictionary<Type, Form> Forms = new Dictionary<Type, Form>();
        public static T ShowForm<T>() where T : Form, new()
        {
            Forms.TryGetValue(typeof(T), out Form window);
            if (window == null)
            {
                window = new T();
                Forms.Add(typeof(T), window);
            }
            if (window.IsDisposed)
            {
                window = new T();
                Forms.Remove(typeof(T));
                Forms.Add(typeof(T), window);
            }
            window.Show();
            return window as T;
        }
        public static void CloseForm<T>() where T : Form, new()
        {
            Forms.TryGetValue(typeof(T), out Form window);
            if (window == null)
                return;
            Forms.Remove(typeof(T));
            window.Close();
        }
        public static void HideForm<T>() where T : Form, new()
        {
            Forms.TryGetValue(typeof(T), out Form window);
            if (window == null)
                return;
            window.Hide();
        }
        public static T GetForm<T>() where T : Form, new()
        {
            Forms.TryGetValue(typeof(T), out Form window);
            if (window == null)
            {
                return default(T);
            }
            if (window.IsDisposed)
            {
                Forms.Remove(typeof(T));
                return default(T);
            }
            return (T)window;
        }
        public static T Show<T>() where T : Window, new()
        {
            Windows.TryGetValue(typeof(T), out Window window);
            if (window == null)
            {
                window = new T();
                Windows.Add(typeof(T), window);
                window.Show();
            }
            // 已经被释放或者隐藏
            else if (window.Visibility == Visibility.Hidden)
            {
                window.Show();
            }
            //else if (!window.IsLoaded)
            //{
            //    window = new T();
            //    window.Show();
            //}
            window.Activate();
            return window as T;
        }
        public static T Show<T>(object dataContext) where T : Window, new()
        {
            Windows.TryGetValue(typeof(T), out Window window);
            if (window == null)
            {
                window = new T();
                window.DataContext = dataContext;
                Windows.Add(typeof(T), window);
                window.Show();
            }
            // 已经被释放或者隐藏
            else if (window.Visibility == Visibility.Hidden)
            {
                window.Show();
            }
            //else if (!window.IsLoaded)
            //{
            //    window = new T();
            //    window.DataContext = dataContext;
            //    window.Show();
            //}
            window.Activate();
            return window as T;
        }
        public static void Close<T>() where T : Window, new()
        {
            Windows.TryGetValue(typeof(T), out Window window);
            if (window == null)
                return;
            Windows.Remove(typeof(T));
            window.Close();
        }
        public static void Hide<T>() where T : Window, new()
        {
            Windows.TryGetValue(typeof(T), out Window window);
            if (window == null)
                return;
            window.Hide();
        }
        public static T GetWindow<T>() where T : Window, new()
        {
            Windows.TryGetValue(typeof(T), out Window window);
            if (window == null)
                return default;
            return (T)window;
        }
        public static void RegisterEvenet(System.Windows.Application window)
        {
            StartExecRobotAction(window);
            StopExecRobotAction(window);
            NoneAction(window);
            PausedAction(window);
            RefreshLogMsgAction(window);
            RefreshVariablesAction(window);
            EndNodeAction(window);
            StartNodeAction(window);
            StartCatchAction();
            StopCatchAction();
            AddQueueAction();
            RemoveQueueAction();
            FlowUpdateAction();
        }
        public static IntPtr WindowPtr = default;
        public static uint Pid = default;

        public static void StartExecRobotAction(System.Windows.Application window)
        {
            var robotComponent = Boot.GetComponent<RobotComponent>();
            var flowComponent = robotComponent.GetComponent<FlowComponent>();
            robotComponent.StartExecRobotAction = () =>
            {
                var bigRobot = GetWindow<BigRobot>();
                if (bigRobot != null)
                {
                    var msg = flowComponent.MainFlow.IsDebug ? "调试" : "运行";
                    bigRobot.ShowFloat($"开始{msg}脚本\r\n {flowComponent.MainFlow.Name}", 3, 2000);
                    Thread.Sleep(2000);
                }
                DispatcherHelper.ExecDispatcher(() =>
                {
                    //更新菜单
                    DispatchViewModel.Instance?.Update();
                    Running.Instance?.Update();
                    LineUp.Instance?.Update();
                    MinimizeWindow();
                    Hide<BigRobot>();
                    Hide<BlueWindow>();
                    Hide<DispatchWindow>();
                    if (flowComponent.MainFlow.IsDebug)
                    {
                        Show<StepFloatRightWindow>();
                        GetWindow<StepFloatRightWindow>().Topmost = true;
                        //加载网页
                        ShowForm<ChromiumWebBrowserFlow>();
                    }
                    else
                    {
                        Show<FloatRightWindow>();
                        GetWindow<FloatRightWindow>().Topmost = true;
                    }
                    //加载网页
                    GetForm<ChromiumWebBrowserFlow>()?.UpdateFlowAddr(flowComponent.MainFlow.IsDebug);
                });
            };
        }
        public static void StopExecRobotAction(System.Windows.Application window)
        {
            var robotComponent = Boot.GetComponent<RobotComponent>();
            var flowComponent = robotComponent.GetComponent<FlowComponent>();
            robotComponent.StopExecRobotAction = () =>
            {
                DispatcherHelper.ExecDispatcher(() =>
                {
                    //更新菜单
                    DispatchViewModel.Instance?.Update();
                    Running.Instance?.Update();
                    History.Instance?.Update();
                    LineUp.Instance?.Update();
                    Show<BigRobot>();
                    Close<FloatRightWindow>();
                    Close<StepFloatRightWindow>();
                    var flowComponent = robotComponent.GetComponent<FlowComponent>();
                    if (flowComponent.FlowHistory.State == 0)
                    {
                        GetWindow<BigRobot>()?.ShowFloat($"{flowComponent.FlowHistory.Name}脚本 执行完毕！", 2);
                        return;
                    }
                    ShowForm<ChromiumWebBrowserFlow>();
                    GetWindow<BigRobot>()?.ShowFloat($"{flowComponent.FlowHistory.Name}脚本 【{flowComponent.FlowHistory.ExNodeName}】 出现异常！", 1);
                });
            };
        }
        public static void EndNodeAction(System.Windows.Application window)
        {
            var robotComponent = Boot.GetComponent<RobotComponent>();
            var flowComponent = robotComponent.GetComponent<FlowComponent>();
            var nodeComponent = flowComponent.GetComponent<NodeComponent>();
            nodeComponent.EndNodeAction = (data) =>
            {
                DispatcherHelper.ExecDispatcher(() =>
                {
                    var index = flowComponent.Steps.Keys.ToList().IndexOf(data.Node.Id);
                    GetWindow<FloatRightWindow>()?.DoDraw(flowComponent.Steps.Count - 1, index);
                    GetForm<ChromiumWebBrowserFlow>()?.UpdateFlowInfo(data.Node.Id.ToString(),
               data.IsSucess ? 2 : 3, data.Msg);
                });
            };
        }

        public static void NoneAction(System.Windows.Application window)
        {
            var robotComponent = Boot.GetComponent<RobotComponent>();
            var flowComponent = robotComponent.GetComponent<FlowComponent>();
            robotComponent.NoneAction = () =>
            {
                DispatcherHelper.ExecDispatcher(() =>
                {
                    if (flowComponent.MainFlow.IsDebug)
                    {
                        HideForm<ChromiumWebBrowserFlow>();
                        Hide<ConsoleWindow>();
                    }
                    GetWindow<FloatRightWindow>()?.UpdateStateImg();
                });
            };
        }
        public static void PausedAction(System.Windows.Application window)
        {
            var robotComponent = Boot.GetComponent<RobotComponent>();
            var flowComponent = robotComponent.GetComponent<FlowComponent>();
            robotComponent.PausedAction = () =>
            {
                DispatcherHelper.ExecDispatcher(() =>
                {
                    if (flowComponent.MainFlow.IsDebug)
                    {
                        ShowForm<ChromiumWebBrowserFlow>();
                        //Show<ConsoleWindow>();
                        GetWindow<FloatRightWindow>()?.UpdateStateImg();
                    }
                });
            };
        }
        public static void RefreshLogMsgAction(System.Windows.Application window)
        {
            var robotComponent = Boot.GetComponent<RobotComponent>();
            var flowComponent = robotComponent.GetComponent<FlowComponent>();
            flowComponent.RefreshLogMsgAction = (x) =>
            {
                GetWindow<ConsoleWindow>()?.RefreshLogMsg();
            };
        }
        public static void RefreshVariablesAction(System.Windows.Application window)
        {
            var robotComponent = Boot.GetComponent<RobotComponent>();
            var flowComponent = robotComponent.GetComponent<FlowComponent>();
            flowComponent.RefreshVariablesAction = (key) =>
            {
                GetWindow<ConsoleWindow>()?.RefreshVariables(key);
            };
        }
        public static void StartNodeAction(System.Windows.Application window)
        {
            var robot = EL.Boot.GetComponent<RobotComponent>();
            var flowComponent = robot.GetComponent<FlowComponent>();
            var nodeComponent = flowComponent.GetComponent<NodeComponent>();
            nodeComponent.StartNodeAction = (node) =>
            {
                //加载网页
                GetForm<ChromiumWebBrowserFlow>()?.UpdateFlowInfo(node.Id.ToString(), 1, "");
            };
        }

        public static void StartCatchAction()
        {
            var robot = EL.Boot.GetComponent<RobotComponent>();
            robot.StartCatchAction = MinimizeWindow;
        }
        public static void AddQueueAction()
        {
            var concurrentQueueComponent = EL.Boot.GetComponent<RobotComponent>().GetComponent<ConcurrentQueueComponent>();
            concurrentQueueComponent.AddQueueAction = () =>
            {
                DispatcherHelper.ExecDispatcher(() =>
                {
                    //更新菜单
                    DispatchViewModel.Instance?.Update();
                    //刷新队列数据
                    LineUp.Instance?.Update();
                });

            };
        }
        public static void RemoveQueueAction()
        {
            var concurrentQueueComponent = EL.Boot.GetComponent<RobotComponent>().GetComponent<ConcurrentQueueComponent>();
            concurrentQueueComponent.RemoveQueueAction = () =>
            {
                DispatcherHelper.ExecDispatcher(() =>
                {
                    //更新菜单
                    DispatchViewModel.Instance?.Update();
                    //刷新队列数据
                    LineUp.Instance?.Update();
                });

            };
        }
        public static void StopCatchAction()
        {
            var robot = EL.Boot.GetComponent<RobotComponent>();
            robot.StopCatchAction = MaximizedWindow;
        }
        public static void FlowUpdateAction()
        {
            var robot = EL.Boot.GetComponent<RobotComponent>();
            robot.FlowUpdateAction = () =>
            {
                DispatcherHelper.ExecDispatcher(() =>
                {
                    DispatchViewModel.Instance?.Update();
                    MyScript.Instance?.Update();
                });
            };
        }
        /// <summary>
        /// 最小化设计器窗口
        /// </summary>
        public static void MinimizeWindow()
        {
            WindowPtr = User32.GetForegroundWindow();
            var robot = Boot.GetComponent<RobotComponent>();
            try
            {
                if (robot.IsSelfMachine)
                {
                    var isIconic = User32.IsIconic(WindowPtr);
                    if (!isIconic)
                        User32.ShowWindow(WindowPtr, ShowWindowTypes.SW_MINIMIZE);
                    User32.GetWindowThreadProcessId(WindowPtr, out uint pid);
                    Pid = pid;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }
        /// <summary>
        /// 最大化设计器窗口
        /// </summary>
        public static void MaximizedWindow()
        {
            var robotComponent = Boot.GetComponent<RobotComponent>();
            try
            {
                if (robotComponent.IsSelfMachine)
                {
                    var isIconic = User32.IsIconic(WindowManager.WindowPtr);
                    if (isIconic)
                        User32.ShowWindow(WindowManager.WindowPtr, ShowWindowTypes.SW_MAXIMIZE);
                    int index = 0;
                    while (index < 50)
                    {
                        var process = Process.GetProcessById((int)WindowManager.Pid);
                        User32.SetForegroundWindow(process.MainWindowHandle);
                        if (process.MainWindowTitle.Contains(robotComponent.ClientNo))
                            break;
                        index++;
                        VirtualKeyShort[] virtualKeyShorts = new VirtualKeyShort[] { VirtualKeyShort.CONTROL, VirtualKeyShort.TAB };
                        EL.Input.Keyboard.TypeSimultaneously(virtualKeyShorts);
                        Thread.Sleep(100);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

    }
}
