using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace EL.Robot.Component
{

    public enum ExecState
    {
        None, IsPaused, IsStop, IsStep, IsBreak, IsContinue
    }
    public class Flow
    {
        /// <summary>
        /// 流程id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 流程名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 输入参数
        /// </summary>
        public List<Parameter> InParams { get; set; }
        public List<Parameter> OutParams { get; set; }
        /// <summary>
        /// 流程节点信息
        /// </summary>
        public List<Node> Steps { get; set; }
        /// <summary>
        /// 全局变量
        /// </summary>
        public List<Variable> Variables { get; set; }
        /// <summary>
        /// 是否调试
        /// </summary>
        public bool IsDebug { get; set; }
        public bool IsPip { get; set; }
        public List<Flow> ChildrenFlows { get; set; }
        #region 流程处理
        [IgnoreDataMember]
        [BsonIgnore]
        [JsonIgnore]
        /// <summary>
        /// 参数管理器
        /// </summary>
        public Dictionary<string, object> ParamsManager { get; set; }

        #endregion


    }
    public class FlowScript
    {
        /// <summary>
        /// 流程id
        /// </summary>
        public long Id { get; set; }
    }

    public class Variable
    {
        public long Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// from:'manually' 
        /// </summary>
        public string ComponentId { get; set; }
        public string Value { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string InitType { get; set; }

        public string Context { get; set; }
        /// <summary>
        /// 作用域
        /// </summary>
        public bool isGlobal { get; set; }
    }
    public enum NodeType
    {
        For, Flow, IF, While, Break, Component, PreHandler
    }
}
