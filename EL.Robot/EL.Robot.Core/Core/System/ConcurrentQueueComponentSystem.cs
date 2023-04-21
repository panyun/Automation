using EL.Robot.Component;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL.Robot.Core
{
    public static class ConcurrentQueueComponentSystem
    {
        public static void Post(this ConcurrentQueueComponent self, QueueObject action)
        {
            if (self.paused) throw new Exception("正在重置队列，请稍后再试！");
            self.queue.Enqueue(action);
            self.AddQueueAction?.Invoke();
        }

        public static bool Remove(this ConcurrentQueueComponent self, long key)
        {
            self.paused = true;
            var queues = self.queue.ToList();
            queues.RemoveAll(q => q.Key == key);
            while (!self.queue.IsEmpty)
                self.queue.TryDequeue(out var temp);
            foreach (var item in queues)
            {
                item.Key = IdGenerater.Instance.GenerateId();
                self.queue.Enqueue(item);
            }
            self.paused = false;
            self.RemoveQueueAction?.Invoke();
            return true;
        }
        public static List<QueueObject> Infos(this ConcurrentQueueComponent self)
        {
            return self.queue.ToList();
        }
        public static void Update(this ConcurrentQueueComponent self)
        {
            while (!self.queue.IsEmpty && !self.paused)
            {
                if (!self.queue.TryDequeue(out QueueObject a))
                {
                    return;
                }
                try
                {
                    a.Action();
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

    }
}
