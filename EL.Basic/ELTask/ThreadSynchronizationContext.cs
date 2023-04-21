using EL.Basic;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL.Async
{
    public class ThreadSynchronizationContext : SynchronizationContext
    {
        public static ThreadSynchronizationContext Instance { get; } = new ThreadSynchronizationContext(Thread.CurrentThread.ManagedThreadId);

        private readonly int threadId;

        // 线程同步队列,发送接收socket回调都放到该队列,由poll线程统一执行
        private readonly ConcurrentQueue<Action> queue = new ConcurrentQueue<Action>();

        private Action a;

        public ThreadSynchronizationContext(int threadId)
        {
            this.threadId = threadId;
        }

        public void Update()
        {
            while (true)
            {
                if (!this.queue.TryDequeue(out a))
                {
                    return;
                }

                try
                {
                    a();
                }
                catch (Exception e)
                {
                    Logger.Error(e.Message, e, null);
                }
            }
        }

        public override void Post(SendOrPostCallback callback, object state)
        {
            this.Post(() => callback(state));
        }

        public void Post(Action action)
        {
            if (Thread.CurrentThread.ManagedThreadId == this.threadId)
            {
                try
                {
                    action();
                }
                catch (Exception e)
                {
                    Logger.Error(e.Message, e, null);
                }

                return;
            }

            this.queue.Enqueue(action);
        }

        public void PostNext(Action action)
        {
            this.queue.Enqueue(action);
        }
    }
    public static class App
    {

        public static ThreadSynchronizationContext ThreadSynchronizationContext => ThreadSynchronizationContext.Instance;
        public static TimeInfo TimeInfo => TimeInfo.Instance;
        public static IdGenerater IdGenerater => IdGenerater.Instance;
        public static List<Action> FrameFinishCallback = new List<Action>();
        public static void Update()
        {
            ThreadSynchronizationContext.Update();
            TimeInfo.Update();
            TimerComponent.Instance.Update();
        }
        public static void FrameFinish()
        {
            foreach (Action action in FrameFinishCallback)
            {
                action.Invoke();
            }
            FrameFinishCallback.Clear();
        }
        public static void Close()
        {
            IdGenerater.Instance.Dispose();
        }
    }
}
