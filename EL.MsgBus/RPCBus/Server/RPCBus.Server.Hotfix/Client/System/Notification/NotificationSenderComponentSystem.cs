using ET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPCBus.Server.Client
{
    [ObjectSystem]
    class NotificationSenderComponentAwakeSystem : AwakeSystem<NotificationSenderComponent>
    {
        private const long SCAN_INTERVAL = 10 * 1000;
        private const long RETRY_INTERVAL = 10 * 1000;


        public override void Awake(NotificationSenderComponent self)
        {
            int zone = self.DomainZone();
            int count = ClientAddressHelper.Count(zone);
            int pos = ClientAddressHelper.IndexOf(zone, self.Id);

            self.RepeatedTimer = TimerComponent.Instance.NewRepeatedTimer(SCAN_INTERVAL, async () =>
            {
                long deadline = TimeHelper.ServerNow() - RETRY_INTERVAL;
                using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.Notification, self.DomainScene().Id))
                {
                    List<Notification> retries = await DBComponent.Instance.Query<Notification>(
                        (f) =>
                            (f.Sender % count == pos && f.CreatedAt < deadline),
                        self.GetType().Name);

                    if (retries.Count == 0)
                    {
                        return;
                    }

                    Log.Debug($"[STAGE]: 发现 {retries.Count} 个需要重试的 Notification ");

                    foreach (var notification in retries)
                    {
                        await self.PublishNotification(notification);
                    }
                }
            });

        }
    }

    [ObjectSystem]
    class NotificationSenderComponentDestroySystem : DestroySystem<NotificationSenderComponent>
    {
        public override void Destroy(NotificationSenderComponent self)
        {
            TimerComponent.Instance.Remove(ref self.RepeatedTimer);
            Log.Info("[STAGE]： Destroy()");
        }
    }


    public static class NotificationSenderComponentSystem
    {
        public static async ETTask Create(this NotificationSenderComponent self, long sender, long receiver, IActorNotification request)
        {
            var notification = Notification.Create(sender, receiver, request);
            await DBComponent.Instance.Save(notification, self.GetType().Name);

            ThreadSynchronizationContext.Instance.PostNext(async () =>
            {
                await self.PublishNotification(notification);
            });
        }

        public static async ETTask PublishNotification(this NotificationSenderComponent self, Notification notification)
        {
            var actorId = ClientAddressHelper.Get(self.DomainZone(), notification.Receiver, 1).InstanceId;
            await ActorMessageSenderComponent.Instance.Call(actorId, notification.Message);

            await ActorMessageSenderComponent.Instance.Call(actorId, new Protos.RemoveNotification()
            {
                Receiver = notification.Receiver,
                NotificationId = notification.Id
            });

            await DBComponent.Instance.Remove<NotificationSenderComponent>(notification.Id);
        }
    }
}
