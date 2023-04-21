using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace EL.Async
{
    [AsyncMethodBuilder(typeof(AsyncELTaskCompletedMethodBuilder))]
    public struct ELTaskCompleted : ICriticalNotifyCompletion
    {
        [DebuggerHidden]
        public ELTaskCompleted GetAwaiter()
        {
            return this;
        }

        [DebuggerHidden]
        public bool IsCompleted => true;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerHidden]
        public void GetResult()
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerHidden]
        public void OnCompleted(Action continuation)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerHidden]
        public void UnsafeOnCompleted(Action continuation)
        {
        }
    }
}
