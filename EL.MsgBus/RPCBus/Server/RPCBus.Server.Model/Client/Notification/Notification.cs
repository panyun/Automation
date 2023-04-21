using ET;

namespace RPCBus.Server.Client
{
    public class Notification : Entity
    {
        /// <summary>
        /// 发送者Id
        /// </summary>
        public long Sender { get; private set; }

        /// <summary>
        /// 接受者Id
        /// </summary>
        public long Receiver { get; private set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public long CreatedAt { get; private set; }

        /// <summary>
        /// 发送的请求
        /// </summary>
        public IActorNotification Message { get; private set; }

        public static Notification Create(long sender, long receiver, IActorNotification message)
        {
            //component = (Entity)Activator.CreateInstance(type);
            message.NotificationId = IdGenerater.Instance.GenerateId();

            var notification = new Notification();
            notification.Id = message.NotificationId;
            notification.CreatedAt = TimeHelper.ServerNow();
            notification.Sender = sender;
            notification.Receiver = receiver;
            notification.Message = message;
            return notification;
        }
    }
}
