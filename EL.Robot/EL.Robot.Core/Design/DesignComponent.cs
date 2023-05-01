﻿using Automation.Com;
using EL.Robot.Component;
using EL.Robot.Core.SqliteEntity;
using System.Reflection;

namespace EL.Robot.Core
{
	public class DesignComponent : Entity
	{
		public Dictionary<long, Flow> DesignFlowDic = new Dictionary<long, Flow>();
		public Flow CurrentDesignFlow { get; set; }
		public List<DesignMsg> LogMsgs { get; set; } = new List<DesignMsg>();

		public Action<DesignMsg> RefreshLogMsgAction { get; set; }
		public Action ClearNodeCmdAction { get; set; }
		public Action<string[]> RefreshNodeCmdAction { get; set; }
		public Action RefreshNodeCmdEndAction { get; set; }
		public List<FlowData> FlowDatas { get; set; } = new List<FlowData>();
		public List<Features> Features { get; set; } = new List<Features>();
		public NodeComponent NodeComponentRoot
		{
			get
			{
				return Boot.GetComponent<RobotComponent>().GetComponent<FlowComponent>().GetComponent<NodeComponent>();
			}
		}
	}


	public class Features
	{
		public long Id { get; set; }
		public string Name { get; set; }
		public string HeadImg { get; set; }
		public long ViewSort { get; set; }
		public long CreateDate { get; set; }
	}


}
