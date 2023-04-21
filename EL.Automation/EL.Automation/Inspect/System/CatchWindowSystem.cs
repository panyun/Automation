using EL;
using EL.Async;
using EL.Overlay;
using Interop.UIAutomationClient;
using System.Diagnostics;
using System.Drawing;

namespace Automation.Inspect
{
    public static class CatchWindowSystem
    {
        public static void CatchElement(this CatchWindowRequest self)
        {
            var inspect = Boot.GetComponent<InspectComponent>();
            var formOver = inspect.GetComponent<FormOverLayComponent>();
            var winInspect = inspect.GetComponent<WinFormInspectComponent>();
            var javaInspect = inspect.GetComponent<JavaFormInspectComponent>();
            formOver.Mode = Mode.UIA2;
            formOver.IsCatchComplete = false;
            formOver.Form.Show();
            formOver.FloatShow(self.Msg, 1500);
            RequestOptionComponent.Index = 0;
            inspect.NewRepeatedTimerId = Boot.GetComponent<TimerComponent>().NewRepeatedTimer(1, async () =>
            {
                try
                {
                    if (formOver.IsCatchComplete)
                    {
                        formOver.CompleteEvent?.Invoke();
                        return;
                    }
                    ElementIns? elementTemp = default;
                    if (formOver.Mode == Mode.Auto)
                    {
                        var e1 = await javaInspect.FromPoint();
                        elementTemp = e1?.ConvertElementInspect() ?? default;
                        if (elementTemp == null)
                        {
                            IUIAutomationElement e = winInspect.ElementFromPoint();
                            elementTemp = e?.ConvertElementInspect() ?? default;
                        }
                    }
                    if (formOver.Mode == Mode.UIA2)
                    {
                        IUIAutomationElement e = winInspect.ElementFromPoint();
                        elementTemp = e?.ConvertElementInspect() ?? default;
                    }
                    //if (formOver.Mode == Mode.UIA2 || formOver.Mode == Mode.Auto)
                    //{
                    //    IUIAutomationElement e = winInspect.ElementFromWindow();

                    //    elementTemp = e?.ConvertElementInspect() ?? default;
                    //}
                    if (formOver.Mode == Mode.MSAA)
                    {
                        var e = winInspect.ElementFromPoint_MSAA();
                        elementTemp = e?.ConvertElementInspect() ?? default;
                        if (e.ElementUIA.CurrentProcessId == Process.GetCurrentProcess().Id)
                            return;
                    }
                    if (formOver.Mode == Mode.JAB)
                    {
                        var e = await javaInspect.FromPoint();
                        elementTemp = e?.ConvertElementInspect() ?? default;
                    }
                    if (elementTemp == inspect.CurrentElement)
                        return;
                    if (elementTemp == default || elementTemp == null)
                        return;
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