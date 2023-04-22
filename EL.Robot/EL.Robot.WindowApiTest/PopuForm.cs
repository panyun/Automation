using EL.Robot.Component;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EL.Robot.WindowApiTest
{
	public partial class PopuForm : Form
	{
		public string Value { get; set; }
		public Control Control { get; set; }
		private Font font = new Font("隶书", 12);
		public Config CurrentConfig;
		public PopuForm(ValueActionType actionType, Point point, List<string> strings = default)
		{
			InitializeComponent();
			this.Width = 250;
			this.Height = 150;
			this.StartPosition = FormStartPosition.Manual;
			this.Location = point;
			if (actionType == ValueActionType.Input)
			{
				Control = new TextBox()
				{
					Multiline = true,
					Height = 110,
					Font = font,
					Dock = DockStyle.Top,
				};
				this.Controls.Add(Control);
			}
			if (actionType == ValueActionType.RequestList)
			{
				if (strings == null || strings.Count == 0)
				{
					this.Controls.Add(new Label()
					{
						Text = "当前未加载到匹配参数",
						Font = font,
						Width = 200,
						Location = new Point(45, 60)
					});
				}
				else
				{
					Control = new ComboBox()
					{
						Dock = DockStyle.Top,
						Location = new Point(20, 20),
						Font = font,
						Height = 30,
						Width = 200
					};
					var cb = Control as ComboBox;
					foreach (var item in strings)
					{
						cb.Items.Add(item);
					}
					cb.SelectedIndex = 0;
					this.Controls.Add(cb);
				}
			}
			var btn = new Button()
			{
				Location = new Point(110, 120),
				Width = 80,
				Height = 20,
				Text = "确定"

			};
			this.Controls.Add(btn);
			btn.Click += (x, y) =>
			{
				this.DialogResult = DialogResult.OK;
				if (Control != null)
					this.Value = Control.Text.Trim();
			};
		}
	}
}
