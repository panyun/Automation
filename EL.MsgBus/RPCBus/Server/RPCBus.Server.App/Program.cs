using CommandLine;
using ET;
using NLog;
using RPCBus.Server.EventType;
using System.Reflection;

namespace RPCBus.Server.App
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            // 异步方法全部会回掉到主线程
            SynchronizationContext.SetSynchronizationContext(ThreadSynchronizationContext.Instance);
            try
            {
                //Game.EventSystem.Add(typeof(Game).Assembly);
                //Game.EventSystem.Add(DllHelper.GetHotfixAssembly());

                //ProtobufHelper.Init();
                //MongoHelper.Init();

                // 命令行参数
                Options options = null;
                Parser.Default.ParseArguments<Options>(args)
                        .WithNotParsed(error => throw new Exception($"命令行格式错误!"))
                        .WithParsed(o => { options = o; });

                Game.Options = options;

                // 初始化日志模块
                {
                    switch (options.Develop)
                    {
                        case 1:
                            {
                                Game.ILog = new DevLogger(options.AppType.ToString());
                                break;
                            }
                        default:
                            {
                                Game.ILog = new NLogger(options.AppType.ToString());
                                break;
                            }
                    }
                    LogManager.Configuration.Variables["appIdFormat"] = $"{options.Process:000000}";
                }
                //Game.ILog = new NLogger(Game.Options.AppType.ToString());
                LogManager.Configuration.Variables["appIdFormat"] = $"{Game.Options.Process:000000}";
                Log.Info($"[APP]: CommandLine");
                Log.Info($"  ┌ AppType = {options.AppType}");
                Log.Info($"  ├ Process = {options.Process}");
                Log.Info($"  ├ Develop = {options.Develop}");
                Log.Info($"  ├ LogLevel = {options.LogLevel}");
                Log.Info($"  ├ Console = {options.Console}");
                Log.Info($"  └ CreateScenes = {options.CreateScenes}");
                Log.Info("");
                // 载入相关的程序集（Model，Hotfix）；
                {
                    Log.Info($"[APP]: Assemblies");

                    Assembly[] assemblies = DllHelper.ListAssemblyInterests();
                    foreach (Assembly assembly in assemblies)
                    {
                        Log.Info($"  [{assembly.GetName().Name}.dll]");
                        Game.EventSystem.Add(assembly);
                    }

                    Log.Info("");
                }
                Game.EventSystem.Publish(new AppStart());

               while (true)
                {
                    try
                    {
                        Thread.Sleep(1);
                        Game.Update();
                        Game.LateUpdate();
                        Game.FrameFinish();
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

    }
}
