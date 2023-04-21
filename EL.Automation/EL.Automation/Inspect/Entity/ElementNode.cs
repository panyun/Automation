using EL;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Automation.Inspect
{
    /// <summary>
    /// 节点树
    /// </summary>
    public class ElementNode
    {
        public ElementNode()
        {
            Id = IdGenerater.Instance.GenerateInstanceId() + "";
        }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<int> ChildIndexs { get; set; }
        public string Id { get; set; }
        public int LevelIndex { get; set; } = 0;
        public int Length { get; set; }
        public int Index { get; set; } = 0;
        public bool IsFindIndex { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        /// <summary>
        /// 当前节点
        /// </summary>
        public ElementUIA CurrentElementWin { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        /// <summary>
        /// 当前节点
        /// </summary>
        public ElementJAB CurrentElementJava { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ElementPlaywright CurrentElementPlaywright { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ElementVcOcr CurrentElementVcOcr { get; set; }
        [JsonIgnore]
        [BsonIgnore]
        public Element CurrentElement
        {
            get
            {
                if (CurrentElementWin != null) return CurrentElementWin;
                if (CurrentElementJava != null) return CurrentElementJava;
                return default;
            }
        }
        //public Element
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        /// <summary>
        /// 父节点
        /// </summary>
        public ElementNode Parent { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        /// <summary>
        /// 子节点
        /// </summary>
        public List<ElementNode> Children { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(new { CurrentElementWin.Name, CurrentElementWin.ClassName, CurrentElementWin.Role });
        }
        public List<ElementNode> GetChildrenNode()
        {
            var list = new List<ElementNode>();
            list.Add(this);
            if (Children != null)
                Children.ForEach(x =>
                {
                    list.AddRange(x.GetChildrenNode());
                });
            return list;
        }
        public List<ElementNode> GetParentNode()
        {
            var list = new List<ElementNode>();
            list.Add(this);
            if (Parent != null)
                list.AddRange(this.Parent.GetParentNode());
            return list;
        }
        public List<ElementUIA> GetElementWins()
        {
            var list = new List<ElementUIA>();
            list.Add(CurrentElementWin);
            if (Children != null)
                Children.ForEach(x =>
                {
                    list.AddRange(x.GetElementWins());
                });
            return list;
        }
        [JsonIgnore]
        public string ParentPath
        {
            get
            {
                if (CurrentElementWin != null)
                {
                    if (Parent == null)
                        return $"{CurrentElementWin.ControlTypeName.ToLower()}[{Index}]";
                    return $"{Parent.ParentPath}/{CurrentElementWin.ControlTypeName.ToLower()}[{Index}]";
                }
                return default;
                //if (Parent == null)
                //    return $"{CurrentElementJava.ControlTypeName.ToLower()}[{Index}]";
                //return $"{Parent.ParentPath}/{CurrentElementJava.ControlTypeName.ToLower()}[{Index}]";
            }
        }
        [JsonIgnore]
        public string ChildrenPath
        {
            get
            {
                if (CurrentElementWin != null)
                {
                    if (Children == null || Children.Count == 0)
                        return $"{CurrentElementWin.ControlTypeName.ToLower()}[{Index}]";
                    return $"{CurrentElementWin.ControlTypeName.ToLower()}[{Index}]/{Children[0].ChildrenPath}";
                }
                return default;
                //if (Children == null || Children.Count == 0)
                //    return $"{CurrentElementJava.ControlTypeName.ToLower()}[{Index}]";
                //return $"{CurrentElementJava.ControlTypeName.ToLower()}[{Index}]/{Children[0].ChildrenPath}";
            }
        }
        /// <summary>
        /// 匹配值
        /// </summary>
        public Dictionary<string, string> CompareValues { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ElementNodeRuntime PathRuntime { get; set; }
    }

}
