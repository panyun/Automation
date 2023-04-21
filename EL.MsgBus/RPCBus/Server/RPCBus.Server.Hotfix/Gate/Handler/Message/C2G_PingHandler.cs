using System;
using ET;
using MongoDB.Bson;
using RPCBus.Protos;
using RPCBus.Server.Model.P2P;

namespace RPCBus.Server.Gate
{
    [MessageHandler]
    public class C2G_PingHandler : AMRpcHandler<Protos.C2G_Ping, Protos.G2C_Ping>
    {
        protected override async ETTask Run(Session session, Protos.C2G_Ping request, Protos.G2C_Ping response, Action reply)
        {
            await ETTask.CompletedTask;
            //增加P2P准备标识
            var PeerRoomComponent = session.DomainScene().GetComponent<PeerRoomComponent>();
            var loginInfo = session.GetComponent<PeerRoomInfo>();
            if (loginInfo != null)
            {
                var peerRoom = PeerRoomComponent.GetPeerRoom(loginInfo);
                if (peerRoom != null)
                {
                    response.Message = new { P2PReady = peerRoom.IsReady }.ToJson();
                }
            }

            response.Time = TimeHelper.ServerNow();
            ThreadSynchronizationContext.Instance.PostNext(() =>
            {
                reply();
            });
        }
    }
}
