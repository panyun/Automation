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
    public class RequestBase : IRequest
    {
        public int RpcId { get; set; }
        public ElementPath ElementPath { get; set; }
        [JsonIgnore]
        [BsonIgnore]
        public ElementType ElementType
        {
            get
            {
                return ElementPath.ElementType;
            }
        }

        /// <summary>
        /// 是否动态搜寻地址
        /// </summary>
        [JsonIgnore]
        [BsonIgnore]
        public bool IsAvigationPath
        {
            get
            {
                if (ElementType.UIAUI == ElementType)
                {
                    if (ElementPath.AvigationType == AvigationType.ConsineSimilarity || ElementPath.AvigationType == AvigationType.Similarity ||
                        ElementPath.AvigationType == AvigationType.Runtime)
                        return true;
                    if (ElementPath.PathNode.CurrentElementWin.NativeElement != default)
                        return false;
                }
                if (ElementType.MSAAUI == ElementType || ElementType == ElementType.JABUI)
                    return true;
                return true;
            }
        }
        [JsonIgnore]
        [BsonIgnore]
        public bool IsMSAA
        {
            get
            {
                return ElementPath.PathNode?.ChildIndexs != default && ElementPath.PathNode?.ChildIndexs.Count > 0;
            }
        }
        /// <summary>
        /// 时间超时
        /// </summary>
        public int TimeOut { get; set; } = 20000;

    }
}
