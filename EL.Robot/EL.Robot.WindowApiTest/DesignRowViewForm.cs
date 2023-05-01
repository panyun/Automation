using EL.Robot.Component;
using EL.Robot.WindowApiTest.Properties;
using Google.Protobuf;
using Org.BouncyCastle.Tls.Crypto;
using System.Linq;

namespace EL.Robot.WindowApiTest
{
	public partial class DesignRowViewForm : UserControl
	{
		public int CmdWidth { get; set; }
		public DesignRowViewForm()
		{
			InitializeComponent();
			pic_block.Image = Resources.Hide;
			var designViewComponent = Boot.GetComponent<DesignViewComponent>();
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
			lbl_DisplayCmd.BackColor = Color.Transparent;
			pic_block.BackColor = Color.Transparent;
			pl_Cmd.BackColor = Color.LightSteelBlue;
			pl_Cmd.MouseHover += (x, y) =>
			{
				if (!designViewComponent.SelectRowViews.Contains(this))
					pl_Cmd.BackColor = Color.LightSkyBlue;
			};
			pl_Cmd.MouseLeave += (x, y) =>
			{
				if (!designViewComponent.SelectRowViews.Contains(this))
					pl_Cmd.BackColor = Color.LightSteelBlue;
			};
			pl_Cmd.Click += (x, y) =>
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
					cl.Click += (x, y) => { designViewComponent.SetCurrentNode(this); };
					cl.MouseHover += (x, y) =>
					{
						if (!designViewComponent.SelectRowViews.Contains(this))
							pl_Cmd.BackColor = Color.LightSkyBlue;
					};
					cl.MouseLeave += (x, y) =>
					{
						if (!designViewComponent.SelectRowViews.Contains(this))
							pl_Cmd.BackColor = Color.LightSteelBlue;
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
