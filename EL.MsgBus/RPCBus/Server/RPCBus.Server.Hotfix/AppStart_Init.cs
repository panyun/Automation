using ET;
using RPCBus.Server.EventType;
using System.Net;

namespace RPCBus.Server.Hotfix
{
    public class AppStart_Init : AEvent<AppStart>
    {
        protected override async ETTask Run(AppStart args)
        {
            switch (Game.Options.AppType)
            {
                case AppType.ExcelExporter:
                    {
                        Game.Options.Console = 1;
                        ExcelExporter.Export();
                        return;
                    }
                case AppType.Proto2CS:
                    {
                        Game.Options.Console = 1;
                        Proto2CS.Export();
                        return;
                    }
            }

            Game.Scene.AddComponent<ConfigComponent>();
            await ConfigComponent.Instance.LoadAsync();
            {
                ClientAddressHelper.Init();
            }

            StartProcessConfig processConfig = StartProcessConfigCategory.Instance.Get(Game.Options.Process);

            Game.Scene.AddComponent<TimerComponent>();
            Game.Scene.AddComponent<OpcodeTypeComponent>();
            Game.Scene.AddComponent<MessageDispatcherComponent>();
            Game.Scene.AddComponent<SessionStreamDispatcher>();
            Game.Scene.AddComponent<CoroutineLockComponent>();
            // 发送普通actor消息
            Game.Scene.AddComponent<ActorMessageSenderComponent>();
            // 发送location actor消息
            Game.Scene.AddComponent<ActorLocationSenderComponent>();
            // 访问location server的组件
            Game.Scene.AddComponent<LocationProxyComponent>();
            Game.Scene.AddComponent<ActorMessageDispatcherComponent>();
            // 数值订阅组件
            Game.Scene.AddComponent<NumericWatcherComponent>();

            Game.Scene.AddComponent<NetThreadComponent>();

            Dumper.StartMachineConfig();
            Dumper.StartProcessConfig();
            Dumper.StartZoneConfig();
            Dumper.StartSceneConfig();
            Dumper.Configuration();
            Dumper.MessageDispatcher();
            Dumper.MessageHandler();
            Dumper.ActorMessageHandler();
            Dumper.Scenes();
            switch (Game.Options.AppType)
            {
                case AppType.Server:
                    {
                        Game.Scene.AddComponent<NetInnerComponent, IPEndPoint, int>(processConfig.InnerIPPort, SessionStreamDispatcherType.SessionStreamDispatcherServerInner);

                        var processScenes = StartSceneConfigCategory.Instance.GetByProcess(Game.Options.Process);
                        foreach (StartSceneConfig startConfig in processScenes)
                        {
                            await SceneFactory.Create(Game.Scene, startConfig.Id, startConfig.InstanceId, startConfig.Zone, startConfig.Name,
                                startConfig.Type, startConfig);
                        }

                        break;
                    }
                case AppType.Watcher:
                    {
                        StartMachineConfig startMachineConfig = WatcherHelper.GetThisMachineConfig();
                        WatcherComponent watcherComponent = Game.Scene.AddComponent<WatcherComponent>();
                        watcherComponent.Start(Game.Options.CreateScenes);
                        Game.Scene.AddComponent<NetInnerComponent, IPEndPoint, int>(NetworkHelper.ToIPEndPoint($"{startMachineConfig.InnerIP}:{startMachineConfig.WatcherPort}"), SessionStreamDispatcherType.SessionStreamDispatcherServerInner);
                        break;
                    }
                case AppType.GameTool:
                    break;
            }

            if (Game.Options.Console == 1)
            {
                Game.Scene.AddComponent<ConsoleComponent>();
            }
        }
    }
}
