using EL.Robot.Component;
using EL.Robot.Core;
using EL.Robot.WindowApiTest.Code;
using System.Runtime.InteropServices;
using System.Text;
using static EL.Robot.Core.ComponentSystem;

namespace EL.Robot.WindowApiTest
{
	public partial class IndexForm : Form
	{
		[DllImport("user32.dll", EntryPoint = "SendMessageA")]
		private static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, StringBuilder lParam);
		[DllImport("user32 ")]
		private static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);
		public Config CurrentConfig;
		public Dictionary<string, Parameter> ParamDic = new Dictionary<string, Parameter>();
		public DesignComponent designComponent = Boot.GetComponent<RobotComponent>().GetComponent<DesignComponent>();
		public ViewLogComponent viewLogComponent = Boot.GetComponent<ViewLogComponent>();
		public DesignViewComponent designViewComponent = Boot.GetComponent<DesignViewComponent>();
		public static IndexForm Ins;
		public IndexForm()
		{
			Ins = this;
			InitializeComponent();
			lbl_name.Text = "";
			DispatcherHelper.BaseForm = this;
			this.StartPosition = FormStartPosition.CenterParent;
			txt_exp.Font = new Font("黑体", 10);
			txt_exp.ForeColor = Color.Black;
			var list = designComponent.LoadRobots();
			RefreshRobots(default, false);
			InitEvent();
		}
		Point mpoint = default;
		public void InitEvent()
		{
			pl_winTop.MouseDown += (e, y) =>
			{
				if (y.Button == MouseButtons.Left)
					mpoint = new Point(y.X, y.Y);
			};
			pl_winTop.MouseMove += (x, y) =>
			{
				if (y.Button == MouseButtons.Left)
					this.Location = new Point(this.Location.X + y.X - mpoint.X, this.Location.Y + y.Y - mpoint.Y);
			};
			pl_components.Visible = false;
			pl_components.Leave += (x, y) =>
			{
				pl_components.Visible = false;
			};
			btn_close.Click += async (x, y) =>
			{
				await designComponent.SaveRobot(false);
				this.Close();
				Application.Exit();
			};
			btn_add.Click += (x, y) =>
			{
				var result = new AddRobotForm().ShowDialog();
				if (result == DialogResult.OK)
				{
					var name = designComponent.CurrentDesignFlow.Name;
					lbl_name.Text = name;
					RefreshRobots(designComponent.CurrentDesignFlow.Id);
					viewLogComponent.WriteLog($"你创建了一个[{name}]流程");
				}
			};
			btn_send.Click += (x, y) =>
			{
				var txt = txt_exp.Text;
				if (txt.Length == 0)
				{
					viewLogComponent.WriteLog("你没有生成指令，我不能为你追加到我的指令集！");
					return;
				}
				var component = Boot.GetComponent<DesignViewComponent>();
				component.AddOrUpdateNode();
				return;
			};
			btn_run.Click += async (x, y) =>
			{
				var logs = await designComponent.RunRobot();
				viewLogComponent.WriteLog("恭喜哟，你成功运行了执行！");
			};
			btn_save.Click += async (x, y) => { await designComponent.SaveRobot(); };

			this.Load += async (x, y) =>
			{
				CommponetRequest commponetRequest = new CommponetRequest()
				{
					ComponentName = nameof(ComponentSystem),
					Data = new ComponentSystemReuqest
					{

						Action = nameof(ComponentSystem.GetCategorys)
					}
				};
				var categorys = await RequestManager.StartAsync(commponetRequest);
				int left = 5;
				foreach (var category in categorys.Data)
				{
					var btn = new Button()
					{
						Text = Helper.GetValue<string>(category, "CategoryName"),
						Width = 80,
						Height = 22,
						Top = 5,
						//Font = new Font("隶书", 8),
						TextAlign = ContentAlignment.MiddleCenter,
						Tag = Helper.GetValue<int>(category, "CategoryId"),
						Left = left,
					};
					btn.Click += async (x, y) =>
					{
						pl_components.Controls.Clear();
						var tag = (x as Button).Tag;
						CommponetRequest commponetRequest = new CommponetRequest()
						{
							ComponentName = nameof(ComponentSystem),
							Data = new ComponentSystemReuqest
							{
								Action = nameof(ComponentSystem.GetComponentsById),
								Paramters = new Dictionary<string, object>()
								{
									{"Id", tag }
								}
							}
						};
						var componentConfigs = await RequestManager.StartAsync(commponetRequest);
						if (componentConfigs.Data is null) return;
						pl_components.Visible = true;
						pl_components.Focus();
						int left = 2;
						int index = 0;
						int top = 5;
						foreach (var item in componentConfigs.Data)
						{
							if (index % 7 == 0)
							{
								left = 2;
								top = top + 25 * (int)Math.Floor(index / 7f);
							}

							var config = item as Config;
							var btn = new Button()
							{
								Text = config.ButtonDisplayName,
								Width = 80,
								Height = 22,
								//Font = new Font("隶书", 8),
								Top = top,
								TextAlign = ContentAlignment.MiddleCenter,
								Tag = config,
								Left = left,
							};
							pl_components.Controls.Add(btn);
							left += 80;
							btn.Click += async (x, y) =>
							{
								var tag = (x as Button).Tag as Config;
								Boot.GetComponent<DesignViewComponent>().CreateNewNode(tag);
							};
							index++;
						}
					};
					pl_Category.Controls.Add(btn);
					left += 60 + 30;
				}

			};
		}
		public void AppendTextColorful(string addtext, Color color, Font font, bool IsaddNewLine)
		{
			if (IsaddNewLine)
			{
				addtext += Environment.NewLine;
			}
			txt_exp.SelectionStart = txt_exp.TextLength;
			txt_exp.SelectionLength = 0;
			txt_exp.SelectionFont = font;
			txt_exp.SelectionColor = color;
			txt_exp.AppendText(addtext);
			txt_exp.SelectionColor = txt_exp.ForeColor;
			txt_exp.HideSelection = false;
		}
		public async void RefreshRobots(long flowId, bool isVisible = true)
		{
			panel5.Visible = !isVisible;
			if (!designComponent.Features.Any()) return;
			flp_robotList.Controls.Clear();
			if (isVisible && flowId != default)
			{
				var flow = await designComponent.StartDesign(flowId);
				if (designComponent.CurrentDesignFlow != null)
					lbl_name.Text = designComponent.CurrentDesignFlow.Name;
				var msgs = designComponent.GetDesignMsg();
				var list = designComponent.Features.OrderByDescending(x => x.ViewSort).ToList();
				foreach (var item in list)
				{
					var robot = new RobotListView(item.Id, item.Name, item.HeadImg);
					robot.Dock = DockStyle.Top;
					flp_robotList.Controls.Add(robot);
				}
				pl_view.Controls.Clear();
				pl_bottom.Controls.Clear();
				pl_view.Controls.Add(designViewComponent.DesignViewForm);
				pl_bottom.Controls.Add(viewLogComponent.LogsViewForm);
				designViewComponent.Main(flow);
				viewLogComponent.Main();
				return;
			}
			var list1 = designComponent.Features.OrderByDescending(x => x.ViewSort).ToList();
			foreach (var item in list1)
			{
				var robot = new RobotListView(item.Id, item.Name, item.HeadImg);
				robot.Dock = DockStyle.Top;
				flp_robotList.Controls.Add(robot);
			}
			return;
		}
		public void CreateExp(Config config)
		{
			pl_cmd.Controls.Add(CreateCmd(config.Parameters, pl_cmd.Location.X));
			pl_cmd.AutoScroll = true;
		}
		public Control CreateCmd(List<Parameter> parameters, int left)
		{
			if (parameters == null || parameters.Count == 0)
				return null;
			if (parameters.Count == 1)
			{
				var pl = CreateControl(parameters.First(), left);
				pl_cmd.Controls.Add(pl);
				return pl;
			}
			return CreateControl(parameters, left);
		}
		public Panel CreateControl(List<Parameter> parameters, int left)
		{
			Panel panel = new Panel();
			panel.Left = left;
			panel.Width = 70;
			panel.Height = 22;
			panel.Top = 10;
			ComboBox comboBox = new()
			{
				DisplayMember = nameof(Parameter.DisplayName),
				Left = 0,
				Width = 70
			};
			comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
			panel.Controls.Add(comboBox);
			comboBox.Width = panel.Width;
			foreach (var item in parameters)
			{
				comboBox.Items.Add(item);
			}
			comboBox.SelectedValueChanged += (x, y) =>
			{
				var param = comboBox.SelectedItem as Parameter;
				foreach (var item in comboBox.Items)
					(item as Parameter).IsFinish = false;
				if (param.IsInput)
				{
					panel.Width = 140;
					var cb = new ComboBox
					{
						Name = param.Key,
						Left = 70,
						Width = 70
					};
					cb.DropDownStyle = ComboBoxStyle.DropDownList;
					cb.DisplayMember = nameof(ValueInfo.DisplayName);
					foreach (var item in param.Values)
					{
						cb.Items.Add(item);
					}
					panel.Controls.Add(cb);
					cb.SelectedValueChanged += async (x, y) =>
					{
						var temp = cb.PointToScreen(cb.Location);
						var p = new Point(temp.X - cb.Width, temp.Y + cb.Height);
						var valueInfo = cb.SelectedItem as ValueInfo;
						if (valueInfo.ActionType == ValueActionType.RequestList)
						{
							var response = await RequestManager.StartAsync(valueInfo.Action);
							var win = new PopuForm(valueInfo, p, response.Data);
							var result = win.ShowDialog();
							if (result == DialogResult.OK)
								param.Value = win.Value;
						}
						if (valueInfo.ActionType == ValueActionType.RequestValue)
						{
							var response = await RequestManager.StartAsync(valueInfo.Action);
							valueInfo.Value = response.Data;
							param.Value = valueInfo;
						}
						if (valueInfo.ActionType == ValueActionType.Input)
						{
							var win = new PopuForm(valueInfo, p);
							var result = win.ShowDialog();
							if (result == DialogResult.OK)
								param.Value = win.Value;
						}
						if (valueInfo.ActionType != ValueActionType.Value)
						{
							param.Value = valueInfo;
						}
					};
					var componment = Boot.GetComponent<DesignViewComponent>();
					componment.EditNode.ParamDic[param.Key] = param;
					//ParamDic[param.Key] = param;
					txt_exp.Text = "";
					componment.GenerateCmd();
				}
				List<Control> cons = new();
				var keys1 = parameters.Select(x => x.Key);
				var keys2 = parameters.SelectMany(x => x.Parameters).Select(x => x.Key);
				var keys = keys1.Concat(keys2);
				foreach (Control item in pl_cmd.Controls)
				{

					if (keys.Contains(item.Name))
						cons.Add(item);
				}
				cons.ForEach(x => pl_cmd.Controls.Remove(x));
				pl_cmd.Controls.Add(CreateCmd(param.Parameters, panel.Right));

			};
			comboBox.SelectedIndex = 0;
			return panel;
		}
		public Panel CreateControl(Parameter param, int left)
		{
			Panel panel = new();
			panel.Name = param.Key;
			panel.Left = left;
			panel.Top = 10;
			panel.Height = 22;
			Label link = new LinkLabel();
			var notExist = param.Parameters == null || param.Parameters.Count == 0;
			if (notExist)
				link = new Label();
			link.TextAlign = ContentAlignment.MiddleRight;
			link.Tag = param;
			link.Text = param.DisplayName + ":";
			link.Width = 70;
			panel.Controls.Add(link);
			panel.Width = 70;
			if (param.IsInput)
			{
				panel.Width += 70;
				var cb = new ComboBox
				{
					Left = 70,
					Width = 70
				};
				cb.DropDownStyle = ComboBoxStyle.DropDownList;
				cb.DisplayMember = nameof(ValueInfo.DisplayName);
				foreach (var item in param.Values)
				{
					cb.Items.Add(item);
				}
				cb.SelectedValueChanged += async (x, y) =>
				{
					var temp = cb.PointToScreen(cb.Location);
					var p = new Point(temp.X - cb.Width, temp.Y + cb.Height);
					var valueInfo = cb.SelectedItem as ValueInfo;
					if (valueInfo.ActionType == ValueActionType.RequestList)
					{
						var response = await RequestManager.StartAsync(valueInfo.Action);
						if (response.Data is List<string> listStr)
						{
							var popu = new PopuForm(valueInfo, p, listStr);
							var result = popu.ShowDialog();
							if (result == DialogResult.OK)
							{
								param.Value = popu.Value;
							}

						}
					}
					if (valueInfo.ActionType == ValueActionType.RequestValue)
					{
						var response = await RequestManager.StartAsync(valueInfo.Action);
						valueInfo.Value = response.Data;
						param.Value = valueInfo;
					}
					if (valueInfo.ActionType == ValueActionType.Input)
					{
						var popu = new PopuForm(valueInfo, p);
						var result = popu.ShowDialog();
						if (result == DialogResult.OK)
							param.Value = popu.Value;
					}
					if (valueInfo.ActionType == ValueActionType.Value)
					{
						param.Value = valueInfo;
					}
					var componment = Boot.GetComponent<DesignViewComponent>();
					componment.EditNode.ParamDic[param.Key] = param;

					this.Invoke(() =>
					{
						txt_exp.Text = "";
						componment.GenerateCmd();
					});
				};
				//cb.Click += (x, y) =>
				//{
				//	pl_key.Controls.Clear();
				//	foreach (var val in param.Values)
				//	{
				//		var lin = new LinkLabel()
				//		{
				//			Tag = param,
				//			Text = param.DisplayName,
				//			Width = 80
				//		};
				//		pl_key.Controls.Add(lin);
				//		lin.Click += (x, y) =>
				//		{
				//			cb.Tag = val;
				//		};
				//	}
				//};
				panel.Controls.Add(cb);
			}
			if (!notExist)
			{
				link.Click += (x, y) =>
				{
					pl_cmd.Controls.Add(CreateCmd(param.Parameters, panel.Right));
				};
			}
			return panel;
		}
		public void ClearCurrentComponent()
		{
			CurrentConfig = null;
			ParamDic.Clear();
			txt_exp.Text = "";
			pl_cmd.Controls.Clear();
		}
		/// <summary>
		/// 鼠标按下
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>

	}
}