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
			this.Invoke(() =>
			{
				var statr = rtb_designLogs.Text.Length;
				foreach (DesignMsg msg in designLogs)
				{
					rtb_designLogs.SelectionStart = rtb_designLogs.Text.Length;
					rtb_designLogs.SelectionColor = Color.Blue;
					if (msg.IsException)
						rtb_designLogs.SelectionColor = Color.Red;
					rtb_designLogs.SelectionFont = new Font("黑体", 11);
					rtb_designLogs.AppendText(msg.ShowMsg + Environment.NewLine);
				}
				rtb_designLogs.ScrollToCaret();
				rtb_designLogs.Select(statr, rtb_designLogs.SelectionLength);
				tabControl1.SelectedTab = tb_FlowDesignLog;
			});

		}

		public void WriteRunLogs(params string[] strs)
		{
			this.Invoke(() =>
			{
				int start = rtb_execLogs.SelectionStart;
				string str = string.Join(Environment.NewLine, strs);
				rtb_execLogs.SelectionStart = rtb_execLogs.Text.Length;
				rtb_execLogs.SelectionColor = Color.Blue;
				rtb_execLogs.SelectionFont = new Font("黑体", 10);
				rtb_execLogs.AppendText(str);
				rtb_execLogs.ScrollToCaret();
				tabControl1.SelectedTab = tb_FlowRunLog;
			});
		}
		public void WriteFlowInfo(params string[] strs)
		{
			this.Invoke(() =>
			{
				int start = rtb_flowInfo.SelectionStart;
				string str = string.Join(Environment.NewLine, strs);
				rtb_flowInfo.SelectionStart = rtb_designLogs.Text.Length;
				rtb_flowInfo.SelectionColor = Color.Blue;
				rtb_flowInfo.SelectionFont = new Font("黑体", 10);
				rtb_flowInfo.AppendText(str);
				rtb_flowInfo.ScrollToCaret();
				tabControl1.SelectedTab = tb_flowInfo;
			});
		}
		public void ClearDesignLogs()
		{

			this.Invoke(() =>
			{
				rtb_designLogs.Clear();
			});
		}
		public void ClearFlowLogs()
		{
			this.Invoke(() =>
			{
				rtb_flowInfo.Clear();
			});
		}
	}
}
