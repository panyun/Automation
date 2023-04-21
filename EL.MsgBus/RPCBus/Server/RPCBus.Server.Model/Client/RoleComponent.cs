using ET;
using MongoDB.Bson.Serialization.Attributes;

namespace RPCBus.Server.Client
{
    [PlayerObject]
    public class RoleComponent : Entity, ISerializeToEntity
    {
        [BsonIgnore]
        public long PlayerId => this.Id;
        public string Nickname { get; set; }
        public string Test { get; set; }
        public string IP { get; set; }
    }
}
