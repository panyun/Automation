using EL.Async;
using EL.Robot.Core.Request;
using Protos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace EL.Robot.Core.Handler
{
    [MessageHandler]
    public class MsgAgentRequestHandler : W_AMHandler<C2G_MsgAgent>
    {
        protected override async ELTask Run(WChannel channel, C2G_MsgAgent message)
        {
            await ELTask.CompletedTask;
            var robot = Boot.GetComponent<RobotComponent>();
            MsgAgentComonpment.Instance.TargetClientId = message.SelfClientId;
            MsgAgentComonpment.Instance.OnRead(message);
        }
    }
}
