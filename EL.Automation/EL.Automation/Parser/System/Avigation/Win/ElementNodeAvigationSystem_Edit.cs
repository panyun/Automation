using Automation.Inspect;
using EL;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Interop.UIAutomationClient;

namespace Automation.Parser
{
    /// <summary>
    /// 节点编辑方法入口
    /// </summary>
    public class ElementNodeAvigationSystem_Edit : IAvigation
    {
        public List<ElementEdit> ElementNodes { get; set; }
        public ElementEdit GetElement(string id)
        {
            return ElementNodes.FirstOrDefault(x => x.Id == id);
        }
        public IEnumerable<Element> TryAvigation(ElementPath elementPath, int timeOut)
        {
            ElementNodes = elementPath.ElementEditNodes;
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
            return FindElement(elementPath.PathNode, elements);
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

            if (elementsRuntime.Count != 1)
            {
                if (temps.Count == 1 && temps1.Count > self.Index && self.IsFindIndex)
                {
                    int index = self.Index;
                    if (elementEdit.GetProperty(nameof(self.Index)).IsEdit)
                        index = elementEdit.GetProperty(nameof(self.Index)).GetValueInt();
                    elementsRuntime = new List<IUIAutomationElement>() { temps1[index] };
                }
                else if (elementsRuntime.Count == 0 && elementsRuntimeTemp.Count > 0)
                    elementsRuntime = elementsRuntimeTemp;
                else if (elementsRuntime.Count == 0 && temps1.Count > 0)
                    elementsRuntime = temps1;
            }
            //PrintComponent.PrintAvigation(self, elementsRuntime);
            return elementsRuntime;
        }

        public List<Element> SearchElement(ElementPath elementPath, int timeOut)
        {
            ElementNodes = elementPath.ElementEditNodes;
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
            //var elementWin = elements.Select(x => x.Convert()).ToList();
            return FindElement(elementPath.PathNode, elements);
        }
        public List<Element> FindElement(ElementNode self, List<IUIAutomationElement> elements)
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
            ElementEdit elementEdit = GetElement(self.Id);
            if (elementEdit.IsEdit)
            {
                if (!elementEdit.IsChecked)
                    return elements.Select(x => (Element)x.Convert()).ToList();
                //然后优先匹配name信息
                var elements_Name = elements.Where(x => elementEdit.GetProperty(nameof(self.CurrentElementWin.Name)).IsEqual(x.CurrentName)).ToList();
                if (elements_Name != null && elements_Name.Count() == 1)
                    return new List<Element>() { elements_Name[0].Convert() };
                int index = self.Index;
                if (elementEdit.GetProperty(nameof(self.Index)).IsEdit)
                    index = elementEdit.GetProperty(nameof(self.Index)).GetValueInt();

                //再去匹配序号
                if (elements.Count > self.Index)
                    return new List<Element>() { elements[index].Convert() };
            }
            //优先查找runtimeId;
            var elementsRuntimeId = elements.Where(x => self.CompareId(x)).ToList();
            if (elementsRuntimeId.Count == 1)
                return elementsRuntimeId.Select(x => (Element)x.Convert()).ToList();

            //再去匹配序号
            if (elements.Count >= self.Index)
                return new List<Element>() { elements[self.Index].Convert() };

            //然后优先匹配name信息
            var elementsName = elements.Where(x => x.CurrentName.EscapeDataString() == self.CurrentElementWin.Name).ToList();
            if (elementsName != null && elementsName.Count() == 1)
                return new List<Element>() { elementsName[0].Convert() };

            //最后匹配className
            var elementsClassName = elements.Where(x => x.CurrentClassName == self.CurrentElementWin.ClassName).ToList();
            if (elementsClassName != null && elementsClassName.Count() == 1)
                return new List<Element>() { elementsClassName[0].Convert() };

            //最后多项匹配
            if (elementsRuntimeId.Count > 1)
            {
                foreach (var item in elementsRuntimeId)
                {
                    if (self.CompareChildrenId(item))
                        return new List<Element>() { item.Convert() };
                }
            }

            #region 多项匹配
            //最后多项匹配
            if (elementsName != null && elementsName.Count() > 1)
            {
                elementsName = elementsName.Where(x => x.CurrentClassName == self.CurrentElementWin.ClassName).ToList();
                // 如果name相等并且className也相等，即返回第一个。
                if (elementsName != null)
                    return new List<Element>() { elementsName[0].Convert() };
            }
            //最后多项匹配
            if (elementsClassName != null && elementsClassName.Count() > 0) return new List<Element>() { elementsClassName[0].Convert() };
            throw new ParserException("未定位到目标元素！");
            //return elements.Select(x => x.Convert()).ToList();
            #endregion
        }
    }
}
