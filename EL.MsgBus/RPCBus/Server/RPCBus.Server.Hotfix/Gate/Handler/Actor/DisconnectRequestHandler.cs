using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ET;
using RPCBus.Protos;

namespace RPCBus.Server.Gate
{
    [ActorMessageHandler]
    class DisconnectRequestHandler : AMActorRpcHandler<Session, Protos.DisconnectRequest, Protos.DisconnectResponse>
    {
        protected override async ETTask Run(Session session, Protos.DisconnectRequest request, Protos.DisconnectResponse response, Action reply)
        {
            Agent agent = session.GetComponent<SessionAgentComponent>().Agent;
            Scene scene = session.DomainScene();
            //scene.GetComponent<AgentComponent>().Remove(agent.UserId);
            Log.Debug($"[GATE]: 请求断开 <{agent.AccountId}:{agent.ClientId}@{agent.SessionId}> 的连接");
            if (session.IsDisposed)
            {
                reply();
                return;
            }

            // 将请求中的 Delay 参数设置为 0， 可以立即断开连接
            if (request.Delay <= 0)
            {
                session.Dispose();
                reply();
                return;
            }
            // 将请求中的 Code 设置为 0 ， 可不告知客户端
            if (request.Code != 0)
            {

                session.Send(new Protos.G2C_Abandoned() { Code = request.Code, Reason = request.Reason });
            }

            TimerComponent.Instance.NewOnceTimer(TimeHelper.ServerNow() + request.Delay, () =>
            {
                session.Dispose();
            });

            reply();
            await ETTask.CompletedTask;
        }
    }
}
