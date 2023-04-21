using EL;
using EL.Async;
using EL.Overlay;
using System.Diagnostics;
using System.Drawing;


namespace Automation.Inspect
{
    public static class CatchElementChatSystem
    {
        public static void CatchElementChat(this CatchElementChatRequest self)
        {
            var inspect = Boot.GetComponent<InspectComponent>();
            var formOver = inspect.GetComponent<FormOverLayComponent>();
            var winInspect = inspect.GetComponent<WinFormInspectComponent>();
            //var javaInspect = inspect.GetComponent<JavaFormInspectComponent>();
            formOver.Form.Show();
            formOver.FloatShow(self.Msg, 1500);
            RequestOptionComponent.Index = 0;
            inspect.NewRepeatedTimerId = Boot.GetComponent<TimerComponent>().NewRepeatedTimer(50, () =>
            {
                try
                {
                    ElementIns? elementTemp = default;
                    var ele = winInspect.ElementFromPoint();
                    var temp = winInspect.GetNativeWindowHandle(ele).ConvertElementInspect();
                    //elementTemp = winInspect.ElementFromPoint().Convert();
                    if (temp == null)
                        return;
                    if (temp is ElementIns)
                    {

                        if (temp.ProcessId == Process.GetCurrentProcess().Id)
                            return;
                        var className = temp.ElementUIA.CurrentClassName.ToLower();
                        if (!className.Contains("chat") && className != "weworkwindow")
                            return;
                    }
                    elementTemp = temp;
                    inspect.CurrentElement = elementTemp;
                    formOver.ELTaskOverLay = ELTask<dynamic>.Create();
                    formOver.Show(Color.Red).Coroutine();
                    formOver.ELTaskOverLay.SetResult(inspect.CurrentElement);
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }

            });
        }
    }
}
