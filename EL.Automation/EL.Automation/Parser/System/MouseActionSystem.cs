using Automation.Inspect;
using EL;
using EL.Capturing;
using EL.Input;
using EL.UIA;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interop.UIAutomationClient;
using System.Xml.Linq;
using EL.Async;
using Microsoft.Playwright;

namespace Automation.Parser
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class MouseActionSystem
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="self"></param>
        public static async ELTask Main(this MouseActionRequest self)
        {
            if (self.ElementType == ElementType.UIAUI)
            {
                self.UIAMain();
                return;
            }
            if (self.ElementType == ElementType.JABUI)
            {
                self.JABMain();
                return;
            }
            if (self.ElementType == ElementType.MSAAUI)
            {
                self.MSAAMain();
                return;
            }
            if (self.ElementType == ElementType.PlaywrightUI)
            {
                await self.PlaywrightAMain();
                return;
            }
            if (self.ElementType == ElementType.VcOcr)
            {
                await self.VcOcrMain();
                return;
            }
        }
        public static async ELTask VcOcrMain(this MouseActionRequest self)
        {
            await ELTask.CompletedTask;
            var elements = self.AvigationElement().Cast<ElementVcOcr>();
            Log.Trace($"已抓到节点数：{elements.Count()}");
            foreach (var element in elements)
            {
                if (self.ActionType == ActionType.ElementEvent)
                    throw new ParserException("当前节点不支持事件点击！");
                Point point = default;
                point = element.ClickablePoint;
                point.X += self.OffsetX;
                point.Y += self.OffsetY;
                Log.Trace("vcocr:" + JsonHelper.ToJson(point));
                switch (self.ClickType)
                {
                    case ClickType.LeftClick:
                        PerformMouseAction(false, Mouse.LeftClick, point);
                        break;
                    case ClickType.RightClick:
                        PerformMouseAction(false, Mouse.RightClick, point);
                        break;
                    case ClickType.LeftDoubleClick:
                        PerformMouseAction(false, Mouse.LeftDoubleClick, point);
                        break;
                    default:
                        PerformMouseAction(false, Mouse.LeftClick, point);
                        break;
                }
            }
        }
        public static async ELTask PlaywrightAMain(this MouseActionRequest self)
        {
            //self.TimeOut = 1000;
            //try
            //{
            //    var element = self.AvigationElement().Cast<ElementUIA>().First();
            //    element.SetForeground();
            //}
            //catch (Exception ex)
            //{

            //}
            var playwrightInspect = Boot.GetComponent<InspectComponent>().GetComponent<PlaywrightInspectComponent>();
            var ele = self.ElementPath.PathNode.CurrentElementPlaywright;
            ele.SetForeground();
            var obj = playwrightInspect.FindLocatorByPath(ele);
            LocatorClickOptions locatorClickOptions = new();
            MouseClickOptions mouseClickOptions = new();
            switch (self.ClickType)
            {
                case ClickType.LeftClick:
                    locatorClickOptions.Button = Microsoft.Playwright.MouseButton.Left;
                    mouseClickOptions.Button = Microsoft.Playwright.MouseButton.Left;
                    break;
                case ClickType.CenterClick:
                    locatorClickOptions.Button = Microsoft.Playwright.MouseButton.Middle;
                    mouseClickOptions.Button = Microsoft.Playwright.MouseButton.Middle;
                    break;
                case ClickType.RightClick:
                    locatorClickOptions.Button = Microsoft.Playwright.MouseButton.Right;
                    mouseClickOptions.Button = Microsoft.Playwright.MouseButton.Right;
                    break;
                case ClickType.LeftDoubleClick:
                    locatorClickOptions.Button = Microsoft.Playwright.MouseButton.Left;
                    mouseClickOptions.Button = Microsoft.Playwright.MouseButton.Left;
                    locatorClickOptions.ClickCount = 2;
                    mouseClickOptions.ClickCount = 2;
                    break;
                default:
                    break;
            }
            switch (self.ActionType)
            {
                case ActionType.ElementEvent:
                    await obj.Item1.ClickAsync(locatorClickOptions);
                    break;
                case ActionType.Mouse:
                    await obj.Item2.Mouse.ClickAsync(ele.ClickablePoint.X, ele.ClickablePoint.Y, mouseClickOptions);
                    break;
                default:
                    break;
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="self"></param>
        public static void JABMain(this MouseActionRequest self)
        {
            var elements = Avigation.Create(self.ElementPath).TryAvigation(self.ElementPath, self.TimeOut).Cast<ElementJAB>();
            Log.Trace($"已抓到节点数：{elements.Count()}");
            foreach (var element in elements)
            {
                element.SetForeground();
                //PrintComponent.PrintAvigation(element);
                Point point = element.ClickablePoint;
                point.X += self.OffsetX;
                point.Y += self.OffsetY;
                switch (self.ClickType)
                {
                    case ClickType.LeftClick:
                        PerformMouseAction(false, Mouse.LeftClick, point);
                        break;
                    case ClickType.RightClick:
                        PerformMouseAction(false, Mouse.RightClick, point);
                        break;
                    case ClickType.LeftDoubleClick:
                        PerformMouseAction(false, Mouse.LeftDoubleClick, point);
                        break;
                    default:
                        PerformMouseAction(false, Mouse.LeftClick, point);
                        break;
                }
            }
        }
        public static void MSAAMain(this MouseActionRequest self)
        {
            var elements = self.AvigationElement().Cast<ElementMSAA>();
            Log.Trace($"已抓到节点数：{elements.Count()}");
            foreach (var element in elements)
            {
                element.SetForeground();
                if (self.ActionType == ActionType.ElementEvent)
                    throw new ParserException("当前节点不支持事件点击！");
                Point point = default;
                point = element.ClickablePoint;
                point.X += self.OffsetX;
                point.Y += self.OffsetY;

                switch (self.ClickType)
                {
                    case ClickType.LeftClick:
                        PerformMouseAction(false, Mouse.LeftClick, point);
                        break;
                    case ClickType.RightClick:
                        PerformMouseAction(false, Mouse.RightClick, point);
                        break;
                    case ClickType.LeftDoubleClick:
                        PerformMouseAction(false, Mouse.LeftDoubleClick, point);
                        break;
                    default:
                        PerformMouseAction(false, Mouse.LeftClick, point);
                        break;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="self"></param>

        public static void UIAMain(this MouseActionRequest self)
        {
            var elements = self.AvigationElement().Cast<ElementUIA>().ToList();
            self.UIAMain(elements);
        }
        public static void UIAMain(this MouseActionRequest self, List<ElementUIA> elementWins)
        {
            Log.Trace($"已抓到节点数：{elementWins.Count()}");
            foreach (var element in elementWins)
            {
                element.SetForeground();
                //element.Focus();
                //PrintComponent.PrintAvigation(element);
                if (self.ActionType == ActionType.ElementEvent)
                {
                    if (element == null) return;
                    var invoke = (IUIAutomationInvokePattern)element.NativeElement.GetCurrentPattern(UIA_PatternIds.UIA_InvokePatternId);
                    if (invoke != null)
                        invoke.Invoke();
                    return;
                }
                Point point = default;
                if (element == default)
                {
                    point = new Point(self.ElementPath.X + self.ElementPath.Width / 2,
                        self.ElementPath.Y + self.ElementPath.Height / 2);
                }
                else
                {
                    if (element.TryGetClickablePoint(out Point clickablePoint))
                        point = clickablePoint;
                    else
                        point = element.ClickablePoint;
                }
                point.X += self.OffsetX;
                point.Y += self.OffsetY;
                switch (self.ClickType)
                {
                    case ClickType.LeftClick:
                        PerformMouseAction(false, Mouse.LeftClick, point);
                        break;
                    case ClickType.RightClick:
                        PerformMouseAction(false, Mouse.RightClick, point);
                        break;
                    case ClickType.LeftDoubleClick:
                        PerformMouseAction(false, Mouse.LeftDoubleClick, point);
                        break;
                    default:
                        PerformMouseAction(false, Mouse.LeftClick, point);
                        break;
                }
            }
        }
        public static void Click(this ElementUIA self, bool moveMouse = false)
        {
            self.PerformMouseAction(moveMouse, Mouse.LeftClick);
        }

        /// <summary>
        /// Performs a double left click on the element.
        /// </summary>
        /// <param name="moveMouse">Flag to indicate, if the mouse should move slowly (true) or instantly (false).</param>
        public static void DoubleClick(this ElementUIA self, bool moveMouse = false)
        {
            self.PerformMouseAction(moveMouse, Mouse.LeftDoubleClick);
        }
        /// <summary>
        /// Performs a right click on the element.
        /// </summary>
        /// <param name="moveMouse">Flag to indicate, if the mouse should move slowly (true) or instantly (false).</param>
        public static void RightClick(this ElementUIA self, bool moveMouse = false)
        {
            self.PerformMouseAction(moveMouse, Mouse.RightClick);
        }
        /// <summary>
        /// Performs a double right click on the element.
        /// </summary>
        /// <param name="moveMouse">Flag to indicate, if the mouse should move slowly (true) or instantly (false).</param>
        public static void RightDoubleClick(this ElementUIA self, bool moveMouse = false)
        {
            self.PerformMouseAction(moveMouse, Mouse.RightDoubleClick);
        }
        private static void PerformMouseAction(bool moveMouse, Action action, Point clickablePoint)
        {
            if (moveMouse)
            {
                Mouse.MoveTo(clickablePoint);
            }
            else
            {
                Mouse.Position = clickablePoint;
            }
            action();
            Wait.UntilInputIsProcessed();
        }
        public static void PerformMouseAction(this ElementJAB self, bool moveMouse, Action action)
        {
            var point = self.ClickablePoint;
            if (moveMouse)
                Mouse.MoveTo(point);
            else
                Mouse.Position = point;
            Mouse.Position = point;
            action();
            Wait.UntilInputIsProcessed();
        }
        private static void PerformMouseAction(this ElementUIA self, bool moveMouse, Action action)
        {
            self.TryGetClickablePoint(out Point clickablePoint);
            if (moveMouse)
                Mouse.MoveTo(clickablePoint);
            else
                Mouse.Position = clickablePoint;
            action();
            Wait.UntilInputIsProcessed();
        }
        public static void PerformMouseAction(this Element self, bool moveMouse, Action action)
        {
            self.TryGetClickablePoint(out Point clickablePoint);
            if (moveMouse)
                Mouse.MoveTo(clickablePoint);
            else
                Mouse.Position = clickablePoint;
            action();
            Wait.UntilInputIsProcessed();
        }
        public static void SetForeground(this Element self)
        {
            if (self is ElementUIA elementUIA)
            {
                elementUIA.NativeElement.SetForeground();
            }
            else if (self is ElementMSAA elementMSAA)
            {
                elementMSAA.MSAAProperties.ElementUIA.SetForeground();
            }
            else if (self is ElementJAB elementJab)
            {
                elementJab.SetForeground();
            }
            else if (self is ElementPlaywright wright)
            {
                wright.SetForeground();
            }
        }
        public static void Focus(this Element self)
        {
            self.SetForeground();
            if (self is ElementUIA elementUIA)
            {
                elementUIA.Focus();
                return;
            }
            self.PerformMouseAction(false, Mouse.LeftDoubleClick);
        }

        //public static void PerformMouseAction(this ElementJava self, bool moveMouse, Action action)
        //{
        //    var clickablePoint = self.ClickablePoint;
        //    if (moveMouse)
        //        Mouse.MoveTo(clickablePoint);
        //    else
        //        Mouse.Position = clickablePoint;
        //    action();
        //    Wait.UntilInputIsProcessed();
        //}
        public static bool TryGetValue<T>(this ElementUIA self, int propertyId, out T val)
        {
            var rtn = self.NativeElement.GetCurrentPropertyValue(propertyId);
            if (rtn == null || rtn == self.NotSupportedValue)
            {
                val = default(T);
                return false;
            }
            val = (T)rtn;
            return true;
        }
        public static bool TryGetClickablePoint(this ElementUIA self, out Point point)
        {
            try
            {
                // Variant 1: Directly try getting the point
                var tagPoint = new tagPOINT { x = 0, y = 0 };
                if (EL.Com.Call(() => self.NativeElement.GetClickablePoint(out tagPoint)) != 0)
                {
                    point = new Point(tagPoint.x, tagPoint.y);
                    return true;
                }
                // Variant 2: Try to get it from the property
                if (self.TryGetValue<object>(UIA_PropertyIds.UIA_ClickablePointPropertyId, out var p))
                {
                    point = ValueConverter.ToPoint(p);
                    return true;
                }
                // Variant 3: Get the center of the bounding rectangle
                if (self.TryGetValue<object>(UIA_PropertyIds.UIA_BoundingRectanglePropertyId, out var r))
                {
                    point = ValueConverter.ToRectangle(r).Center();
                    return true;
                }
            }
            catch
            {
                // Noop
            }
            point = Point.Empty;
            return false;
        }
        public static bool TryGetClickablePoint(this Element self, out Point point)
        {
            if (self is ElementUIA eleUIa)
            {
                try
                {
                    // Variant 1: Directly try getting the point
                    var tagPoint = new tagPOINT { x = 0, y = 0 };
                    if (EL.Com.Call(() => eleUIa.NativeElement.GetClickablePoint(out tagPoint)) != 0)
                    {
                        point = new Point(tagPoint.x, tagPoint.y);
                        return true;
                    }
                    // Variant 2: Try to get it from the property
                    if (eleUIa.TryGetValue<object>(UIA_PropertyIds.UIA_ClickablePointPropertyId, out var p))
                    {
                        point = ValueConverter.ToPoint(p);
                        return true;
                    }
                    // Variant 3: Get the center of the bounding rectangle
                    if (eleUIa.TryGetValue<object>(UIA_PropertyIds.UIA_BoundingRectanglePropertyId, out var r))
                    {
                        point = ValueConverter.ToRectangle(r).Center();
                        return true;
                    }
                }
                catch
                {
                    // Noop
                }
            }
            else
            {
                point = self.ClickablePoint;
                return true;
            }
            point = Point.Empty;
            return false;
        }
        /// <summary>
        /// Captures the object as screenshot in <see cref="Bitmap"/> format.
        /// </summary>
        public static Bitmap Capture(this ElementUIA self)
        {
            return CaptureComponent.Instance.Element(self.NativeElement).Bitmap;
        }
    }
}
