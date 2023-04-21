using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
namespace EL.Async
{
    [AsyncMethodBuilder(typeof(ELAsyncTaskMethodBuilder))]
    public class ELTask : ICriticalNotifyCompletion
    {
        public static ELTaskCompleted CompletedTask
        {
            get
            {

                return new ELTaskCompleted();
            }
        }


        private static readonly Queue<ELTask> queue = new Queue<ELTask>();

        /// <summary>
        /// 请不要随便使用ETTask的对象池，除非你完全搞懂了ETTask!!!
        /// 假如开启了池,await之后不能再操作ETTask，否则可能操作到再次从池中分配出来的ETTask，产生灾难性的后果
        /// SetResult的时候请现将tcs置空，避免多次对同一个ETTask SetResult
        /// </summary>
        public static ELTask Create(bool fromPool = false)
        {
            if (!fromPool)
            {
                return new ELTask();
            }

            if (queue.Count == 0)
            {
                return new ELTask() { fromPool = true };
            }
            return queue.Dequeue();
        }

        private void Recycle()
        {
            if (!this.fromPool)
            {
                return;
            }
            this.state = AwaiterStatus.Pending;
            this.callback = null;
            queue.Enqueue(this);
            // 太多了，回收一下
            if (queue.Count > 1000)
            {
                queue.Clear();
            }
        }

        private bool fromPool;
        private AwaiterStatus state;
        private object callback; // Action or ExceptionDispatchInfo

        private ELTask()
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerHidden]
        private async ELVoid InnerCoroutine()
        {
            await this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerHidden]
        public void Coroutine()
        {
            InnerCoroutine().Coroutine();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerHidden]
        public ELTask GetAwaiter()
        {
            return this;
        }


        public bool IsCompleted
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [DebuggerHidden]
            get
            {
                return this.state != AwaiterStatus.Pending;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerHidden]
        public void UnsafeOnCompleted(Action action)
        {
            if (this.state != AwaiterStatus.Pending)
            {
                action?.Invoke();
                return;
            }

            this.callback = action;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerHidden]
        public void OnCompleted(Action action)
        {
            this.UnsafeOnCompleted(action);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerHidden]
        public void GetResult()
        {
            switch (this.state)
            {
                case AwaiterStatus.Succeeded:
                    this.Recycle();
                    break;
                case AwaiterStatus.Faulted:
                    ExceptionDispatchInfo c = this.callback as ExceptionDispatchInfo;
                    this.callback = null;
                    c?.Throw();
                    this.Recycle();

                    break;
                default:
                    throw new NotSupportedException("ETTask does not allow call GetResult directly when task not completed. Please use 'await'.");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerHidden]
        public void SetResult()
        {
            if (this.state != AwaiterStatus.Pending)
            {
                throw new InvalidOperationException("TaskT_TransitionToFinal_AlreadyCompleted");
            }

            this.state = AwaiterStatus.Succeeded;

            Action c = this.callback as Action;
            this.callback = null;
            c?.Invoke();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerHidden]
        public void SetException(Exception e)
        {
            if (this.state != AwaiterStatus.Pending)
            {
                throw new InvalidOperationException("TaskT_TransitionToFinal_AlreadyCompleted");
            }

            this.state = AwaiterStatus.Faulted;

            Action c = this.callback as Action;
            this.callback = ExceptionDispatchInfo.Capture(e);
            c?.Invoke();
        }
    }

    [AsyncMethodBuilder(typeof(ETAsyncTaskMethodBuilder<>))]
    public class ELTask<T> : ICriticalNotifyCompletion
    {
        private static readonly Queue<ELTask<T>> queue = new Queue<ELTask<T>>();

        /// <summary>
        /// 请不要随便使用ETTask的对象池，除非你完全搞懂了ETTask!!!
        /// 假如开启了池,await之后不能再操作ETTask，否则可能操作到再次从池中分配出来的ETTask，产生灾难性的后果
        /// SetResult的时候请现将tcs置空，避免多次对同一个ETTask SetResult
        /// </summary>
        public static ELTask<T> Create(bool fromPool = false)
        {
            if (!fromPool)
            {
                return new ELTask<T>();
            }

            if (queue.Count == 0)
            {
                return new ELTask<T>() { fromPool = true };
            }
            return queue.Dequeue();
        }

        private void Recycle()
        {
            if (!this.fromPool)
            {
                return;
            }
            this.callback = null;
            this.value = default;
            this.state = AwaiterStatus.Pending;
            queue.Enqueue(this);
            // 太多了，回收一下
            if (queue.Count > 1000)
            {
                queue.Clear();
            }
        }

        private bool fromPool;
        private AwaiterStatus state;
        private T value;
        private object callback; // Action or ExceptionDispatchInfo

        private ELTask()
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerHidden]
        private async ELVoid InnerCoroutine()
        {
            await this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerHidden]
        public void Coroutine()
        {
            InnerCoroutine().Coroutine();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerHidden]
        public ELTask<T> GetAwaiter()
        {
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerHidden]
        public T GetResult()
        {
            switch (this.state)
            {
                case AwaiterStatus.Succeeded:
                    T v = this.value;
                    this.Recycle();
                    return v;
                case AwaiterStatus.Faulted:
                    ExceptionDispatchInfo c = this.callback as ExceptionDispatchInfo;
                    this.callback = null;
                    c?.Throw();
                    this.Recycle();
                    return default;
                default:
                    throw new NotSupportedException("ETask does not allow call GetResult directly when task not completed. Please use 'await'.");
            }
        }


        public bool IsCompleted
        {
            [DebuggerHidden]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return state != AwaiterStatus.Pending;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerHidden]
        public void UnsafeOnCompleted(Action action)
        {
            if (this.state != AwaiterStatus.Pending)
            {
                action?.Invoke();
                return;
            }

            this.callback = action;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerHidden]
        public void OnCompleted(Action action)
        {
            this.UnsafeOnCompleted(action);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerHidden]
        public void SetResult(T result)
        {
            if (this.state != AwaiterStatus.Pending)
            {
                //throw new InvalidOperationException("TaskT_TransitionToFinal_AlreadyCompleted");
                return;
            }
            this.state = AwaiterStatus.Succeeded;
            this.value = result;
            Action c = this.callback as Action;
            this.callback = null;
            c?.Invoke();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerHidden]
        public void SetException(Exception e)
        {
            if (this.state != AwaiterStatus.Pending)
            {
                // throw new InvalidOperationException("TaskT_TransitionToFinal_AlreadyCompleted");
                return;
            }

            this.state = AwaiterStatus.Faulted;

            Action c = this.callback as Action;
            this.callback = ExceptionDispatchInfo.Capture(e);
            c?.Invoke();
        }
    }
    public static class ETTaskHelper
    {
        private class CoroutineBlocker
        {
            private int count;

            private List<ELTask> tcss = new List<ELTask>();

            public CoroutineBlocker(int count)
            {
                this.count = count;
            }

            public async ELTask WaitAsync()
            {
                --this.count;
                if (this.count < 0)
                {
                    return;
                }

                if (this.count == 0)
                {
                    List<ELTask> t = this.tcss;
                    this.tcss = null;
                    foreach (ELTask ttcs in t)
                    {
                        ttcs.SetResult();
                    }

                    return;
                }

                ELTask tcs = ELTask.Create(true);

                tcss.Add(tcs);
                await tcs;
            }
        }

        public static async ELTask<bool> WaitAny<T>(ELTask<T>[] tasks, ELCancellationToken cancellationToken = null)
        {
            if (tasks.Length == 0)
            {
                return false;
            }

            CoroutineBlocker coroutineBlocker = new CoroutineBlocker(2);

            foreach (ELTask<T> task in tasks)
            {
                RunOneTask(task).Coroutine();
            }

            async ELVoid RunOneTask(ELTask<T> task)
            {
                await task;
                await coroutineBlocker.WaitAsync();
            }

            await coroutineBlocker.WaitAsync();

            if (cancellationToken == null)
            {
                return true;
            }

            return !cancellationToken.IsCancel();
        }

        public static async ELTask<bool> WaitAny(ELTask[] tasks, ELCancellationToken cancellationToken = null)
        {
            if (tasks.Length == 0)
            {
                return false;
            }

            CoroutineBlocker coroutineBlocker = new CoroutineBlocker(2);

            foreach (ELTask task in tasks)
            {
                RunOneTask(task).Coroutine();
            }

            async ELVoid RunOneTask(ELTask task)
            {
                await task;
                await coroutineBlocker.WaitAsync();
            }

            await coroutineBlocker.WaitAsync();

            if (cancellationToken == null)
            {
                return true;
            }

            return !cancellationToken.IsCancel();
        }

        public static async ELTask<bool> WaitAll<T>(ELTask<T>[] tasks, ELCancellationToken cancellationToken = null)
        {
            if (tasks.Length == 0)
            {
                return false;
            }

            CoroutineBlocker coroutineBlocker = new CoroutineBlocker(tasks.Length + 1);

            foreach (ELTask<T> task in tasks)
            {
                RunOneTask(task).Coroutine();
            }

            async ELVoid RunOneTask(ELTask<T> task)
            {
                await task;
                await coroutineBlocker.WaitAsync();
            }

            await coroutineBlocker.WaitAsync();

            if (cancellationToken == null)
            {
                return true;
            }

            return !cancellationToken.IsCancel();
        }

        public static async ELTask<bool> WaitAll<T>(List<ELTask<T>> tasks, ELCancellationToken cancellationToken = null)
        {
            if (tasks.Count == 0)
            {
                return false;
            }

            CoroutineBlocker coroutineBlocker = new CoroutineBlocker(tasks.Count + 1);

            foreach (ELTask<T> task in tasks)
            {
                RunOneTask(task).Coroutine();
            }

            async ELVoid RunOneTask(ELTask<T> task)
            {
                await task;
                await coroutineBlocker.WaitAsync();
            }

            await coroutineBlocker.WaitAsync();

            if (cancellationToken == null)
            {
                return true;
            }

            return !cancellationToken.IsCancel();
        }

        public static async ELTask<bool> WaitAll(ELTask[] tasks, ELCancellationToken cancellationToken = null)
        {
            if (tasks.Length == 0)
            {
                return false;
            }

            CoroutineBlocker coroutineBlocker = new CoroutineBlocker(tasks.Length + 1);

            foreach (ELTask task in tasks)
            {
                RunOneTask(task).Coroutine();
            }

            await coroutineBlocker.WaitAsync();

            async ELVoid RunOneTask(ELTask task)
            {
                await task;
                await coroutineBlocker.WaitAsync();
            }

            if (cancellationToken == null)
            {
                return true;
            }

            return !cancellationToken.IsCancel();
        }

        public static async ELTask<bool> WaitAll(List<ELTask> tasks, ELCancellationToken cancellationToken = null)
        {
            if (tasks.Count == 0)
            {
                return false;
            }

            CoroutineBlocker coroutineBlocker = new CoroutineBlocker(tasks.Count + 1);

            foreach (ELTask task in tasks)
            {
                RunOneTask(task).Coroutine();
            }

            await coroutineBlocker.WaitAsync();

            async ELVoid RunOneTask(ELTask task)
            {
                await task;
                await coroutineBlocker.WaitAsync();
            }

            if (cancellationToken == null)
            {
                return true;
            }

            return !cancellationToken.IsCancel();
        }
    }
}
