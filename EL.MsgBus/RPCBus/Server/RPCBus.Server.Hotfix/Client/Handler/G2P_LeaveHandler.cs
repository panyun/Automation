using ET;
using RPCBus.Protos;
using RPCBus.Server.EventType;
using RPCBus.Server.Gate;
using RPCBus.Server.Model.P2P;

namespace RPCBus.Server.Client
{
    [ActorMessageHandler]
    public class G2P_LeaveHandler : AMActorHandler<PlayerActor, Protos.G2P_Leave>
    {
        protected override async ETTask Run(PlayerActor playerActor, Protos.G2P_Leave message)
        {
            async ETTask HandleAsync(Player player)
            {
                Log.Debug($"[STAGE]: <{playerActor.AccountId}:{playerActor.ClientId}@{playerActor.SessionId}> 正在退出");
                PlayerActor actor = playerActor.DomainScene().GetComponent<PlayerActorComponent>().Get(playerActor.ClientId);
                if (actor == playerActor)
                {
                    await player.Save();
                    //playerActor.DomainScene().GetComponent<PlayerActorComponent>().Remove(playerActor.PlayerId);
                    Log.Debug($"[STAGE]: <{playerActor.AccountId}:{playerActor.ClientId}@{playerActor.SessionId}> 移除观察者");
                }

                var leaveEvent = EventHelper.Create<LeaveGame>();
                leaveEvent.Actor = playerActor;
                leaveEvent.Player = player;
                EventHelper.Dispatch(leaveEvent, playerActor.ClientId);
                var t = Game.Scene.Children.Values.Cast<Scene>().Where(x => x.SceneType == SceneType.Gate).FirstOrDefault();
                var agent = t.GetComponent<AgentComponent>().Get(playerActor.SessionId);
                t.GetComponent<PeerRoomComponent>().LeaveRoom(playerActor.SessionId);
                t.GetComponent<AgentComponent>().Remove(playerActor.SessionId);
                var isExists = t.GetComponent<AgentComponent>().Users.Values.ToList().Exists(x => x.AccountId == playerActor.AccountId);
                if (isExists)
                {
                    t.GetComponent<AgentComponent>().Users.Where(t => t.Value.AccountId == agent.AccountId).ForEach(x =>
                    {
                        x.Value.Session.Send(new G2C_OffLine() { AccountId = agent.AccountId, ClientId = agent.ClientId, ClientType = agent.ClientType.ToString() });
                    });
                }
                Log.Debug($"[STAGE]: <{playerActor.AccountId}:{playerActor.ClientId}@{playerActor.SessionId}> 退出");
            }
            await PlayerHelper.HandleAsync(playerActor, HandleAsync);

        }
    }
}
