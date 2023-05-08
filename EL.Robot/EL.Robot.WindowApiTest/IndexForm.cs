using EL.Robot.Component;
using EL.Robot.Component.DTO;
using EL.Robot.Core;
using EL.Robot.WindowApiTest.Code;
using SixLabors.Fonts.Tables.AdvancedTypographic;
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
		public int ControlWidth = 85;

		public static IndexForm Ins;
		public IndexForm()
		{
			DesignComponent designComponent = Boot.GetComponent<RobotComponent>().GetComponent<DesignComponent>();
			Ins = this;
			InitializeComponent();
			lbl_name.Text = "";
			DispatcherHelper.BaseForm = this;
			this.StartPosition = FormStartPosition.CenterScreen;
			txt_exp.Font = new Font("黑体", 10);
			txt_exp.ForeColor = Color.Black;
			pl_cmd.AutoScroll = true;
			pl_bottom.Visible = false;
			var list = designComponent.LoadRobots();
			RefreshRobots(default, false);
			InitEvent();
		}
		Point mpoint = default;
		public void InitEvent()
		{
			var component = Boot.GetComponent<DesignFlowViewComponent>();
			var viewLogComponent = Boot.GetComponent<ViewLogComponent>();
			var designComponent = Boot.GetComponent<RobotComponent>().GetComponent<DesignComponent>();
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
			pl_winTop.DoubleClick += (x, y) =>
			{
				if (this.WindowState != FormWindowState.Maximized)
					this.WindowState = FormWindowState.Maximized;
				else this.WindowState = FormWindowState.Normal;
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
					viewLogComponent.WriteDesignLog($"你创建了一个[{name}]流程");
				}
			};
			btn_send.Click += (x, y) =>
			{
				var txt = txt_exp.Text;
				if (txt.Length == 0)
				{
					viewLogComponent.WriteDesignLog("你没有生成指令，我不能为你追加到我的指令集！", true);
					return;
				}

				component.AddOrUpdateRows();
				return;
			};
			btn_run.Click += async (x, y) =>
			{
				var logs = await component.RunRobot();
				viewLogComponent.WriteDesignLog("恭喜哟，你成功运行了执行！");
			};
			lbl_preExec.Click += async (x, y) =>
			{
				var nodes = component.PreExecNodes();
				
			};
			btn_save.Click += async (x, y) =>
			{
				try
				{
					await Boot.GetComponent<DesignFlowViewComponent>().SaveFlowAsync();
					RefreshRobots(designComponent.CurrentDesignFlow.Id);
				}
				catch (Exception ex)
				{
					viewLogComponent.WriteDesignLog("当前保存出错，请退出后重新打开", true);
				}

			};

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
								Boot.GetComponent<DesignFlowViewComponent>().CreateNewNode(tag);
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
			var viewLogComponent = Boot.GetComponent<ViewLogComponent>();
			var designViewComponent = Boot.GetComponent<DesignViewComponent>();
			var designComponent = Boot.GetComponent<RobotComponent>().GetComponent<DesignComponent>();
			panel5.Visible = !isVisible;
			if (!designComponent.Features.Any()) return;
			flp_robotList.Controls.Clear();
			var list1 = designComponent.Features.OrderByDescending(x => x.ViewSort).ToList();
			foreach (var item in list1)
			{
				var robot = new RobotListView(item.Id, item.Name, item.HeadImg);
				robot.Dock = DockStyle.Top;
				flp_robotList.Controls.Add(robot);
			}
			flp_robotList.Refresh();
			if (isVisible && flowId != default)
			{
				var flow = await designComponent.StartDesign(flowId);
				if (designComponent.CurrentDesignFlow != null)
					lbl_name.Text = designComponent.CurrentDesignFlow.Name;
				lbl_name.Refresh();
				pl_view.Controls.Clear();
				pl_bottom.Controls.Clear();
				var fv = Boot.GetComponent<DesignFlowViewComponent>();
				pl_view.Controls.Add(fv.DesignFlowViewForm);
				pl_bottom.Controls.Add(viewLogComponent.LogsViewForm);
				viewLogComponent.Main();
				fv.Main(flow);
				pl_bottom.Visible = true;
				return;
			}
		}
		public void CreateExp(Config config)
		{
			var con = CreateCmd(config.Parameters, pl_cmd.Location.X);
			if (con != null)

				pl_cmd.Controls.Add(con);

		}
		public Control CreateCmd(List<Parameter> parameters, int left)
		{
			if (parameters == null || parameters.Count == 0)
				return null;
			if (parameters.Count == 1)
			{
				if (parameters.First() == null) return default;
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
			panel.Width = ControlWidth;
			panel.Height = 22;
			panel.Top = 10;
			ComboBox comboBox = new()
			{
				DisplayMember = nameof(Parameter.DisplayName),
				Left = 0,
				Width = ControlWidth
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
						Left = ControlWidth,
						Width = ControlWidth
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
					var componment = Boot.GetComponent<DesignFlowViewComponent>();
					componment.ComponentInfo.ParamDic[param.Key] = param;
					//var componment = Boot.GetComponent<DesignViewComponent>();
					//componment.EditNode.ParamDic[param.Key] = param;
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
			link.Width = ControlWidth;
			panel.Controls.Add(link);
			panel.Width = ControlWidth;
			if (param.IsInput)
			{
				panel.Width += ControlWidth;
				var cb = new ComboBox
				{
					Left = ControlWidth,
					Width = ControlWidth
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
								param.Value = popu.Value;
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
						param.Value = valueInfo;
					var componment = Boot.GetComponent<DesignFlowViewComponent>();
					componment.ComponentInfo.ParamDic[param.Key] = param;
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