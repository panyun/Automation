using Automation.Inspect;
using EL;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Interop.UIAutomationClient;
using CommandLine;

namespace Automation.Parser
{
    /// <summary>
    /// 节点编辑方法入口
    /// </summary>
    public class ElementNodeAvigationSystem_Similarity : IAvigation
    {
        public List<ElementEdit> ElementNodes { get; set; }
        private ElementPath ElementPath;
        public ElementEdit GetElement(string id)
        {
            return ElementNodes.FirstOrDefault(x => x.Id == id);
        }
        public IEnumerable<Element> TryAvigation(ElementPath elementPath, int timeOut)
        {
            ElementNodes = elementPath.ElementEditNodes;
            ElementPath = elementPath;
            List<IUIAutomationElement> elements = default;
            if (timeOut < 1000) timeOut = 10000;
            int sunTime = 0;
            Stopwatch sw = new Stopwatch();
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
            if (elements == default || elements.Count == 0)
            {
                var parser = Boot.GetComponent<ParserComponent>();
                var requestLog = parser.GetComponent<RequestOptionComponent>();
                throw new ParserException($"@{requestLog.ElementPathId}:通过节点元素路径未找到目标元素！超时时间：{requestLog.TimeOut}");
            }
            return elements.Select(x => (Element)x.Convert()).ToList();
        }
        /// <summary>
        /// 精度搜素
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public List<IUIAutomationElement> SearchElement(ElementNode self)
        {
            var inspect = Boot.GetComponent<InspectComponent>();
            var winInspect = inspect.GetComponent<WinFormInspectComponent>();
            if (self.Parent == null)
                return new List<IUIAutomationElement>() { winInspect.RootElement };
            var temps = SearchElement(self.Parent);
            if (temps == default) return default;
            var elementsRuntime = new List<IUIAutomationElement>();
            var elementsRuntimeTemp = new List<IUIAutomationElement>();
            var temps1 = new List<IUIAutomationElement>();
            ElementEdit elementEdit = GetElement(self.Id);
            if (!elementEdit.IsChecked)
                return temps;
            var namePro = elementEdit.GetProperty(nameof(ElementUIA.Name));
            if (elementEdit.IsSimilarity) // 相似元素
            {
                foreach (var element in temps)
                {
                    var arrarElements = element.FindAll(TreeScope.TreeScope_Children, winInspect.CreateCondition(self.CurrentElementWin));
                    for (int i = 0; i < arrarElements.Length; i++)
                    {
                        var e = arrarElements.GetElement(i);
                        temps1.Add(e);
                    }
                }
                return temps1;
            }
            foreach (var element in temps)
            {
                var arrarElements = element.FindAll(TreeScope.TreeScope_Children, winInspect.CreateCondition(self.CurrentElementWin));
                for (int i = 0; i < arrarElements.Length; i++)
                {
                    var e = arrarElements.GetElement(i);
                    temps1.Add(e);
                    if (!elementEdit.IsEdit)
                    {
                        if (self.CompareId(e))
                            elementsRuntime.Add(e);
                        else if (self.CompareTempId(e))
                            elementsRuntimeTemp.Add(e);
                        continue;
                    }
                    Log.Trace($"currentName:{e.CurrentName}; p:{elementEdit.GetProperty("name").Value}; isPass:{elementEdit.GetProperty(nameof(ElementUIA.Name)).IsEqual(e.CurrentName)}");
                    if (namePro.IsActive && namePro.IsEqual(e.CurrentName))
                    {
                        elementsRuntime.Add(e);
                        break;
                    }
                }
            }
            //PrintComponent.PrintAvigation(self, elementsRuntime);
            //最后一级
            if (ElementPath.PathNode.LevelIndex == self.LevelIndex)
                return elementsRuntimeTemp.Concat(elementsRuntime).ToList();
            return elementsRuntime;
        }
    }
}
