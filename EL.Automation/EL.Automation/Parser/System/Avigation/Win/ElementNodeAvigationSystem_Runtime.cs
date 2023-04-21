using Automation.Inspect;
using EL;
using System.Diagnostics;
using System.Text;
using Interop.UIAutomationClient;

namespace Automation.Parser
{
    /// <summary>
    /// 定位主体方法
    /// </summary>
    public class ElementNodeAvigationSystem_Runtime : IAvigation
    {
        /// <summary>
        /// 精度搜素
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public List<IUIAutomationElement> SearchElement(ElementPath self)
        {
            var runtime = self.PathNode.PathRuntime;
            var inspect = Boot.GetComponent<InspectComponent>();
            var winfromInspect = inspect.GetComponent<WinFormInspectComponent>();
            var elemnt = winfromInspect.UIAFactory.ElementFromHandle(runtime.Handle);
            var con = winfromInspect.UIAFactory.CreatePropertyCondition(UIA_PropertyIds.UIA_RuntimeIdPropertyId, runtime.RuntimeId);
            var elements = elemnt.FindAll(TreeScope.TreeScope_Descendants | TreeScope.TreeScope_Element, con);
            if (elements == default || elements.Length == 0)
                return default;

            return new List<IUIAutomationElement>() { elements.GetElement(0) };
        }
        public IEnumerable<Element> TryAvigation(ElementPath elementPath, int timeOut)
        {
            List<IUIAutomationElement> elements = default;
            if (timeOut < 1000) timeOut = 10000;
            Stopwatch sw = new();
            sw.Restart();
            int index = 0;
            Log.Trace("--------搜索路径开始------");
            while (true)
            {
                elements = ElementNodeSystem.TryCall(() => SearchElement(elementPath));
                if (elements != default && elements.Count > 0)
                {
                    Log.Trace($"循环次数{index},用时：{sw.Elapsed.TotalMilliseconds}");
                    break;
                }
                // 进入正常的路径引擎
                var avigation = new ElementNodeAvigationSystem();
                return avigation.TryAvigation(elementPath, timeOut);
            }
            Log.Trace($"--------搜索路径结束{sw.Elapsed.TotalMilliseconds}------");
            sw.Stop();
            var parser = Boot.GetComponent<ParserComponent>();
            var requestLog = parser.GetComponent<RequestOptionComponent>();
            if (elements == default || elements.Count == 0)
            {
                throw new ParserException($"@{requestLog.ElementPathId}:通过节点元素路径未找到目标元素！超时时间：{requestLog.TimeOut}");
            }
            return elements.Select(x => (Element)x.Convert()).ToList();
        }
    }

}
