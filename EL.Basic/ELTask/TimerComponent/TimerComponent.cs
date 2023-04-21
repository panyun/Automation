using EL.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL.Async
{

    public enum TimerClass
    {
        None,
        OnceWaitTimer,
        OnceTimer,
        RepeatedTimer,
    }
    public class TimerAction : Entity
    {
        public TimerClass TimerClass;

        public object Callback;

        public long Time;
    }
    public class TimerComponent : Entity
    {
        public static TimerComponent Instance
        {
            get;
            set;
        }

        /// <summary>
        /// key: time, value: timer id
        /// </summary>
        private readonly MultiMap<long, long> TimeId = new MultiMap<long, long>();

        private readonly Queue<long> timeOutTime = new Queue<long>();

        private readonly Queue<long> timeOutTimerIds = new Queue<long>();

        // 记录最小时间，不用每次都去MultiMap取第一个值
        private long minTime;

        public void Update()
        {
            if (this.TimeId.Count == 0)
            {
                return;
            }

            long timeNow = TimeHelper.ServerNow();

            if (timeNow < this.minTime)
            {
                return;
            }

            foreach (KeyValuePair<long, List<long>> kv in this.TimeId)
            {
                long k = kv.Key;
                if (k > timeNow)
                {
                    minTime = k;
                    break;
                }

                this.timeOutTime.Enqueue(k);
            }

            while (this.timeOutTime.Count > 0)
            {
                long time = this.timeOutTime.Dequeue();
                foreach (long timerId in this.TimeId[time])
                {
                    this.timeOutTimerIds.Enqueue(timerId);
                }

                this.TimeId.Remove(time);
            }

            while (this.timeOutTimerIds.Count > 0)
            {
                long timerId = this.timeOutTimerIds.Dequeue();

                TimerAction timerAction = this.GetChild<TimerAction>(timerId);
                if (timerAction == null)
                {
                    continue;
                }
                Run(timerAction);
            }
        }

        private void Run(TimerAction timerAction)
        {
            switch (timerAction.TimerClass)
            {
                case TimerClass.OnceWaitTimer:
                    {
                        ETTask<bool> tcs = timerAction.Callback as ETTask<bool>;
                        this.Remove(timerAction.Id);
                        tcs.SetResult(true);
                        break;
                    }
                case TimerClass.OnceTimer:
                    {
                        Action action = timerAction.Callback as Action;
                        this.Remove(timerAction.Id);
                        action?.Invoke();
                        break;
                    }
                case TimerClass.RepeatedTimer:
                    {
                        Action action = timerAction.Callback as Action;
                        long tillTime = TimeHelper.ServerNow() + timerAction.Time;
                        this.AddTimer(tillTime, timerAction);
                        action?.Invoke();
                        break;
                    }
            }
        }

        private void AddTimer(long tillTime, TimerAction timer)
        {
            this.TimeId.Add(tillTime, timer.Id);
            if (tillTime < this.minTime)
            {
                this.minTime = tillTime;
            }
        }

        public async ETTask<bool> WaitTillAsync(long tillTime, ELCancellationToken cancellationToken = null)
        {
            if (TimeHelper.ServerNow() >= tillTime)
            {
                return true;
            }

            ETTask<bool> tcs = ETTask<bool>.Create(true);

            TimerAction timer = new TimerAction()
            {
                TimerClass = TimerClass.OnceWaitTimer,
                Time = 0,
                Callback = tcs
            };
            this.AddChild(timer);
            this.AddTimer(tillTime, timer);
            long timerId = timer.Id;

            void CancelAction()
            {
                if (this.Remove(timerId))
                {
                    tcs.SetResult(false);
                }
            }

            bool ret;
            try
            {
                cancellationToken?.Add(CancelAction);
                ret = await tcs;
            }
            finally
            {
                cancellationToken?.Remove(CancelAction);
            }
            return ret;
        }

        public async ETTask<bool> WaitFrameAsync(ELCancellationToken cancellationToken = null)
        {
            return await WaitAsync(1, cancellationToken);
        }

        public async ETTask<bool> WaitAsync(long time, ELCancellationToken cancellationToken = null)
        {
            if (time == 0)
            {
                return true;
            }
            long tillTime = TimeHelper.ServerNow() + time;

            ETTask<bool> tcs = ETTask<bool>.Create(true);

            var timer = this.AddChild(new TimerAction()
            {
                Callback = tcs,
                TimerClass = TimerClass.OnceWaitTimer,
                Time = 0
            });
            this.AddTimer(tillTime, timer);
            long timerId = timer.Id;

            void CancelAction()
            {
                if (this.Remove(timerId))
                {
                    tcs.SetResult(false);
                }
            }

            bool ret;
            try
            {
                cancellationToken?.Add(CancelAction);
                ret = await tcs;
            }
            finally
            {
                cancellationToken?.Remove(CancelAction);
            }
            return ret;
        }

        public long NewFrameTimer(Action action)
        {
#if NOT_CLIENT
			return NewRepeatedTimerInner(100, action);
#else
            return NewRepeatedTimerInner(1, action);
#endif
        }

        /// <summary>
        /// 创建一个RepeatedTimer
        /// </summary>
        private long NewRepeatedTimerInner(long time, Action action)
        {
            long tillTime = TimeHelper.ServerNow() + time;
            var timer = this.AddChild(new TimerAction()
            {
                TimerClass = TimerClass.RepeatedTimer,
                Time = time,
                Callback = action,
            });
            this.AddTimer(tillTime, timer);
            return timer.Id;
        }

        public long NewRepeatedTimer(long time, Action action)
        {
            return NewRepeatedTimerInner(time, action);
        }

        public void Remove(ref long id)
        {
            this.Remove(id);
            id = 0;
        }

        public bool Remove(long id)
        {
            if (id == 0)
            {
                return false;
            }

            TimerAction timerAction = this.GetChild<TimerAction>(id);
            if (timerAction == null)
            {
                return false;
            }
            timerAction.Dispose();
            return true;
        }

        public long NewOnceTimer(long tillTime, Action action)
        {
            if (tillTime < TimeHelper.ServerNow())
            {
                Logger.Error($"new once time too small: {tillTime}");
            }
            var timer = this.AddChild(new TimerAction()
            {
                TimerClass = TimerClass.OnceTimer,
                Time = 0,
                Callback = action,

            });
            this.AddTimer(tillTime, timer);
            return timer.Id;
        }
    }
    public static class TimeHelper
    {
        public const long OneDay = 86400000;
        public const long Hour = 3600000;
        public const long Minute = 60000;

        /// <summary>
        /// 客户端时间
        /// </summary>
        /// <returns></returns>
        public static long ClientNow()
        {
            return App.TimeInfo.ClientNow();
        }

        public static long ClientNowSeconds()
        {
            return ClientNow() / 1000;
        }

        public static DateTime DateTimeNow()
        {
            return DateTime.Now;
        }

        public static long ServerNow()
        {
            return App.TimeInfo.ServerNow();
        }

        public static DateTime ToDateTime(long timestamp)
        {
            return App.TimeInfo.ToDateTime(timestamp);
        }

        public static long ClientFrameTime()
        {
            return App.TimeInfo.ClientFrameTime();
        }

        public static long ServerFrameTime()
        {
            return App.TimeInfo.ServerFrameTime();
        }
    }
    public class MultiMap<T, K> : SortedDictionary<T, List<K>>
    {
        private readonly List<K> Empty = new List<K>();

        public void Add(T t, K k)
        {
            List<K> list;
            this.TryGetValue(t, out list);
            if (list == null)
            {
                list = new List<K>();
                this.Add(t, list);
            }
            list.Add(k);
        }

        public bool Remove(T t, K k)
        {
            List<K> list;
            this.TryGetValue(t, out list);
            if (list == null)
            {
                return false;
            }
            if (!list.Remove(k))
            {
                return false;
            }
            if (list.Count == 0)
            {
                this.Remove(t);
            }
            return true;
        }

        /// <summary>
        /// 不返回内部的list,copy一份出来
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public K[] GetAll(T t)
        {
            List<K> list;
            this.TryGetValue(t, out list);
            if (list == null)
            {
                return new K[0];
            }
            return list.ToArray();
        }

        /// <summary>
        /// 返回内部的list
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public new List<K> this[T t]
        {
            get
            {
                this.TryGetValue(t, out List<K> list);
                return list ?? Empty;
            }
        }

        public K GetOne(T t)
        {
            List<K> list;
            this.TryGetValue(t, out list);
            if (list != null && list.Count > 0)
            {
                return list[0];
            }
            return default;
        }

        public bool Contains(T t, K k)
        {
            List<K> list;
            this.TryGetValue(t, out list);
            if (list == null)
            {
                return false;
            }
            return list.Contains(k);
        }
    }
}
