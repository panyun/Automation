using EL;
using EL.Async;
using EL.Basic.Component.Clipboard;
using EL.Capturing;
using EL.Overlay;
using EL.Sqlite;
using System.Diagnostics;
using System.IO.Pipes;
using System.Security.Principal;

namespace Automation.Inspect
{
    /// <summary>
    /// 界面探测器入口
    /// </summary>
    public class InspectComponentAwake : AwakeSystem<InspectComponent>
    {
        public override void Awake(InspectComponent self)
        {
            self.InitComponent();
        }
    }
    public static class InspectComponentSystem
    {
        public static void CatchElement(this InspectComponent self, string value)
        {
            var json = value.Substring(5);
            var request = JsonHelper.FromJson<CatchUIRequest>(json);
            self.CatchElement(request);
        }
        public static void CatchElement(this InspectComponent self, CatchUIRequest request)
        {
            var inspect = Boot.GetComponent<InspectComponent>();
            inspect.Action = (x, y) =>
            {
                CatchUIResponse response = new CatchUIResponse();
                if (y == null || x == null)
                {
                    response.Error = 601;
                    response.Message = "未捕获到界面元素信息";
                }
                response.ElementPath = y;
                if (InspectRequestManager.FinishAction != null && !InspectRequestManager.FinishAction.IsCompleted)
                {
                    InspectRequestManager.FinishAction.SetResult(response);
                }
                var formOver = self.GetComponent<FormOverLayComponent>();
                formOver.IsCatchStart = false;
            };
            self.InitEvent();
            request.CatchElement();
        }
        public static void InitComponent(this InspectComponent self)
        {
            //self.AddComponent<TimerComponent>();
            //桌面软件探测
            var inspect = self.AddComponent<WinFormInspectComponent>();
            try
            {
                var playwright = self.AddComponent<PlaywrightInspectComponent>();
            }
            catch (Exception ex)
            {


            }
            var vcOcr = self.AddComponent<VcOcrInspectComponent>();
            var inspectJava = self.AddComponent<JavaFormInspectComponent>();
            //路径 组件
            inspect.AddComponent<WinPathComponent>();
            //Ie 探测器
            self.AddComponent<IEInspectComponent>();
            //剪切板组件
            self.AddComponent<ClipboardComponent>();
            //截图组件
            self.AddComponent<CutComponent>();
            //截图组件
            self.AddComponent<CaptureComponent>();
            //高亮显示
            self.AddComponent<FormOverLayComponent>();
            self.AddComponent<ClipboardComponent>();
            self.AddComponent<SqliteComponent, string>("WinXinMsg.db");
        }
        //public static void CatchElementEx(this InspectComponent self)
        //{
        //    var formOver = self.GetComponent<FormOverLayComponent>();
        //    var winInspect = self.GetComponent<WinFormInspectComponent>();
        //    var javaInspect = self.GetComponent<JavaFormInspectComponent>();
        //    formOver.From.Show();
        //    self.NewRepeatedTimerId = Boot.GetComponent<TimerComponent>().NewRepeatedTimer(1, () =>
        //    {
        //        try
        //        {
        //            Element elementTemp = default;
        //            elementTemp = javaInspect.FromPoint();
        //            if (elementTemp == null)
        //                elementTemp = winInspect.ElementFromPoint();
        //            if (elementTemp == self.CurrentElement)
        //                return;
        //            if (elementTemp.TryElementWin(out var winElement)
        //            && winElement.NativeElement.CurrentProcessId == Process.GetCurrentProcess().Id)
        //                return;
        //            self.CurrentElement = elementTemp;
        //            formOver.ELTaskOverLay = ELTask<dynamic>.Create();
        //            formOver.Show(Color.Red).Coroutine();
        //            formOver.ELTaskOverLay.SetResult(self.CurrentElement);
        //        }
        //        catch (Exception ex)
        //        {
        //            Log.Error(ex);
        //        }

        //    });
        //}

        private static void InitEvent(this InspectComponent self)
        {
            var formOver = self.GetComponent<FormOverLayComponent>();
            formOver.IsCatchStart = true;
            formOver.Mode = Mode.UIA2;
            formOver.IsCatchComplete = false;
            formOver.CompleteEvent = () =>
            {
                formOver.KeyboardHookStop();
                formOver.MouseHookStop();
                self.CatchComplete().Coroutine();
                Boot.GetComponent<TimerComponent>().Remove(self.NewRepeatedTimerId);
            };
            formOver.ExitEvent = () =>
            {
                formOver.KeyboardHookStop();
                formOver.MouseHookStop();
                self.Exit().Coroutine();
                Boot.GetComponent<TimerComponent>().Remove(self.NewRepeatedTimerId);
            };
            formOver.ModeEvent = () =>
            {
                formOver.Mode++;
                formOver.Mode = formOver.Mode == Mode.None ? Mode.Auto : formOver.Mode;
                formOver.Form.UpdateText(self.CurrentElement);
            };
            formOver.ScreenshotEvent = () =>
            {
                if (!formOver.IsScreenshot)
                    return;
                formOver.Form.HideEx();
                ElementCvInfo elementCvInfo = new();
                CaptureImageTool captureImageTool = new();
                if (captureImageTool.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    elementCvInfo.Img = captureImageTool.ImageData;
                    elementCvInfo.Rect = captureImageTool.SelectImageRect;
                    self.CurrentElement = new ElementIns(elementCvInfo);
                    formOver.IsCatchComplete = true;
                }
            };
            formOver.KeyboardHookStart();
            formOver.MouseHookStart();
        }

        private static async ELTask Exit(this InspectComponent self)
        {
            await ELTask.CompletedTask;
            var formOver = self.GetComponent<FormOverLayComponent>();
            CancellationTokenSource cancellationToken = new CancellationTokenSource();
            formOver.MsgForm.Invoke(() =>
            {
                formOver.FloatShow("正在返回数据！");
            });
            formOver.Form.Clear();
            formOver.Form.HideEx();
            self.Action(default, default);
            formOver.MsgForm.Invoke(() =>
            {
                formOver.MsgForm.Hide();
            });
        }

        private static async ELTask CatchComplete(this InspectComponent self)
        {
            await ELTask.CompletedTask;
            var formOver = self.GetComponent<FormOverLayComponent>();
            CancellationTokenSource cancellationToken = new CancellationTokenSource();
            formOver.MsgForm.Invoke(() =>
            {
                formOver.FloatShow("正在返回数据！");
            });
            formOver.Form.Clear();
            formOver.Form.HideEx();
            try
            {
                if (self.CurrentElement.ElementType == ElementType.UIAUI)
                {
                    ElementPath path = default;
                    var elementWin = self.CurrentElement.ElementUIA.Convert();
                    var winInspect = self.GetComponent<WinFormInspectComponent>();
                    var pathComponent = winInspect.GetComponent<WinPathComponent>();
                    path = pathComponent.GetPathInfo(elementWin.NativeElement);
                    self.Action(elementWin, path);
                    return;
                }
                if (self.CurrentElement.ElementType == ElementType.JABUI)
                {
                    var pathcomponent = self.GetComponent<JavaFormInspectComponent>().GetComponent<JavaPathComponent>();
                    var path = pathcomponent.GetPathInfo(self.CurrentElement.ElementJAB.AccessibleNode);
                    self.Action(self.CurrentElement.ElementJAB, path);
                    return;
                }
                if (self.CurrentElement.ElementType == ElementType.MSAAUI)
                {
                    ElementPath path = default;
                    var elementWin = self.CurrentElement.ElementMSAA.Convert();
                    var winInspect = self.GetComponent<WinFormInspectComponent>();
                    var pathComponent = winInspect.GetComponent<WinPathComponent>();
                    path = pathComponent.GetPathInfo(elementWin);
                    self.Action(elementWin, path);
                    return;
                }
                if (self.CurrentElement.ElementType == ElementType.PlaywrightUI)
                {
                    ElementPath path = default;
                    var elementWin = self.CurrentElement.ElementPlaywright;
                    var winInspect = self.GetComponent<WinFormInspectComponent>();
                    var pathComponent = winInspect.GetComponent<WinPathComponent>();
                    path = pathComponent.GetPathInfo(elementWin);
                    self.Action(elementWin, path);
                    return;
                }
                if (self.CurrentElement.ElementType == ElementType.VcOcr)
                {
                    ElementPath path = default;
                    var element = self.CurrentElement.ElementVcOcr;
                    var winInspect = self.GetComponent<WinFormInspectComponent>();
                    var pathComponent = winInspect.GetComponent<WinPathComponent>();
                    path = await pathComponent.GetPathInfo(element);
                    self.Action(element.Convert(), path);
                }
            }
            catch (Exception ex)
            {
                self.Action(default, default);
                Log.Error(ex);
            }
            finally
            {
                formOver.MsgForm.Invoke(() =>
                {
                    formOver.MsgForm.Hide();
                });
            }
        }
        public static void ExitApp(this InspectComponent self, IResponse response)
        {

            RequestOptionComponent.RequestTime.Stop();
            if (response != null)
            {
                var responseStr = JsonHelper.ToJson(response);
                InspectRequestManager.SendRespose(responseStr);
                //self.GetComponent<ClipboardComponent>().CopyToClipboard(responseStr);
                //Log.Trace($"————Main End！运行时间：{RequestOptionComponent.RequestTime.ElapsedMilliseconds}——--");
                //Log.Trace($"————Response：\r\n {responseStr} \r\n——--");
                Debug.WriteLine($"————Main End！运行时间：{RequestOptionComponent.RequestTime.ElapsedMilliseconds}——--");
                Debug.WriteLine($"————Response：\r\n {responseStr} \r\n——--");
            }
            if (RequestOptionComponent.IsWindow)
                return;
            if (!RequestOptionComponent.IsCloseApp)
                return;
            Thread.Sleep(50);
            Environment.Exit(0);
        }

    }
}
