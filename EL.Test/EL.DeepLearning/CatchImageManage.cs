 using Automation.Inspect;
using Automation.Parser;
using EL.Capturing;
using EL.DeepLearning.Common;
using EL.Input;
using EL.UIA;
using Interop.UIAutomationClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EL.UIA.ControlTypeConverter;

namespace EL.DeepLearning
{
    public class CatchImageManage
    {
        public static ElementUIA elementUIA = new ElementUIA();
        public static string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "images");
        static CatchImageManage()
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
        public static void Save(CaptureImage image, EWindow eWindow, string ProcessName)
        {
            image.ToFile(Path.Combine(path, $"{ProcessName}_{eWindow.ID}.jpg"));
            var json = eWindow.ToJson();
            File.WriteAllText(Path.Combine(path, $"{ProcessName}_{eWindow.ID}.json"), json);
        }
        public (bool success, EWindow window, CaptureImage captureImage, string ProcessName) Process(IUIAutomationElement element, string ProcessName)
        {
            if (element != null)
            {
                var RootWindow = new EWindow();
                SetValue(RootWindow, element);
                //var mainRectangle = Win32Helper.GetWindowRectangleEx(element.CurrentNativeWindowHandle);
                //RootWindow.Rectangle = mainRectangle;
                var image = CaptureComponent.Instance.Rectangle(RootWindow.Rectangle);

                var inspect = Boot.GetComponent<InspectComponent>().GetComponent<WinFormInspectComponent>();
                _Process(element, RootWindow, inspect.UIAFactory, RootWindow.Rectangle);
                return (true, RootWindow, image, ProcessName);
            }
            return default;
        }
        public static void SetValue(EWindow eWindow, IUIAutomationElement element)
        {
            eWindow.Rectangle = ValueConverter.ToRectangle(element.GetCurrentPropertyValue(UIA_PropertyIds.UIA_BoundingRectanglePropertyId));
            eWindow.ControlType = ToControlType(element.CurrentControlType);
            eWindow.Name = element.GetName();
            eWindow.ClassName = element.CurrentClassName;
            eWindow.LocalizedControlType = element.CurrentLocalizedControlType;
            eWindow.Value = element.GetValue();
            eWindow.Text = element.GetText();
            eWindow.Role = element.CurrentAriaRole;
        }
        public void _Process(IUIAutomationElement automationElement, EWindow Parent, IUIAutomation UIAFactory, Rectangle RootRectangle)
        {
            if (automationElement != null && Parent != null)
            {
                //获取子集
                var children = automationElement.FindAll(TreeScope.TreeScope_Children, UIAFactory.CreateTrueCondition());
                if (children?.Length > 0)
                {
                    Parent.Child = new List<EWindow>();
                    for (int i = 0; i < children.Length; i++)
                    {
                        var subEle = children.GetElement(i);
                        var IsOffscreen = (bool)subEle.GetCurrentPropertyValue(UIA_PropertyIds.UIA_IsOffscreenPropertyId);
                        if (IsOffscreen == true)
                        {
                            continue;
                        }
                        var subWindow = new EWindow();
                        subWindow.RootRectangle = RootRectangle;
                        SetValue(subWindow, subEle);
                        _Process(subEle, subWindow, UIAFactory, RootRectangle);

                        Parent.Child.Add(subWindow);
                    }
                }
            }
        }
        public bool SmartProcess(IUIAutomationElement element, Process process)
        {
            //InputSimulator inputSimulator = new InputSimulator();
            //var inspect = Boot.GetComponent<InspectComponent>().GetComponent<WinFormInspectComponent>();
            ////var rootHanld = Win32Helper.GetForegroundWindow();
            //var result = Process(element, process.ProcessName);
            ////UIA_PropertyIds.UIA_ControlTypePropertyId
            //var Condition = inspect.UIAFactory.CreatePropertyCondition(UIA_PropertyIds.UIA_ControlTypePropertyId, ControlType.Button);
            //var list = element.FindAll(TreeScope.TreeScope_Descendants, Condition);
            //var length = list.Length;
            //if (result.success)
            //{

            //    //var invoke = (IUIAutomationInvokePattern)element.NativeElement.GetCurrentPattern(UIA_PatternIds.UIA_InvokePatternId);

            //    //遍历所有应用程序
            //    //启动应用程序获取程序的窗口句柄
            //    //遍历进程的所有窗口，判断当前前置窗口是不是进程的窗口
            //    //获取最顶端的窗口，遍历它，并点击它。
            //    //var list = new List<IUIAutomationElement>();
            //    //if (process.MainWindowHandle != IntPtr.Zero)
            //    //{
            //    //    if (element == null)
            //    //    {
            //    //        element = inspect.UIAFactory.GetRootElement();
            //    //    }
            //    //    var children = element.FindAll(TreeScope.TreeScope_Children, inspect.UIAFactory.CreateTrueCondition());
            //    //    if (children?.Length > 0)
            //    //    {
            //    //        for (int i = 0; i < children.Length; i++)
            //    //        {
            //    //            var subEle = children.GetElement(i);
            //    //            if (subEle.CurrentProcessId == process.Id)
            //    //            {
            //    //                list.Add(subEle);
            //    //                var rec = subEle.ToRectangle();
            //    //                Show(rec);
            //    //            }
            //    //        }
            //    //    }
            //    //}
            //    Save(result.captureImage, result.window, result.ProcessName);

            //    var Buttons = result.window.GetEWindows().Where(t => t.ControlType == ControlType.Button && t.Name != "置顶" && t.Name != "最小化" && t.Name != "最大化" && t.Name != "关闭").ToList();
            //    foreach (var item in Buttons)
            //    {
            //        var rec = item.Rectangle;
            //        var point = item.Rectangle.Center();
            //        Form1.Show(rec, 5000);
            //        var movePoint = point.GetMovePoint();
            //        inputSimulator.Mouse.MoveMouseTo(movePoint.X, movePoint.Y);
            //        Thread.Sleep(100);
            //        inputSimulator.Mouse.LeftButtonClick();
            //        Thread.Sleep(100);
            //        inputSimulator.Mouse.LeftButtonClick();
            //        Thread.Sleep(2000);
            //        //var subHanld = Win32Helper.GetForegroundWindow();
            //        //if (subHanld != rootHanld)
            //        //{
            //        //    var subElementFromHandle = inspect.UIAFactory.ElementFromHandle(subHanld);
            //        //    if (subElementFromHandle != null && subElementFromHandle.CurrentProcessId != process.Id)
            //        //    {
            //        //        var subProcess = System.Diagnostics.Process.GetProcessById(subElementFromHandle.CurrentProcessId);
            //        //        var subResult = Process(subElementFromHandle, subProcess);
            //        //        if (subResult.success)
            //        //        {
            //        //            Form1.Show(subResult.window.Rectangle, 5000);
            //        //        }
            //        //    }
            //        //}
            //        var image = CaptureComponent.Instance.Rectangle(result.window.Rectangle);
            //        var change = OpenCVImage.UIChange(result.captureImage.Bitmap, image.Bitmap);
            //        if (change)
            //        {
            //            var resultSub = Process(element, process.ProcessName);
            //            Save(resultSub.captureImage, resultSub.window, resultSub.ProcessName);
            //        }
            //    }
            //}
            return default;
        }
    }
}
