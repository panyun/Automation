using ET;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RPCBus.Server.Client
{
    /// <summary>
    /// 玩家数据实体
    /// </summary>
    [BsonIgnoreExtraElements]
    public class Player : Entity
    {
        public Player(long accountId, long clientId)
        {
            AccountId = accountId;  
            ClientId = clientId;
        }
        [BsonIgnore]    
        public long AccountId { get; set; }
        [BsonIgnore]
        public long ClientId { get; set; }  
        override public K GetComponent<K>()
        {
            K component = base.GetComponent<K>();

            if (component is ISerializeToEntity)
            {
                addDirty(component);
            }

            return component;
        }

        override public Entity GetComponent(Type type)
        {
            Entity component = base.GetComponent(type);
            if (component is ISerializeToEntity)
            {
                addDirty(component);
            }
            return component;
        }

        /// <summary>
        /// 组件将在一次存盘时回写数据
        /// </summary>
        /// <param name="child"></param>
        public void SaveComponent(Entity child)
        {
            if (child is ISerializeToEntity)
            {
                addDirty(child);
            }
        }

        public long LastTimestamp { get; set; }

        [BsonIgnore]
        public long CheckRepeatedTimer;

        [BsonIgnore]
        public long SaveRepeatedTimer;

        [BsonIgnore]
        public HashSet<Entity> DirtyQueue = new HashSet<Entity>();
        private void addDirty(Entity entity)
        {
            if (this.DirtyQueue.Contains(entity))
            {
                return;
            }
            this.DirtyQueue.Add(entity);
            Log.Debug($"[STAGE]: 组件 {entity.GetType().Name} 被添加到更新队列");
        }
    }
}
