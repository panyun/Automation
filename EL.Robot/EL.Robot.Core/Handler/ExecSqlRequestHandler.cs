using Automation;
using Automation.Inspect;
using Automation.Parser;
using EL.Async;
using EL.Robot.Core.Request;
using EL.Robot.Core.SqliteEntity;
using EL.Sqlite;
using Protos;
using System.Configuration;
using Utils;

namespace EL.Robot.Core.Handler
{
    [MessageHandler]
    public class ExecSqlRequestHandler : W_AMHandler<ExecSqlRequest>
    {
        protected override async ELTask Run(WChannel channel, ExecSqlRequest message)
        {
            await ELTask.CompletedTask;
            var sqliteComponent = Boot.GetComponent<RobotComponent>().GetComponent<SqliteComponent>();
            object obj = default;
            int error = 0;
            string errorMsg = string.Empty;
            try
            {
                if (message.Type == 1)
                {
                    obj = await sqliteComponent.ExecuteNonQueryAsync(message.Command);
                    Boot.GetComponent<RobotComponent>().FlowUpdateAction?.Invoke();
                }
                else
                    obj = sqliteComponent.Query<FlowData>(message.Command);
            }
            catch (Exception ex)
            {
                error = 601;
                errorMsg = ex.Message;
            }
            MsgAgentComonpment.Instance.OnReply(message.AgentRequest as C2G_MsgAgent, new ExecSqlResponse()
            {
                Error = error,
                Message = errorMsg,
                Data = obj,
                RpcId = message.RpcId
            }).Coroutine();
        }
    }
}
