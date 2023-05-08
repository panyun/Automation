using EL.Async;
using EL.Robot.Component;
using NPOI.SS.Formula.Eval;
using NPOI.SS.Formula.Functions;
using System.Diagnostics;
using System.Windows.Forms;

namespace EL.Robot.WindowApiTest
{
	public partial class DesignFlowViewForm : UserControl
	{
		public FlowLayoutPanel pl_Content;
		public DesignFlowViewForm()
		{
			InitializeComponent();
			pl_Content = new FlowLayoutPanel()
			{
				Dock = DockStyle.Fill
			};
			pl_Content.AutoScroll = true;
			this.Controls.Add(pl_Content);
			var designViewComponent = Boot.GetComponent<DesignFlowViewComponent>();
			designViewComponent.HideExpansionNodeAction = (node, hide) =>
			{
				Hide(node, hide);
			};
		}
		public void LoadFlow(Flow flow)
		{
			pl_Content.Controls.Clear();
			pl_Content.Tag = new NodePanel()
			{
				Node = null,
				Width = 740,
			};
			var flowView = Boot.GetComponent<DesignFlowViewComponent>();

			flowView.LoadRows(pl_Content, flow.Steps);
		}
		public void Hide(DesignFlowRowViewForm designFlowRowViewForm, bool isHide)
		{
			var flowView = Boot.GetComponent<DesignFlowViewComponent>();
			var fl = flowView.FindPanle(pl_Content, designFlowRowViewForm.Tag as Node);
			if (isHide)
				fl.Hide();
			else
				fl.Show();
		}
		int count = 0;
		public void Count(FlowLayoutPanel fl)
		{
			foreach (var item in fl.Controls)
			{
				if (item is DesignFlowRowViewForm dfr)
				{
					count++;
					dfr.UpdateIndex(count);
					continue;
				}
				Count(item as FlowLayoutPanel);
			}
		}
		public void Scroll()
		{
			var comp = Boot.GetComponent<DesignFlowViewComponent>();
			int val = pl_Content.VerticalScroll.Value + 33;
			val = Math.Min(val, pl_Content.VerticalScroll.Maximum);
			pl_Content.VerticalScroll.Value = val;
		}
	}
}
