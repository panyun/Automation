using EL.Robot.Component;
using EL.Robot.Core;
using EL.Robot;
using System.Security.Cryptography.X509Certificates;
using static EL.Robot.Core.ComponentSystem;
using System.Runtime.InteropServices;
using System.Text;

namespace EL.Robot.WindowApiTest
{
	public partial class IndexForm : Form
	{
		[DllImport("user32.dll", EntryPoint = "SendMessageA")]
		private static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, StringBuilder lParam);

		[DllImport("user32 ")]
		private static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

		public IndexForm()
		{
			InitializeComponent();
			DispatcherHelper.BaseForm = this;
			this.StartPosition = FormStartPosition.CenterScreen;
			pl_components.Visible = false;
			pl_key.Visible = false;
			txt_exp.LostFocus += (x, y) =>
			{
				pl_key.Visible = false;
			};
			pl_components.Height = pl_Category.Height;
			pl_components.Leave += (x, y) =>
			{
				pl_components.Visible = false;
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
				int left = 30;
				foreach (var category in categorys.Data)
				{
					var btn = new Button()
					{
						Text = Helper.GetValue<string>(category, "CategoryName"),
						Width = 85,
						Height = 22,
						Top = 5,
						TextAlign = ContentAlignment.MiddleCenter,
						Tag = Helper.GetValue<int>(category, "CategoryId"),
						Left = left,
					};
					btn.Click += async (x, y) =>
					{
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
						int left = 30;
						foreach (var item in componentConfigs.Data)
						{
							var config = item as Config;
							var btn = new Button()
							{
								Text = config.DisplayName,
								Width = 85,
								Height = 22,
								Top = 5,
								TextAlign = ContentAlignment.MiddleCenter,
								Tag = config,
								Left = left,
							};

							pl_components.Controls.Add(btn);
							left += 60 + 30;
							btn.Click += async (x, y) =>
							{
								var tag = (x as Button).Tag as Config;
								CommponetRequest commponetRequest = new CommponetRequest()
								{
									ComponentName = nameof(ComponentSystem),
									Data = new ComponentSystemReuqest
									{
										Action = nameof(ComponentSystem.GetExpression),
										Paramters = new Dictionary<string, object>()
										{
											{nameof(tag.ComponentName), tag.ComponentName }
										}
									}
								};

								var exp = await RequestManager.StartAsync(commponetRequest);
								txt_exp.Text = exp.Data;
							};
						}
					};
					pl_Category.Controls.Add(btn);
					left += 60 + 30;
				}
				var btn_catchElement = new Button()
				{
					Text = "捕获",
					Width = 85,
					Height = 22,
					Top = 5,
					TextAlign = ContentAlignment.MiddleCenter,
					Left = left,
				};
				btn_catchElement.Click += async (x, y) =>
				{
					CommponetRequest requestBase = new()
					{
						ComponentName = nameof(CatchElementComponent)
					};
					var json = BsonHelper.ToJson(requestBase);
					var obj = BsonHelper.FromJson<CommponetRequest>(json);
					var result = await RequestManager.StartAsync(json);
				};
				pl_Category.Controls.Add(btn_catchElement);
			};

		}

		private void txt_exp_TextChanged(object sender, EventArgs e)
		{
			var p = GetCursorPos();
			pl_key.Visible = true;
			var y = (int)((p.X * 20) + txt_exp.Top- (p.X*1.5f));
			if (y > txt_exp.Top + txt_exp.Height)
			{
				y = txt_exp.Top + txt_exp.Height;
			}
			//查询字典
			pl_key.Location = new Point(pl_key.Location.X, y);
		}
		//获取位置的函数
		private Point GetCursorPos()
		{
			int EM_LINEINDEX = 0xBB;
			int EM_LINEFROMCHAR = 0xC9;
			int EM_GETSEL = 0xB0;
			int EM_GETLINE = 0xC4;
			int i = 0, j = 0, k = 0;
			int lParam = 0, wParam = 0;
			i = (int)SendMessage(txt_exp.Handle, EM_GETSEL, wParam, lParam);
			j = i / 65536;
			int lineNo = (int)SendMessage(txt_exp.Handle, EM_LINEFROMCHAR, j, lParam) + 1;
			k = (int)SendMessage(txt_exp.Handle, EM_LINEINDEX, -1, lParam);
			int colNo = j - k + 1;
			Point ret = new Point(lineNo, colNo);
			return ret;
		}

	}
}