using ET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPCBus.Server.Hotfix
{
    class Dumper
    {
        public static void Scenes()
        {
            var processScenes = StartSceneConfigCategory.Instance.GetByProcess(Game.Options.Process);
            Log.Info($"[SCENE]: 载入 {processScenes.Count} 个场景!");

            foreach (StartSceneConfig startConfig in processScenes)
            {
                Log.Info($"  [Id={startConfig.Id}, Type={startConfig.Type}, Name={startConfig.Name}, ActorId={startConfig.InstanceId}, ZoneId={startConfig.Zone}]");
            }

            Log.Info("");

        }

        public static void StartMachineConfig()
        {
            StartProcessConfig processConfig = StartProcessConfigCategory.Instance.Get(Game.Options.Process);
            StartMachineConfig machineConfig = StartMachineConfigCategory.Instance.Get(processConfig.MachineId);

            Log.Info("[MACHINE]: StartMachineConfig");
            Log.Info($"  ┌ MachineId = {machineConfig.Id}");
            Log.Info($"  ├ InnerIP = {machineConfig.InnerIP}");
            Log.Info($"  ├ OuterIP = {machineConfig.OuterIP}");
            Log.Info($"  ├ ClientIP = {machineConfig.ClientIP}");
            Log.Info($"  └ WatcherPort = {machineConfig.WatcherPort}");
            Log.Info("");
        }

        public static void StartProcessConfig()
        {
            StartProcessConfig processConfig = StartProcessConfigCategory.Instance.Get(Game.Options.Process);

            Log.Info("[PROCESS]: StartProcessConfig");
            Log.Info($"  ┌ ProcessId = {processConfig.Id}");
            Log.Info($"  ├ AppName = {processConfig.AppName}");
            Log.Info($"  ├ MachineId = {processConfig.MachineId}");
            Log.Info($"  ├ SceneId = {processConfig.SceneId}");
            Log.Info($"  └ InnerAddr = {processConfig.InnerIPPort}");
            Log.Info("");
        }

        public static void StartZoneConfig()
        {
            StartProcessConfig processConfig = StartProcessConfigCategory.Instance.Get(Game.Options.Process);
            Log.Info("[ZONE]: StartZoneConfig");

            Dictionary<int, bool> zones = new Dictionary<int, bool>();
            List<StartSceneConfig> list = StartSceneConfigCategory.Instance.GetByProcess(processConfig.MachineId);
            foreach (StartSceneConfig sceneConfig in list)
            {
                if (!zones.ContainsKey(sceneConfig.Zone))
                {
                    StartZoneConfig zoneConfig = StartZoneConfigCategory.Instance.Get(sceneConfig.Zone);
                    Log.Info($"  ┌ ZoneID = {zoneConfig.Id}");
                    Log.Info($"  ├ DbConnection = {zoneConfig.DBConnection}");
                    Log.Info($"  └ DBName = {zoneConfig.DBName}");
                    zones.Add(sceneConfig.Zone, true);
                }
            }

            Log.Info("");
        }

        public static void StartSceneConfig()
        {
            StartProcessConfig processConfig = StartProcessConfigCategory.Instance.Get(Game.Options.Process);
            Log.Info("[SCENE]: StartSceneConfig");

            List<StartSceneConfig> list = StartSceneConfigCategory.Instance.GetByProcess(processConfig.MachineId);
            foreach (StartSceneConfig sceneConfig in list)
            {
                Log.Info($"  ┌ SceneId = {sceneConfig.Id}");
                Log.Info($"  ├ Type = {sceneConfig.Type}");
                Log.Info($"  ├ ProcessId = {sceneConfig.Process}");
                Log.Info($"  ├ ZoneId = {sceneConfig.Zone}");
                Log.Info($"  ├ Name = {sceneConfig.Name}");
                Log.Info($"  └ OuterAddr = {sceneConfig.OuterIPPort}");
            }

            Log.Info("");
        }

        public static void Configuration()
        {
            Log.Info($"[CONFIG]: 载入 {ConfigComponent.Instance.AllConfig.Count} 个配置文件!");
            foreach (object v in ConfigComponent.Instance.AllConfig.Values)
            {
                string typeName = v.GetType().Name;
                string fileName = typeName.Remove(typeName.IndexOf("Category"));
                Log.Info($"  [{fileName}]");
            }

            Log.Info("");
        }

        public static void MessageDispatcher()
        {
            int count = 0;
            foreach (ISessionStreamDispatcher v in SessionStreamDispatcher.Instance.Dispatchers)
            {
                if (v != null)
                {
                    count++;
                }
            }

            Log.Info($"[HANDLER]: 载入 {count} 个 SessionStreamDispatcher!");
            foreach (ISessionStreamDispatcher v in SessionStreamDispatcher.Instance.Dispatchers)
            {
                if (v != null)
                {
                    Log.Info($"  [{v.GetType().FullName}]");
                }
            }
            Log.Info("");
        }

        public static void MessageHandler()
        {
            Log.Info($"[HANDLER]: 载入 {MessageDispatcherComponent.Instance.Handlers.Count} 个 MessageHandler!");
            foreach (KeyValuePair<ushort, List<IMHandler>> kv in MessageDispatcherComponent.Instance.Handlers)
            {
                ushort opcode = kv.Key;
                Type type = OpcodeTypeComponent.Instance.GetType(opcode);

                bool isRequest = typeof(IRequest).IsAssignableFrom(type);
                if (isRequest)
                {
                    Log.Info($"  [Opcode={opcode}, ProtoType={type}， IRequest");
                }
                else
                {
                    Log.Info($"  [Opcode={opcode}, ProtoType={type}, IMessage");
                }

                foreach (IMHandler v in kv.Value)
                {
                    if (isRequest)
                    {
                        Log.Info($"    {v.GetType().Name}：AMRpcHandler<{v.GetMessageType()}, {v.GetResponseType()}>");
                    }
                    else
                    {
                        Log.Info($"    {v.GetType().Name}：AMHandler<{v.GetMessageType()}>");
                    }
                }
            }

            Log.Info("");
        }

        public static void ActorMessageHandler()
        {
            Log.Info($"[HANDER]: 载入 {ActorMessageDispatcherComponent.Instance.ActorMessageHandlers.Count} 个 ActorMessageHandler!");
            foreach (KeyValuePair<Type, IMActorHandler> kv in ActorMessageDispatcherComponent.Instance.ActorMessageHandlers)
            {
                ushort opcode = OpcodeTypeComponent.Instance.GetOpcode(kv.Key);
                bool isOuterActorMessage = OpcodeTypeComponent.Instance.IsOutrActorMessage(opcode);

                Type entityType = kv.Value.GetEntityType();
                Type requestType = kv.Value.GetRequestType();
                Type responseType = kv.Value.GetResponseType();

                bool isActorMessage = typeof(IActorMessage).IsAssignableFrom(kv.Key);
                bool isActorRequest = typeof(IActorRequest).IsAssignableFrom(kv.Key);
                bool isActorNotification = typeof(IActorNotification).IsAssignableFrom(kv.Key);
                bool isActorResponse = typeof(IActorResponse).IsAssignableFrom(responseType);

                bool isActorLocationMessage = typeof(IActorLocationMessage).IsAssignableFrom(kv.Key);
                bool isIActorLocationRequest = typeof(IActorLocationRequest).IsAssignableFrom(kv.Key);
                bool isIActorLocationResponse = typeof(IActorLocationResponse).IsAssignableFrom(responseType);

                if (isActorLocationMessage)
                {
                    Log.Info($"  [Opcode={opcode}, ProtoType={kv.Key}, IActorLocationMessage]");
                    Log.Info($"    {kv.Value.GetType().Name}：AMActorLocationHandler<{entityType}, {requestType}>");
                }
                else if (isIActorLocationRequest)
                {
                    Log.Info($"  [Opcode={opcode}, ProtoType={kv.Key}, IActorLocationRequest]");
                    Log.Info($"    {kv.Value.GetType().Name}：AMActorLocationRpcHandler<{entityType}, {requestType}, {responseType}>");
                }
                else if (isActorMessage)
                {
                    Log.Info($"  [Opcode={opcode}, ProtoType={kv.Key}, IActorMessage]");
                    Log.Info($"    {kv.Value.GetType().Name}：AMActorHandler<{entityType}, {requestType}>");
                }
                else if (isActorNotification)
                {
                    Log.Info($"  [Opcode={opcode}, ProtoType={kv.Key}, IActorNotification]");
                    Log.Info($"    {kv.Value.GetType().Name}：AMActorNotificationHandler<{entityType}, {requestType}>");
                }
                else if (isActorRequest)
                {
                    Log.Info($"  [Opcode={opcode}, ProtoType={kv.Key}, IActorRequest]");
                    Log.Info($"    {kv.Value.GetType().Name}：AMActorRpcHandler<{entityType}, {requestType}, {responseType}>");
                }
                else
                {
                    throw new Exception($"  不支持的Actor消息类型: {kv.Key.Name}");
                }
            }

            Log.Info("");
        }
    }
}
