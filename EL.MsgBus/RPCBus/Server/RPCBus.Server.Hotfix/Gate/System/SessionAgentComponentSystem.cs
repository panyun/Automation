using System.Collections.Generic;
using ET;

namespace RPCBus.Server.Gate
{
    [ObjectSystem]
    public class SessionUserComponentSystemDestroySystem : DestroySystem<SessionAgentComponent>
    {
        public override void Destroy(SessionAgentComponent self)
        {
            Agent agent = self.Agent;
            Session session = self.GetParent<Session>();

            Log.Debug($"[GATE]: 执行 <{agent.AccountId}:{agent.ClientId}@{agent.SessionId}> 的下线逻辑");

            if (agent.PlayerActorId > 0)
            {
                ActorMessageSenderComponent.Instance.Send(agent.PlayerActorId, new Protos.G2P_Leave() { SessionId = agent.SessionId });
            }

           // self.DomainScene().GetComponent<AgentComponent>()?.Remove(agent.Id);
            agent.Dispose();

            Log.Debug($"[GATE]: <{agent.AccountId}:{agent.ClientId}@{agent.SessionId}> 下线");
        }
    }
}
