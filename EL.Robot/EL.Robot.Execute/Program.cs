using Automation;
using Automation.Inspect;
using EL;
using EL.Async;
using EL.InstallationService;
using EL.InstallationService.Common;
using EL.Overlay;
using EL.Robot.Component;
using EL.Robot.Core;
using EL.Robot.Core.SqliteEntity;
using EL.Robot.Execute.Common;
using System.Text;
using JsonHelper = EL.JsonHelper;

public partial class Program
{
    [STAThread]
    static async Task Main(string[] args)
    {
        Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        //不是pip桌面调用，直接返回
        if (!PIPHelper.IsPIP())
        {
            return;
        }
        //如果已经存在，则退出
        var mutex = new Mutex(true, nameof(EL.Robot.Execute), out var ret);
        if (!ret)
        {
            return;
        }
        //与服务进行通讯
        var port = SystemServerHelper.GetServerPort();
        if (port < 1)
        {
            return;
        }
        Init();

        bool Wating = true;
        bool Ready = false;
        var RoomName = TaskSchedulerHelper.GetTaskSchedulerPath();
        WebSocketHelper webSocketHelper = new($"ws://127.0.0.1:{port}");
        webSocketHelper.OnOpen = async () =>
        {
            await webSocketHelper.Send(new Message<string>() { room = RoomName, role = RoleType.Client, type = MessageType.Join, data = "PIP登录" }.ToJson());
            Wating = true;
            Console.WriteLine("已连接服务");
            _ = Task.Run(async () =>
            {
                while (Wating)
                {
                    await webSocketHelper.Send(new Message<dynamic>() { type = MessageType.Ping }.ToJson());
                    Thread.Sleep(20 * 1000);
                }
            });
        };
        webSocketHelper.OnReceive = async (data) =>
        {
            var oldData = Encoding.UTF8.GetString(data);
            try
            {
                var message = oldData.ToObj<Message<dynamic>>();
                switch (message.type)
                {
                    case MessageType.Ready:
                        {
                            Ready = true;
                        }
                        break;
                    case MessageType.Stop:
                        {
                            Ready = false;
                        }
                        break;
                    case MessageType.Data:
                        {
                            if (Ready)
                            {
                                var exeJson = oldData.ToObj<Message<string>>();
                                Console.WriteLine($"执行器收到的脚本:{exeJson.data}");
                                var jsData = exeJson.data.ToObj<FlowScript>();
                                await RunScriptAsync(jsData.Id, async (data) =>
                                {
                                    await webSocketHelper.Send(new Message<string>() { type = MessageType.Data, data = data }.ToJson());
                                });
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        };

        webSocketHelper.OnClose = () =>
        {
            Wating = false;
            Console.WriteLine("已断开服务");
        };
        await webSocketHelper.Start();

        //等待连接关闭
        while (Wating)
        {
            SpinWait.SpinUntil(() => false, 1000);
        }
    }
    static async Task RunScriptAsync(string execJson, Action<string> action = null)
    {
        var flow = JsonHelper.FromJson<Flow>(execJson);
        await Boot.GetComponent<RobotComponent>().LocalMain(flow, false);
        action?.Invoke("执行完毕!");
    }
    static async Task RunScriptAsync(long flowID, Action<string> action = null)
    {
        var flow = RobotDataManagerService.GetFlowById(flowID);
        await Boot.GetComponent<RobotComponent>().LocalMain(flow, false);
        action?.Invoke("执行完毕!");
    }

    static void Init()
    {
        Boot.App = new AppMananger();
        var log = new FileLogger();
        log.SetLevel(EL.LogLevel.Trace);
        Boot.SetLog(log);
        Log.Trace($"————Main Start！——--");
        //加载程序集
        Boot.App.EventSystem.Add(typeof(RobotComponent).Assembly);
        Boot.App.EventSystem.Add(typeof(WinFormInspectComponent).Assembly);
        Boot.App.EventSystem.Add(typeof(FormOverLayComponent).Assembly);
        var robot = Boot.AddComponent<RobotComponent>();
        robot.AddComponent<HotkeyComponent>();
        RequestOptionComponent.IsWindow = true;
        SynchronizationContext.SetSynchronizationContext(ThreadSynchronizationContext.Instance);
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
            while (true)
            {
                try
                {
                    Thread.Sleep(1);
                    Boot.Update(); //定时器等待 .流程计划计时对比
                }
                catch (Exception e)
                {
                    Log.Error(e.Message, e);
                }
            }
        });
    }
    static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        LogInfo.Write(e.ExceptionObject.ToString());
    }

}
public static class LogInfo
{
    private static object Lock = new object();
    public static void Write(string data)
    {
        lock (Lock)
        {
            using (StreamWriter sw = new StreamWriter($"log_{DateTime.Now.ToString("yyyyMMdd")}.txt", true))
            {
                sw.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff")} {data}");
            }
        }
    }
}