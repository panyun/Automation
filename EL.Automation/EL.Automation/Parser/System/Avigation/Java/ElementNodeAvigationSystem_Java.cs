using Automation.Inspect;
using EL;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Interop.UIAutomationClient;
using WindowsAccessBridgeInterop;

namespace Automation.Parser
{
    public class ElementNodeAvigationSystem_Java : IAvigation
    {
        //[return: MarshalAs(UnmanagedType.Bool)]
        //[DllImport("WindowsAccessBridge-64.dll", CallingConvention = CallingConvention.Cdecl, ThrowOnUnmappableChar = true, CharSet = CharSet.Unicode)]
        //public extern static bool setTextContents(Int32 vmID, AccessibleContext accessibleContext, string text);
        public List<Element> FindElement(ElementNode self, List<AccessibleNode> elements)
        {
            var parser = Boot.GetComponent<ParserComponent>();
            var requestLog = parser.GetComponent<RequestOptionComponent>();

            if (elements == default || elements.Count == 0)
            {
                throw new ParserException($"@{requestLog.ElementPathId}:通过节点元素路径未找到目标元素！超时时间：{requestLog.TimeOut}");
            }
            elements = elements.Distinct().ToList();
            if (elements.Count == 1)
                return elements.Select(x => (Element)x.Convert()).ToList();
            return default;
        }

        public IEnumerable<Element> TryAvigation(ElementPath elementPath, int timeOut)
        {
            List<AccessibleNode> elements = default;
            if (timeOut < 1000) timeOut = 10000;
            int sunTime = 0;
            Stopwatch sw = new Stopwatch();
            var inspect = Boot.GetComponent<InspectComponent>();
            var javaInspect = inspect.GetComponent<JavaFormInspectComponent>();
            javaInspect.RefreshJvms().Coroutine();
            sw.Restart();
            while (true)
            {
                elements = ElementNodeSystem.TryCall(() => SearchElement(elementPath.PathNode));
                if (elements != default && elements.Count > 0)
                    break;
                sunTime += (int)sw.ElapsedMilliseconds;
                if (sunTime > timeOut) break;
            }
            sw.Stop();
            return FindElement(elementPath.PathNode, elements);
        }

        public List<AccessibleNode> SearchElement(ElementNode self)
        {
            var inspect = Boot.GetComponent<InspectComponent>();
            var javaInspect = inspect.GetComponent<JavaFormInspectComponent>();

            if (self.Parent == null)
            {
                var nodes = javaInspect.Jvms.SelectMany(x => x.Windows).ToArray();
                if (nodes == null) return default;
                if (nodes.Length > self.Index)
                {
                    nodes[self.Index].Refresh();
                    return new List<AccessibleNode>() { nodes[self.Index] };
                }
                return default;
            }
            var temp = SearchElement(self.Parent);
            if (temp == default) return default;
            var arrarElements = temp[0].GetChildren().ToArray();
            if (arrarElements.Length > self.Index)
            {
                arrarElements[self.Index].Refresh();
                return new List<AccessibleNode>() { arrarElements[self.Index] };
            }
            return default;
        }
    }

}
