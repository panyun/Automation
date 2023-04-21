using ET;

namespace RPCBus.Server.Client
{
    public static class EventHelper
    {
        public static T Create<T>() where T : struct
        {
            return (T)Activator.CreateInstance(typeof(T));
        }

        public static void Dispatch<T>(T context, long key, Action action = null) where T : struct
        {
            // 下一帧再派发事件
            ThreadSynchronizationContext.Instance.PostNext(async () => {
                // 取得协程锁
                using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.PlayerActor, key))
                {
                    await DispatchAsync(context, action);
                }
            });
        }

        public static async ETTask DispatchAsync<T>(T context, Action action = null) where T : struct
        {
            await Game.EventSystem.Publish(context);
            if (action != null)
            {
                action.Invoke();
            }
        }
    }
}
