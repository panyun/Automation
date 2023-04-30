using EL.Robot.Component;
using EL.Robot.Core;

namespace EL.Robot.WindowApiTest
{
	public partial class LogsViewForm : UserControl
	{
		public LogsViewForm()
		{
			InitializeComponent();
		}
		public void WriteDesignLogs(params DesignMsg[] designLogs)
		{
			foreach (DesignMsg msg in designLogs)
			{
				rtb_designLogs.SelectionStart = rtb_designLogs.Text.Length;
				rtb_designLogs.ForeColor = Color.Red;
				rtb_designLogs.Font = new Font("黑体", 11);
				rtb_designLogs.AppendText(msg.ShowMsg + Environment.NewLine);
			}
			//rtb_designLogs.ScrollToCaret();
			//rtb_designLogs.Select(start, str.Length);
			tabControl1.SelectedTab = tb_FlowDesignLog;
		}

		public void WriteRunLogs(params string[] strs)
		{
			int start = rtb_designLogs.SelectionStart;
			string str = string.Empty;
			foreach (string msg in strs)
			{
				str = msg;
				rtb_designLogs.SelectionStart = rtb_designLogs.Text.Length;
				rtb_designLogs.ForeColor = Color.Red;
				rtb_designLogs.Font = new Font("黑体", 11);
				rtb_designLogs.AppendText(str + Environment.NewLine);
				str += (str + Environment.NewLine);
			}
			rtb_designLogs.ScrollToCaret();
			rtb_designLogs.Select(start, str.Length);
			rtb_designLogs.SelectedText = str;
			tabControl1.SelectedTab = tb_FlowDesignLog;
		}
		public void WriteRunLogs(params ExecLog[] execLogs)
		{
			int start = rtb_designLogs.SelectionStart;
			string str = string.Empty;
			foreach (ExecLog msg in execLogs)
			{
				rtb_designLogs.SelectionStart = rtb_designLogs.Text.Length;
				rtb_designLogs.ForeColor = Color.Red;
				rtb_designLogs.Font = new Font("黑体", 11);
				rtb_designLogs.AppendText(msg.ShowMsg + Environment.NewLine);
			}
			rtb_designLogs.ScrollToCaret();
			rtb_designLogs.Select(start, str.Length);
			rtb_designLogs.SelectedText = str;
			tabControl1.SelectedTab = tb_FlowDesignLog;
		}
		public void WriteFlowInfo(params string[] strings)
		{
			int start = rtb_flowInfo.SelectionStart;
			string str = string.Empty;

			foreach (var msg in strings)
			{
				str = msg + Environment.NewLine;
				rtb_flowInfo.SelectionStart = rtb_designLogs.Text.Length;
				rtb_flowInfo.ForeColor = Color.Red;
				rtb_flowInfo.Font = new Font("黑体", 11);
				rtb_flowInfo.AppendText(str);
			}
			rtb_flowInfo.ScrollToCaret();
			rtb_flowInfo.Select(start, str.Length);
			rtb_flowInfo.SelectedText = str;
			tabControl1.SelectedTab = tb_flowInfo;
		}
		public void WriteFlowInfo(params Node[] nodes)
		{
			int start = rtb_flowInfo.SelectionStart;
			string str = string.Empty;

			foreach (Node node in nodes)
			{
				str = node.DisplayExp + Environment.NewLine;
				rtb_flowInfo.SelectionStart = rtb_designLogs.Text.Length;
				rtb_flowInfo.ForeColor = Color.Red;
				rtb_flowInfo.Font = new Font("黑体", 11);
				rtb_flowInfo.AppendText(str);
			}
			rtb_flowInfo.ScrollToCaret();
			rtb_flowInfo.Select(start, str.Length);
			rtb_flowInfo.SelectedText = str;
			tabControl1.SelectedTab = tb_flowInfo;
		}
		public void ClearDesignLogs()
		{
			rtb_designLogs.Clear();
		}
		public void ClearFlowLogs()
		{
			rtb_flowInfo.Clear();
		}
	}
}
