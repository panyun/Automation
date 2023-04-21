using Automation.Com;
using Automation.Inspect;
using EL;
using EL.Async;
using EL.Capturing;
using EL.Http;
using EL.Input;
using EL.Sqlite;
using EL.UIA;
using EL.WindowsAPI;
using Interop.UIAutomationClient;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Automation.Parser
{
    /// <summary>
    /// 聊天消息识别
    /// </summary>
    public static class IdentifyMsgActionSystem
    {
        public static Dictionary<long, CatchWXIdentifyMsgAction> Dic = new Dictionary<long, CatchWXIdentifyMsgAction>();
        //public static CatchWXIdentifyMsgAction Current { get; set; }
        public static string OrcImgUrl
        {
            get
            {
                var imgUri = ConfigurationManager.AppSettings["OcrImgUri"];
                if (string.IsNullOrEmpty(imgUri))
                    return "http://192.168.0.120:10200/ocr_img";
                return imgUri;
            }
        }
        public static CatchWXIdentifyMsgAction Main(this StartMsgActionRequest self)
        {
            var temp = new CatchWXIdentifyMsgAction(self);
            Dic.Add(temp.Id, temp);
            return temp;
            //var elements = self.AvigationElement();
            //var element = elements.FirstOrDefault();
            //var httpComponent = Boot.GetComponent<HttpComponent>();
            //List<string> msgs = new List<string>();
            //List<Msg> temps = new List<Msg>();
            //NewMsgRepeatedTimerId = Boot.GetComponent<TimerComponent>().NewRepeatedTimer(5000, async () =>
            //{
            //    try
            //    {
            //        if (msgs.Count > 10000)
            //            msgs.Clear();
            //        var img = self.GetCap(element.BoundingRectangle).Bitmap;

            //        var jobj = await httpComponent.PostImg<JObject>(OrcImgUrl, img, element.Name);
            //        if (jobj == null || jobj["texts"] == null) return;
            //        temps.Clear();
            //        for (int i = 0; i < jobj["texts"].Count(); i++)
            //        {
            //            var msg = new Msg(jobj["texts"][i] + "");
            //            if (!msgs.Contains(msg.Message))
            //            {
            //                if (Current.ActionAddMsg != null)
            //                    Current.ActionAddMsg(msg);
            //                temps.Add(msg);
            //                msgs.Add(msg.Message);
            //            }
            //        }
            //        if (temps.Count > 0)
            //            SendMsg(self, temps);
            //    }
            //    catch (Exception ex)
            //    {
            //        Log.Error(ex);
            //    }
            //});
            return default;
        }
        public static CatchWXIdentifyMsgAction Get(long Id)
        {
            Dic.TryGetValue(Id, out var action);
            return action;
        }
        public static void MostTop(IUIAutomationElement element)
        {
            //User32.SetWindowLong(element.CurrentNativeWindowHandle, WindowLongParam.GWL_EXSTYLE, WindowStyles.WS_EX_TOPMOST);
            var inter = element.GetNativeWindowHandle();
            User32.SetForegroundWindow(inter.CurrentNativeWindowHandle);
        }
        public static Point GetClickablePoint(IUIAutomationElement uIAutomationElement)
        {
            var rec = uIAutomationElement.GetCurrentPropertyValue(UIA_PropertyIds.UIA_BoundingRectanglePropertyId);
            var boundingRectangle = ValueConverter.ToRectangle(rec);
            return new Point(boundingRectangle.Left + boundingRectangle.Width / 2, boundingRectangle.Top + boundingRectangle.Height / 2);
        }
    }

    public class CatchWXIdentifyMsgAction : IDisposable
    {
        public InspectChat Window;
        public string GroupName = string.Empty;
        public Array RuntimeId = default;
        public string MsgCompareId = "1FC5BB436359FAE7478DA01C714F1AA0";
        public long Id { get; set; }
        public Task Task { get; set; }
        public CancellationTokenSource cts = new CancellationTokenSource();
        public Action<Msg> ActionAddMsg { get; set; }
        public StartMsgActionRequest self { get; set; }
        public IUIAutomationElement element;
        public CatchWXIdentifyMsgAction(StartMsgActionRequest request)
        {
            element = GetRootMsg(request);
            self = request;
            Id = IdGenerater.Instance.GenerateId();
            Window = new InspectChat(this);
            Start();
        }
        public string OrcImgUrl
        {
            get
            {
                var imgUri = ConfigurationManager.AppSettings["OcrImgUri"];
                if (string.IsNullOrEmpty(imgUri))
                    return "http://192.168.0.120:10200/ocr_img";
                return imgUri;
            }
        }
        public void Start()
        {
            var inspect = Boot.GetComponent<InspectComponent>();
            var winInspect = inspect.GetComponent<WinFormInspectComponent>();
            Window.Show();
            Task = Task.Run(() =>
            {
                List<Msg> msgs = new List<Msg>();
                int index = 0;
                IUIAutomationElement uIAutomationElement = default;
                IUIAutomationElementArray arrarElements = null;
                int processId = default;
                int[] tempRuntimeId = default;
                IdentifyMsgActionSystem.MostTop(element);
                var win = element.Convert();
                var point = new Point(win.BoundingRectangle.X + 1, win.BoundingRectangle.Y + 1);
                Mouse.Click(point);
                while (index < 20)
                {
                    index++;
                    Wait.UntilInputIsProcessed(TimeSpan.FromMilliseconds(50));
                    Mouse.Scroll(10);
                }
                index = 0;
                while (!cts.IsCancellationRequested)
                {
                    try
                    {
                        FindMsg(self, winInspect, ref element, msgs, ref index, ref uIAutomationElement, ref arrarElements, out processId, ActionAddMsg);
                        if (msgs.Count == 0) continue;
                        var temps = msgs.Select(x => x.Clone()).ToList();
                        SendMsg(self, temps);
                    }
                    catch (Exception)
                    {

                    }
                    Thread.Sleep(1000);
                }
            }, cts.Token);
        }
        public IUIAutomationElement GetRootMsg(StartMsgActionRequest self)
        {
            var inspect = Boot.GetComponent<InspectComponent>();
            var winInspect = inspect.GetComponent<WinFormInspectComponent>();
            var pathComponent = winInspect.GetComponent<WinPathComponent>();
            var elements = Avigation.Create(self.ElementPath).TryAvigation(self.ElementPath, self.TimeOut);
            var element = elements.FirstOrDefault();
            element.TryElementWin(out var elementWin);
            GroupName = elementWin.Name;
            var array = elementWin.NativeElement.FindAll(Interop.UIAutomationClient.TreeScope.TreeScope_Descendants, winInspect.UIAFactory.CreateTrueCondition());
            IUIAutomationElement elementMsg = default;
            //find 消息节点
            for (int i = 0; i < array.Length; i++)
            {
                var ele = array.GetElement(i);
                if (ele.CompareId(MsgCompareId))
                {
                    elementMsg = ele;
                    break;
                }
            }
            return elementMsg;
        }
        public void FindMsg(StartMsgActionRequest self, WinFormInspectComponent winInspect, ref IUIAutomationElement? element, List<Msg> msgs, ref int index, ref IUIAutomationElement uIAutomationElement, ref IUIAutomationElementArray arrarElements, out int processId, Action<Msg> action)
        {
            if (!IsWinXinProcess(self.ElementPath.ProcessName, out processId))
                return;
            if (element != null)
            {
                arrarElements = element.FindAll(TreeScope.TreeScope_Children,
                                        winInspect.UIAFactory.CreateTrueCondition());
            }
            try
            {
                if (element == null || arrarElements == null || element.CurrentProcessId != processId || !element.CompareId(MsgCompareId))
                {
                    uIAutomationElement = default;
                    while (true)
                    {
                        element = GetRootMsg(self);
                        if (element != null && element.CompareId(MsgCompareId))
                            return;
                    }
                }
            }
            catch (Exception)
            {
                element = null;
                uIAutomationElement = null;
                RuntimeId = default;
                return;
            }
            try
            {
                index = 0;
                if (RuntimeId != default)
                {
                    for (int i = arrarElements.Length - 1; i >= 0; i--)
                    {
                        var e = arrarElements.GetElement(i);
                        var b = (int[])e.GetRuntimeId();
                        if (Enumerable.SequenceEqual((int[])RuntimeId, b))
                        {
                            index = i + 1;
                            break;
                        }
                    }
                }
                msgs.Clear();
                for (int i = index; i < arrarElements.Length; i++)
                {
                    uIAutomationElement = arrarElements.GetElement(i);
                    RuntimeId = uIAutomationElement.GetRuntimeId();
                    string msg = uIAutomationElement.CurrentName;
                    var elementAllNodes = winInspect.GetAllChildrenNode(uIAutomationElement);
                    var elements = elementAllNodes.GetElementWins();
                    if (uIAutomationElement.CurrentName.StartsWith("[语音]"))
                    {
                        msg = FindVoiceMsg(element, uIAutomationElement, msg, elementAllNodes);
                    }
                    var wXMsg = new Msg(msg);
                    wXMsg.GroupName = GroupName;
                    elements = elements.Where(x => !string.IsNullOrWhiteSpace(x.Name) && x.ControlType == UIA_ControlTypeIds.UIA_ButtonControlTypeId).ToList();
                    if (elements != null && elements.Count > 0)
                        wXMsg.Nickname = elements[0].Name;
                    action?.Invoke(wXMsg);
                    msgs.Add(wXMsg);
                }

            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }
        private string FindVoiceMsg(IUIAutomationElement? element, IUIAutomationElement uIAutomationElement, string msg, ElementNode elementAllNodes)
        {
            Regex r = new Regex("\\d+\\.?\\d*");
            bool ismatch = r.IsMatch(uIAutomationElement.CurrentName);
            int time = 500;
            if (ismatch)
            {
                time = int.Parse(r.Matches(uIAutomationElement.CurrentName)[0].Value) * 1000;
            }
            var elementChildrenNodes = elementAllNodes.GetChildrenNode();
            var elementMin = elementChildrenNodes.FirstOrDefault(x => x.LevelIndex == elementChildrenNodes.Min(x => x.LevelIndex));
            var ele = uIAutomationElement.Convert();
            msg += ele.Value;

            if (string.IsNullOrWhiteSpace(ele.Value))
            {
                IdentifyMsgActionSystem.MostTop(element);
                Point point = default;
                int index = 0;
                var win = element.Convert();
                point = new Point(win.BoundingRectangle.X + 1, win.BoundingRectangle.Y + 1);
                Mouse.Click(point);
                while (index < 100)
                {
                    point = IdentifyMsgActionSystem.GetClickablePoint(elementMin.CurrentElementWin.NativeElement);
                    if (element.Convert().BoundingRectangle.Contains(point)) break;
                    Wait.UntilInputIsProcessed(TimeSpan.FromMilliseconds(100));
                    Mouse.Scroll(-2);
                    index++;
                }
                Wait.UntilInputIsProcessed(TimeSpan.FromMilliseconds(100));
                point = IdentifyMsgActionSystem.GetClickablePoint(elementMin.CurrentElementWin.NativeElement);
                Mouse.RightClick(point);
                Wait.UntilInputIsProcessed(TimeSpan.FromMilliseconds(100));
                Keyboard.Type(VirtualKeyShort.DOWN);
                Wait.UntilInputIsProcessed(TimeSpan.FromMilliseconds(100));
                Keyboard.Type(VirtualKeyShort.ENTER);
                Wait.UntilInputIsProcessed(TimeSpan.FromMilliseconds(time));
                ele = uIAutomationElement.Convert();
                msg += ele.Value;
            }
            return msg;
        }
        public bool IsWinXinProcess(string processName, out int wxProcessId)
        {
            var process = Process.GetProcessesByName(processName);
            wxProcessId = process != null && process.Length > 0 ? process[0].Id : default;
            return process != null && process.Length > 0;
        }
        public void Close()
        {
            Window.Close();
        }
        public void SendMsg(StartMsgActionRequest self, List<Msg> msgs)
        {
            switch (self.OutType)
            {
                case OutType.Sqlite:
                    _ = self.Params.TryGetValue("ConnectionString", out string connectionString);
                    if (string.IsNullOrWhiteSpace(connectionString))
                    {
                        Log.Error("sqlite连接字符串异常！");
                        throw new ParserException("sqlite连接字符串异常");
                    }
                    Log.Trace(connectionString);
                    WriteDB(msgs, connectionString);
                    break;
                case OutType.HttpApi:
                    _ = self.Params.TryGetValue("url", out string url);
                    if (string.IsNullOrWhiteSpace(url))
                    {
                        Log.Error("url网址异常！");
                        throw new ParserException("url网址异常");
                    }
                    Log.Trace(url);
                    HttpApi(msgs, url).Coroutine();
                    break;
                default:
                    break;
            }
        }

        public void WriteDB(List<Msg> wXMsgs, string connectionString)
        {
            var inspect = Boot.GetComponent<InspectComponent>();
            var sqliteComponent = inspect.GetComponent<SqliteComponent>();
            if (!sqliteComponent.IsExist("WinXinMsg"))
            {
                sqliteComponent.CreateDataFile("WinXinMsg.db");
                sqliteComponent.CreateTable(@"CREATE TABLE  if not exists 'WinXinMsg' ( ""Id"" INTEGER NOT NULL, ""Msg"" TEXT, ""Time"" INTEGER,""Nickname"" TEXT,  PRIMARY KEY ( ""Id"" ) );");
            }
            var cmds = new List<dynamic>();
            Func<SqliteCommand, List<Task<int>>> sqliteCommand = (command) =>
            {
                List<Task<int>> result = new List<Task<int>>();
                foreach (var x in wXMsgs)
                {
                    command.Parameters.Clear();
                    command.CommandText = $"insert into WinXinMsg (Id,Msg,Nickname,Time) values (@Id,@Msg,@Nickname,@Time)";
                    command.Parameters.AddRange(new SqliteParameter[] {
                          new SqliteParameter("@Id", x.Id),
                         new SqliteParameter("@Msg", string.IsNullOrWhiteSpace(x.Message)?"":x.Message),
                         new SqliteParameter("@Nickname", string.IsNullOrWhiteSpace(x.Nickname)?"":x.Nickname),
                         new SqliteParameter("@Time", x.Time),
                    });
                    try
                    {
                        result.Add(command.ExecuteNonQueryAsync());
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);
                    }
                }
                return result;
            };
            try
            {
                sqliteComponent.BulkExecute(sqliteCommand, connectionString);

            }
            catch (Exception)
            {

            }
        }
        public async ELTask HttpApi(List<Msg> wXMsgs, string url)
        {
            var httpComponent = Boot.GetComponent<HttpComponent>();
            var json = JsonHelper.ToJson(new { WXMsg = wXMsgs });
            var result = await httpComponent.Post<JObject>(url, json);
            //if (result == default || result["meta"]["errcode"].Value<int>() != 0)
            //    Log.Error($"消息写入失败！error:{JsonHelper.ToJson(result)};\r\n Wxmsg:{json}");
        }

        public void Dispose()
        {
            IdentifyMsgActionSystem.Dic.Remove(Id);
            cts.Cancel();
            int index = 0;
            Task.Run(() =>
            {
                while (index < 1000)
                {
                    index++;
                    Thread.Sleep(100);
                    if (Task.IsCompleted)
                    {
                        Task.Dispose();
                        break;
                    }
                }
            });
            ((IDisposable)cts).Dispose();
            Window.Dispose();
        }
    }
}
