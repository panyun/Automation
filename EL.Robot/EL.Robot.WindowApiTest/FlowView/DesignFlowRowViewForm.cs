using EL.Robot.Component;
using EL.Robot.WindowApiTest.Properties;
using Google.Protobuf;
using NPOI.HSSF.Record;
using Org.BouncyCastle.Tls.Crypto;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace EL.Robot.WindowApiTest
{
	public partial class DesignFlowRowViewForm : UserControl
	{
		public int Index { get; set; }
		public DesignFlowRowViewForm()
		{
			InitializeComponent();
			var designViewComponent = Boot.GetComponent<DesignFlowViewComponent>();
			pl_Cmd.Click += (x, y) =>
			{
				base.OnClick(y);
			};
			pl_Cmd.DoubleClick += (x, y) =>
			{
				base.OnDoubleClick(y);
			};
			lbl_DisplayCmd.DoubleClick += (x, y) =>
			{
				base.OnDoubleClick(y);
			};
			lbl_DisplayCmd.Click += (x, y) =>
			{
				base.OnClick(y);
			};
			this.pic_block.Click += (x, y) =>
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
				var view = Boot.GetComponent<DesignFlowViewComponent>();
				view.HideExpansionNodeAction?.Invoke(this, (bool)pic_block.Tag);
			};
			this.DoubleClick += (x, y) =>
			{
				designViewComponent.EditNode(this);
			};
			this.Click += (x, y) =>
			{
				var mouse = y as MouseEventArgs;
				designViewComponent.SetCurrentRow(this);
				if (mouse != null && mouse.Button == MouseButtons.Right)
				{
					if (designViewComponent.SelectRowViews.Count > 0)
						designViewComponent.ContextMenuStrip.Show(MousePosition.X, MousePosition.Y);
				}
			};
		}
		public void Update(Node row, int index = default)
		{
			var np = this.Parent.Tag as NodePanel;
			lbl_DisplayCmd.Text = row.DisplayExp;
			pic_block.Visible = row.IsBlock;
			UpdateIndex(index);
			pic_block.Image = Resources.Hide;
			pic_block.Tag = false;
			pl_Cmd.Width = np.Width;
			if (this.Parent is FlowLayoutPanel pl2)
				Line(pl2);
			Boot.GetComponent<DesignFlowViewComponent>().DesignFlowViewForm.Scroll();
			this.Refresh();
		}
		public void UpdateIndex(int index)
		{
			if (index == default) return;
			Index = index;
			lbl_Index.Text = index + "";

		}
		public void SetBackColor(Color color)
		{
			if (pl_Cmd.BackColor != color)
			{
				pl_Cmd.BackColor = color;
				this.Refresh();
			}
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
					lbl.Left = this.Width - 740;
				this.Controls.Add(lbl);
				Line(pl);
			}
		}
	}

}
