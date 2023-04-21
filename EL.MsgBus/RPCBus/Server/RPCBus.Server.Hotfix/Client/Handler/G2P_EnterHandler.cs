using ET;
using RPCBus.Server.EventType;

namespace RPCBus.Server.Client
{
    [ActorMessageHandler]
    public class G2P_EnterHandler : AMActorRpcHandler<Scene, Protos.G2P_Enter, Protos.P2G_Enter>
    {
        protected override async ETTask Run(Scene scene, Protos.G2P_Enter request, Protos.P2G_Enter response, Action reply)
        {
            Log.Debug($"[STAGE]: <{request.AccountId}:{request.ClientId}@{request.SessionId}> 开始进入");

            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.PlayerActor, request.ClientId))
            {
                var reg = new Registration(request.ClientId, request.Nickname);
                //Player player = await scene.GetComponent<PlayerComponent>().Create(reg);
                var player = new Player(request.AccountId, request.ClientId);
                if (player == null)
                {
                    throw new Exception("初始用户信息为null!");
                }

                // 踢掉之前的客户端
                PlayerActor oldPlayerActor = scene.GetComponent<PlayerActorComponent>().Remove(request.ClientId);
                if (oldPlayerActor != null)
                {
                    ThreadSynchronizationContext.Instance.PostNext(async () => await kick(oldPlayerActor.ClientActorId, request.ClientId));
                }

                PlayerActor playerActor = scene.GetComponent<PlayerActorComponent>().AddChild<PlayerActor>();
                playerActor.AccountId = request.AccountId;
                playerActor.ClientId = request.ClientId;
                playerActor.SessionId = request.SessionId;
                playerActor.ClientActorId = request.ClientActorId;
                playerActor.ClientType = request.ClientType;
                playerActor.AddComponent<MailBoxComponent, MailboxType>(MailboxType.MessageDispatcher);
                scene.GetComponent<PlayerActorComponent>().Add(playerActor);
                response.PlayerActorId = playerActor.InstanceId;
                reply();
                var enterEvent = EventHelper.Create<EnterGame>();
                enterEvent.Actor = playerActor;
                enterEvent.Player = player;
                EventHelper.Dispatch(enterEvent, request.ClientId);
                Log.Debug($"[STAGE]: <{request.AccountId}:{request.ClientId}@{request.SessionId}> 就绪");
                scene.GetComponent<PlayerComponent>().AddCache(request.ClientId, player);
            }
        }

        private static async ETTask<bool> kick(long actorId, long playerId)
        {
            Log.Debug($"[STAGE]: 执行 {playerId} 的顶号逻辑");

            Protos.DisconnectRequest req = new Protos.DisconnectRequest()
            {
                Delay = 3000,
                Code = 1,
                Reason = "您的帐号正在其它设备登录"
            };

            Protos.DisconnectResponse ret = await ActorMessageSenderComponent.Instance.Call<Protos.DisconnectResponse>(actorId, req, false);
            switch (ret.Error)
            {
                case ErrorCode.NOERR:
                    {
                        return true;
                    }
                case ET.ErrorCode.ERR_NotFoundActor:
                    {
                        // 可能已经下下线了
                        return true;
                    }
                default:
                    {
                        Log.Error($"[STAGE]: 执行踢掉玩家 {playerId} 的 Actor 逻辑时， 出现异常 {ret.Error}");
                        return false;
                    }
            }

        }

    }
}
