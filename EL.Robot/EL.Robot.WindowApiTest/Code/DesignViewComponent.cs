using EL.Robot.Component;
using EL.Robot.Core;
using EL.Robot.Core.SqliteEntity;
using EL.Robot.WindowApiTest.Code;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;

namespace EL.Robot.WindowApiTest
{
	public class DesignViewComponent : Entity
	{
		public List<DesignRowViewForm> RowViews { get; set; }
		public IndexForm IndexForm { get; set; }
		public DesignViewForm DesignViewForm { get; set; }
		public Action<RowData> UpdateRowViewAction { get; set; }
		public Action InitViewAction { get; set; }
		public Action<Node, bool> HideExpansionNodeAction { get; set; }
		public ContextMenuStrip ContextMenuStrip { get; set; }
		public List<DesignRowViewForm> SelectRowViews { get; set; } = new List<DesignRowViewForm>();
		public Flow CurrentFlow { get; set; }
		public EditNode EditNode { get; set; }
		private DesignRowViewForm currentRowView;
		public DesignRowViewForm CurrentRowView
		{
			get
			{
				return currentRowView;
			}
			set
			{
				currentRowView = value;
				SelectRowViews.Clear();
				if (currentRowView != default)
				{
					SelectRowViews.Add(currentRowView);
					foreach (var item in RowViews)
						item.SetBackColor(Color.LightSteelBlue);
					currentRowView.SetBackColor(Color.LightSkyBlue);
				}
			}
		}
		public Node CurrentNode
		{
			get
			{
				if (CurrentRowView.Tag is Node np)
					return np;
				return default;
			}
		}
		public FlowLayoutPanel VewPanl { get; set; }
	}
	public class EditNode
	{
		public Config Config { get; set; }
		public Dictionary<string, Parameter> ParamDic = new Dictionary<string, Parameter>();
		public Node Node { get; set; }
		public DesignRowViewForm CurrentDesignRowViewForm { get; set; }
	}

	public class DesignViewComponentAwake : AwakeSystem<DesignViewComponent>
	{
		public override void Awake(DesignViewComponent self)
		{
			self.IndexForm = new IndexForm();
			self.RowViews = new List<DesignRowViewForm>();
			self.DesignViewForm = new DesignViewForm
			{
				Visible = true,
				Dock = DockStyle.Fill
			};
			self.ContextMenuStrip = new ContextMenuStrip();
			ToolStripMenuItem editItem = new ToolStripMenuItem
			{
				Text = "编辑",
			};
			editItem.Click += (x, y) =>
			{
				var editItem = x as ToolStripMenuItem;
				self.EditNode(editItem.Tag as DesignRowViewForm);
			};
			self.ContextMenuStrip.Items.Add(editItem);
		}
	}

	public static class DesignViewComponentSystem
	{

		public static void Main(this DesignViewComponent self, Flow flow)
		{
			self.CurrentFlow = flow;
			self.RowViews.Clear();
			self.EditNode = null;
			self.CurrentRowView = null;
			self.DesignViewForm.LoadFlow();
		}
		public static void SetCurrentNode(this DesignViewComponent self, DesignRowViewForm row)
		{
			var node = row.Tag as Node;
			self.WriteLog("当前选中操作节点" + $"[{node.DisplayExp}]");
			foreach (var item in self.RowViews)
				item.SetBackColor(Color.LightSteelBlue);
			if ((Control.ModifierKeys & Keys.Control) == Keys.Control || (Control.ModifierKeys & Keys.Shift) == Keys.Shift)
			{
				self.SelectRowViews.Add(row);
				self.SelectRowViews.ForEach(x => x.SetBackColor(Color.LightSkyBlue));
			}
			else
			{
				self.CurrentRowView = row;
				self.EditNode(row);
			}

		}
		private static void WriteLog(this DesignViewComponent self, string msg)
		{
			var viewLogComponent = Boot.GetComponent<ViewLogComponent>();
			viewLogComponent.WriteLog(msg);
		}
		public static void UpdateRowView(this DesignViewComponent self, DesignRowViewForm rowViewForm, RowData rowInfo)
		{
			rowViewForm.Update(rowInfo);
		}
		public static void EditNode(this DesignViewComponent self, DesignRowViewForm rowViewForm)
		{
			self.CurrentRowView = rowViewForm;
			if (self.SelectRowViews.Count != 1 && self.SelectRowViews[0].Tag == null)
			{
				self.WriteLog("多个节点不能编辑");
				return;
			}
			var edit = new EditNode
			{
				Node = self.SelectRowViews[0].Tag as Node
			};

			if (edit.Node == null || edit.Node.ComponentName == (nameof(StartComponent)))
			{
				self.WriteLog("开始节点不能编辑");
				return;
			}
			edit.CurrentDesignRowViewForm = rowViewForm;
			var designComponent = Boot.GetComponent<RobotComponent>().GetComponent<DesignComponent>();
			var component = designComponent.GetComponentInfo(edit.Node.ComponentName);
			component.GetConfig();
			edit.Config = component.Config;
			var parameters = JsonHelper.FromJson<List<Parameter>>(JsonHelper.ToJson(edit.Node.Parameters));
			edit.ParamDic.Clear();
			foreach (var item in parameters)
				edit.ParamDic.Add(item.Key, item);
			self.SetParamDic(edit.Config.Parameters, edit.ParamDic);
			self.IndexForm.ClearCurrentComponent();
			self.IndexForm.CreateExp(edit.Config);
			self.EditNode = edit;
			self.GenerateCmd();
		}
		public static void CreateNewNode(this DesignViewComponent self, Config config)
		{
			self.IndexForm.ClearCurrentComponent();
			var edit = new EditNode();
			edit.Config = config;
			self.EditNode = edit;
			edit.Node = new Node();
			self.CreateParamDic(config.Parameters, edit.ParamDic);
			self.IndexForm.CreateExp(config);
			self.GenerateCmd();

		}
		public static void AddOrUpdateNode(this DesignViewComponent self)
		{
			if (self.EditNode == null) return;
			if (self.EditNode.Node != null && self.EditNode.Node.Id != default(int))
			{
				self.EditNode.Node.IsNew = true;
				for (int i = 0; i < self.EditNode.Node.Parameters.Count; i++)
				{
					self.EditNode.ParamDic.TryGetValue(self.EditNode.Node.Parameters[i].Key, out var para);
					self.EditNode.Node.Parameters[i] = para;
				}
				self.EditNode.CurrentDesignRowViewForm.Update(self.EditNode.Node.GetRowData());
				self.EditNode.Node.Id = default;
				self.WriteLog($"编辑[{self.EditNode.Node.DisplayExp}]命令");
				return;
			}
			var parameters = JsonHelper.FromJson<List<Parameter>>(JsonHelper.ToJson(self.EditNode.ParamDic.Values.ToList()));
			var designComponent = Boot.GetComponent<RobotComponent>().GetComponent<DesignComponent>();
			self.EditNode.Node = new Node()
			{
				ComponentName = self.EditNode.Config.ComponentName,
				Id = IdGenerater.Instance.GenerateId(),
				Name = self.EditNode.Config.CmdDisplayName,
				IsBlock = self.EditNode.Config.IsBlock,
				Parameters = parameters
			};
			int index = default;
			if (self.CurrentRowView == null)
			{
				index = self.RowViews.Count;
				self.CurrentRowView = self.RowViews[self.RowViews.Count - 1];
			}
			else index = self.RowViews.IndexOf(self.CurrentRowView) + 1;
			var nodes = designComponent.CreateNode(self.EditNode.Node, index);
			self.AddRowsViews(nodes);
			self.DesignViewForm.Update();
			self.DesignViewForm.Refresh();
			
			self.DesignViewForm.Refresh();
			self.WriteLog($"新增[{self.EditNode.Node.DisplayExp}]命令");
			self.EditNode.Node = default;
		}
		public static void AddRowsViews(this DesignViewComponent self, List<Node> nodes)
		{
			int index = -1;
			foreach (var item in nodes)
			{
				var indexNode = self.CurrentFlow.DesignSteps.IndexOf(item);
				var control = self.FindLayout(item);
				DesignRowViewForm designRowViewForm = new();
				designRowViewForm.Tag = item;

				if (control.Controls.Count > 0 && index == -1)
				{
					try
					{
						index = control.Controls.GetChildIndex(self.CurrentRowView);
					}
					catch (Exception)
					{
						index = -1;
					}
				}
				control.Controls.Add(designRowViewForm);
				self.RowViews.Insert(indexNode, designRowViewForm);
				index++;
				control.Controls.SetChildIndex(designRowViewForm, index);
				var row = item.GetRowData(indexNode + "");
				self.UpdateRowView(designRowViewForm, row);
				int tempIndex = 0;
				foreach (var node in self.RowViews)
				{
					tempIndex++;
					node.UpdateIndex(tempIndex + "");
				}
				self.CurrentRowView = designRowViewForm;
				if (item.IsBlock)
				{
					FlowLayoutPanel flowLayoutPanel = new FlowLayoutPanel();
					flowLayoutPanel.AutoSize = true;
					flowLayoutPanel.Margin = new Padding(0, 0, 0, 0);
					int width = 733;
					if (control.Tag != null && control.Tag is NodePanel np)
						width = np.Width;
					flowLayoutPanel.Tag = new NodePanel()
					{
						Node = item,
						Width = width - 35,

					};
					control.Controls.Add(flowLayoutPanel);
					index++;
					control.Controls.SetChildIndex(flowLayoutPanel, index);
				}

			}
		}
		public static FlowLayoutPanel FindLayout(this DesignViewComponent self, Node node)
		{
			var fl = self.CurrentRowView.Parent as FlowLayoutPanel;
			if (node.DesignParent == null)
				return fl;
			if (!self.CurrentNode.IsBlock)
				return FindPanel(fl.Parent as FlowLayoutPanel, node);
			if (self.CurrentNode.LinkNode == node)
				return FindPanel(fl.Parent as FlowLayoutPanel, node);
			return FindPanel(fl, node);
		}
		private static FlowLayoutPanel FindPanel(FlowLayoutPanel pl, Node node)
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
		#region private
		private static void SetParamDic(this DesignViewComponent self, List<Parameter> parameters, Dictionary<string, Parameter> paramDic)
		{
			if (parameters is null) return;
			for (int i = 0; i < parameters.Count; i++)
			{
				paramDic.TryGetValue(parameters[i].Key, out var temp);
				parameters[i] = temp;
			}
			foreach (var item in parameters)
			{
				if (item != null)
					self.SetParamDic(item.Parameters, paramDic);
			}
			return;
		}
		public static void CreateParamDic(this DesignViewComponent self, List<Parameter> parameters, Dictionary<string, Parameter> dic)
		{
			if (parameters is null) return;
			foreach (var item in parameters)
				dic.Add(item.Key, item);
			foreach (var item in parameters)
				self.CreateParamDic(item.Parameters, dic);
			return;
		}
		public static void GenerateCmd(this DesignViewComponent self)
		{
			self.IndexForm.AppendTextColorful(
				$"【{self.EditNode.Config.CmdDisplayName}的指令】", Color.MediumBlue, new Font("黑体", 10), false);
			var color = Color.BlueViolet;
			self.IndexForm.AppendTextColorful(" ->", color, new Font("黑体", 10), false);
			if (self.EditNode.ParamDic.Values.Any())
				foreach (var item in self.EditNode.ParamDic.Values)
					self.IndexForm.AppendTextColorful(item.DisplayExp, color, new Font("黑体", 10), false);
		}
		private static void SetBackColor(this DesignViewComponent self)
		{

		}

		public static RowData GetRowData(this Node self, string index = "")
		{
			return new RowData()
			{
				Id = self.Id,
				DisplayExp = self.DisplayExp,
				Index = index,
				Name = self.Name,
				IsBlock = self.IsBlock,
			};
		}
		#endregion

	}
}
