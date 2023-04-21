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
    public class ElementNodeAvigationSystem : IAvigation
    {
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
            if (temps == default || temps.Count == 0)
                return default;
            var elementsRuntime = new List<IUIAutomationElement>();
            var elementsRuntimeTemp = new List<IUIAutomationElement>();
            var temps1 = new List<IUIAutomationElement>();
            IUIAutomationElementArray arrarElements = default;
            foreach (var element in temps)
            {
                arrarElements = element.FindAll(TreeScope.TreeScope_Children,
                  winInspect.CreateCondition(self.CurrentElementWin));
                Log.Trace($"1 {self.CurrentElementWin.Role} {self.CurrentElementWin.ControlType}");
                for (int i = 0; i < arrarElements.Length; i++)
                {
                    var e = arrarElements.GetElement(i);
                    temps1.Add(e);
                    if (self.CompareId(e))
                        elementsRuntime.Add(e);
                    else if (self.CompareTempId(e))
                        elementsRuntimeTemp.Add(e);
                }
            }
            if (elementsRuntime.Count != 1)
            {
                if (temps.Count == 1 && temps1.Count > self.Index && self.IsFindIndex)
                    elementsRuntime = new List<IUIAutomationElement>() { temps1[self.Index] };
                else if (elementsRuntime.Count == 0 && elementsRuntimeTemp.Count > 0)
                    elementsRuntime = elementsRuntimeTemp;
                else if (elementsRuntime.Count == 0 && temps1.Count > 0)
                    elementsRuntime = temps1;
            }

            //PrintComponent.PrintAvigation(self, elementsRuntime);
            //Log.Trace($"解析路径结束 层级：{self.LevelIndex}" +
            //    $" 节点数：{arrarElements.Length} " +
            //    $"查找对应项数：{elementsRuntime.Count}");
            return elementsRuntime;
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
                elements = ElementNodeSystem.TryCall(() => SearchElement(elementPath.PathNode));

                if (elements != default && elements.Count > 0)
                {
                    Log.Trace($"循环次数{index},用时：{sw.Elapsed.TotalMilliseconds}");
                    break;
                }
                if (sw.Elapsed.TotalMilliseconds > timeOut) break;
            }
            Log.Trace($"--------搜索路径结束{sw.Elapsed.TotalMilliseconds}------");
            sw.Stop();
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

            //优先查找runtimeId;
            var elementsRuntimeId = elements.Where(x => self.CompareId(x)).ToList();
            if (elementsRuntimeId.Count == 1)
                return elementsRuntimeId.Select(x => (Element)x.Convert()).ToList();

            //再去匹配序号
            if (elements.Count > self.Index)
                return new List<Element>() { elements[self.Index].Convert() };

            //然后优先匹配name信息
            var elementsName = elements.Where(x => self.CompareNameId(x)).ToList();
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
