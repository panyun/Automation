using ET;
using RPCBus.Server.Gate;

namespace RPCBus.Server.Realm
{
    [MessageHandler]
    public class C2R_LoginHandler : AMRpcHandler<Protos.C2R_Login, Protos.R2C_Login>
    {
        protected override async ETTask Run(Session session, Protos.C2R_Login request, Protos.R2C_Login response, Action reply)
        {
            //if (string.IsNullOrWhiteSpace(request.Account))
            //{
            //    session.Dispose();
            //    throw new Exception($"来自 {session.RemoteAddress} 的无效账号");
            //}

            // TODO: 校验账号合法性
            string sessionId = Convert.ToString(IdGenerater.Instance.GenerateId(), 16);
            Log.Debug($"[REALM]: 为账号 <{request.AccountId}>:<{request.ClientId}> 的主机 {session.RemoteAddress} 创建会话 <{sessionId}>");

            //   long playerId = await this.getAccountId(session, request.UserId, request.Type);
            Log.Debug($"[REALM]: 会话 {sessionId} 关联的用户为 <{request.AccountId}>:<{request.ClientId}>");


            // 向gate请求一个key,客户端可以拿着这个key连接gate 
            Protos.R2G_GetLoginKey req = new Protos.R2G_GetLoginKey();
            req.SessionId = sessionId;
            req.Nickname = $"{Agent.GetType(request.ClientType)}-{request.ClientId}";
            req.AccountId = request.AccountId;
            req.ClientId = request.ClientId;
            req.ClientType = request.ClientType;
            Log.Debug($"[REALM]: <{request.ClientId}@{sessionId}> 开始登录");

            StartSceneConfig gateConfig = GateAddressHelper.Random(session.DomainZone());
            Protos.G2R_GetLoginKey ret = await ActorMessageSenderComponent.Instance.Call<Protos.G2R_GetLoginKey>(gateConfig.InstanceId, req);
            response.Key = ret.Key;
            response.Address = gateConfig.StartProcessConfig.ClientIP;
            response.Port = gateConfig.OuterPort;
            response.AccountId = request.AccountId;
            response.ClientId = request.ClientId;
            reply();

            Log.Debug($"[REALM]: <{request.AccountId}:{request.ClientId}@{sessionId}> 重定向至 gate {gateConfig.Id}");

        }

        private async ETTask<long> getAccountId(Session session, string identity, int type)
        {
            IList<Account> lst = await DBComponent.Instance.Query<Account>(f => f.Identity == identity);
            if (lst.Count > 0)
            {
                return lst[0].PlayerId;
            }

            using (Account account = session.DomainScene().AddChild<Account>())
            {
                account.PlayerId = await DBComponent.Instance.GetIncrementId<Account>();
                account.Identity = identity;
                account.Type = type;
                await DBComponent.Instance.Save(account);
                return account.PlayerId;
            }
        }
    }
}
