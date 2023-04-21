using Automation.Parser;
using EL;
using EL.Input;
using EL.MSAA;
using EL.UIA;
using Interop.UIAutomationClient;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Engineering;
using System.Diagnostics;
using System.Drawing;

namespace Automation.Inspect
{
    public class WinFormInspectComponentAwake : AwakeSystem<WinFormInspectComponent>
    {
        public override void Awake(WinFormInspectComponent self)
        {

            //创建对象
            WinFormInspectComponent.Instance = self;
            self.UIAFactory = new CUIAutomation();
            self.AddComponent<WinPathComponent>();

            //获取节点根
            self.RootElement = self.UIAFactory.GetRootElement();
            self.ControlViewWalker = self.UIAFactory.ControlViewWalker;
            self.RawViewWalker = self.UIAFactory.RawViewWalker;
            // 读取缓存元素节点

        }
    }
    public static partial class WinFormInspectComponentSystem
    {

        /// <summary>
        /// 获取流程信息
        /// </summary>
        /// <param name="self"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        public static ProcessInfo GetProcessInfo(this WinFormInspectComponent self, IUIAutomationElement e)
        {
            var process = Process.GetProcessById(e.CurrentProcessId);
            if (process == null) return null;
            return new ProcessInfo(process);
        }




        /// <summary>
        /// 根据节点获取下一级目录
        /// </summary>
        /// <param name="self"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        //public static List<ElementUIA> LoadChildren(this WinFormInspectComponent self, IUIAutomationElement element)
        //{
        //    return element.FindAll(TreeScope.TreeScope_Children, self.UIAFactory.CreateTrueCondition()).Converts();
        //}


        /// <summary>
        /// 根据x,y获取坐标点
        /// </summary>
        /// <param name="self"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        //public static IUIAutomationElement ElementFromPoint(this WinFormInspectComponent self, int x, int y)
        //{
        //    return self.UIAFactory.ElementFromPoint(new tagPOINT() { x = x, y = y });
        //}
        /// <summary>
        /// 根据x,y获取坐标点
        /// </summary>
        /// <param name="self"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static IUIAutomationElement ElementFromPoint(this WinFormInspectComponent self)
        {
            try
            {
                Point point = new Point(Mouse.Position.X, Mouse.Position.Y);
                var element = self.UIAFactory.ElementFromPoint(new tagPOINT() { x = point.X, y = point.Y });
                if (element == null) return default;
                var elements = self.GetChild(element, point);
                var elementTemp = element;
                foreach (var item in elements)
                {
                    var area = (item.CurrentBoundingRectangle.right - item.CurrentBoundingRectangle.left) * (item.CurrentBoundingRectangle.bottom - item.CurrentBoundingRectangle.top);
                    var areaElement = (elementTemp.CurrentBoundingRectangle.right - elementTemp.CurrentBoundingRectangle.left) * (elementTemp.CurrentBoundingRectangle.bottom - elementTemp.CurrentBoundingRectangle.top);
                    if (area < areaElement)
                        elementTemp = item;
                }
                // return elementTemp;
                return self.GetParentElementText(elementTemp);
            }
            catch (Exception ex)
            {
                return default;
            }
        }
        public static bool CanCatch(this WinFormInspectComponent self, IUIAutomationElement ele)
        {
            var e = ele.GetNativeWindowHandle();
            var child = e.FindAll(TreeScope.TreeScope_Descendants, self.UIAFactory.CreateTrueCondition());
            return child.Length > 2;
        }
        public static string GetXPath(this WinFormInspectComponent self, IUIAutomationElement ele)
        {
            var parent = self.ControlViewWalker.GetParentElement(ele);
            if (parent == null)
                return "";
            var temp = ControlTypeConverter.ToAriaRoleType(ele.CurrentControlType) + "";
            if (temp.ToLower() == "document")
                return "";
            var path = self.GetXPath(parent);

            return path + "/" + temp;
        }
        public static (string, string, string) GetALLText(this WinFormInspectComponent self, IUIAutomationElement ele)
        {
            var type = EL.UIA.ControlTypeConverter.ToControlType(ele.CurrentControlType);
            if (type == EL.UIA.ControlTypeConverter.ControlType.Text)
            {
                var temp = self.ControlViewWalker.GetParentElement(ele);
                if (!string.IsNullOrEmpty(temp.CurrentName))
                    return (temp.CurrentName, temp.GetValue(), temp.CurrentHelpText);
            }
            return (ele.CurrentName, ele.GetValue(), ele.CurrentHelpText);
        }
        public static IUIAutomationElement GetParentElementText(this WinFormInspectComponent self, IUIAutomationElement ele)
        {
            var type = EL.UIA.ControlTypeConverter.ToControlType(ele.CurrentControlType);
            if (type == EL.UIA.ControlTypeConverter.ControlType.Text)
            {
                var temp = self.ControlViewWalker.GetParentElement(ele);
                if (!string.IsNullOrEmpty(temp.CurrentName))
                    return temp;
            }
            return ele;
        }
        /// <summary>
        /// 获取应用桌面
        /// </summary>
        /// <returns></returns>
        public static IUIAutomationElement ElementFromWindow(this WinFormInspectComponent self)
        {
            return self.WindowFromPoint();
        }
        /// <summary>
        /// 根据x,y获取坐标点
        /// </summary>
        /// <param name="self"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static IUIAutomationElement WindowFromPoint(this WinFormInspectComponent self)
        {
            try
            {
                Point point = new Point(Mouse.Position.X, Mouse.Position.Y);
                IUIAutomationElement element = self.UIAFactory.ElementFromPoint(new tagPOINT() { x = point.X, y = point.Y });

                if (element == null) return default;
                element = self.UIAFactory.ElementFromHandle(element.CurrentNativeWindowHandle);
                return element;
            }
            catch (Exception ex)
            {
                return default;
            }
        }
        //public static IUIAutomationElement ElementFromPointEx(this WinFormInspectComponent self)
        //{
        //    Point point = new Point(Mouse.Position.X, Mouse.Position.Y);
        //    var element = self.ElementFromPoint(Mouse.Position.X, Mouse.Position.Y);
        //    var process = Process.GetProcessById(element.CurrentProcessId);
        //    var mainWindowElement = self.UIAFactory.ElementFromHandle(process.MainWindowHandle);
        //    var elements = self.GetChild(element, point);
        //    var elementTemp = element;
        //    foreach (var item in elements)
        //    {
        //        var area = (item.CurrentBoundingRectangle.right - item.CurrentBoundingRectangle.left) * (item.CurrentBoundingRectangle.bottom - item.CurrentBoundingRectangle.top);
        //        var areaElement = (elementTemp.CurrentBoundingRectangle.right - elementTemp.CurrentBoundingRectangle.left) * (elementTemp.CurrentBoundingRectangle.bottom - elementTemp.CurrentBoundingRectangle.top);
        //        if (area < areaElement)
        //            elementTemp = item;
        //    }
        //    return elementTemp;
        //}
        //public static List<IUIAutomationElement> ElementFromPointsEx(this WinFormInspectComponent self)
        //{
        //    Point point = new Point(Mouse.Position.X, Mouse.Position.Y);
        //    var element = self.ElementFromPoint(Mouse.Position.X, Mouse.Position.Y);
        //    var process = Process.GetProcessById(element.CurrentProcessId);
        //    var mainWindowElement = self.UIAFactory.ElementFromHandle(process.MainWindowHandle);
        //    return self.GetChilds(new List<IUIAutomationElement>() { mainWindowElement }, point);
        //}
        public static List<IUIAutomationElement> GetChilds(this WinFormInspectComponent self, List<IUIAutomationElement> el, Point point)
        {
            List<IUIAutomationElement> ret = el;
            List<IUIAutomationElement> temps = new List<IUIAutomationElement>();
            foreach (var element in el)
            {
                var childs = element.FindAll(TreeScope.TreeScope_Descendants, self.UIAFactory.CreateTrueCondition());
                if (childs != null)
                    for (int i = 0; i < childs.Length; i++)
                        temps.Add(childs.GetElement(i));
            }
            if (temps == null || temps.Count == 0) return ret;
            List<IUIAutomationElement> temp1s = new List<IUIAutomationElement>();
            for (int i = 0; i < temps.Count; i++)
            {
                var element = temps[i];
                tagRECT rects = element.CurrentBoundingRectangle;
                var rect = Rectangle.FromLTRB(rects.left, rects.top, rects.right, rects.bottom);

                if (rect.Contains(point) && !element.CurrentClassName.ToLower().Contains("shadow"))
                {
                    temp1s.Add(element);
                }
            }
            var min = double.MaxValue;
            IUIAutomationElement element1 = null;
            foreach (var item in temp1s)
            {
                tagRECT rects = item.CurrentBoundingRectangle;
                var rect = Rectangle.FromLTRB(rects.left, rects.top, rects.right, rects.bottom);
                var temp = rect.Height * rect.Width;
                if (min > temp)
                {
                    min = temp;
                    element1 = item;
                }

            }
            return new List<IUIAutomationElement>() { element1 };

        }
        public static List<ElementUIA> GetChilds(this WinFormInspectComponent self, IUIAutomationElement el)
        {
            var childs = el.FindAll(TreeScope.TreeScope_Descendants, self.UIAFactory.CreateTrueCondition());
            return childs.Converts();
        }
        public static List<IUIAutomationElement> GetChild(this WinFormInspectComponent self, IUIAutomationElement el, Point point)
        {
            List<IUIAutomationElement> list = new List<IUIAutomationElement>();
            IUIAutomationElement ret = el;
            var childs = el.FindAll(TreeScope.TreeScope_Children, self.UIAFactory.CreateTrueCondition());

            if (childs == null || childs.Length == 0) return new List<IUIAutomationElement>() { ret };

            for (int i = 0; i < childs.Length; i++)
            {
                var element = childs.GetElement(i);
                tagRECT rects = element.CurrentBoundingRectangle;
                var rect = Rectangle.FromLTRB(rects.left, rects.top, rects.right, rects.bottom);
                if (rect.Contains(point) && !element.CurrentClassName.ToLower().Contains("shadow"))
                    list.AddRange(self.GetChild(element, point));
            }
            return list;
        }
        /// <summary>
        /// 根据x,y获取坐标点
        /// </summary>
        /// <param name="self"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static ElementUIA ElementFromHandle(this WinFormInspectComponent self, IntPtr ptr)
        {
            return self.UIAFactory.ElementFromHandle(ptr).Convert();
        }

        /// <summary>
        /// 获取父节点
        /// </summary>
        /// <param name="self"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        public static IUIAutomationElement GetParent(this WinFormInspectComponent self, IUIAutomationElement element)
        {
            return self.ControlViewWalker.GetParentElement(element);
        }

        /// <summary>
        /// 获取所有的父节点
        /// </summary>
        /// <param name="self"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        public static ElementNode GetAllParentNode(this WinFormInspectComponent self, IUIAutomationElement element)
        {
            var node = new ElementNode();
            if (element == default)
                return default;
            var parent = self.ControlViewWalker.GetParentElement(element);
            if (parent == default)
            {
                node.CurrentElementWin = element.Convert_Json();
                if (node.CurrentElementWin == null) return default;
                node.CurrentElementWin.Id = node.Id;
                var temp1 = FindIndex(self, node.CurrentElementWin);
                node.Index = temp1.Item1;
                node.Length = temp1.Item2;
                node.IsFindIndex = temp1.Item3;
                node.GenerateCompareId();
                return node;
            }
            node.Parent = GetAllParentNode(self, parent);
            if (node.Parent == default) return default;
            node.CurrentElementWin = element.Convert_Json();
            node.CurrentElementWin.Id = node.Id;
            var temp = FindIndex(self, node.CurrentElementWin);
            node.Index = temp.Item1;
            node.Length = temp.Item2;
            node.IsFindIndex = temp.Item3;
            if (node.Parent != null)
                node.LevelIndex = node.Parent.LevelIndex + 1;
            node.GenerateCompareId();
            return node;
        }


        /// <summary>
        /// 获取同级元素的下标
        /// </summary>
        /// <param name="self"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        private static (int, int, bool) FindIndex(this WinFormInspectComponent self, ElementUIA element)
        {
            var parent = self.ControlViewWalker.GetParentElement(element.NativeElement);
            if (parent == null) return (0, 0, true);
            var array = parent.FindAll(TreeScope.TreeScope_Children, self.CreateCondition(element));
            int index = 0;
            bool isFindIndex = false;
            for (int i = 0; i < array.Length; i++)
            {
                var e = array.GetElement(i);
                if (Equal(element.NativeElement, e))
                {
                    isFindIndex = true;
                    break;
                }
                index++;
            }
            return (index, array.Length, isFindIndex);
        }
        public static bool Equal(IUIAutomationElement a, IUIAutomationElement b)
        {
            var aId = a.GetRuntimeId();
            var bId = b.GetRuntimeId();
            if (aId?.Length > 0 && bId?.Length > 0)
                return Enumerable.SequenceEqual(aId, bId);
            return false;
        }
        public static IUIAutomationCondition CreateCondition_Temp(this WinFormInspectComponent self, ElementUIA element)
        {
            var inspect = Boot.GetComponent<InspectComponent>();
            var winInspect = inspect.GetComponent<WinFormInspectComponent>();
            var conditionType = winInspect.UIAFactory.CreatePropertyCondition(UIA_PropertyIds.UIA_ControlTypePropertyId, element.ControlType);
            var conditionRole = winInspect.UIAFactory.CreatePropertyCondition(UIA_PropertyIds.UIA_AriaRolePropertyId, element.Role == default ? "" : element.Role);
            var conditionClassName = winInspect.UIAFactory.CreatePropertyCondition(UIA_PropertyIds.UIA_ClassNamePropertyId, element.ClassName == default ? "" : element.ClassName);
            var conditionName = winInspect.UIAFactory.CreatePropertyCondition(UIA_PropertyIds.UIA_ClassNamePropertyId, element.Name == default ? "" : element.Name);
            var list = new List<IUIAutomationCondition>() { conditionType, conditionRole, conditionClassName, conditionName };
            return winInspect.UIAFactory.CreateAndConditionFromArray(list.ToArray());

        }
        public static IUIAutomationCondition CreateCondition(this WinFormInspectComponent self, ElementUIA element)
        {
            return self.CreateCondition(element.ControlType, element.Role);
        }
        public static IUIAutomationCondition CreateCondition(this WinFormInspectComponent self, int controlType, string role)
        {
            var inspect = Boot.GetComponent<InspectComponent>();
            var winInspect = inspect.GetComponent<WinFormInspectComponent>();
            var conditionType = winInspect.UIAFactory.CreatePropertyCondition(UIA_PropertyIds.UIA_ControlTypePropertyId, controlType);
            if (!string.IsNullOrEmpty(role))
            {
                var conditionRole = winInspect.UIAFactory.CreatePropertyCondition(UIA_PropertyIds.UIA_AriaRolePropertyId, role == default ? "" : role);
                var list = new List<IUIAutomationCondition>() { conditionType, conditionRole };
                return winInspect.UIAFactory.CreateAndConditionFromArray(list.ToArray());
            }
            var listTemp = new List<IUIAutomationCondition>() { conditionType };
            return winInspect.UIAFactory.CreateAndConditionFromArray(listTemp.ToArray());

        }
        public static IUIAutomationCondition CreateAllCondition(this WinFormInspectComponent self)
        {
            var inspect = Boot.GetComponent<InspectComponent>();
            var winInspect = inspect.GetComponent<WinFormInspectComponent>();
            return winInspect.UIAFactory.CreateTrueCondition();

        }
        /// <summary>
        /// 获取所有的子节点
        /// </summary>
        /// <param name="self"></param>
        /// <param name="element"></param>
        /// <param name="LevelIndex"></param>
        /// <returns></returns>
        public static ElementNode GetAllChildrenNode(this WinFormInspectComponent self, IUIAutomationElement element, int LevelIndex = 5)
        {
            if (element == null)
                return null;
            IUIAutomationElementArray children = null;
            try
            {
                children = element.FindAll(TreeScope.TreeScope_Children, self.UIAFactory.CreateTrueCondition());
                if (children == null)
                    return null;
            }
            catch (Exception e)
            {
                return null;
            }
            var node = new ElementNode();
            node.LevelIndex = LevelIndex;
            node.CurrentElementWin = element.Convert();
            //node.GenerateCompareId();
            LevelIndex--;
            if (LevelIndex < 0)
                return node;
            for (int i = 0; i < children.Length; i++)
            {
                if (node.Children == null) node.Children = new List<ElementNode>();
                node.Children.Add(GetAllChildrenNode(self, children.GetElement(i), LevelIndex));
            }
            return node;
        }

        public static List<ElementNode> GetAllChildrens(this WinFormInspectComponent self, IUIAutomationElement element)
        {
            var list = new List<ElementNode>();
            if (element == null)
                return list;
            try
            {
                IUIAutomationElementArray children = element.FindAll(TreeScope.TreeScope_Children, self.UIAFactory.CreateTrueCondition());
                if (children == null)
                    return list;
                for (int i = 0; i < children.Length; i++)
                {
                    var node = new ElementNode();
                    node.CurrentElementWin = children.GetElement(i).Convert();
                    //node.GenerateCompareId();
                    list.Add(node);
                }
            }
            catch (Exception e)
            {
            }
            return list;
        }

        public static List<ElementNode> GetRootNodes(this WinFormInspectComponent self, int LevelIndex = 3)
        {
            if (self.RootNodes == null || self.RootNodes.Count == 0)
                self.RootNodes = self.GetAllChildrenNode(self.RootElement, LevelIndex).GetChildrenNode();
            return self.RootNodes;
        }
        //public static List<ElementNode> GetRootElements(this WinFormInspectComponent self, int LevelIndex = 3)
        //{
        //    if (self.RootNodes == null || self.RootNodes.Count == 0)
        //        self.RootNodes = self.GetAllChildrenNode(self.RootElement, LevelIndex).GetNode();
        //    return self.RootNodes;
        //}
        public static ElementNode GetAllExplorerNode(this WinFormInspectComponent self, IUIAutomationElement element, int LevelIndex = 10)
        {
            if (element == null) return null;
            var children = element.FindAll(TreeScope.TreeScope_Children, self.UIAFactory.CreateTrueCondition());
            if (children == null)
                return null;
            var node = new ElementNode();
            node.LevelIndex = LevelIndex;
            node.CurrentElementWin = element.Convert();
            LevelIndex--;
            //if (LevelIndex < 0 || !SystemKey.Contains(node.CurrentElementWin.ProcessInfo.ProcessName.ToLower()))
            //    return node;
            for (int i = 0; i < children.Length; i++)
            {
                if (node.Children == null) node.Children = new List<ElementNode>();
                node.Children.Add(GetAllExplorerNode(self, children.GetElement(i), LevelIndex));
            }
            return node;
        }

        /// <summary>
        /// 查找存在句柄的窗口。
        /// </summary>
        /// <param name="self"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        public static IUIAutomationElement GetNativeWindowHandle(this WinFormInspectComponent self, IUIAutomationElement element)
        {
            return element.GetNativeWindowHandle();
        }
        /// <summary>
        /// 查找存在句柄的窗口。
        /// </summary>
        /// <param name="self"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        public static IUIAutomationElement GetNativeWindowHandle(this IUIAutomationElement element)
        {
            var winFromInspect = Boot.GetComponent<InspectComponent>().GetComponent<WinFormInspectComponent>();
            while (true)
            {
                try
                {
                    if (element == null) return null;
                    var type = EL.UIA.ControlTypeConverter.ToControlType(element.CurrentControlType);
                    if (element.CurrentNativeWindowHandle != default)
                        break;
                    if (winFromInspect.ControlViewWalker.GetParentElement(element) == null)
                        break;
                    element = winFromInspect.ControlViewWalker.GetParentElement(element);
                }
                catch (Exception)
                {

                }
            }
            return element;
        }
        /// <summary>
        /// 查找最小节点信息
        /// </summary>
        /// <param name="self"></param>
        /// <param name="el"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static IUIAutomationElement GetMinElement(this WinFormInspectComponent self, IUIAutomationElement el, Point point)
        {
            IUIAutomationElement ret = el;
            var childs = el.FindAll(TreeScope.TreeScope_Children, self.UIAFactory.CreateTrueCondition());
            if (childs == null || childs.Length == 0) return ret;
            for (int i = 0; i < childs.Length; i++)
            {
                var element = childs.GetElement(i);
                tagRECT rects = element.CurrentBoundingRectangle;
                var rect = Rectangle.FromLTRB(rects.left, rects.top, rects.right, rects.bottom);
                if (rect.Contains(point) && !element.CurrentClassName.ToLower().Contains("shadow"))
                    return self.GetMinElement(element, point);
            }
            return ret;
        }

        public static string GetRuntimeChildrenId(this WinFormInspectComponent self, IUIAutomationElement el)
        {
            var childs = self.GetAllChildrenRuntimeId(el);
            if (childs != null)
            {
                var json = JsonHelper.ToJson(childs);
                return ElemenetSystem.GetCompareId(json);
            }
            return null;
        }
        public static List<string> GetAllChildrenRuntimeId(this WinFormInspectComponent self, IUIAutomationElement element, int LevelIndex = int.MaxValue)
        {
            if (element == null) return null;
            var children = element.FindAll(TreeScope.TreeScope_Children, self.UIAFactory.CreateTrueCondition());
            if (children == null)
                return null;
            List<string> runtimeIds = new List<string>();
            runtimeIds.Add(ElemenetSystem.GetCompareId(element));
            LevelIndex--;
            if (LevelIndex < 0)
                return runtimeIds;
            for (int i = 0; i < children.Length; i++)
            {

                runtimeIds.AddRange(GetAllChildrenRuntimeId(self, children.GetElement(i), LevelIndex));
            }
            return runtimeIds;
        }
        public static bool IsExistChild(this WinFormInspectComponent self, IUIAutomationElement element)
        {
            var d = element.FindAll(TreeScope.TreeScope_Descendants, self.UIAFactory.CreateTrueCondition());
            if (d != default && d.Length > 0) return true;
            return false;
        }
        public static bool Contain(this WinFormInspectComponent self, ElementIns e, Point p)
        {
            if (e == null) return false;
            if (e.ElementType != ElementType.UIAUI)
                return false;
            if (self.IsExistChild(e.ElementUIA))
                return false;
            return e.Rectangle.Contains(p);
        }
        #region Convert Entity

        #endregion

    }
    public static partial class WinFormInspectComponentSystem
    {
        public static MSAAProperties ElementFromPoint_MSAA(this WinFormInspectComponent self)
        {
            Point point = new Point(Mouse.Position.X, Mouse.Position.Y);
            var element = self.ElementFromPoint();
            //var process = Process.GetProcessById(element.CurrentProcessId);
            //MSAAComponent mSAAComponent = new(process.MainWindowHandle);
            var eleWindow = element.GetNativeWindowHandle();
            if (eleWindow == default) return default;
            MSAAComponent mSAAComponent = new(eleWindow.CurrentNativeWindowHandle);
            if (mSAAComponent == default) return default;
            List<int> pathIndexs = new();
            var massElement = self.GetChild_MSAA(mSAAComponent, point, pathIndexs);
            massElement.ElementUIA = element;
            massElement.ChildIndexs = pathIndexs;
            return massElement;
        }
        public static MSAAProperties GetChild_MSAA(this WinFormInspectComponent self, MSAAComponent mSAAComponent, Point point, List<int> pathIndexs)
        {
            var childs = mSAAComponent.GetAccessibleChildren();
            if (childs == null || childs.Count == 0)
                return mSAAComponent.Properties;
            for (int i = 0; i < childs.Count; i++)
            {
                var element = childs[i];
                var rect = element.Properties.BoundingRectangle;
                if (rect.Contains(point))
                {
                    pathIndexs.Add(i);
                    return self.GetChild_MSAA(element, point, pathIndexs);
                }
            }
            return mSAAComponent.Properties;
        }
    }
    public class NotFindPathException : Exception
    {

        public NotFindPathException(string? message) : base(message)
        {

        }
    }
}
