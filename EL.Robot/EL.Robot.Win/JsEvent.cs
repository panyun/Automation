using EL.Robot.Core;
using EL.Robot.Core.Request;
using System.Runtime.InteropServices;

namespace EL.Robot.Win
{
    [ComVisible(true)]
    public class jsEvent
    {
        public void LocationComponent(string nodeId)
        {
            long.TryParse(nodeId, out long componentId);
            var robot = Boot.GetComponent<RobotComponent>();
            var logs = robot.GetComponent<FlowComponent>().LogMsgs;
            MsgAgentComonpment.Instance.Send(new LocationRequest()
            {
                ComponentId = componentId,
                LogMsgs = logs
            }).Coroutine();
        }
    }
}
