using System.Net;
using ET;

namespace RPCBus.Server
{
    public static class SceneFactory
    {
        public static async ETTask<Scene> Create(
            Entity parent,
            int id,
            long instanceId,
            int zone,
            string name,
            SceneType sceneType,
            StartSceneConfig startSceneConfig = null)
        {
            await ETTask.CompletedTask;

            Scene scene = EntitySceneFactory.CreateScene(id, instanceId, zone, sceneType, name, parent);

            // 每个 Scene 都时一个 Actor 对象
            scene.AddComponent<MailBoxComponent, MailboxType>(MailboxType.UnOrderMessageDispatcher);

            var zoneConfig = StartZoneConfigCategory.Instance.Get(zone);
            switch (scene.SceneType)
            {
                case SceneType.Realm:
                    //scene.AddComponent<NetKcpComponent, IPEndPoint, ISessionStreamDispatcher>(startSceneConfig.OuterIPPort);
                    //scene.AddComponent<NetKcpComponent, IPEndPoint, ISessionStreamDispatcher>(startSceneConfig.OuterIPPort, new SessionStreamDispatcherServerOuter());
                    scene.AddComponent<NetKcpComponent, IPEndPoint, int>(startSceneConfig.OuterIPPort, SessionStreamDispatcherType.SessionStreamDispatcherServerOuter);
                    scene.AddComponent<DBComponent, string, string>(zoneConfig.DBConnection, zoneConfig.DBName);

                    //scene.AddComponent<NetKcpComponent, IPEndPoint>(startSceneConfig.OuterIPPort);
                    break;
                case SceneType.Gate:
                    scene.AddComponent<NetKcpComponent, IPEndPoint, int>(startSceneConfig.OuterIPPort, SessionStreamDispatcherType.SessionStreamDispatcherServerOuter);
                    //scene.AddComponent<NetKcpComponent, IPEndPoint, ISessionStreamDispatcher>(startSceneConfig.OuterIPPort, new SessionStreamDispatcherServerOuter());
                    //scene.AddComponent<NetKcpComponent, IPEndPoint>(startSceneConfig.OuterIPPort);
                    scene.AddComponent<DBComponent, string, string>(zoneConfig.DBConnection, zoneConfig.DBName);
                    scene.AddComponent<Gate.AgentComponent>();
                    scene.AddComponent<Model.P2P.PeerRoomComponent>();
                    scene.AddComponent<Gate.SessionKeyComponent>();
                    break;
                case SceneType.Client:
                    scene.AddComponent<DBComponent, string, string>(zoneConfig.DBConnection, zoneConfig.DBName);
                    scene.AddComponent<Client.PlayerComponent>();
                    scene.AddComponent<Client.PlayerActorComponent>();
                    //scene.AddComponent<Client.NotificationSenderComponent>();
                    break;
            }
            return scene;
        }
    }
}
