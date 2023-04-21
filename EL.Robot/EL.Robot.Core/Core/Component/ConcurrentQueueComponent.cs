using System.Collections.Concurrent;

namespace EL.Robot.Core
{
    public class ConcurrentQueueComponent : Entity
    {
        public readonly ConcurrentQueue<QueueObject> queue = new();
        public bool paused = false;
        public Action AddQueueAction;
        public Action RemoveQueueAction;
    }
    public class QueueObject
    {
        public QueueObject(long key, object data, Action action)
        {
            this.Key = key;
            this.Data = data;
            this.Action = action;
        }
        public QueueObject()
        {

        }
        /// <summary>
        /// key
        /// </summary>
        public long Key { get; set; }
        /// <summary>
        /// 数据对象
        /// </summary>
        public object Data { get; set; }
        /// <summary>
        /// 委托对象
        /// </summary>
        public Action Action { get; set; }
    }
}
