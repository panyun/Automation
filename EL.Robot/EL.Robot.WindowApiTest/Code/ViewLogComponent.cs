using EL.Robot.Core;
using NPOI.OpenXmlFormats.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace EL.Robot.WindowApiTest.Code
{
	public class ViewLogComponent : Entity
	{
		public LogsViewForm LogsViewForm { get; set; }
	}
	public class ViewLogComponentAwake : AwakeSystem<ViewLogComponent>
	{
		public override void Awake(ViewLogComponent self)
		{
			self.LogsViewForm = new LogsViewForm();
			self.LogsViewForm.Dock = DockStyle.Fill;
		}
	}
	public static class ViewLogComponentSystem
	{
		public static void Main(this ViewLogComponent self)
		{
			var designComponent = Boot.GetComponent<RobotComponent>().GetComponent<DesignComponent>();
			designComponent.RefreshLogMsgAction = (x) =>
			{
				self.LogsViewForm.WriteDesignLogs(x);
			};
			designComponent.RefreshNodeCmdEndAction = async () =>
			{

			};
			designComponent.RefreshNodeCmdAction = (x) =>
			{
				self.LogsViewForm.WriteFlowInfo(x);
			};
			designComponent.ClearNodeCmdAction = () =>
			{
				self.LogsViewForm.ClearFlowLogs();
			};
			self.LogsViewForm.ClearDesignLogs();
			var msgs = designComponent.GetDesignMsg();
			self.LogsViewForm.WriteDesignLogs(msgs.ToArray());
			self.LogsViewForm.ClearFlowLogs();
			designComponent.RefreshAllStepCMD();

		}

		public static void WriteDesignLog(this ViewLogComponent self, string msg, bool isEx = false)
		{
			var designComponent = Boot.GetComponent<RobotComponent>().GetComponent<DesignComponent>();
			designComponent.WriteDesignLog(msg, isEx);
		}

	}
}
