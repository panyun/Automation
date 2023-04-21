using EL;
using Interop.UIAutomationClient;

namespace Automation.Inspect
{
    /// <summary>
    /// 展示空间树
    /// </summary>
    public class WinFormInspectComponent : Entity
    {
        /// <summary>
        /// 缓存节点
        /// </summary>
        public List<ElementPath> ElementPaths { get; set; }

        public object NotSupportedValue => WinFormInspectComponent.Instance.UIAFactory.ReservedNotSupportedValue;

        public Dictionary<string, ElementUIA> Elements = new Dictionary<string, ElementUIA>();
        public List<ElementNode> RootNodes = new List<ElementNode>();
        public static WinFormInspectComponent? Instance { get; set; }
        /// <summary>
        /// 根节点
        /// </summary>
        public IUIAutomationElement RootElement { get; set; }
        /// <summary>
        /// UIA 对象
        /// </summary>
        public IUIAutomation UIAFactory { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IUIAutomationTreeWalker ControlViewWalker { get; set; }
        public IUIAutomationTreeWalker RawViewWalker { get; set; }
        public IUIAutomationTreeWalker ContentViewWalker { get; set; }
        public IUIAutomationCacheRequest CacheRequest { get { return UIAFactory.CreateCacheRequest(); } }

    }

}
