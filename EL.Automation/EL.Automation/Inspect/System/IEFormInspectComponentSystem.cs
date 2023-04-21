using EL;
using EL.Capturing;
using EL.Input;
using EL.WindowsAPI;
using MSHTML;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using Interop.UIAutomationClient;

namespace Automation.Inspect
{
    public static class IEFormInspectComponentSystem
    {

        /// <summary>
        /// 是否是IE判断
        /// </summary>
        /// <returns></returns>
        public static bool IsIE(this IEInspectComponent self)
        {
            var uIAutomation = new CUIAutomation();
            var point = Mouse.Position;
            var elemnt = uIAutomation.ElementFromPoint(new Interop.UIAutomationClient.tagPOINT() { x = point.X, y = point.Y });
            var process = Process.GetProcessById(elemnt.CurrentProcessId);
            return process.ProcessName.ToLower() == "iexplore";
        }
        public static IHTMLElement GetElement(this IEInspectComponent self)
        {
            var point = Mouse.Position;
            IntPtr hWnd = User32.WindowFromPoint(point);
            StringBuilder buffer = new StringBuilder();
            User32.GetClassName(hWnd, buffer, 25);
            if (buffer.ToString() != "Internet Explorer_Server")
            {
                Log.Error("当前程序不支持非IE浏览器！");
                throw new Exception("当前程序不支持非IE浏览器！");
            }
            int lpdwResult = 0;
            if (!User32.SendMessageTimeout(hWnd, self.WM_HTML_GETOBJECT, 0, 0, 2, 1000, ref lpdwResult))
                return default;
            if (Oleacc.ObjectFromLresult(lpdwResult, ref self.IID_IHTMLDocumentGuid, 0, out object _ComObject))
                return default;
            var doc = (IHTMLDocument2)_ComObject;
            User32.ScreenToClient(hWnd, ref point);
            var radio = 100.0;
            EL.Com.TryCall(() =>
            {
                object htmlWindowObject = GetProperty(doc, "parentWindow");
                radio = (double)InvokeScript(htmlWindowObject, "eval", "(function() { return parseInt(window.screen.deviceXDPI / window.screen.logicalXDPI * 100);})()");
                return true;
            });
            radio = radio / 100.0;
            point = new Point((int)(point.X / radio), (int)(point.Y / radio));
            IHTMLElement element = doc.elementFromPoint(point.X, point.Y);
            return element;
        }
        /// <summary>
        /// 获取Xpath路径
        /// </summary>
        /// <param name="self"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        public static string GetXPath(this IEInspectComponent self, IHTMLElement element)
        {
            if (element.tagName.ToLower() == "html") return "/HTML";
            var currentControlName = $"{element.tagName}";
            if (element.tagName.ToLower() != "body")
                currentControlName += "[" + FindIndex(element) + "]";
            var parent = ((IHTMLDOMNode)element).parentNode;
            if (parent.nodeType == 1)
                return $"{self.GetXPath((IHTMLElement)parent)}/{currentControlName}";
            return $"{self.GetXPath((IHTMLElement)parent)}";
        }

        public static string GetIdPath(this IEInspectComponent self, IHTMLElement element)
        {
            if (element.id != null)
                return "//*[@id='" + element.id + "']";
            if (element.tagName.ToLower() == "html") return "/HTML";
            var currentControlName = $"{element.tagName}";
            if (element.tagName.ToLower() != "body")
                currentControlName += "[" + FindIndex(element) + "]";
            var parent = ((IHTMLDOMNode)element).parentNode;
            if (parent.nodeType == 1)
                return $"{self.GetIdPath((IHTMLElement)parent)}/{currentControlName}";
            return $"{self.GetIdPath((IHTMLElement)parent)}";
        }
        /// <summary>
        /// 获取截图
        /// </summary>
        /// <param name="self"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        public static CaptureImage GetCap(this IEInspectComponent self, IHTMLElement element)
        {
            var point = Mouse.Position;
            IntPtr hWnd = User32.WindowFromPoint(point);
            User32.RECT rect = new User32.RECT();
            User32.GetWindowRect(hWnd, ref rect);
            var rec = ((IHTMLElement2)element).getBoundingClientRect();
            var widthMax = User32.GetSystemMetrics(SystemMetric.SM_CXSCREEN);
            var heightMax = User32.GetSystemMetrics(SystemMetric.SM_CYSCREEN);
            rec.left = Math.Max(0, rec.left);
            rec.top = Math.Max(0, rec.top);
            rec.right = Math.Min(widthMax, rec.right);
            rec.bottom = Math.Min(heightMax, rec.bottom);
            var bounds = new Rectangle(rec.left + rect.Left, rec.top + rect.Top, rec.right - rec.left, rec.bottom - rec.top);
            return CaptureComponent.Instance.Rectangle(bounds);
        }

        static object InvokeScript(object callee, string method, params object[] args)
        {
            return callee.GetType().InvokeMember(method,
                BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.Public,
                null, callee, args);
        }
        static object GetProperty(object callee, string property)
        {
            return callee.GetType().InvokeMember(property,
                BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.Public,
                null, callee, new System.Object[] { });
        }


        private static int FindIndex(IHTMLElement element)
        {
            IHTMLDOMChildrenCollection collection = (IHTMLDOMChildrenCollection)((IHTMLDOMNode)element).parentNode.childNodes;
            int index = 0;
            for (int i = 0; i < collection.length; i++)
            {
                // 实际测试中发生过
                if (collection.item(i) == null || ((IHTMLDOMNode)collection.item(i)).nodeType != 1)
                    continue;
                if (element == collection.item(i))
                    return index + 1;
                if (element.tagName == ((IHTMLElement)collection.item(i)).tagName)
                    index++;
            }
            return 1;
        }
    }
}
