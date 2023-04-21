using Automation.Inspect;
using Automation.Parser;
using EL.Capturing;
using EL.CollectWebPages.Common;
using EL.CollectWebPages.Model;
using EL.Input;
using EL.Overlay;
using EL.UIA;
using Interop.UIAutomationClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EL.UIA.ControlTypeConverter;

namespace EL.CollectWebPages.BLL
{
    public class CatchImageManage
    {
        public static ElementUIA elementUIA = new ElementUIA();
        public string RootPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "images");
        public string subImagePath;
        public CatchImageManage(string RootPath, string subImagePath)
        {
            this.RootPath = Path.Combine(RootPath, "images", subImagePath);
            this.subImagePath = subImagePath;
            if (!Directory.Exists(this.RootPath))
            {
                Directory.CreateDirectory(this.RootPath);
            }
        }
        public bool CheckImageFile()
        {
            var result = Directory.GetFiles(RootPath);
            return result?.Count() > 0;
        }
        public void Save(EWindow eWindow, string ProcessName)
        {
            var json = eWindow.ToJson();
            File.WriteAllText(Path.Combine(RootPath, $"{ProcessName}_{eWindow.ID}.json"), json);
        }
        public (bool success, EWindow window, string ProcessName) Process(IUIAutomationElement element, string ProcessName)
        {
            if (element != null)
            {
                var RootWindow = new EWindow();
                SetValue(RootWindow, element);
                var inspect = Boot.GetComponent<InspectComponent>().GetComponent<WinFormInspectComponent>();
                _Process(element, RootWindow, inspect.UIAFactory, RootWindow.Rectangle);
                return (true, RootWindow, ProcessName);
            }
            return default;
        }
        public void SetValue(EWindow eWindow, IUIAutomationElement element, int times = 5)
        {
            Exception EX = null;
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    eWindow.Rectangle = ValueConverter.ToRectangle(element.GetCurrentPropertyValue(UIA_PropertyIds.UIA_BoundingRectanglePropertyId));
                    eWindow.ControlType = ToControlType(element.CurrentControlType);
                    eWindow.Name = element.GetName();
                    eWindow.ClassName = element.CurrentClassName;
                    eWindow.LocalizedControlType = element.CurrentLocalizedControlType;
                    eWindow.Value = element.GetValue();
                    eWindow.Text = element.GetText();
                    eWindow.Role = element.CurrentAriaRole;
                    return;
                }
                catch (Exception ex)
                {
                    EX = ex;
                }
            }
            throw EX;
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
        public static IUIAutomationElement GetUIAutomation(IUIAutomationElement ElementFromHandle, IUIAutomationElement Root)
        {
            var CurrentEle = GetRootWindow(ElementFromHandle, Root);
            if (CurrentEle == null)
            {
                CurrentEle = ElementFromHandle;
            }
            CurrentEle = GetRealEle(CurrentEle, true);
            return CurrentEle;
        }
        private static IUIAutomationElement GetRootWindow(IUIAutomationElement CurrentEle, IUIAutomationElement Root)
        {
            var type = ToControlType(CurrentEle.CurrentControlType);
            if (type != ControlType.Window)
            {
                var subEle = CurrentEle;
                IUIAutomationElement before = null;
                var id = Root.CurrentProcessId;
                while (subEle != null)
                {
                    subEle = WinFormInspectComponent.Instance.ControlViewWalker.GetParentElement(subEle);
                    if (subEle != null)
                    {
                        if (subEle.CurrentProcessId != id)
                        {
                            before = subEle;
                        }
                        var subType = ToControlType(subEle.CurrentControlType);
                        if (subType == ControlType.Window)
                        {
                            return subEle;
                        }
                    }
                    else
                    {
                        return before;
                    }
                }
            }
            return CurrentEle;
        }
        private static IUIAutomationElement GetRealEle(IUIAutomationElement CurrentEle, bool isWeb = false)
        {
            var reCurrentEle = CurrentEle;
            var inspect = Boot.GetComponent<InspectComponent>().GetComponent<WinFormInspectComponent>();

            if (isWeb)
            {
                var webChildren = CurrentEle.FindAll(TreeScope.TreeScope_Descendants, inspect.UIAFactory.CreatePropertyCondition(UIA_PropertyIds.UIA_ControlTypePropertyId, UIA_ControlTypeIds.UIA_DocumentControlTypeId));
                if (webChildren?.Length > 0)
                {
                    for (int i = 0; i < webChildren.Length; i++)
                    {
                        var subEle = webChildren.GetElement(i);
                        return subEle;
                        //var rec = subEle.ToRectangle();
                        //Show(rec);
                    }
                }
            }
            var rect = CurrentEle.ToRectangle();
            var children = CurrentEle.FindAll(TreeScope.TreeScope_Children, inspect.UIAFactory.CreateTrueCondition());
            if (children?.Length > 0)
            {
                try
                {
                    for (int i = 0; i < children.Length; i++)
                    {
                        var subEle = children.GetElement(i);
                        try
                        {
                            var subRec = subEle.ToRectangle();
                            var vWidth = rect.Width - subRec.Width;
                            var vHeight = rect.Height - subRec.Height;

                            var v = Math.Abs(vWidth - vHeight);
                            if (v <= 3)
                            {
                                reCurrentEle = subEle;
                                break;
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
            return reCurrentEle;
        }
        public static void Show(Rectangle rectangle, int delay = 2000)
        {
            var formOver = Boot.GetComponent<InspectComponent>().GetComponent<FormOverLayComponent>();
            formOver.LightHighShow(Color.Red, rectangle);
            Thread.Sleep(delay);
            formOver.LightHighHide();
        }
    }
}
