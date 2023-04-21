namespace ET
{
    public enum CoroutineLockType
    {
        None = 0,
        Location,                  // location进程上使用
        ActorLocationSender,       // ActorLocationSender中队列消息 
        Mailbox,                   // Mailbox中队列
        UnitId,                    // Map服务器上线下线时使用
        PlayerId,                  // Player服务器上下线时使用
        PlayerEnter,               // Stage服务器上下线时使用
        PlayerActor,               // Stage服务器中玩家逻辑串行化
        Notification,
        DB,
        Resources,
        ResourcesLoader,

        Max, // 这个必须在最后
    }
}