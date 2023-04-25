using EL.Robot.Core;
using NPOI.SS.Formula.Functions;
using System.ComponentModel;

namespace EL.Robot.WindowApiTest
{
	public partial class RobotListView : UserControl
	{
		public DesignComponent designComponent = Boot.GetComponent<RobotComponent>().GetComponent<DesignComponent>();
		public RobotListView(long id, string name, string img)
		{
			InitializeComponent();
			this.Height = 65;
			lbl_Name.Text = name;
			lbl_Name.MouseLeave += (x, y) =>
			{
				this.BackColor = Color.FromArgb(236, 233, 231);
			};
			lbl_Name.MouseHover += (x, y) =>
			{
				this.BackColor = Color.Gainsboro;
			};
			pictureBox1.MouseLeave += (x, y) =>
			{
				this.BackColor = Color.FromArgb(236, 233, 231);
			};
			pictureBox1.MouseHover += (x, y) =>
			{
				this.BackColor = Color.Gainsboro;
			};
			this.Click += (x, y) =>
			{
			
				IndexForm.Ins.RefreshRobots((long)Tag);

			};
			pictureBox1.Click += (x, y) =>
			{
	 
				IndexForm.Ins.RefreshRobots((long)Tag);
			};
			lbl_Name.Click += (x, y) =>
			{
			 
				IndexForm.Ins.RefreshRobots((long)Tag);
			};

			this.Tag = id;
			this.MouseLeave += (x, y) =>
			{
				this.BackColor = Color.FromArgb(236, 233, 231);
			};
			this.MouseHover += (x, y) =>
			{
				this.BackColor = Color.Gainsboro;
			};
		}
		
	}

}
