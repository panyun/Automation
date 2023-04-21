using ET;
using System;

namespace RPCBus.Server.Client
{
    [ActorMessageHandler]
    public abstract class AMActorNotificationHandler<E, Message> : IMActorHandler where E : Entity where Message : class, IActorNotification
    {
        protected abstract ETTask Run(E entity, Message message);

        public async ETTask Handle(Entity entity, object actorMessage, Action<IActorResponse> reply)
        {
            Message msg = actorMessage as Message;
            if (msg == null)
            {
                Log.Error($"消息类型转换错误: {actorMessage.GetType().FullName} to {typeof(Message).Name}");
                return;
            }

            E e = entity as E;
            if (e == null)
            {
                Log.Error($"Actor类型转换错误: {entity.GetType().Name} to {typeof(E).Name} --{typeof(Message).Name}");
                return;
            }

            await this.Run(e, msg);

            IActorResponse response = (IActorResponse)Activator.CreateInstance(GetResponseType());
            response.RpcId = msg.RpcId;
            reply.Invoke(response);
        }

        public Type GetRequestType()
        {
            return typeof(Message);
        }

        public Type GetResponseType()
        {
            return typeof(ActorResponse);
        }

        public Type GetEntityType()
        {
            return typeof(E);
        }
    }
}
