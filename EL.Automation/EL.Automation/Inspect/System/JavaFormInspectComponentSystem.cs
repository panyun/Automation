using Automation.Parser;
using EL;
using EL.Async;
using EL.Input;
using EL.Overlay;
using NPOI.SS.UserModel;
using System.Drawing;
using System.Windows.Forms;
using WindowsAccessBridgeInterop;

namespace Automation.Inspect
{
    public class JavaFormInspectComponentAwake : AwakeSystem<JavaFormInspectComponent>
    {
        public override async void Awake(JavaFormInspectComponent self)
        {
            //创建对象
            try
            {
                self.AddComponent<JavaPathComponent>();
                self.AccessBridge = new AccessBridge();
                self.AccessBridge.Initialize();
                var inspect = Boot.GetComponent<InspectComponent>();
                self.RefreshJvms().Coroutine();
                RequestOptionComponent.IsJVM = true;
            }
            catch (Exception ex)
            {
                Log.Error("系统未安装 AccessBridge  https://docs.oracle.com/javase/accessbridge/2.0.2/setup.htm");
            }
        }
    }
    public static class JavaFormInspectComponentSystem
    {
        public static async ELTask RefreshJvms(this JavaFormInspectComponent self)
        {
            try
            {
                self.ELTaskJvm = ELTask<List<AccessibleJvm>>.Create();
                self.Jvms = self.AccessBridge.EnumJvms();
                if (self.ELTaskJvm != null && !self.ELTaskJvm.IsCompleted)
                    self.ELTaskJvm.SetResult(self.Jvms);
                Log.Trace($"JavaWinform Jvms count:{self.Jvms.Count}");
            }
            catch (Exception)
            {
                Log.Error("系统未安装 AccessBridge  https://docs.oracle.com/javase/accessbridge/2.0.2/setup.htm");
            }
        }

        public static async ELTask<ElementJAB> FromPoint(this JavaFormInspectComponent self)
        {
            var ejs = await self.FromPoint(Mouse.Position);
            if (!ejs?.Any() ?? true) return default;
            var min = double.MaxValue;
            ElementJAB element = default;
            foreach (var item in ejs)
            {
                var rect = item.BoundingRectangle;
                var temp = rect.Height * rect.Width;
                if (min > temp)
                {
                    min = temp;
                    element = item;
                }
            }
            return element;
        }
        public static async ELTask<List<ElementJAB>> FromPoint(this JavaFormInspectComponent self, Point point)
        {
            self.RefreshJvms().Coroutine();
            self.Jvms = await self.ELTaskJvm;
            if (self.Jvms == null || self.Jvms.Count == 0)
                return null;
            List<ElementJAB> javaElements = new List<ElementJAB>();
            List<AccessibleNode> elements = new List<AccessibleNode>();
            foreach (var item in self.Jvms)
            {
                foreach (var accessibleWindow in item.Windows)
                {
                    accessibleWindow.Refresh();
                    var t = accessibleWindow.GetNodePathAt(point);
                    var controlInfo = accessibleWindow.GetInfo();
                    var rec = new Rectangle(controlInfo.x, controlInfo.y, controlInfo.width, controlInfo.height);
                    if (!rec.Contains(point)) continue;
                    if (!accessibleWindow.GetChildren().Any()) continue;
                    foreach (var node in accessibleWindow.GetChildren())
                    {
                        elements = FindMinElements(node, point);
                        if (elements != null && elements.Count > 0)
                        {
                            var javas = elements.Select(x => new ElementJAB(x, accessibleWindow.Hwnd, controlInfo.name));
                            javaElements.AddRange(javas);
                        }
                    }
                }
            }
            return javaElements;
        }
        public static AccessibleNode FindMinElement(AccessibleNode node, Point point)
        {
            var nodes = new List<AccessibleNode>();
            //如果不在范围内则返回
            var rec = node.GetScreenRectangle();
            if (!rec.HasValue) return default;
            if (!rec.Value.Contains(point)) return default;
            if (!node.GetChildren().Any())
                return node;
            foreach (var child in node.GetChildren())
            {
                var rtn = FindMinElement(child, point);
                if (rtn == null) continue;
                return node;
            }
            return node;
        }
        public static List<AccessibleNode> FindMinElements(AccessibleNode node, Point point)
        {
            var nodes = new List<AccessibleNode>();
            //如果不在范围内则返回
            var rec = node.GetScreenRectangle();
            if (!rec.HasValue) return default;
            if (!rec.Value.Contains(point)) return default;
            if (!node.GetChildren().Any())
                return new List<AccessibleNode>() { node };
            foreach (var child in node.GetChildren())
            {
                var rtn = FindMinElements(child, point);
                if (rtn == null) continue;
                nodes.AddRange(rtn);
            }
            if (nodes.Count == 0)
                nodes.Add(node);
            return nodes;
        }
        /// <summary>
        /// 根据节点获取下一级目录
        /// </summary>
        /// <param name="self"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        public static List<ElementJAB> LoadChildren(this JavaFormInspectComponent self, AccessibleNode element)
        {
            var list = new List<ElementJAB>();
            var children = element.GetChildren();
            foreach (var child in children)
                list.Add(child.Convert());
            return list;
        }

        /// <summary>
        /// 获取父节点
        /// </summary>
        /// <param name="self"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        public static ElementJAB GetParent(this JavaFormInspectComponent self, AccessibleNode element)
        {
            return element.GetParent().Convert();
        }
        /// <summary>
        /// 获取所有的父节点
        /// </summary>
        /// <param name="self"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        public static ElementNode GetAllParentNode(this JavaFormInspectComponent self, AccessibleNode element)
        {
            if (element == null) return null;
            var parent = element.GetParent();
            if (parent == null)
                return null;
            var node = new ElementNode();
            node.Parent = GetAllParentNode(self, parent);
            node.CurrentElementJava = element.Convert();
            node.GenerateCompareId_Java();
            node.Index = FindIndex(self, element);
            if (node.Parent != null)
                node.LevelIndex = node.Parent.LevelIndex + 1;
            return node;
        }
        /// <summary>
        /// 获取同级元素的下标
        /// </summary>
        /// <param name="self"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        private static int FindIndex(this JavaFormInspectComponent self, AccessibleNode element)
        {
            if (element is AccessibleWindow)
                return self.Jvms.SelectMany(x => x.Windows).ToList().IndexOf(element as AccessibleWindow);
            return element.GetParent().GetChildren().ToList().IndexOf(element);
        }
        public static AccessibleWindow GetWindowInfo(this AccessibleNode self)
        {

            if (self == null) return null;
            if (self is AccessibleWindow) return self as AccessibleWindow;
            var parent = self.GetParent();
            if (parent == null) return null;
            if (parent is AccessibleWindow) return parent as AccessibleWindow;
            return GetWindowInfo(parent);
        }

        public static ElementJAB Convert(this AccessibleNode self)
        {
            var windos = self.GetWindowInfo();
            if (windos == null)
                return new ElementJAB(self, default, default);
            return new ElementJAB(self, windos.Hwnd, windos.GetInfo().name);
        }

        public static AccessibleNode Convert(this ElementJAB javaElement)
        {
            return javaElement.AccessibleNode;
        }


    }
}
#region MyRegion
//ThreadSynchronizationContext.Instance.PostNext(async () =>
//{
//    await Boot.GetComponent<TimerComponent>().WaitAsync(1000);
//    while (true)
//    {
//        self.Jvms = self.AccessBridge.EnumJvms();
//        if (self.Jvms.Count() > 0)
//            break;
//    }
//});
//Task.Run(async () =>
//{
//    await Boot.GetComponent<TimerComponent>().WaitAsync(1000);
//    while (true)
//    {
//        self.Jvms = self.AccessBridge.EnumJvms();
//        if (self.Jvms.Count() > 0)
//            break;
//    }

//}).ContinueWith(t =>
//{

//});
#endregion
