using EL;
using System.Drawing;
using Interop.UIAutomationClient;

namespace Automation.Inspect
{
    public class WinPathComponent : Entity
    {
        public Dictionary<IUIAutomationElement, ElementPath> PathNodes = new Dictionary<IUIAutomationElement, ElementPath>();
        /// <summary>
        /// 查询的节点集合
        /// </summary>
        public Dictionary<int, ElementNode> ElementNodes { get; set; } = new Dictionary<int, ElementNode>();
    }
   

}
