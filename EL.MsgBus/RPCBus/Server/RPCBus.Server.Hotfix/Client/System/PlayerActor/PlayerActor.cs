using ET;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPCBus.Server.Client
{
    public static class PlayerActorExtension
    {
        public static void Send(this PlayerActor self, IActorMessage message)
        {
            if (self.IsDisposed || self.ClientActorId == 0)
            {
                return;
            }

            ActorMessageSenderComponent.Instance.Send(self.ClientActorId, message);
        }

        public static async ETTask<IActorResponse> Call(this PlayerActor self, IActorRequest request, bool needException = true)
        {
            if (self.IsDisposed || self.ClientActorId == 0)
            {
                return null;
            }
            return await ActorMessageSenderComponent.Instance.Call(self.ClientActorId, request, needException);
        }


        public static void Disconnect(this PlayerActor self, int code, string reason)
        {
            Log.Debug($"[STAGE]: 尝试断开 <{self.AccountId}:{self.ClientId}@{self.SessionId}> 的连接 Code:{code}, Reason:{reason}");

            if (self.IsDisposed)
            {
                return;
            }

            if (self.ClientActorId > 0)
            {
                ThreadSynchronizationContext.Instance.PostNext(async () =>
                {
                    Protos.DisconnectRequest req = new Protos.DisconnectRequest()
                    {
                        Delay = 3000,
                        Code = code,
                        Reason = reason
                    };
                    StartSceneConfig stageCfg = ClientAddressHelper.GetGate(1);
                     ActorMessageSenderComponent.Instance.Send(stageCfg.InstanceId, req);
                });
            }
        }

        public static void Disconnect(this PlayerActor self, Exception exception)
        {
            var frame = new StackTrace(exception, true).GetFrame(0);
            var message = $"{Path.GetFileName(frame.GetFileName())}:{frame.GetFileLineNumber()} | {exception.Message}";
            self.Disconnect(500, message);
        }

        public static void Disconnect(this PlayerActor self, ETException exception)
        {
            self.Disconnect(exception.ErrorCode, exception.Message);
        }
    }
}
