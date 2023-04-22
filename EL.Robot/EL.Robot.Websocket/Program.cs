// See https://aka.ms/new-console-template for more information
using EL;
using EL.Async;

public partial class Program
{

    [STAThread]
    static async Task Main(string[] args)
    {
        EL.Boot.App = new AppMananger();
        var log = new FileLogger();
        log.SetLevel(LogLevel.Trace);
        Boot.SetLog(log);
        Log.Trace($"————Main Start！——--");
        //VariablesTest.Get();
        //加载程序集
        Boot.App.EventSystem.Add(typeof(RobotComponent).Assembly);
        var inspect = Boot.AddComponent<RobotComponent>();
        var test = inspect.AddComponent<WebSocketClientTestComponent>();
        //var timeComponent = inspect.AddComponent<TimerComponent>();
        // 异步方法全部会回掉到主线程
        SynchronizationContext.SetSynchronizationContext(ThreadSynchronizationContext.Instance);


        //Boot.GetComponent<TimerComponent>().NewOnceTimer(TimeHelper.ServerNow(), async () =>
        //{

        //});
        _ = Task.Run(() =>
        {
            while (true)
            {
                try
                {
                    System.Threading.Thread.Sleep(1);
                    Boot.Update();
                }
                catch (Exception e)
                {
                    Log.Error(e.Message, e);
                }
            }
        });
        string line;
        if (args != null && args.Length > 0)
        {
            await Exec(test, args);
        }
        //Log.Info("CONSOLE CMD [-login] [-e]  [-?]");
        while (true)
        {
            line = await Task.Factory.StartNew(() =>
            {
                var ip = System.Configuration.ConfigurationManager.AppSettings["ip"];
                Log.Info($"CONSOLE CMD [-login] [-e]  [-?] Current Ip [{ip}]");
                return Console.In.ReadLine();
            });
            var lines = line.Split(' ');
            await Exec(test, lines);
        }
    }

    private static async ELTask Exec(WebSocketClientTestComponent test, string[] lines)
    {
        await ELTask.CompletedTask;
        var ip = System.Configuration.ConfigurationManager.AppSettings["ip"];
        switch (lines[0])
        {
            case "-login":
                try
                {
                    int userId = 1;
                    int type = 1;
                    if (lines.Length > 1)
                    {
                        userId = int.Parse(lines[1]);
                    }
                    if (lines.Length > 2)
                    {
                        type = int.Parse(lines[2]);
                    }
                    var t = (R2C_Login)await test.Call(new C2R_Login()
                    {
                        AccountId = userId,
                        ClientId = userId,
                        ClientType = type
                    }, $"ws://{ip}:10001");
                    /*"wss://msgbus.rpaii.com/auth/");*/
                    var enter = await test.Call(new C2G_Enter()
                    {
                        Key = t.Key
                    },  $"ws://{ip}:10002");
                    //"wss://msgbus.rpaii.com/bus/");
                    //RoboatListComponent.CurrentUser = t;
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            while (true)
                            {
                                Thread.Sleep(10);
                                if (test.WebSocket.State == System.Net.WebSockets.WebSocketState.Open || test.WebSocket.State == System.Net.WebSockets.WebSocketState.Connecting)
                                {
                                    var r = await test.Call(new C2G_Ping()
                                    {
                                    });
                                    Log.Info("rec--> {0}", r.RpcId);
                          
                                }
                            }
                        }
                        catch (Exception ex)
                        {

                            Console.WriteLine($"异常:{ex}");
                        }

                        Console.WriteLine("ping  自动退出!");
                    });
                    //_ = Task.Run(async () =>
                    //{
                    //    while (true)
                    //    {
                    //        var buffer = new byte[1024 * 1024];
                    //        var webSocket = test.WebSocket;
                    //        try
                    //        {
                    //            var bufferData = new List<byte>();
                    //            while (webSocket != null && webSocket.State == WebSocketState.Open)
                    //            {
                    //                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    //                if (result.MessageType == WebSocketMessageType.Close)
                    //                {
                    //                    Console.WriteLine($"主动断开连接!");
                    //                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                    //                    break;
                    //                }
                    //                bufferData.AddRange(buffer.AsSpan(0, result.Count).ToArray());
                    //                if (result.EndOfMessage)
                    //                {
                    //                    Console.WriteLine($"获取到的结果:{Encoding.UTF8.GetString(bufferData.ToArray())}");
                    //                    bufferData.Clear();
                    //                }
                    //            }
                    //            Log.Info($"websocket断开连接");
                    //        }
                    //        catch (Exception e)
                    //        {
                    //            Console.WriteLine($"异常:WebsocketState: {webSocket.State} ex： {e?.Message}");
                    //        }
                    //    }
                    //    Console.WriteLine("自动退出!");
                    //});
                }
                catch (Exception)
                {
                    Console.WriteLine("[-login] [-e]  [-?]");
                }
                break;
            case "-query":
                var userIds = string.Join("[\n]", RoboatServerComponent.OnLineClients.Select(x => x.ClientId));
                Console.WriteLine(userIds);

                break;
            case "-run":

                var runRequest = new RunRequest()
                {
                    Flow = default
                };
                //StringBuilder stringBuilder = new StringBuilder();
                //for (int i = 0; i < 1024 * 1024 * 2; i++)
                //{
                //    stringBuilder.Append("dddtews单独的单独的地方地方都是");
                //}
                //var content = OuterOpcode.RunRequest + Utils.JsonHelper.ToJson(runRequest);
                await test.CallTest(new C2G_MsgAgent()
                {
                    Content = "",
                    SelfClientId = RoboatServerComponent.CurrentUser.ClientId,
                    TargetClientId = long.Parse(lines[1])
                }, $"ws://{ip}:10002");
                break;
            case "-catch":

                var catchRequest = new CatchRequest()
                {
                };
                var content = OuterOpcode.RunRequest + Utils.JsonHelper.ToJson(catchRequest);
                var e = await test.Call(new C2G_MsgAgent()
                {
                    Content = content,
                    SelfClientId = 2,
                    TargetClientId = long.Parse("4")
                }, $"ws://{ip}:10002");
                break;
            case "-e":
                System.Environment.Exit(0);
                break;
            case "-?":
                Console.WriteLine("[-login] [-e]");
                break;

            default:
                break;
        }
    }
}


