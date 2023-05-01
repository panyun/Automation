using EL.Async;
using EL.Robot.Component;

namespace EL.Robot.WindowApiTest
{
	public class NodePanel
	{
		public int Width { get; set; }
		public Node Node { get; set; }
	}
	public partial class DesignViewForm : UserControl
	{
		public Action<Flow> InitAction { get; set; }
		public Action<Node> SelectAction { get; set; }
		public Action<Node, bool> HideExpansionAction { get; set; }

		public DesignViewForm()
		{
			InitializeComponent();
			this.Height = 300;
			var designViewComponent = Boot.GetComponent<DesignViewComponent>();
			designViewComponent.HideExpansionNodeAction = (node, hide) =>
			{
				Hide(this.pl_Content, node, hide);
			};
			pl_Content.VerticalScroll.SmallChange = 50;
		}
		public void LoadFlow()
		{
			var designViewComponent = Boot.GetComponent<DesignViewComponent>();
			designViewComponent.VewPanl = pl_Content;
			pl_Content.Controls.Clear();
			//创建控件
			DesignRowViewForm designRowViewForm;
			int index = 0;
			int width = 733;
			int widthTemp = 35;
			List<Node> nodes = new();
			FlowLayoutPanel panel = default;
			foreach (var item in designViewComponent.CurrentFlow.DesignSteps)
			{
				index++;
				designRowViewForm = new();
				designRowViewForm.Dock = DockStyle.Top;
				designViewComponent.RowViews.Add(designRowViewForm);
				var isExists = nodes.Exists(x => x.Id == item.Id);
				if (isExists && panel != null)
				{
					if (panel.Parent is FlowLayoutPanel pl)
					{
						if (pl.Tag != null)
							panel = panel.Parent as FlowLayoutPanel;
						else panel = null;
					}
				}
				if (panel != null)
				{
					panel.Controls.Add(designRowViewForm);
				}
				else
					this.pl_Content.Controls.Add(designRowViewForm);
				var row = item.GetRowData(index + "");
				designRowViewForm.BorderStyle = BorderStyle.None;
				designRowViewForm.Tag = item;
				designViewComponent.UpdateRowView(designRowViewForm, row);
				if (item.LinkNode != null)
				{
					nodes.Add(item.LinkNode);
					if (panel == null)
					{
						panel = new FlowLayoutPanel
						{
							Tag = new NodePanel()
							{
								Node = item,
								Width = width - widthTemp,
							},
							Margin = new Padding(0, 0, 0, 0),
							AutoSize = true,
							Dock = DockStyle.Top,
						};

						this.pl_Content.Controls.Add(panel);
					}
					else
					{
						var np = new NodePanel()
						{
							Node = item,
						};
						panel = new FlowLayoutPanel
						{
							Tag = np,
							AutoSize = true,
							Margin = new Padding(0, 0, 0, 0),
							Dock = DockStyle.Top,
						};
						var temp1 = FindPanel(pl_Content, item);
						if (temp1 != null)
						{
							np.Width = (temp1.Tag as NodePanel).Width - widthTemp;
							temp1.Controls.Add(panel);
						}
						else
						{

						}

					}

				}
			}
		}
		public void Scroll(int row)
		{
			var comp = Boot.GetComponent<DesignViewComponent>();
			var val = (pl_Content.VerticalScroll.Maximum / comp.RowViews.Count) + 1;
			val = Math.Min(val * row, pl_Content.VerticalScroll.Maximum);
			if (pl_Content.VerticalScroll.Value + val > pl_Content.VerticalScroll.Maximum)
			{
				pl_Content.VerticalScroll.Value = pl_Content.VerticalScroll.Maximum;
			}
			else
				pl_Content.VerticalScroll.Value += val;
		}
		public FlowLayoutPanel FindPanel(FlowLayoutPanel pl, Node node)
		{
			if (node.DesignParent == null) return default;
			foreach (var item in pl.Controls)
			{
				if (item is FlowLayoutPanel plNode && plNode.Tag != null && plNode.Tag is NodePanel nodeTag)
				{
					if (node.DesignParent.Id == nodeTag.Node.Id)
						return plNode;
					var temp = FindPanel(plNode, node);
					if (temp != null) return temp;
				}
			}
			return default;
		}
		public void Hide(FlowLayoutPanel pl, Node node, bool isHide)
		{
			foreach (var control in pl.Controls)
			{
				if (control is FlowLayoutPanel fl)
				{
					var nodeTemp = (NodePanel)fl.Tag;
					if (nodeTemp.Node.Id == node.Id)
					{
						if (isHide)
							fl.Hide();
						else
							fl.Show();
					}
					Hide(fl, node, isHide);
				}
			}
		}
	}
}
