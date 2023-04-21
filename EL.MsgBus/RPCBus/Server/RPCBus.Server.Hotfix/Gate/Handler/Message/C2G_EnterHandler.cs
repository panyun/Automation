using ET;
using RPCBus.Protos;
using System;
using RPCBus.Server.Client;

namespace RPCBus.Server.Gate
{
    [MessageHandler]
    public class C2G_EnterHandler : AMRpcHandler<Protos.C2G_Enter, Protos.G2C_Enter>
    {

        protected override async ETTask Run(Session session, Protos.C2G_Enter request, Protos.G2C_Enter response, Action reply)
        {
            Scene scene = session.DomainScene();
            // 校验 KEY 的合法性
            Passport passport = scene.GetComponent<SessionKeyComponent>().TakeOut(request.Key);
            if (passport == null)
            {
                response.Error = ET.ErrorCode.ERR_ConnectGateKeyError;
                response.Message = "[GATE]: 无效的 KEY 值";
                reply();
                session.Dispose();
                return;
            }

            Log.Debug($"[GATE]: <{passport.AccountId}:{passport.ClientId}@{passport.SessionId}> 开始进入 {session.RemoteAddress}");

            // 初始化 Agent 对象
            Agent agent = scene.GetComponent<AgentComponent>().AddChild<Agent>();
            agent.AccountId = passport.AccountId;
            agent.ClientId = passport.ClientId;
            agent.SessionId = passport.SessionId;
            agent.SessionTServiceId = session.Id;
            agent.ClientType = passport.ClientType;
            agent.Session = session;
            scene.GetComponent<AgentComponent>().Add(agent);
            session.AddComponent<SessionAgentComponent>().Agent = agent;
            session.AddComponent<MailBoxComponent, MailboxType>(MailboxType.GateSession);

            try
            {
                // 获得玩家的 Player 进程
                StartSceneConfig stageCfg = ClientAddressHelper.Get(scene.DomainZone(), passport.ClientId, agent.ClientType);
                Protos.G2P_Enter req = new Protos.G2P_Enter();

                req.AccountId = passport.AccountId;
                req.ClientId = passport.ClientId;
                req.Nickname = passport.Nickname;
                req.SessionId = passport.SessionId;
                req.ClientActorId = session.InstanceId;
                req.ClientType = passport.ClientType;
                // 通知 Player 进程, 玩家进入
                Protos.P2G_Enter ret = await ActorMessageSenderComponent.Instance.Call<Protos.P2G_Enter>(stageCfg.InstanceId, req);
                agent.PlayerActorId = ret.PlayerActorId;
                Log.Debug($"[GATE]: <{passport.AccountId}:{passport.ClientType}@{passport.SessionId}> 就绪");

                //广播
                List<G2C_OnLine> onLines = new List<G2C_OnLine>();
                //scene.GetComponent<AgentComponent>().Users.ForEach(x =>
                //{
                //    onLines.Add(new G2C_OnLine() { UserId = x.Value.UserId, ClientType = GetType(x.Value.ClientType) });
                //    x.Value.Session.Send(new G2C_OnLine() { UserId = agent.UserId, ClientType = GetType(agent.ClientType) });
                //});
                //群发逻辑
                var users = scene.GetComponent<AgentComponent>().Users.Where(t => t.Value.AccountId == agent.AccountId).ToList();
                onLines = users.Select(x => new G2C_OnLine() { AccountId = x.Value.AccountId, ClientId = x.Value.ClientId, ClientType = x.Value.ClientType.ToString() }).ToList();
                foreach (var item in users)
                {
                    foreach (var online in onLines)
                    {
                        item.Value.Session.Send(online);
                    }
                }
                response.Message = JsonHelper.ToJson(onLines.ToArray());
            }
            catch (Exception e)
            {
                Log.Error(e);
                session.Dispose();
                return;
            }

            reply();

            await ETTask.CompletedTask;
        }
    }
}
