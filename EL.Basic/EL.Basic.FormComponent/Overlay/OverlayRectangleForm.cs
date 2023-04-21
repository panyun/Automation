#if NETFRAMEWORK || NETCOREAPP

using EL.Input;
using EL.WindowsAPI;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Label = System.Windows.Forms.Label;

namespace EL.Overlay
{
	public class OverlayRectangleForm : Form
	{
		private Color lineColor;
		public Color LineColor
		{
			get
			{
				return Color.FromArgb(100, lineColor);
			}
		}
		Panel panel;
		TableLayoutPanel tableLayoutPanel;
		readonly List<Panel> Panels = new();
		int SH = Screen.PrimaryScreen.Bounds.Height; //1080
		int SW = Screen.PrimaryScreen.Bounds.Width; //1920
		int lableFont = 15;
		Panel pl_CatchInfo = new();
		public OverlayRectangleForm()
		{
			Height = Screen.PrimaryScreen.Bounds.Height;
			Width = Screen.PrimaryScreen.Bounds.Width;
			FormBorderStyle = FormBorderStyle.None;
			ShowInTaskbar = false;
			Left = 0;
			Top = 0;
			Visible = false;
			TransparencyKey = Color.Gray;
			BackColor = Color.Gray;
			Cursor = Cursors.Cross;
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			AllowTransparency = true;
			pl_CatchInfo.Height = (int)(SH * 0.18f);
			pl_CatchInfo.Width = (int)(SW * 0.1f);
			pl_CatchInfo.BackColor = Color.White;
			pl_CatchInfo.Left = 5;
			pl_CatchInfo.Top = 5;
			this.Controls.Add(pl_CatchInfo);
			tableLayoutPanel = new TableLayoutPanel();
			pl_CatchInfo.Controls.Add(tableLayoutPanel);
			tableLayoutPanel.Dock = DockStyle.Fill;     //顶部填充
			tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, tableLayoutPanel.Width * 0.4f));    //利用百分比计算，0.2f表示占用本行长度的20%
			tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, tableLayoutPanel.Width * 0.6f));
			// 动态添加一行
			tableLayoutPanel.RowCount = 4;
			// 行高
			tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize, tableLayoutPanel.Height * 0.2f));
			// 设置cell样式，增加线条
			tableLayoutPanel.CellBorderStyle = TableLayoutPanelCellBorderStyle.OutsetPartial;
			tableLayoutPanel.Controls.Add(new Label
			{
				Text = "位置坐标",
				Font = new Font("隶书", 13, FontStyle.Bold),
				TextAlign = ContentAlignment.MiddleRight
			}, 0, 0);
			tableLayoutPanel.Controls.Add(new Label
			{
				Text = "控件类型",
				Font = new Font("隶书", 13, FontStyle.Bold),
				TextAlign = ContentAlignment.MiddleRight
			}, 0, 1);
			tableLayoutPanel.Controls.Add(new Label
			{
				Text = "探测技术",
				Font = new Font("隶书", 13, FontStyle.Bold),
				TextAlign = ContentAlignment.MiddleRight
			}, 0, 2);
			tableLayoutPanel.Controls.Add(new Label
			{
				Text = "捕获热键",
				Font = new Font("隶书", 13, FontStyle.Bold),
				TextAlign = ContentAlignment.MiddleRight
			}, 0, 3);
			tableLayoutPanel.Controls.Add(new Label
			{
				Text = "切换热键",
				Font = new Font("隶书", 13, FontStyle.Bold),
				TextAlign = ContentAlignment.MiddleRight
			}, 0, 4);
			tableLayoutPanel.Controls.Add(new Label
			{
				Text = "退出热键",
				Font = new Font("隶书", 13, FontStyle.Bold),
				TextAlign = ContentAlignment.MiddleRight
			}, 0, 5);
			//
			tableLayoutPanel.Controls.Add(new Label()
			{
				Font = new Font("隶书", 13, FontStyle.Bold),
				TextAlign = ContentAlignment.MiddleLeft
			}, 1, 0);
			tableLayoutPanel.Controls.Add(new Label()
			{
				Font = new Font("隶书", 13, FontStyle.Bold),
				TextAlign = ContentAlignment.MiddleLeft
			}, 1, 1);
			tableLayoutPanel.Controls.Add(new Label()
			{
				Font = new Font("隶书", 13, FontStyle.Bold),
				TextAlign = ContentAlignment.MiddleLeft
			}, 1, 2);
			tableLayoutPanel.Controls.Add(new Label()
			{
				Font = new Font("隶书", 13, FontStyle.Bold),
				TextAlign = ContentAlignment.MiddleLeft
			}, 1, 3);
			tableLayoutPanel.Controls.Add(new Label()
			{
				Font = new Font("隶书", 13, FontStyle.Bold),
				TextAlign = ContentAlignment.MiddleLeft
			}, 1, 4);
			tableLayoutPanel.Controls.Add(new Label()
			{
				Font = new Font("隶书", 13, FontStyle.Bold),
				TextAlign = ContentAlignment.MiddleLeft
			}, 1, 5);
			#region ceshi
			//Opacity = 0.7;
			//SetStyle(ControlStyles.Opaque, true);
			//this.BackColor = Color.Transparent;
			//lable = new Label();
			//lable.TabIndex = 1;
			//lable.Name = "lable";
			//lable.Top = 0;
			//lable.Left = 0;
			////lable.BackColor = Color.Transparent;
			//lable.BackColor = Color.FromArgb(100, Color.Black);
			//lable.Height = lableH;
			//lable.Width = lableW;
			//lable.TextAlign = ContentAlignment.MiddleLeft;
			//lable.ForeColor = Color.White;
			//lable.Font = new Font("黑体", lableFont, FontStyle.Regular);
			//pl_CatchInfo.BackColor = Color.FromArgb(100, Color.Black);
			//lable.BackColor = Color.FromArgb(100, Color.Black);
			//this.TopMost = true;
			#endregion
		
		}

		public void ShowEx()
		{
			this.Invoke(() =>
			{
				this.Show();
			});
		}
		public void HideEx()
		{
			this.Invoke(() =>
			{
				if (this.Visible)
				{
					this.Hide();
					this.Refresh();
				}
			});
		}
		TableLayoutPanel table = new();
		public void UpdateText(dynamic element)
		{
			this.Invoke(() =>
			{
				var mouseX = Mouse.Position.X;
				var mouseY = Mouse.Position.Y;
				if (pl_CatchInfo.ClientRectangle.Contains(Mouse.Position))
					pl_CatchInfo.Left = SW - pl_CatchInfo.Width-5;
				if (!pl_CatchInfo.ClientRectangle.Contains(Mouse.Position))
					pl_CatchInfo.Left = 5;
				var type = "未识别";
				if (element != null)
					type = element.ControlType;
				tableLayoutPanel.GetControlFromPosition(1, 0).Text = $"({mouseX},{mouseY})";
				tableLayoutPanel.GetControlFromPosition(1, 1).Text = $@"{type}";
				tableLayoutPanel.GetControlFromPosition(1, 2).Text = $"{FormOverLayComponent.Instance.Mode}";
				tableLayoutPanel.GetControlFromPosition(1, 3).Text = $"{FormOverLayComponent.Instance.FunctionKey}+{FormOverLayComponent.Instance.ComplateHotKey}";
				tableLayoutPanel.GetControlFromPosition(1, 4).Text = $"{FormOverLayComponent.Instance.FunctionKey}+{FormOverLayComponent.Instance.ModeHotKey}";
				tableLayoutPanel.GetControlFromPosition(1, 5).Text = $"{FormOverLayComponent.Instance.FunctionKey}+{FormOverLayComponent.Instance.ExitHotKey}";
				this.Refresh();
			});
		}

		public void SetWindow(dynamic element, Color color)
		{
			this.Invoke(() =>
			{
				UpdateText(element);
				lineColor = color;
				pl_CatchInfo.Visible = true;
				AddPanels(new List<dynamic> { element.Rectangle });
			});
		}
		void AddPanels(List<dynamic> rectangles)
		{
			for (int i = 0; i < Panels.Count; i++)
				Controls.Remove(Panels[i]);
			Panels.Clear();
			rectangles.ForEach(rectangle =>
			{
				panel = new Panel
				{
					Left = rectangle.X - 5,
					Top = rectangle.Y - 5,
					Height = rectangle.Height + 10,
					Width = rectangle.Width + 10
				};
				panel.Paint += Panel_Paint;
				Panels.Add(panel);
				Controls.Add(panel);
			});
		}

		public void Clear()
		{
			this.Invoke(() =>
			{
				Panels.ForEach(x =>
				{
					if (Controls.Contains(x))
						Controls.Remove(x);
				});
				Panels.Clear();
			});
		}
		public void LightHigh(Rectangle element, Color color)
		   => LightHighMany(new List<dynamic> { element }, color);
		public void LightHigh(dynamic element, Color color)
		 => LightHighMany(new List<dynamic> { element }, color);
		public void LightHighMany(List<dynamic> BoundingRectangles, Color color)
		{
			this.Invoke(() =>
			{
				lineColor = color;
				pl_CatchInfo.Visible = false;
				AddPanels(BoundingRectangles);
			});
		}
		protected override void WndProc(ref Message m)
		{
			base.WndProc(ref m);
			var hotkey = FormOverLayComponent.Instance.GetComponent<HotkeyComponent>();
			hotkey.ProcessHotKey(m);
		}
		private void Panel_Paint(object sender, PaintEventArgs e)
		{
			System.Drawing.Graphics graphics = e.Graphics;
			ControlPaint.DrawBorder(e.Graphics,
							 ((Panel)sender).ClientRectangle,
							  lineColor,//7f9db9
							  3,
							  ButtonBorderStyle.Solid,
							  lineColor,
							  3,
							  ButtonBorderStyle.Solid,
							  lineColor,
							  3,
							  ButtonBorderStyle.Solid,
							  lineColor,
							  3,
							  ButtonBorderStyle.Solid);
		}
		protected override bool ShowWithoutActivation => true;
		protected override CreateParams CreateParams
		{
			get
			{
				var result = base.CreateParams;
				result.ExStyle |= (int)WindowStyles.WS_EX_TOOLWINDOW;
				result.ExStyle |= (int)WindowStyles.WS_EX_TRANSPARENT;
				result.ExStyle |= (int)WindowStyles.WS_EX_NOACTIVATE;
				result.ExStyle |= (int)WindowStyles.WS_EX_LAYERED;
				result.ExStyle |= (int)WindowStyles.WS_EX_TOPMOST;
				return result;
			}
		}
	}
}
#endif