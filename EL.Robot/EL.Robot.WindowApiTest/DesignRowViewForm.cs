using EL.Robot.Component;
using EL.Robot.WindowApiTest.Properties;
using Google.Protobuf;
using Org.BouncyCastle.Tls.Crypto;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace EL.Robot.WindowApiTest
{
	public partial class DesignRowViewForm : UserControl
	{
		//window标准拖拽事件
		[DllImport("user32.dll", EntryPoint = "ReleaseCapture")]
		public static extern void ReleaseCapture();
		[DllImport("user32.dll", EntryPoint = "SendMessage")]
		public static extern void SendMessage(int hwnd, int wMsg, int wParam, int lParam);
		public int CmdWidth { get; set; }
		public DesignRowViewForm()
		{
			InitializeComponent();
			this.AllowDrop = true;
			this.pl_Cmd.AllowDrop = true;
			var lineTemp = new Label()
			{
				Height = 3,
				Width = pl_Cmd.Width,
				Dock = DockStyle.Bottom,
				BackColor = Color.Blue,
				BorderStyle = BorderStyle.Fixed3D
			};
			pl_Cmd.Controls.Add(lineTemp);
			lineTemp.Hide();
			var designViewComponent = Boot.GetComponent<DesignViewComponent>();
			pl_Cmd.DragDrop += (x, y) =>
			{
				lineTemp.Hide();
				y.Effect = DragDropEffects.All;
				var data = y.Data;
				designViewComponent.MoveRows(this);
			};
			pl_Cmd.DragOver += (x, y) =>
			{
				y.Effect = DragDropEffects.All;
				lineTemp.BringToFront();
				lineTemp.Show();
			};
			pl_Cmd.DragLeave += (x, y) =>
			{
				lineTemp.Hide();
			};
			this.Leave += (x, y) =>
			{
				if (!designViewComponent.SelectRowViews.Contains(this))
					pl_Cmd.BackColor = Color.LightSkyBlue;
			};
			this.MouseDown += (x, y) =>
			{
				designViewComponent.SetCurrentNode(this);
				if (y.Button == MouseButtons.Left)
				{
					this.DoDragDrop(this, DragDropEffects.All);
				}
			};

			this.MouseHover += (x, y) =>
			{
				if (!designViewComponent.SelectRowViews.Contains(this))
					pl_Cmd.BackColor = Color.LightSkyBlue;
			};
			this.MouseLeave += (x, y) =>
			{
				if (!designViewComponent.SelectRowViews.Contains(this))
					pl_Cmd.BackColor = Color.LightSteelBlue;
			};
			this.Click += (x, y) =>
			{
				designViewComponent.SetCurrentNode(this);
			};
			pic_block.Image = Resources.Hide;
			designViewComponent.UpdateRowViewAction = Update;
			pic_block.Click += (x, y) =>
			{
				if ((bool)pic_block.Tag)
				{
					pic_block.Image = Resources.Hide;
					pic_block.Tag = false;
				}
				else
				{
					pic_block.Image = Resources.Expansion;
					pic_block.Tag = true;

				}
				var view = Boot.GetComponent<DesignViewComponent>();
				view.HideExpansionNodeAction?.Invoke((Node)Tag, (bool)pic_block.Tag);
			};
			this.DoubleClick += (x, y) =>
			{
				if ((bool)pic_block.Tag)
				{
					pic_block.Image = Resources.Hide;
					pic_block.Tag = false;
				}
				else
				{
					pic_block.Image = Resources.Expansion;
					pic_block.Tag = true;

				}
				var view = Boot.GetComponent<DesignViewComponent>();
				view.HideExpansionNodeAction?.Invoke((Node)Tag, (bool)pic_block.Tag);
			};
			lbl_DisplayCmd.BackColor = Color.Transparent;
			pic_block.BackColor = Color.Transparent;
			pl_Cmd.BackColor = Color.LightSteelBlue;
			pl_Cmd.Click += (x, y) =>
			{
				base.OnClick(y);
			};
			pl_Cmd.MouseDown += (x, y) =>
			{
				if (y.Button == MouseButtons.Left)
					base.OnMouseDown(y);
			};
			pl_Cmd.MouseHover += (x, y) =>
			{
				base.OnMouseHover(y);
			};
			pl_Cmd.MouseLeave += (x, y) =>
			{
				base.OnMouseLeave(y);
			};


			this.Click += (x, y) =>
			{
				var mouse = y as MouseEventArgs;
				if (mouse != null && mouse.Button == MouseButtons.Left)
					designViewComponent.SetCurrentNode(this);
				if (mouse != null && mouse.Button == MouseButtons.Right)
				{
					if (designViewComponent.SelectRowViews.Count > 0)
						designViewComponent.ContextMenuStrip.Show(MousePosition.X, MousePosition.Y);
				}
			};
			foreach (var item in pl_Cmd.Controls)
			{
				if (item is Control cl)
				{
					cl.DoubleClick += (x, y) =>
					{
						base.OnDoubleClick(y);
					};
					cl.Click += (x, y) =>
					{
						base.OnClick(y);
					};
					cl.MouseHover += (x, y) =>
					{
						base.OnMouseHover(y);
					};
					cl.MouseLeave += (x, y) =>
					{
						base.OnMouseLeave(y);
					};
					cl.MouseDown += (x, y) =>
					{
						if (y.Button == MouseButtons.Left)
							base.OnMouseDown(y);
					};
				}
			}
		}
		public void SetBackColor(Color color)
		{
			if (pl_Cmd.BackColor != color)
			{
				pl_Cmd.BackColor = color;
				this.Refresh();
			}
		}
		public void UpdateIndex(string index)
		{
			lbl_Index.Text = index;
		}
		public void Update(RowData row)
		{
			if (row == null) return;
			if (!string.IsNullOrEmpty(row.Index))
				lbl_Index.Text = row.Index;
			lbl_DisplayCmd.Text = row.DisplayExp;
			pic_block.Visible = row.IsBlock;
			pic_block.Image = Resources.Hide;
			pic_block.Tag = false;
			if (this.Parent is FlowLayoutPanel pl && pl.Tag is NodePanel np)
				pl_Cmd.Width = np.Width;
			if (this.Parent is FlowLayoutPanel pl2)
				Line(pl2);
			var designViewComponent = Boot.GetComponent<DesignViewComponent>();
			designViewComponent.DesignViewForm.Scroll(1);
			this.Refresh();
		}
		public void Line(FlowLayoutPanel panel)
		{
			if (panel.Parent is FlowLayoutPanel pl)
			{
				var lbl = new Label()
				{
					Top = -5,
					Height = panel.Height + 10,
					Width = 3,
					BackColor = Color.Blue,
					BorderStyle = BorderStyle.Fixed3D
				};
				if (pl.Tag != null && pl.Tag is NodePanel np)
					lbl.Left = this.Width - np.Width;
				else
					lbl.Left = this.Width - 733;
				this.Controls.Add(lbl);
				Line(pl);
			}
		}
	}

}
