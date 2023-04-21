using Automation;
using Automation.Inspect;
using Automation.Parser;
using EL.Async;
using EL.Basic.Component.Clipboard;
using EL.Robot.Core.Request;
using EL.Sqlite;
using Protos;
using System.Configuration;
using System.Diagnostics;
using System.Windows.Threading;
using Utils;
using WpfInspect;

namespace EL.Robot.Core.Handler
{
    [MessageHandler]
    public class EditRequestHandler : W_AMHandler<EditRequest>
    {
        protected override async ELTask Run(WChannel channel, EditRequest message)
        {
            await ELTask.CompletedTask;
            await Boot.GetComponent<TimerComponent>().WaitAsync(1);
            DispatcherHelper.ExecInspect((inspect, parser) =>
              {
                  var isWindows = RequestOptionComponent.IsWindow;
                  Debug.WriteLine(isWindows.ToString());
                  Thread.Sleep(100);
                  var elSqlComponent = Boot.AddComponent<ElSqliteComponent>();
                  var robot = Boot.GetComponent<RobotComponent>();
                  //(bool isPass, string msg) = DataRequestComonpment.Try(5, () =>
                  //{
                  //    return DataRequestComonpment.GetData(message.ElementPath);
                  //});
                  //if (!isPass)
                  //{
                  //    MsgAgentComonpment.Instance.OnReply(message.AgentRequest as C2G_MsgAgent, new RunMsg()
                  //    {
                  //        Error = 600,
                  //        Message = msg,
                  //        RpcId = message.RpcId
                  //    }).Coroutine();
                  //    return;
                  //}
                  if (DispatcherHelper.mainWindowInstance == null)
                  {
                      DispatcherHelper.mainWindowInstance = new MainWindow();
                      DispatcherHelper.mainWindowInstance.Closed += (x, y) =>
                      {
                          DispatcherHelper.mainWindowInstance = null;
                      };
                  }
                  var mainWindow = DispatcherHelper.mainWindowInstance;
                  try
                  {
                   
                      //var elementPath = message.ElementPath;
                      mainWindow.Topmost = true;
                      mainWindow.Show();
                      mainWindow.Topmost = false;
                      mainWindow._vm.LoadElementModelByRobot(message.ElementPath, async (element, elementPath) =>
                      {

                          await MsgAgentComonpment.Instance.OnReply(message.AgentRequest as C2G_MsgAgent, new EditResponse()
                          {
                              Error = 0,
                              Message = "",
                              RpcId = message.RpcId,
                              ElementPath = elementPath,
                          });
                      });
                  }
                  catch (Exception ex)
                  {
                      Log.Error(ex);
                  }
              });
        }
    }
}
