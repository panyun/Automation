using EL.Robot.Component;

namespace EL.Robot.Core
{



	public class DesignComponent : Entity
	{
		public Dictionary<long, Flow> DesignFlowDic = new Dictionary<long, Flow>();
		public Flow CurrentDesignFlow { get; set; }
		public List<DesignMsg> LogMsgs { get; set; } = new List<DesignMsg>();
		public Action<DesignMsg> RefreshLogMsgAction { get; set; }
	}



}
