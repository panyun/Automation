using EL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsAccessBridgeInterop;

namespace Automation.Inspect
{
    public class JavaPathComponent : Entity
    {
        public Dictionary<AccessibleNode, ElementPath> PathNodes = new Dictionary<AccessibleNode, ElementPath>();
        /// <summary>
        /// 查询的节点集合
        /// </summary>
        public Dictionary<AccessibleNode, ElementNode> ElementNodes { get; set; } = new Dictionary<AccessibleNode, ElementNode>();
    }
}
