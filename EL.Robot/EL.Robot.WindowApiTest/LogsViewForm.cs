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
			int start = rtb_designLogs.SelectionStart;
			string str = string.Empty;
			foreach (DesignMsg msg in designLogs)
			{
				rtb_designLogs.SelectionStart = rtb_designLogs.Text.Length;
				rtb_designLogs.ForeColor = Color.Red;
				rtb_designLogs.Font = new Font("黑体", 11);
				rtb_designLogs.AppendText(msg.ShowMsg + Environment.NewLine);
				str += (msg.ShowMsg + Environment.NewLine);
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
				str += (msg.ShowMsg + Environment.NewLine);
			}
			rtb_designLogs.ScrollToCaret();
			rtb_designLogs.Select(start, str.Length);
			rtb_designLogs.SelectedText = str;
			tabControl1.SelectedTab = tb_FlowDesignLog;
		}
		public void ClearAllLogs()
		{
			rtb_designLogs.Clear();
			rtb_rowLogs.Clear();
			lv_paramters.Clear();
		}
	}
}
