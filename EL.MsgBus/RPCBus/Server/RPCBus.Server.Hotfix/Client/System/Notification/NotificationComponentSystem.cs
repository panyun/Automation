using ET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPCBus.Server.Client
{
    public static class NotificationComponentSystem
    {
        public static void Add(this NotificationComponent self, IActorNotification notification)
        {
            self.Add(notification.NotificationId);
        }

        public static void Add(this NotificationComponent self, long notificationId)
        {
            self.Receipts.Add(notificationId);
        }

        public static bool Exists(this NotificationComponent self, IActorNotification notification)
        {
            return self.Exists(notification.NotificationId);
        }

        public static bool Exists(this NotificationComponent self, long notificationId)
        {
            return self.Receipts.Contains(notificationId);
        }

        public static void Remove(this NotificationComponent self, IActorNotification notification)
        {
            self.Remove(notification.NotificationId);
        }

        public static void Remove(this NotificationComponent self, long notificationId)
        {
            self.Receipts.Remove(notificationId);
        }

    }
}
