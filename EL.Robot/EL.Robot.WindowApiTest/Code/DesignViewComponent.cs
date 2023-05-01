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
		public List<DesignRowViewForm> CopyRows { get; set; } = new List<DesignRowViewForm>();
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
				Text = "删除",
			};
			editItem.Click += (x, y) =>
			{
				try
				{
					self.RemoveRows();
				}
				catch (Exception)
				{
					var view = Boot.GetComponent<ViewLogComponent>();
					view.WriteDesignLog("删除节点出错，请及时保存数据并重新打开！", true);
				}

			};
			ToolStripMenuItem copy = new ToolStripMenuItem
			{
				Text = "复制",
			};
			copy.Click += (x, y) =>
			{
				var designComponent = Boot.GetComponent<RobotComponent>().GetComponent<DesignComponent>();
				if (self.SelectRowViews.Count == 0) return;
				self.CopyRows.Clear();
				self.CopyRows.AddRange(self.SelectRowViews);
			};
			ToolStripMenuItem paste = new ToolStripMenuItem
			{
				Text = "粘贴",
			};
			paste.Click += (x, y) =>
			{
				try
				{
					self.PasteRows();
				}
				catch (Exception)
				{
					var view = Boot.GetComponent<ViewLogComponent>();
					view.WriteDesignLog("拷贝节点出错，请及时保存数据并重新打开！", true);
				}

			};
			self.ContextMenuStrip.Items.Add(copy);
			self.ContextMenuStrip.Items.Add(paste);
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
		public static void PasteRows(this DesignViewComponent self)
		{
			var designComponent = Boot.GetComponent<RobotComponent>().GetComponent<DesignComponent>();
			if (self.CopyRows.Count == 0) return;
			var list = new List<Node>();
			var longs = new List<long>();
			foreach (var item in self.CopyRows)
			{
				var node = (item.Tag as Node);
				if (longs.Contains((item.Tag as Node).Id)) continue;
				if (node.IsEnd) continue;
				var temps = self.FindRows(item, self.CurrentNode.DesignParent);
				if (temps.nodes.Any())
				{
					list.AddRange(temps.nodes);
					longs.AddRange(temps.longs);
				}
			}
			int index = self.RowViews.Count;
			if (self.CurrentRowView == null)
				self.CurrentRowView = self.RowViews[index - 1];
			else
				index = self.RowViews.IndexOf(self.CurrentRowView) + 1;
			if (self.CurrentNode.IsBlock)
			{
				index = self.RowViews.IndexOf(self.CurrentRowView) - 1;
				self.CurrentRowView = self.RowViews[index];

			}


			self.CurrentFlow.DesignSteps.InsertRange(index, list);
			self.AddRowsViews(list);
			designComponent.RefreshAllStepCMD();
		}
		public static (List<Node> nodes, List<long> longs) FindRows(this DesignViewComponent self, DesignRowViewForm rowViewForm, Node parentNode = null)
		{
			List<long> longs = new List<long>();
			var list = new List<Node>();
			var designComponent = Boot.GetComponent<RobotComponent>().GetComponent<DesignComponent>();
			var viewLogComponent = Boot.GetComponent<ViewLogComponent>();
			var node = rowViewForm.Tag as Node;
			var newNode = JsonHelper.FromJson<Node>(JsonHelper.ToJson(node));
			newNode.Id = IdGenerater.Instance.GenerateId();
			newNode.DesignParent = parentNode;
			list.Add(newNode);
			longs.Add(node.Id);
			if (node.LinkNode == null)
				return (list, longs);
			var rowViews = self.RowViews.Where(x => (x.Tag as Node).DesignParent != null && (x.Tag as Node).DesignParent.Id == node.Id).ToList();
			if (rowViews.Any())
				foreach (var item in rowViews)
				{
					var node1 = (item.Tag as Node);
					if (node1.IsEnd) continue;
					var temps = self.FindRows(item, newNode);
					if (temps.nodes.Any())
					{
						list.AddRange(temps.nodes);
						longs.AddRange(temps.longs);
					}
				}
			var end = self.RowViews.FirstOrDefault(x => (x.Tag as Node).Id == node.LinkNode.Id);
			var endData = (end.Tag as Node);
			longs.Add(endData.Id);
			var newEnd = JsonHelper.FromJson<Node>(JsonHelper.ToJson(endData));
			newEnd.Id = IdGenerater.Instance.GenerateId();
			newEnd.DesignParent = parentNode;
			newNode.LinkNode = newEnd;
			newNode.DesignParent = parentNode;
			list.Add(newEnd);
			return (list, longs);
		}
		public static void RemoveRows(this DesignViewComponent self)
		{
			var viewLogComponent = Boot.GetComponent<ViewLogComponent>();
			if (!self.SelectRowViews.Any())
				viewLogComponent.WriteDesignLog("当前没有选中节点", true);
			int index = default;
			foreach (var item in self.SelectRowViews)
			{
				var node = item.Tag as Node;
				if (node.ComponentName == nameof(StartComponent))
					continue;
				index = self.RowViews.IndexOf(item);
				self.RemoveRow(item);
				if (node.LinkNode != null)
				{
					var end = self.RowViews.FirstOrDefault(x => (x.Tag as Node).Id == node.LinkNode.Id);
					if (end != null)
					{
						index = self.RowViews.IndexOf(item);
						self.RemoveRow(end);
					}
				}
				if (node.ComponentName == nameof(BlockEndComponent))
				{
					var start = self.RowViews.FirstOrDefault(x => (x.Tag as Node).LinkNode != null && (x.Tag as Node).LinkNode.Id == node.Id);
					if (start != null)
						self.RemoveRow(start);
				}
			}
			int tempIndex = 0;
			foreach (var nodeTemp in self.RowViews)
			{
				tempIndex++;
				nodeTemp.UpdateIndex(tempIndex + "");
			}
			if (index >= self.RowViews.Count || index <= 0)
				index = self.RowViews.Count - 1;
			self.SetCurrentNode(self.RowViews[index]);
			self.SelectRowViews.Clear();
			self.DesignViewForm.Refresh();
		}
		private static void RemoveRow(this DesignViewComponent self, DesignRowViewForm rowViewForm)
		{
			var viewLogComponent = Boot.GetComponent<ViewLogComponent>();
			var node = rowViewForm.Tag as Node;
			self.CurrentFlow.DesignSteps.Remove(node);
			var fl = rowViewForm.Parent as FlowLayoutPanel;
			if (fl != null)
				fl.Controls.Remove(rowViewForm);
			self.RowViews.Remove(rowViewForm);
			viewLogComponent.WriteDesignLog($"删除[{node.DisplayExp}]命令");
			var rowViews = self.RowViews.Where(x => (x.Tag as Node).DesignParent != null && (x.Tag as Node).DesignParent.Id == node.Id).ToList();
			foreach (var item in rowViews)
				self.RemoveRow(item);
		}
		public static void SetCurrentNode(this DesignViewComponent self, DesignRowViewForm row)
		{
			var viewLogComponent = Boot.GetComponent<ViewLogComponent>();
			var node = row.Tag as Node;
			viewLogComponent.WriteDesignLog("当前选中操作节点" + $"[{node.DisplayExp}]");
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

		public static void UpdateRowView(this DesignViewComponent self, DesignRowViewForm rowViewForm, RowData rowInfo)
		{
			rowViewForm.Update(rowInfo);
		}
		public static void EditNode(this DesignViewComponent self, DesignRowViewForm rowViewForm)
		{
			var viewLogComponent = Boot.GetComponent<ViewLogComponent>();
			self.CurrentRowView = rowViewForm;
			if (self.SelectRowViews.Count != 1 && self.SelectRowViews[0].Tag == null)
			{
				viewLogComponent.WriteDesignLog("多个节点不能编辑");
				return;
			}
			var edit = new EditNode
			{
				Node = self.SelectRowViews[0].Tag as Node
			};

			if (edit.Node == null || edit.Node.ComponentName == (nameof(StartComponent)))
			{
				viewLogComponent.WriteDesignLog("开始节点不能编辑");
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
			var designComponent = Boot.GetComponent<RobotComponent>().GetComponent<DesignComponent>();
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
				designComponent.WriteDesignLog($"编辑[{self.EditNode.Node.DisplayExp}]命令");
				return;
			}
			var parameters = JsonHelper.FromJson<List<Parameter>>(JsonHelper.ToJson(self.EditNode.ParamDic.Values.ToList()));

			self.EditNode.Node = new Node()
			{
				ComponentName = self.EditNode.Config.ComponentName,
				Id = IdGenerater.Instance.GenerateId(),
				Name = self.EditNode.Config.CmdDisplayName,
				IsBlock = self.EditNode.Config.IsBlock,
				Parameters = parameters
			};
			int index = self.RowViews.Count;
			if (self.CurrentRowView == null)
				self.CurrentRowView = self.RowViews[index - 1];
			else
				index = self.RowViews.IndexOf(self.CurrentRowView) + 1;
			var nodes = designComponent.CreateNode(self.EditNode.Node, index);
			self.AddRowsViews(nodes);
			designComponent.RefreshAllStepCMD();
			var viewLog = Boot.GetComponent<ViewLogComponent>();
			designComponent.WriteDesignLog($"编辑[{self.EditNode.Node.DisplayExp}]命令");
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
				try
				{
					index = control.Controls.GetChildIndex(self.CurrentRowView) + 1;
					var start = (self.CurrentRowView.Tag as Node);
					if (start.IsBlock && start.LinkNode == item)
						index = index + 1;
				}
				catch (Exception)
				{
					index = 0;
				}
				control.Controls.Add(designRowViewForm);
				self.RowViews.Insert(indexNode, designRowViewForm);
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

			var block = self.CurrentFlow.DesignSteps.FirstOrDefault(x => x.LinkNode != null && x.LinkNode == node);
			if (block != null)
			{
				self.CurrentRowView = self.RowViews.FirstOrDefault(x => (x.Tag as Node).Id == block.Id);
				return self.RowViews.FirstOrDefault(x => (x.Tag as Node).Id == block.Id).Parent as FlowLayoutPanel;
			}
			var fl = self.CurrentRowView.Parent as FlowLayoutPanel;
			if (node.DesignParent == null)
				return fl;
			if (!self.CurrentNode.IsBlock || self.CurrentNode.LinkNode == node)
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
