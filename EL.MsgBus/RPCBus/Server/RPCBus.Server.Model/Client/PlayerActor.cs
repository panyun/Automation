using ET;
using MongoDB.Bson.Serialization.Attributes;

namespace RPCBus.Server.Client
{
    /// <summary>
    /// 表示一个在线的玩家
    /// </summary>
    public class PlayerActor : Entity
    {
        public long AccountId { get; set; }
        public long ClientId { get; set; }
        [BsonIgnore]
        public string SessionId { get; set; }
        [BsonIgnore]
        /// <summary>
        /// 向 Client 发送消息的 ActorId
        /// </summary>
        public long ClientActorId { get; set; }
        public string Account { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public long ClientType { get; set; }
        /// <summary>
        /// 玩家进入游戏和离开游戏通知
        /// </summary>
        [BsonIgnore]
        public ETCancellationToken EnterGameCancellationToken = new ETCancellationToken();
    }
}
