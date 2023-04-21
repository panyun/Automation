using Automation.Inspect;
using EL;
using EL.UIA;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Interop.UIAutomationClient;
using EL.Overlay;
using EL.Async;
using System.Drawing;
using System.Threading;
using WindowsAccessBridgeInterop;

namespace Automation.Parser
{
    public class ElementExpand
    {
        public string Name { get; set; }
        public string CompareId { get; set; }
        public string CompareTempId { get; set; }
        public string CompareChildrenId { get; set; }
    }

    /// <summary>
    /// 定位入口
    /// </summary>
    public static class ElementNodeSystem
    {
        #region GenerateRuntime Java
        /// <summary>
        /// 生成比较值
        /// </summary>
        public static void GenerateCompareId_Java(this ElementNode self)
        {
            if (self.CurrentElementJava == null) return;
            var pro = self.CurrentElementJava.Properties.FirstOrDefault(x => x.Name == "Focused element");
            if (pro != null)
                self.CurrentElementJava.Properties.Remove(pro);
            self.CompareValues = new Dictionary<string, string>()
            {
                {nameof(ElementExpand.Name).ToLower(), ElemenetSystem.GetCompareId(self.CurrentElementJava.Name) },
                       {nameof(ElementExpand.CompareId).ToLower(), ElemenetSystem.GetCompareId(JsonHelper.ToJson(self.CurrentElementJava.Properties)) },
                {nameof(AccessibleNode.JvmId),self.CurrentElementJava.AccessibleNode.JvmId+""},
            };
        }
        public static bool GetCompareGJvmId(this ElementNode self, AccessibleNode accessibleNode)
        {
            return self.CompareValues[nameof(AccessibleNode.JvmId)] == accessibleNode.JvmId + "";
        }
        #endregion
        #region GenerateRuntime Win
        public static bool CompareId(this IUIAutomationElement uIAutomationElement, string compareId)
        {
            return ElemenetSystem.GetCompareId(uIAutomationElement) == compareId;
        }
        public static string GetCompareValue(this ElementNode self, string key)
        {
            self.CompareValues.TryGetValue(key.ToLower(), out var value);
            return value;
        }
        public static ElementExpand GetElementExpand(this ElementNode self)
        {
            ElementExpand elementExpand = new ElementExpand();
            elementExpand.Name = self.CompareValues[nameof(ElementExpand.Name).ToLower()];
            elementExpand.CompareId = self.CompareValues[nameof(ElementExpand.CompareId).ToLower()];
            elementExpand.CompareTempId = self.CompareValues[nameof(ElementExpand.CompareTempId).ToLower()];
            elementExpand.CompareChildrenId = self.CompareValues[nameof(ElementExpand.CompareChildrenId).ToLower()];
            return elementExpand;
        }
        /// <summary>
        /// 生成比较值
        /// </summary>
        public static void GenerateCompareId(this ElementNode self)
        {
            if (self.CurrentElementWin.NativeElement == null) return;
            self.CompareValues = new Dictionary<string, string>()
            {
                {nameof(ElementExpand.Name).ToLower(), ElemenetSystem.GetCompareId(self.CurrentElementWin.NativeElement.CurrentName) },
                       {nameof(ElementExpand.CompareId).ToLower(), ElemenetSystem.GetCompareId(self.CurrentElementWin.NativeElement) },
                       {nameof(ElementExpand.CompareTempId).ToLower(), ElemenetSystem.GetCompareTempId(self.CurrentElementWin.NativeElement) },
            };
            //PrintComponent.PrintCatch(self);
        }
        public static void GenerateCompareChildrenId(this ElementNode self)
        {
            var inspect = Boot.GetComponent<InspectComponent>();
            if (self.CompareValues == null) self.CompareValues = new Dictionary<string, string>();
            var winformInspect = inspect.GetComponent<WinFormInspectComponent>();
            if (!self.CompareValues.ContainsKey(nameof(ElementExpand.CompareChildrenId).ToLower()))
            {
                self.CompareValues.Add(nameof(ElementExpand.CompareChildrenId).ToLower(), winformInspect.GetRuntimeChildrenId(self.CurrentElementWin.NativeElement));
            }
        }
        public static bool CompareId(this ElementNode self, IUIAutomationElement uIAutomationElement)
        {
            return self.GetCompareValue(nameof(ElementExpand.CompareId).ToLower()) ==
                   ElemenetSystem.GetCompareId(uIAutomationElement);
        }
        public static bool CompareTempId(this ElementNode self, IUIAutomationElement uIAutomationElement)
        {
            return self.GetCompareValue(nameof(ElementExpand.CompareTempId).ToLower()) ==
       ElemenetSystem.GetCompareTempId(uIAutomationElement);
        }
        public static bool CompareChildrenId(this ElementNode self, IUIAutomationElement uIAutomationElement)
        {
            var inspect = Boot.GetComponent<InspectComponent>();
            var winformInspect = inspect.GetComponent<WinFormInspectComponent>();
            return self.GetCompareValue(nameof(ElementExpand.CompareChildrenId).ToLower()) ==
winformInspect.GetRuntimeChildrenId(uIAutomationElement);
        }
        public static bool CompareNameId(this ElementNode self, IUIAutomationElement uIAutomationElement)
        {
            return self.GetCompareValue(nameof(ElementExpand.Name).ToLower()) ==
                  ElemenetSystem.GetCompareId(uIAutomationElement.CurrentName);
        }
        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string EscapeDataString(this string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return default;
            //str = Uri.EscapeDataString(str);
            //str = Regex.Replace(str, @"//",@"/");
            str = Regex.Replace(str, @"\\", @"/");
            str = Regex.Replace(str, "/+", @"/");
            return str;
        }

        public static T TryCall<T>(Func<T> action)
        {
            try
            {
                return (T)action.Invoke();
            }
            catch (Exception ex)
            {

            }
            return default;
        }
        public static void TryCall(Action action)
        {
            try
            {
                action.Invoke();
            }
            catch (Exception ex)
            {

            }
        }
        public static void LightHighMany(this LightProperty self, Action action = default, params Element[] elements)
        {
            var inspect = Boot.GetComponent<InspectComponent>();
            var winFormInspectComponent = inspect.GetComponent<WinFormInspectComponent>();
            var formOver = inspect.GetComponent<FormOverLayComponent>();
            if (elements == null || elements.Length == 0) return;
            var el = elements[0];
            el.SetForeground();
            if (!self.IsLight) return;
            List<Rectangle> rect = new List<Rectangle>();
            foreach (var element in elements)
                rect.Add(element.BoundingRectangle);
            self.LightHighMany(action, rect.ToArray());
        }
        public static void LightHighShow(this LightProperty self, Rectangle rectangle)
        {
            if (!self.IsLight) return;
            var inspect = Boot.GetComponent<InspectComponent>();
            var formOver = inspect.GetComponent<FormOverLayComponent>();
            formOver.LightHighShow(self.Color, rectangle);
        }
        public static void LightHighMany(this LightProperty self, Action action = default, params Rectangle[] rectangles)
        {
            if (!self.IsLight) return;
            var inspect = Boot.GetComponent<InspectComponent>();
            var winFormInspectComponent = inspect.GetComponent<WinFormInspectComponent>();
            var formOver = inspect.GetComponent<FormOverLayComponent>();
            if (rectangles != default && rectangles.Count() > 0)
            {
                formOver.ELTaskOverLay = ELTask<dynamic>.Create();
                if (self.CancellationTokenSource == default)
                    formOver.LightHighMany(self.Color,
                                       self.Count, self.Time, action).Coroutine();
                else
                    formOver.LightHighMany(self.Color, self.CancellationTokenSource, action).Coroutine();
                var list = new List<dynamic>();
                foreach (var rectangle in rectangles)
                {
                    list.Add(rectangle);
                }
                list = list.OrderBy(x => x.Width * x.Height).ToList();
                formOver.ELTaskOverLay.SetResult(list);
            }
        }

        public static void LightHighMany(this LightProperty self, params Element[] elements)
        {
            self.LightHighMany(default, elements);
        }
    }
    /// <summary>
    /// 精准定位 备用
    /// </summary>
    public static class ElementNodeTempSystem
    {
        private static ElementUIA FindSystemElement(this ElementPath self)
        {
            ElementUIA element = default;
            var inspect = Boot.GetComponent<InspectComponent>();
            var winInspect = inspect.GetComponent<WinFormInspectComponent>();
            var root = FindRoot(self.PathNode);
            var nodes = winInspect.GetRootNodes().Where(x =>
            x.CurrentElementWin.ControlTypeName == root.CurrentElementWin.ControlTypeName &&
            x.CurrentElementWin.Name == root.CurrentElementWin.Name);
            foreach (var item in nodes)
            {
                var node = winInspect.GetAllChildrenNode(item.CurrentElementWin.NativeElement, self.PathNode.LevelIndex);
                nodes = node.GetChildrenNode().Where(x =>
           x.CurrentElementWin.ControlTypeName == self.PathNode.CurrentElementWin.ControlTypeName &&
           x.CurrentElementWin.Name == self.PathNode.CurrentElementWin.Name);
                foreach (var tempNode in nodes)
                {
                    element = tempNode.CurrentElementWin;
                    var parentNode = winInspect.GetAllParentNode(tempNode.CurrentElementWin.NativeElement);
                    if (parentNode.NodeEquals(self.PathNode))
                        break;
                }
            }
            return element;
        }
        private static Process FindProcess(string processName, string name, string className)
        {
            var inspect = Boot.GetComponent<InspectComponent>();
            var processList = Process.GetProcessesByName(processName);
            List<Process> processs = new List<Process>();
            foreach (var proc in processList)
            {
                try
                {
                    var handle = IntPtr.Zero;
                    if (proc.MainWindowHandle != IntPtr.Zero)
                        handle = proc.MainWindowHandle;
                    else handle = proc.Handle;
                    var winformInspect = inspect.GetComponent<WinFormInspectComponent>();
                    var ele = winformInspect.ElementFromHandle(handle);
                    if (ele.Name.EscapeDataString() == name && ele.ClassName == className)
                        processs.Add(proc);
                }
                catch (Exception ex)
                {
                }
            }
            if (processs.Count > 0)
                return processs[0];
            return default;
        }
        private static List<ElementUIA> FindElement(this ElementNode self, int levelIndex)
        {
            var inspect = Boot.GetComponent<InspectComponent>();
            var winInspect = inspect.GetComponent<WinFormInspectComponent>();
            if (self.Parent == null)
                return new List<ElementUIA>() { winInspect.RootElement.Convert() };
            var temps = FindElement(self.Parent, levelIndex);
            if (temps == default) return default;
            var elements = new List<ElementUIA>();
            foreach (var element in temps)
            {
                var arrarElements = element.NativeElement.FindAll(TreeScope.TreeScope_Children,
                    winInspect.CreateCondition(element));
                for (int i = 0; i < arrarElements.Length; i++)
                {
                    var e = arrarElements.GetElement(i);
                    var runtimeId = ElemenetSystem.GetCompareId(e);
                    var runtimeTempId = ElemenetSystem.GetCompareTempId(e);
                    var className = e.CurrentClassName.EscapeDataString();
                    var name = e.CurrentName.EscapeDataString();
                    var oldClassName = self.CurrentElementWin.ClassName.EscapeDataString();
                    var oldName = self.CurrentElementWin.Name.EscapeDataString();
                    if (self.CompareId(e) ||
                      self.CompareChildrenId(e))
                    {
                        if (levelIndex == self.LevelIndex)
                        {
                            if ((className == null || className.Contains(oldClassName) || oldClassName.Contains(className.EscapeDataString())))
                            {
                                elements.Add(e.Convert());
                            }
                        }
                        if ((className == null || className.Contains(oldClassName) || oldClassName.Contains(className.EscapeDataString())) && name == oldName)
                        {
                            elements.Add(e.Convert());
                        }
                    }

                }
            }
            if (elements.Count > 0)
            {
                var json = JsonHelper.ToJson(new
                {
                    elements[0].NativeElement.CurrentAcceleratorKey,
                    elements[0].NativeElement.CurrentAccessKey,
                    elements[0].NativeElement.CurrentAriaRole,
                    elements[0].NativeElement.CurrentAutomationId,
                    elements[0].NativeElement.CurrentControlType,
                    elements[0].NativeElement.CurrentFrameworkId,
                    elements[0].NativeElement.CurrentItemStatus,
                    elements[0].NativeElement.CurrentOrientation,
                });
                Debug.WriteLine(json);
                Debug.WriteLine($"Find:{self.GetCompareValue(nameof(ElementExpand.CompareId))}");
                Log.Trace(json);
                Log.Trace($"Find:{self.GetCompareValue(nameof(ElementExpand.CompareId))}");
            }
            return elements;
        }

        private static ElementNode RemoveRoot(ElementNode node)
        {
            if (node.Parent == null)
                return null;
            node.Parent = RemoveRoot(node.Parent);
            return node;
        }
        private static ElementUIA FindElement(this ElementNode self, string processName, int levelIndex)
        {
            var inspect = Boot.GetComponent<InspectComponent>();
            var winInspect = inspect.GetComponent<WinFormInspectComponent>();
            if (self.Parent == null)
            {
                return default;
            }
            var element = FindElement(self.Parent, processName, levelIndex);
            if (element == null)
            {
                var win = self.CurrentElementWin as ElementUIA;
                var process = FindProcess(processName, self.CurrentElementWin.Name, win.ClassName);
                if (process == default)
                {
                    var node = winInspect.GetRootNodes().FirstOrDefault(x =>
             x.CurrentElementWin.ControlTypeName == self.CurrentElementWin.ControlTypeName &&
             x.CurrentElementWin.Name == self.CurrentElementWin.Name && x.CurrentElementWin.ControlType == self.CurrentElementWin.ControlType);
                    if (node != null)
                        return node.CurrentElementWin;
                    return default;
                }
                else
                {
                    var handle = IntPtr.Zero;
                    handle = process.MainWindowHandle;
                    var winformInspect = inspect.GetComponent<WinFormInspectComponent>();
                    ElementUIA temp1 = winformInspect.ElementFromHandle(handle);
                    self.CurrentElementWin.NativeElement = temp1.NativeElement;
                    return self.CurrentElementWin;
                }
            }
            var winFormInspectComponent = inspect.GetComponent<WinFormInspectComponent>();
            var array = element.NativeElement.FindAll(TreeScope.TreeScope_Children, winInspect.CreateCondition(element));
            IUIAutomationElement currentElement = null;
            for (int i = 0; i < array.Length; i++)
            {
                var e = array.GetElement(i);
                if (levelIndex == self.LevelIndex && !string.IsNullOrWhiteSpace(self.CurrentElementWin.Role))
                {
                    currentElement = e;
                    break;
                }
                if (e.CurrentName.StringCompare(self.CurrentElementWin.Name))
                {
                    currentElement = e;
                    break;
                }
            }
            if (currentElement == null)
                return default;
            var temp = currentElement.Convert();
            self.CurrentElementWin.NativeElement = temp.NativeElement;
            self.CurrentElementWin.BoundingRectangle = temp.BoundingRectangle;
            return self.CurrentElementWin;
        }
        private static bool NodeEquals(this ElementNode node, ElementNode node2)
        {
            if (node == null) return false;
            if (node2 == null) return false;
            if (node.LevelIndex != node2.LevelIndex) return false;
            bool isEqueals = false;
            if (node.CurrentElementWin.ControlTypeName == node2.CurrentElementWin.ControlTypeName
                && node.CurrentElementWin.Name == node2.CurrentElementWin.Name
                && (string.IsNullOrWhiteSpace(node.CurrentElementWin.Value) || node.CurrentElementWin.Value == node2.CurrentElementWin.Value)
                )
            {
                if (node.Parent == null && node2.Parent == null) return true;
                isEqueals = NodeEquals(node.Parent, node2.Parent);
                return isEqueals;
            }
            return false;
        }
        private static ElementNode FindRoot(this ElementNode node)
        {
            if (node.Parent == null) return node;
            return FindRoot(node.Parent);
        }
        private static List<ElementUIA> FindElementByRoot(this ElementNode self)
        {
            var inspect = Boot.GetComponent<InspectComponent>();
            var winInspect = inspect.GetComponent<WinFormInspectComponent>();
            List<ElementUIA> elements = default;
            if (self.Parent == null)
            {
                var nodes = winInspect.GetRootNodes().Where(x =>
                x.CurrentElementWin.ControlTypeName == self.CurrentElementWin.ControlTypeName &&
                x.CurrentElementWin.Name == self.CurrentElementWin.Name).ToList();
                return nodes.Select(x => x.CurrentElementWin).ToList();
            }
            elements = FindElementByRoot(self.Parent);
            if (elements == null || elements.Count == 0) return default;
            var temps = new List<ElementUIA>();
            foreach (var tempNode in elements)
            {
                #region findall  
                var conditionName = winInspect.UIAFactory.CreatePropertyCondition(UIA_PropertyIds.UIA_NamePropertyId, self.CurrentElementWin.Name);
                var conditionClassName = winInspect.UIAFactory.CreatePropertyCondition(UIA_PropertyIds.UIA_ClassNamePropertyId, self.CurrentElementWin.ClassName);
                var conditionType = winInspect.UIAFactory.CreatePropertyCondition(UIA_PropertyIds.UIA_ControlTypePropertyId, self.CurrentElementWin.ControlType);
                var list = new List<IUIAutomationCondition>() { conditionClassName, conditionType };
                var condition = winInspect.UIAFactory.CreateAndConditionFromArray(list.ToArray());
                var sonElements = tempNode.NativeElement.FindAll(TreeScope.TreeScope_Children, condition);
                #endregion
                if (sonElements.Length == 0)
                    continue;
                for (int i = 0; i < sonElements.Length; i++)
                {
                    if (sonElements.GetElement(i).CurrentName.EscapeDataString() == self.CurrentElementWin.Name.EscapeDataString())
                        temps.Add(sonElements.GetElement(i).Convert());

                }
                return temps;
            }
            return default;
        }

        private static bool StringCompare(this string str, string s)
        {
            return str.EscapeDataString() == s.EscapeDataString();
        }
    }
}
