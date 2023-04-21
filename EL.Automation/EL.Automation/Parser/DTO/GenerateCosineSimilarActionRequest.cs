using Automation.Inspect;
using EL;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automation.Parser
{
   
    public class GenerateCosineSimilarActionRequest : RequestBase
    {
        /// <summary>
        /// 余弦相似度值
        /// </summary>
        public double CosineValue { get; set; }
        public LightProperty LightProperty { get; set; } = new LightProperty();
        [JsonIgnore]
        [BsonIgnore]
        public int ProcessId { get; set; }
    }

    public class GenerateCosineSimilarActionResponse : IResponse
    {
        public int Error { get; set; }
        public string Message { get; set; }
        public int RpcId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ElementPath ElementPath { get; set; }
        /// <summary>
        /// 相似条数
        /// </summary>
        public int Count { get; set; }
        public string StackTrace { get; set; }
    }
}
