using Automation.Inspect;
using Automation.Parser;
using EL;
using EL.Async;
using EL.Http;
using EL.Input;
using EL.Overlay;
using EL.Sqlite;
using EL.UIA;
using EL.WindowsAPI;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Interop.UIAutomationClient;

namespace Automation.Com
{
    public static class WeiXinMsgCatchServer
    {
        public static long NewMsgRepeatedTimerId = default;
        public static Action<WXMsg> ActionAddMsg { get; set; }
        public static object objectLock = new object();
        public static void Main(ElementPath para)
        {
            IUIAutomationElement uIAutomationElement = default;
            var inspect = Boot.GetComponent<InspectComponent>();
            var winInspect = inspect.GetComponent<WinFormInspectComponent>();

            var element = GetRootMsg(para);
            List<WXMsg> msgs = new List<WXMsg>();
            int index = 0;
            IUIAutomationElementArray arrarElements = null;
            int processId = default;
            int[] tempRuntimeId = default;

            while (index < 10)
            {
                index++;
                Mouse.Click(element.GetClickablePoint());
                Wait.UntilInputIsProcessed(TimeSpan.FromMilliseconds(50));
                Keyboard.Type(VirtualKeyShort.PRIOR);
            }
            index = 0;
            NewMsgRepeatedTimerId = Boot.GetComponent<TimerComponent>().NewRepeatedTimer(100, () =>
          {
              lock (objectLock)
              {
                  if (!IsWinXinProcess(para.ProcessName, out processId))
                      return;
                  if (element != null)
                  {
                      arrarElements = element.FindAll(TreeScope.TreeScope_Children,
                                              winInspect.UIAFactory.CreateTrueCondition());
                  }
                  try
                  {
                      if (uIAutomationElement != null)
                          tempRuntimeId = (int[])uIAutomationElement.GetRuntimeId();
                      if (element == null || arrarElements == null || element.CurrentProcessId != processId || !(element.CurrentControlType == 50008 && element.CurrentName == "消息"))
                      {
                          uIAutomationElement = default;
                          while (true)
                          {
                              element = GetRootMsg(para);
                              if (element != null &&
                                  element.CurrentProcessId == processId &&
                                  element.CurrentControlType == 50008 &&
                                  element.CurrentName == "消息")
                                  return;
                          }
                      }
                  }
                  catch (Exception)
                  {
                      element = null;
                      uIAutomationElement = null;
                      return;
                  }
                  try
                  {
                      index = 0;
                      if (tempRuntimeId != default)
                      {
                          for (int i = arrarElements.Length - 1; i >= 0; i--)
                          {
                              var e = arrarElements.GetElement(i);
                              var b = (int[])e.GetRuntimeId();
                              if (Enumerable.SequenceEqual(tempRuntimeId, b))
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
                          string msg = uIAutomationElement.CurrentName;
                          var elementAllNodes = winInspect.GetAllChildrenNode(uIAutomationElement);
                          var elements = elementAllNodes.GetElementWins();
                          if (uIAutomationElement.CurrentName.Contains("语音"))
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
                                  Point point = default;
                                  while (true)
                                  {
                                      point = GetClickablePoint(elementMin.CurrentElementWin.NativeElement);
                                      if (element.GetRectangle().Contains(point)) break;
                                      Mouse.Click(element.Convert().ClickablePoint);
                                      Wait.UntilInputIsProcessed(TimeSpan.FromMilliseconds(50));
                                      Mouse.Scroll(-2);
                                  }
                                  Wait.UntilInputIsProcessed(TimeSpan.FromMilliseconds(100));
                                  point = GetClickablePoint(elementMin.CurrentElementWin.NativeElement);
                                  Mouse.RightClick(point);
                                  Wait.UntilInputIsProcessed(TimeSpan.FromMilliseconds(50));
                                  Keyboard.Type(VirtualKeyShort.DOWN);
                                  Wait.UntilInputIsProcessed(TimeSpan.FromMilliseconds(50));
                                  Keyboard.Type(VirtualKeyShort.ENTER);
                                  Wait.UntilInputIsProcessed(TimeSpan.FromMilliseconds(time));
                                  ele = uIAutomationElement.Convert();
                                  msg += ele.Value;
                              }
                          }
                          var wXMsg = new WXMsg(msg);
                          elements = elements.Where(x => !string.IsNullOrWhiteSpace(x.Name) && x.ControlType == UIA_ControlTypeIds.UIA_ButtonControlTypeId).ToList();
                          if (elements != null && elements.Count > 0)
                              wXMsg.Nickname = elements[0].Name;
                          ActionAddMsg?.Invoke(wXMsg);
                          msgs.Add(wXMsg);
                      }
                      if (msgs.Count == 0) return;
                      var temps = msgs.Select(x => x.Clone()).ToList();
                      SendMsg(temps).Coroutine();
                      WriteDB(temps);
                  }
                  catch (Exception ex)
                  {
                      Log.Error(ex);
                  }
              }

          });
        }
        public static Point GetClickablePoint(IUIAutomationElement uIAutomationElement)
        {
            var rec = uIAutomationElement.GetCurrentPropertyValue(UIA_PropertyIds.UIA_BoundingRectanglePropertyId);
            var boundingRectangle = ValueConverter.ToRectangle(rec);
            return new Point(boundingRectangle.Left + boundingRectangle.Width / 2, boundingRectangle.Top + boundingRectangle.Height / 2);
        }
        public static bool IsWinXinProcess(string processName, out int wxProcessId)
        {
            var process = Process.GetProcessesByName(processName);
            wxProcessId = process != null && process.Length > 0 ? process[0].Id : default;
            return process != null && process.Length > 0;
        }
        public static IUIAutomationElement GetRootMsg(ElementPath para)
        {
            if (para == null) throw new ArgumentNullException("请先加载探测内容！");
            var inspect = Boot.GetComponent<InspectComponent>();
            var winInspect = inspect.GetComponent<WinFormInspectComponent>();
            var sqliteComponent = Boot.GetComponent<SqliteComponent>();
            var node = para.PathNode.GetParentNode().FirstOrDefault(x => x.GetCompareValue(nameof(ElementExpand.CompareId)) == "1FC5BB436359FAE7478DA01C714F1AA0" || (x.CurrentElementWin.ControlType == 50008 && x.CurrentElementWin.Name == "消息"));
            var elements = new ElementNodeAvigationSystem().SearchElement(node);
            if (elements == null) return default;
            return elements[0];
        }
        public static IUIAutomationCondition CreateCondition()
        {
            var inspect = Boot.GetComponent<InspectComponent>();
            var winInspect = inspect.GetComponent<WinFormInspectComponent>();
            var conditionType = winInspect.UIAFactory.CreatePropertyCondition(UIA_PropertyIds.UIA_ControlTypePropertyId, 50000);
            var list = new List<IUIAutomationCondition>() { conditionType };
            return winInspect.UIAFactory.CreateAndConditionFromArray(list.ToArray());
        }
        public static void WriteDB(List<WXMsg> wXMsgs)
        {
            var sqliteComponent = Boot.GetComponent<SqliteComponent>();
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
                         new SqliteParameter("@Msg", string.IsNullOrWhiteSpace(x.Msg)?"":x.Msg),
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
            sqliteComponent.BulkExecute(sqliteCommand);
        }
        public static async ELTask SendMsg(List<WXMsg> wXMsgs)
        {
            string url = @"http://wunlzt.cdwh.gov.cn/apis/events/0595a674ca32c1ef24cd66a3607b66f1";
            var httpComponent = Boot.GetComponent<HttpComponent>();
            var json = JsonHelper.ToJson(new { WXMsg = wXMsgs });
            var result = await httpComponent.Post<JObject>(url, json);
            if (result == default || result["meta"]["errcode"].Value<int>() != 0)
                Log.Error($"消息写入失败！error:{JsonHelper.ToJson(result)};\r\n Wxmsg:{json}");
        }
    }
    public class WXMsg
    {
        public WXMsg()
        {

        }
        public WXMsg(string msg)
        {
            Id = (long)IdGenerater.Instance.GenerateInstanceId();
            Time = TimeHelper.ServerNow();
            Msg = msg;
        }
        public long Id { get; set; }
        public string Msg { get; set; }
        public string Nickname { get; set; }
        public long Time { get; set; }
        public WXMsg Clone()
        {
            return new WXMsg()
            {
                Id = Id,
                Msg = Msg,
                Nickname = Nickname,
                Time = Time
            };
        }
    }
}
