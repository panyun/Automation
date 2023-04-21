using ET;
using System;

namespace RPCBus.Server.Gate
{
    [ActorMessageHandler]
    public class R2G_GetLoginKeyHandler : AMActorRpcHandler<Scene, Protos.R2G_GetLoginKey, Protos.G2R_GetLoginKey>
    {
        protected override async ETTask Run(Scene scene, Protos.R2G_GetLoginKey request, Protos.G2R_GetLoginKey response, Action reply)
        {
            long key = RandomHelper.RandInt64();
            scene.GetComponent<SessionKeyComponent>().Add(key, new Passport()
            {
                AccountId = request.AccountId,
                ClientId = request.ClientId,
                Nickname = request.Nickname,
                SessionId = request.SessionId,
                ClientType = request.ClientType
            });

            response.Key = key;
            reply();

            await ETTask.CompletedTask;
        }
    }
}
