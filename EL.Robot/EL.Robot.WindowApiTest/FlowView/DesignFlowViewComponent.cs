using EL.Robot.Component;
using EL.Robot.Core;
using EL.Robot.WindowApiTest.Code;
using Control = System.Windows.Forms.Control;

namespace EL.Robot.WindowApiTest
{
	public class DesignFlowViewComponent : Entity
	{
		public DesignFlowViewForm DesignFlowViewForm { get; set; }
		public IndexForm IndexForm { get; set; }
		public Action<DesignFlowRowViewForm, bool> HideExpansionNodeAction { get; set; }
		public Flow CurrentFlow { get; set; }
		public ComponentInfo ComponentInfo { get; set; }
		public ContextMenuStrip ContextMenuStrip { get; set; }
		public Node CurrentNode
		{
			get
			{
				return CurrentRowView.Tag as Node;
			}
		}
		public DesignFlowRowViewForm _currentRowView;
		public DesignFlowRowViewForm CurrentRowView
		{
			get
			{
				return _currentRowView;
			}
		}
		public List<DesignFlowRowViewForm> SelectRowViews { get; set; } = new List<DesignFlowRowViewForm>();
		public List<DesignFlowRowViewForm> CopyRows { get; set; } = new List<DesignFlowRowViewForm>();
	}


	public class DesignFlowViewComponentAwake : AwakeSystem<DesignFlowViewComponent>
	{
		public override void Awake(DesignFlowViewComponent self)
		{
			self.IndexForm = new IndexForm();
			self.DesignFlowViewForm = new DesignFlowViewForm
			{
				Visible = true,
				Dock = DockStyle.Fill
			};
			self.ContextMenuStrip = new ContextMenuStrip();
			ToolStripMenuItem deleteItem = new ToolStripMenuItem
			{
				Text = "删除",
			};
			ToolStripMenuItem editItem = new ToolStripMenuItem
			{
				Text = "编辑",
			};
			editItem.Click += (x, y) =>
			{
				try
				{
					self.EditNode(self.CurrentRowView);
				}
				catch (Exception)
				{
					var view = Boot.GetComponent<ViewLogComponent>();
					view.WriteDesignLog("编辑节点出错，请及时保存数据并重新打开！", true);
				}

			};
			deleteItem.Click += (x, y) =>
			{
				try
				{
					self.RemoveRows();
				}
				catch (Exception ex)
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
			self.ContextMenuStrip.Items.Add(editItem);
			self.ContextMenuStrip.Items.Add(copy);
			self.ContextMenuStrip.Items.Add(paste);
			self.ContextMenuStrip.Items.Add(deleteItem);
		}
	}

	public static class DesignFlowViewComponentSystem
	{
		public static void Main(this DesignFlowViewComponent self, Flow flow)
		{
			self.CurrentFlow = flow;
			self._currentRowView = null;
			self.DesignFlowViewForm.LoadFlow(flow);
			self.Refresh(self.DesignFlowViewForm.pl_Content);
		}
		public static void SetCurrentRow(this DesignFlowViewComponent self, DesignFlowRowViewForm row)
		{
			if (self.CurrentRowView == row) return;
			var viewLogComponent = Boot.GetComponent<ViewLogComponent>();
			var node = row.Tag as Node;
			//viewLogComponent.WriteDesignLog("当前选中操作节点" + $"[{node.DisplayExp}]");
			self.InitColor(self.DesignFlowViewForm.pl_Content);
			if ((Control.ModifierKeys & Keys.Control) == Keys.Control || (Control.ModifierKeys & Keys.Shift) == Keys.Shift)
			{
				if (!self.SelectRowViews.Contains(row))
					self.SelectRowViews.Add(row);
				self.SelectRowViews.ForEach(x => x.SetBackColor(Color.LightSkyBlue));
				return;
			}
			self._currentRowView = row;
			self.SelectRowViews.Clear();
			self.SelectRowViews.Add(self._currentRowView);
			row.SetBackColor(Color.LightSkyBlue);
		}
		public static void InitColor(this DesignFlowViewComponent self, FlowLayoutPanel fl)
		{
			foreach (var item in fl.Controls)
			{
				if (item is DesignFlowRowViewForm dfr)
				{
					dfr.SetBackColor(Color.LightSteelBlue);
					continue;
				}
				self.InitColor(item as FlowLayoutPanel);
			}
		}
		public static void PasteRows(this DesignFlowViewComponent self)
		{
			var designComponent = Boot.GetComponent<RobotComponent>().GetComponent<DesignComponent>();
			if (self.CopyRows.Count == 0) return;
			var rows = self.GetFlowRows();
			var panel = self.FindPanle(self.CurrentRowView.Parent as FlowLayoutPanel, self.CurrentRowView.Tag as Node);
			var list = new List<Node>();
			var longs = new List<long>();
			foreach (var item in self.CopyRows)
			{
				var node = (item.Tag as Node);
				if (longs.Contains((item.Tag as Node).Id)) continue;
				if (node.IsEnd) continue;
				var temp = JsonHelper.FromJson<Node>(JsonHelper.ToJson(node));
				temp.Id = IdGenerater.Instance.GenerateId();
				list.Add(temp);
				if (node.IsBlock)
				{
					var pl = self.FindPanle(item.Parent as FlowLayoutPanel, item.Tag as Node);
					var nodes = self.PasteStep(pl);
					if (nodes.nodes1.Any())
					{
						temp.Steps = nodes.nodes1;
						longs.AddRange(nodes.longs1);
					}
					var endNode = node.LinkNode;
					var tempEnd = JsonHelper.FromJson<Node>(JsonHelper.ToJson(endNode));
					tempEnd.Id = IdGenerater.Instance.GenerateId();
					temp.LinkNode = tempEnd;
					list.Add(tempEnd);
				}

			}
			self.PasteRow(panel, list);
			self.Refresh(self.DesignFlowViewForm.pl_Content);
		}
		public static void PasteRow(this DesignFlowViewComponent self, FlowLayoutPanel pl, List<Node> nodes)
		{
			var flowView = Boot.GetComponent<DesignFlowViewComponent>();
			DesignFlowRowViewForm designFlowRowViewForm;
			if (!nodes.Any()) return;
			foreach (Node node in nodes)
			{
				var fl = flowView.CreateRows(pl, node, false);
				if (node.IsBlock)
					self.PasteRow(fl, node.Steps);
			}
		}
		public static (List<Node> nodes1, List<long> longs1) PasteStep(this DesignFlowViewComponent self, FlowLayoutPanel fl)
		{
			var longs = new List<long>();
			List<Node> list = new List<Node>();
			foreach (var item in fl.Controls)
			{
				if (item is DesignFlowRowViewForm rv)
				{
					var node = rv.Tag as Node;
					var temp = JsonHelper.FromJson<Node>(JsonHelper.ToJson(node));
					temp.Id = IdGenerater.Instance.GenerateId();
					list.Add(temp);
					longs.Add(node.Id);
					continue;
				}
				var flTemp = item as FlowLayoutPanel;
				var temps = self.PasteStep(flTemp);
				var np = flTemp.Tag as NodePanel;
				np.Node.Steps.Clear();
				np.Node.Steps.AddRange(temps.nodes1);
				longs.AddRange(temps.longs1);
			}
			return (list, longs);
		}

		public static void RemoveRows(this DesignFlowViewComponent self)
		{
			var viewLogComponent = Boot.GetComponent<ViewLogComponent>();
			if (!self.SelectRowViews.Any())
				viewLogComponent.WriteDesignLog("当前没有选中节点", true);
			var rows = self.GetFlowRows();
			var index = self.SelectRowViews.Min(x => x.Index);
			foreach (var item in self.SelectRowViews)
			{
				var node = item.Tag as Node;
				if (node.ComponentName == nameof(StartComponent))
					continue;
				if (item.Parent == null) continue;
				if (node.IsBlock)
				{
					var panle = self.FindPanle(item.Parent as FlowLayoutPanel, item.Tag as Node);
					if (item.Parent.Controls.Contains(panle))
						item.Parent.Controls.Remove(panle);
					var end = rows.FirstOrDefault(x => (x.Tag as Node).Id == node.LinkNode.Id);
					if (end != null && item.Parent.Controls.Contains(end))
						item.Parent.Controls.Remove(end);
				}
				if (item.Parent.Controls.Contains(item))
					item.Parent.Controls.Remove(item);
				viewLogComponent.WriteDesignLog($"删除{(item.Tag as Node).DisplayExp}命令");
			}
			self.SelectRowViews.Clear();
			self.Refresh(self.DesignFlowViewForm.pl_Content);
			rows = self.GetFlowRows();
			index = Math.Min(index, rows.Count - 1);
			var row = rows[index];
			if (row == null)
				row = rows.LastOrDefault();
			self.SetCurrentRow(row);
		}
		public static void EditNode(this DesignFlowViewComponent self, DesignFlowRowViewForm rowViewForm)
		{
			var viewLogComponent = Boot.GetComponent<ViewLogComponent>();
			self.SetCurrentRow(rowViewForm);
			self.ComponentInfo = new ComponentInfo
			{
				Node = self.CurrentNode
			};
			self.ComponentInfo.RowView = rowViewForm;
			if (self.ComponentInfo.Node == null || self.ComponentInfo.Node.ComponentName == (nameof(StartComponent)))
			{
				viewLogComponent.WriteDesignLog("开始节点不能编辑");
				return;
			}
			var designComponent = Boot.GetComponent<RobotComponent>().GetComponent<DesignComponent>();
			var component = designComponent.GetComponentInfo(self.ComponentInfo.Node.ComponentName);
			component.GetConfig();
			self.ComponentInfo.Config = component.Config;
			var parameters = JsonHelper.FromJson<List<Parameter>>(JsonHelper.ToJson(self.ComponentInfo.Node.Parameters));
			self.ComponentInfo.ParamDic.Clear();
			foreach (var item in parameters)
				self.ComponentInfo.ParamDic.Add(item.Key, item);
			self.SetParamDic(self.ComponentInfo.Config.Parameters, self.ComponentInfo.ParamDic);
			self.IndexForm.ClearCurrentComponent();
			self.IndexForm.CreateExp(self.ComponentInfo.Config);
			self.GenerateCmd();
			viewLogComponent.WriteDesignLog($"编辑{self.ComponentInfo.Node.DisplayExp}命令");
		}
		public static List<DesignFlowRowViewForm> GetFlowRows(this DesignFlowViewComponent self, FlowLayoutPanel fl = default)
		{
			List<DesignFlowRowViewForm> list = new List<DesignFlowRowViewForm>();
			if (fl == null) fl = self.DesignFlowViewForm.pl_Content;
			foreach (var item in fl.Controls)
			{
				if (item is DesignFlowRowViewForm dfr)
				{
					list.Add(dfr);
					continue;
				}
				var temp = self.GetFlowRows(item as FlowLayoutPanel);
				if (temp.Any())
					list.AddRange(temp);

			}
			return list;
		}
		public static void Refresh(this DesignFlowViewComponent self, FlowLayoutPanel fl)
		{
			var designComponent = Boot.GetComponent<RobotComponent>().GetComponent<DesignComponent>();
			//var viewLog = Boot.GetComponent<ViewLogComponent>();
			int index = 1;
			List<string> strings = new List<string>();
			self.GetFlowRows().ForEach(x =>
			{
				x.UpdateIndex(index);
				strings.Add(index + (x.Tag as Node).DisplayExp);
				index++;
			});
			designComponent.RefreshNodeCmdAction?.Invoke(strings.ToArray());
		}
		#region add
		public static void AddOrUpdateRows(this DesignFlowViewComponent self)
		{
			var designComponent = Boot.GetComponent<RobotComponent>().GetComponent<DesignComponent>();
			if (self.ComponentInfo == null) return;
			if (self.ComponentInfo.Node != null && self.ComponentInfo.Node.Id != default(int))
			{
				self.ComponentInfo.Node.IsNew = true;
				for (int i = 0; i < self.ComponentInfo.Node.Parameters.Count; i++)
				{
					self.ComponentInfo.ParamDic.TryGetValue(self.ComponentInfo.Node.Parameters[i].Key, out var para);
					self.ComponentInfo.Node.Parameters[i] = para;
				}
				self.ComponentInfo.RowView.Update(self.ComponentInfo.Node);
				self.ComponentInfo.Node.Id = default;
				designComponent.WriteDesignLog($"编辑[{self.ComponentInfo.Node.DisplayExp}]命令成功");
				return;
			}
			var parameters = JsonHelper.FromJson<List<Parameter>>(JsonHelper.ToJson(self.ComponentInfo.ParamDic.Values.ToList()));
			var node = new Node()
			{
				ComponentName = self.ComponentInfo.Config.ComponentName,
				Id = IdGenerater.Instance.GenerateId(),
				Name = self.ComponentInfo.Config.CmdDisplayName,
				IsBlock = self.ComponentInfo.Config.IsBlock,
				Parameters = parameters
			};
			var end = CreateEndBloack(node);
			var fl = self.FindPanle(self.CurrentRowView.Parent as FlowLayoutPanel, self.CurrentNode);
			self.CreateRows(fl, node, false);
			self.CreateRows(fl, end, false);
			self.Refresh(self.DesignFlowViewForm.pl_Content);
			designComponent.WriteDesignLog($"添加[{node.DisplayExp}]命令成功");
		}
		public static Node CreateEndBloack(Node node)
		{
			if (!node.IsBlock) return null;
			var dc = Boot.GetComponent<RobotComponent>().GetComponent<DesignComponent>();
			var component = dc.GetComponentInfo(nameof(BlockEndComponent));
			component.GetConfig();
			var temp = new Node()
			{
				Id = IdGenerater.Instance.GenerateId(),
				Name = component.Config.CmdDisplayName,
				ComponentName = component.Config.ComponentName,
			};
			node.LinkNode = temp;
			return temp;
		}
		public static void LoadRows(this DesignFlowViewComponent self, FlowLayoutPanel pl, List<Node> nodes, int no = 1)
		{
			var flowView = Boot.GetComponent<DesignFlowViewComponent>();
			DesignFlowRowViewForm designFlowRowViewForm;
			if (!nodes.Any()) return;
			foreach (Node node in nodes)
			{
				var fl = flowView.CreateRows(pl, node, true, no);
				if (node.IsBlock)
					self.LoadRows(fl, node.Steps, no);
				no++;
			}
		}
		public static FlowLayoutPanel CreateRows(this DesignFlowViewComponent self, FlowLayoutPanel pl, Node node, bool isLoad = true, int no = default)
		{
			if (node == null) return null;
			var designFlowRowViewForm = new DesignFlowRowViewForm
			{
				Dock = DockStyle.Top,
				Tag = node,
			};
			var temp = self.CurrentRowView;
			pl.Controls.Add(designFlowRowViewForm);
			FlowLayoutPanel flowLayoutPanel = null;
			if (isLoad)
			{
				self._currentRowView = designFlowRowViewForm;
				if (!node.IsBlock)
				{
					designFlowRowViewForm.Update(node, no);
					return null;
				}
				var np = pl.Tag as NodePanel;
				var width = np.Width - 30;
				flowLayoutPanel = new()
				{
					Tag = new NodePanel
					{
						Width = width,
						Node = node,
					},
					AutoSize = true,
					Margin = new Padding(0, 0, 0, 0),
					Dock = DockStyle.Top,
				};
				pl.Controls.Add(flowLayoutPanel);
				designFlowRowViewForm.Update(node, no);
				return flowLayoutPanel;
			}
			self.SetCurrentRow(designFlowRowViewForm);
			int index = default;
			if (node.ComponentName == nameof(BlockEndComponent))
			{
				self.SetCurrentRow(temp);
				var rows = self.GetFlowRows();
				var start = rows.FirstOrDefault(x => (x.Tag as Node).LinkNode != null && (x.Tag as Node).LinkNode == node);
				index = pl.Controls.GetChildIndex(start) + 2;
				pl.Controls.SetChildIndex(designFlowRowViewForm, index);
				designFlowRowViewForm.Update(node);
				return flowLayoutPanel;
			}
			if (temp.Tag is Node node1 && node1.IsBlock)
				pl.Controls.SetChildIndex(designFlowRowViewForm, index);
			else
			{
				index = pl.Controls.GetChildIndex(temp) + 1;
				pl.Controls.SetChildIndex(designFlowRowViewForm, index);
			}
			if (node.IsBlock)
			{
				var np = pl.Tag as NodePanel;
				var width = np.Width - 30;
				flowLayoutPanel = new()
				{
					Tag = new NodePanel
					{
						Width = width,
						Node = node,
					},
					AutoSize = true,
					Margin = new Padding(0, 0, 0, 0),
					Dock = DockStyle.Top,
				};
				pl.Controls.Add(flowLayoutPanel);
				index++;
				pl.Controls.SetChildIndex(flowLayoutPanel, index);
			}
			designFlowRowViewForm.Update(node);
			return flowLayoutPanel;
		}
		public static void CreateNewNode(this DesignFlowViewComponent self, Config config)
		{
			self.IndexForm.ClearCurrentComponent();
			self.ComponentInfo = new ComponentInfo();
			self.ComponentInfo.Config = config;
			self.CreateParamDic(config.Parameters, self.ComponentInfo.ParamDic);
			self.IndexForm.CreateExp(config);
			self.GenerateCmd();

		}
		public static FlowLayoutPanel FindPanle(this DesignFlowViewComponent self, FlowLayoutPanel pl, Node node)
		{
			if (node == null || !node.IsBlock) return pl;
			foreach (var item in pl.Controls)
			{
				if (item is FlowLayoutPanel fl)
				{
					if (fl.Tag is NodePanel np && np.Node != null && np.Node.Id == node.Id)
						return fl;
					var temp = self.FindPanle(fl, node);
					if (temp != null) return temp;
				}
			}
			return null;
		}
		public static async Task SaveFlowAsync(this DesignFlowViewComponent self)
		{
			var nodes = self.GenerateFlow(self.DesignFlowViewForm.pl_Content);
			self.CurrentFlow.Steps.Clear();
			self.CurrentFlow.Steps = nodes;
			var designComponent = Boot.GetComponent<RobotComponent>().GetComponent<DesignComponent>();
			await designComponent.SaveRobot();
		}
		public static List<Node> GenerateFlow(this DesignFlowViewComponent self, FlowLayoutPanel fl)
		{
			List<Node> list = new List<Node>();
			foreach (var item in fl.Controls)
			{
				if (item is DesignFlowRowViewForm rv)
				{
					list.Add(rv.Tag as Node);
					continue;
				}
				var flTemp = item as FlowLayoutPanel;
				var temps = self.GenerateFlow(flTemp);
				var np = flTemp.Tag as NodePanel;
				if (temps.Any())
				{
					np.Node.Steps.Clear();
					np.Node.Steps.AddRange(temps);
				}
			}
			return list;
		}
		public static async Task<List<ExecLog>> RunRobot(this DesignFlowViewComponent self)
		{
			var designComponent = Boot.GetComponent<RobotComponent>().GetComponent<DesignComponent>();
			await self.SaveFlowAsync();
			var logs = await designComponent.RunRobot();
			var viewLog = Boot.GetComponent<ViewLogComponent>();
			viewLog.WriteRunLog(logs.Select(x => x.ShowMsg).ToArray());
			return logs;
		}
		public static async Task PreExecNodes(this DesignFlowViewComponent self)
		{
			var designComponent = Boot.GetComponent<RobotComponent>().GetComponent<DesignComponent>();
			if (self.ComponentInfo == null) return;
			Node node1 = default;
			if (self.ComponentInfo.Node != null && self.ComponentInfo.Node.Id != default(int))
			{
				self.ComponentInfo.Node.IsNew = true;
				node1 = JsonHelper.FromJson<Node>(JsonHelper.ToJson(self.ComponentInfo.Node));
				node1.Id = IdGenerater.Instance.GenerateId();
				for (int i = 0; i < self.ComponentInfo.Node.Parameters.Count; i++)
				{
					self.ComponentInfo.ParamDic.TryGetValue(self.ComponentInfo.Node.Parameters[i].Key, out var para);
					node1.Parameters[i] = para;
				}
				node1.Id = default;
			}
			var parameters = JsonHelper.FromJson<List<Parameter>>(JsonHelper.ToJson(self.ComponentInfo.ParamDic.Values.ToList()));
			node1 = new Node()
			{
				ComponentName = self.ComponentInfo.Config.ComponentName,
				Id = IdGenerater.Instance.GenerateId(),
				Name = self.ComponentInfo.Config.CmdDisplayName,
				IsBlock = self.ComponentInfo.Config.IsBlock,
				Parameters = parameters
			};
			var list = await designComponent.PreviewNodes(new List<Node>() { node1 });
			var viewLog = Boot.GetComponent<ViewLogComponent>();
			viewLog.WriteRunLog(list.Select(x => x.ShowMsg).ToArray());

		}
		#endregion

		#region private
		private static void SetParamDic(this DesignFlowViewComponent self, List<Parameter> parameters, Dictionary<string, Parameter> paramDic)
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
		public static void CreateParamDic(this DesignFlowViewComponent self, List<Parameter> parameters, Dictionary<string, Parameter> dic)
		{
			if (parameters is null) return;
			foreach (var item in parameters)
				dic.Add(item.Key, item);
			foreach (var item in parameters)
				self.CreateParamDic(item.Parameters, dic);
			return;
		}
		public static void GenerateCmd(this DesignFlowViewComponent self)
		{
			self.IndexForm.AppendTextColorful(
				$"【{self.ComponentInfo.Config.CmdDisplayName}的指令】", Color.MediumBlue, new Font("黑体", 10), false);
			var color = Color.BlueViolet;
			self.IndexForm.AppendTextColorful(" ->", color, new Font("黑体", 10), false);
			if (self.ComponentInfo.ParamDic.Values.Any())
				foreach (var item in self.ComponentInfo.ParamDic.Values)
					self.IndexForm.AppendTextColorful(item.DisplayExp, color, new Font("黑体", 10), false);
		}
		private static void SetBackColor(this DesignFlowViewComponent self)
		{

		}
		#endregion
	}
	public class ComponentInfo
	{
		public Config Config { get; set; }
		public Dictionary<string, Parameter> ParamDic = new Dictionary<string, Parameter>();
		public Node Node { get; set; }
		public DesignFlowRowViewForm RowView { get; set; }
	}
}
