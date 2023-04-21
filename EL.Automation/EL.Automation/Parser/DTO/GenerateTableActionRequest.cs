using Automation.Inspect;
using EL;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;

namespace Automation.Parser
{

    public class GenerateTableActionRequest : RequestBase
    {
        public LightProperty LightProperty { get; set; } = new LightProperty()
        {
            Count = 3,
            Time = 500
        };
    }

    public class GenerateTableActionResponse : ResponseBase
    {
        public string DataTableJson { get; set; }
        [JsonIgnore]
        [BsonIgnore]
        public DataTable DataTable { get; set; }
    }
}
