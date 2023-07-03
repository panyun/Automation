using AutoMapper;
using CommunityToolkit.Mvvm.ComponentModel;
using EL.Robot.Component;
using EL.Robot.Core;
using MongoDB.Bson.Serialization.Attributes;
using Mysqlx.Crud;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using ViewModel;

namespace MiniRobotForm.Mode
{
    [AutoMap(typeof(Flow))]
    public class FlowModel : ObservableObject
    {
        #region Design
      
        public List<Node> DesignSteps { get; set; } = new List<Node>();

        #endregion
        public string HeadImg { get; set; }
        /// <summary>
        /// 流程id
        /// </summary>
        public long Id { get; set; }
        public long CreateDate { get; set; }
        public long ViewSort { get; set; }
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
        public ObservableCollection<NodeModel> Steps
        {
            get;
            set;
        } = new ObservableCollection<NodeModel> { };

        /// <summary>
        /// 是否调试
        /// </summary>
        public bool IsDebug { get; set; }
        public string Note { get; set; }
        public bool IsPip { get; set; }
        public List<Flow> ChildrenFlows { get; set; }
        #region 流程处理
        /// <summary>
        /// 参数管理器
        /// </summary>
        public Dictionary<string, ValueInfo> ParamsManager
        {
            get;
            set;
        } = new Dictionary<string, ValueInfo>();

        #endregion


    }

}
