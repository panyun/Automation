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
    public class ElementNodeAvigationSystem_ConsineSimilarity : IAvigation
    {
        public List<ElementEdit> ElementNodes { get; set; }
        private ElementPath ElementPath;
        public List<IUIAutomationElement> Elements = new();
        /// <summary>
        /// 流程名称
        /// </summary>
        private List<int> CurrentProcessIds { get; set; }
        public ElementEdit GetElement(string id)
        {
            return ElementNodes.FirstOrDefault(x => x.Id == id);
        }
        public IEnumerable<Element> TryAvigation(ElementPath elementPath, int timeOut)
        {
            ElementNodes = elementPath.ElementEditNodes;
            ElementPath = elementPath;
            if (elementPath.PathNode.CurrentElementWin.NativeElement != null)
            {
                CurrentProcessIds = new List<int>()
                {
                      elementPath.PathNode.CurrentElementWin.NativeElement.CurrentProcessId
                };
            }
            else
            {
                CurrentProcessIds = new List<int>();
                var ids = Process.GetProcessesByName(elementPath.ProcessName);
                for (int i = 0; i < ids.Length; i++)
                {
                    CurrentProcessIds.Add(ids[i].Id);
                }
            }

            if (timeOut < 1000) timeOut = 10000;
            int sunTime = 0;
            Stopwatch sw = new Stopwatch();
            sw.Restart();
            while (true)
            {

                ElementNodeSystem.TryCall(() => SearchElement(elementPath));
                if (Elements != default && Elements.Count > 0)
                    break;
                sunTime += (int)sw.ElapsedMilliseconds;
                if (sunTime > timeOut) break;
            }
            sw.Stop();
            if (Elements == default || Elements.Count == 0)
            {
                var parser = Boot.GetComponent<ParserComponent>();
                var requestLog = parser.GetComponent<RequestOptionComponent>();
                throw new ParserException($"@{requestLog.ElementPathId}:通过节点元素路径未找到目标元素！超时时间：{requestLog.TimeOut}");
            }
            var rtn = Elements.Select(x => x.Convert()).ToList();
            return rtn.Distinct();
        }
        /// <summary>
        /// 精度搜素
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public List<IUIAutomationElement> SearchElementTemp(ElementNode self)
        {
            var inspect = Boot.GetComponent<InspectComponent>();
            var winInspect = inspect.GetComponent<WinFormInspectComponent>();
            if (self.Parent == null)
                return new List<IUIAutomationElement>() { winInspect.RootElement };
            var temps = SearchElementTemp(self.Parent);
            if (temps == default) return default;
            var elementsRuntime = new List<IUIAutomationElement>();
            var elementsRuntimeTemp = new List<IUIAutomationElement>();
            var temps1 = new List<IUIAutomationElement>();
            ElementEdit elementEdit = GetElement(self.Id);
            if (!elementEdit.IsChecked)
                return temps;
            var namePro = elementEdit.GetProperty(nameof(ElementUIA.Name));
            int index = 0;
            //是否为统一界面和同一程序
            foreach (var element in temps)
            {
                index++;
                var id = (int)element.GetCurrentPropertyValue(UIA_PropertyIds.UIA_ProcessIdPropertyId);
                if (CurrentProcessIds.Contains(id))
                {
                    var win = element.Convert();
                    var temp = element.GetNativeWindowHandle().CurrentName.EscapeDataString();
                    if (win.NativeMainTitle == temp) //在同一进程\同一界面
                    {
                        var all = element.FindAll(TreeScope.TreeScope_Children, winInspect.CreateAllCondition());

                        for (int i = 0; i < all.Length; i++)
                        {
                            var e = all.GetElement(i);
                            elementsRuntime.Add(e);
                            Elements.Add(e);
                        }
                        continue;
                    }
                }

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
            PrintComponent.PrintAvigation(self, elementsRuntime);
            return elementsRuntime;
        }
        /// <summary>
        /// 精度搜素
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public void SearchElement(ElementPath self)
        {
            var element = self.PathNode.CurrentElementWin.NativeElement;
            if (element == default)
            {
                SearchElementTemp(self.PathNode);
                return;
            }
            var windowElement = element.GetNativeWindowHandle();
            var inspect = Boot.GetComponent<InspectComponent>();
            var winInspect = inspect.GetComponent<WinFormInspectComponent>();
            var conditionProcess = winInspect.UIAFactory.CreatePropertyCondition(UIA_PropertyIds.UIA_ProcessIdPropertyId, element.CurrentProcessId);
            var conditionAutomationIdProperty = winInspect.UIAFactory.CreatePropertyCondition(UIA_PropertyIds.UIA_AutomationIdPropertyId, element.CurrentAutomationId);
            var list = new List<IUIAutomationCondition>() { conditionProcess, conditionAutomationIdProperty };
            var con = winInspect.UIAFactory.CreateAndConditionFromArray(list.ToArray());
            var elements = windowElement.FindAll(TreeScope.TreeScope_Descendants, con);
            for (int i = 0; i < elements.Length; i++)
                Elements.Add(elements.GetElement(i));
        }
    }
}
