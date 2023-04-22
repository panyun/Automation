using EL.Robot.Component;
using EL.Robot.Core;

namespace EL.Robot.WindowApiTest
{
	public partial class AddRobotForm : Form
	{
		public AddRobotForm()
		{
			InitializeComponent();
			this.FormBorderStyle = FormBorderStyle.None;
			this.StartPosition = FormStartPosition.CenterScreen;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private async void button2_Click(object sender, EventArgs e)
		{
			Flow flow = new Flow();
			flow.Name = textBox1.Text.Trim();
			flow.HeadImage = null;
			var design = Boot.GetComponent<RobotComponent>().GetComponent<DesignComponent>();
			design.CreateRobot(flow);
			this.DialogResult = DialogResult.OK;
			this.Close();

			//add
		}
	}
}
