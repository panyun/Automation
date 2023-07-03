using AutoMapper;
using CommunityToolkit.Mvvm.ComponentModel;
using EL.Robot.Component;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ViewModel;

namespace MiniRobotForm.Mode
{
    [AutoMap(typeof(Node))]
    public class NodeModel : ObservableObject
    {
        private string _Note;
        public string Note { get => _Note; set => SetProperty(ref _Note, value); }
        public int Index { get; set; } = 1;
        public long Id { get; set; }
        public bool IsEnd
        {
            get
            {
                if (ComponentName == "BlockEndComponent") return true;
                return false;
            }
        }
        public bool IsView { get; set; } = true;
        public bool IsBlock { get; set; } = false;
        public Node LinkNode { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        public string ComponentName { get; set; }
        public string Annotation { get; set; }
        public bool Ignore { get; set; }
        public bool Debug { get; set; }
        public bool IsLock { get; set; }
        public string OutParameterName { get; set; }
        public List<Parameter> Parameters
        {
            get; set;
        }
        public string DisplayExp
        {
            get; set;
        }
        public List<Node> Children { get; set; } = new List<Node>();
        public List<Parameter> InParams { get; set; }
        #region 参数处理
        public Dictionary<string, ValueInfo> DictionaryParam { get; set; }
        public string Img { get; set; }
        #endregion
    }
    
}
