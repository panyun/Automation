using Automation.Inspect;
using EL;
using EL.Capturing;
using EL.Input;
using EL.WindowsAPI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interop.UIAutomationClient;
using static EL.UIA.ControlTypeConverter;

namespace Automation.Com
{
    public class ElementServer
    {
        public ElementServer()
        {
            //创建
            Boot.App = new AppMananger();
            //加载程序集
            Boot.App.EventSystem.Add(typeof(WinFormInspectComponent).Assembly);
            var inspect = Boot.AddComponent<WinFormInspectComponent>();
            Boot.SetLog(new FileLogger());
        }
        public ActionResponse Select(string titleName, string className)
        {
            ActionResponse response = new ActionResponse();
            Log.Info($"--Start  GetCaptureInfo   titleName: {titleName}");
            var inspect = Boot.GetComponent<WinFormInspectComponent>();
            var intptrMain = User32.FindWindow(null, "TestWindow");
            var element = inspect.ElementFromHandle(intptrMain);
            var nodes = inspect.GetAllChildrenNode(element.NativeElement);
            var elements_ = nodes.GetElementWins();
            element = elements_.FirstOrDefault(x => x.Name == titleName && x.ControlTypeName == ControlType.TabItem + "") as ElementUIA;
            var pattern = (IUIAutomationSelectionItemPattern)element.NativeElement.GetCurrentPattern(UIA_PatternIds.UIA_SelectionItemPatternId);
            pattern.Select();
            response.IsSuccess = true;
            return response;
        }
        public ActionResponse Click(int x, int y)
        {
            Log.Info($"--Start  Click   x: {x} y:{y}");
            ActionResponse response = new ActionResponse();
            //ElementMouseSystem.Click(new Point(x, y), false);
            return response;
        }
        public ActionResponse SelectIndex(int index)
        {
            Log.Info($"--Start  SelectIndex   index: {index}");
            ActionResponse response = new ActionResponse();
            try
            {
                var inspect = Boot.GetComponent<WinFormInspectComponent>();
                var intptrMain = User32.FindWindow(null, "统一柜面管理平台");
                //var intptrMain = User32.FindWindow(null, "TestWindow");
                var element = inspect.ElementFromHandle(intptrMain);
                //Log.Info($"{JsonHelper.ToJson(new { element.Name })}");
                var nodes = inspect.GetAllChildrenNode(element.NativeElement);
                var elements_ = nodes.GetElementWins();
                var nodes_ = nodes.GetChildrenNode();
                var paneItem = elements_.Where(x => x.ControlTypeName == ControlType.Pane + "").ToList()[0];
                element = paneItem as ElementUIA;
                var childs = element.NativeElement.FindAll(TreeScope.TreeScope_Descendants, inspect.UIAFactory.CreateTrueCondition());
                for (int i = 0; i < childs.Length; i++)
                {
                    Log.Info($"{i}: CurrentName:{childs.GetElement(i).CurrentName}, type:{childs.GetElement(i).GetControlTypeName()},{childs.GetElement(i).CurrentBoundingRectangle.left} {childs.GetElement(i).CurrentBoundingRectangle.top}");
                }
                int x = element.NativeElement.CurrentBoundingRectangle.left + 27 + index * 100;
                int y = element.NativeElement.CurrentBoundingRectangle.top + 10;
                //ElementMouseSystem.Click(new Point(x, y), false);
            }
            catch (Exception ex)
            {
                response.SetError(ex.Message, 1);
                Log.Error(ex);
            }
            Log.Info($"--end  SelectIndex   index: {index}");
            response.IsSuccess = true;
            return response;
        }
    }
    public class ActionResponse : Response
    {
        public bool IsSuccess { get; set; }
    }
}
