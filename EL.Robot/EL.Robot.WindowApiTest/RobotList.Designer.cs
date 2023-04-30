namespace EL.Robot.WindowApiTest
{
	partial class RobotListView
	{
		/// <summary> 
		/// 必需的设计器变量。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// 清理所有正在使用的资源。
		/// </summary>
		/// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region 组件设计器生成的代码

		/// <summary> 
		/// 设计器支持所需的方法 - 不要修改
		/// 使用代码编辑器修改此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{
			pictureBox1 = new PictureBox();
			lbl_Name = new Label();
			((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
			SuspendLayout();
			// 
			// pictureBox1
			// 
			pictureBox1.BackColor = SystemColors.Info;
			pictureBox1.Dock = DockStyle.Left;
			pictureBox1.Location = new Point(0, 0);
			pictureBox1.Name = "pictureBox1";
			pictureBox1.Size = new Size(65, 70);
			pictureBox1.TabIndex = 0;
			pictureBox1.TabStop = false;
			// 
			// lbl_Name
			// 
			lbl_Name.AutoSize = true;
			lbl_Name.Font = new Font("Microsoft YaHei UI", 10.5F, FontStyle.Regular, GraphicsUnit.Point);
			lbl_Name.Location = new Point(78, 4);
			lbl_Name.Name = "lbl_Name";
			lbl_Name.Size = new Size(50, 20);
			lbl_Name.TabIndex = 1;
			lbl_Name.Text = "label1";
			// 
			// RobotListView
			// 
			AutoScaleDimensions = new SizeF(7F, 17F);
			AutoScaleMode = AutoScaleMode.Font;
			Controls.Add(lbl_Name);
			Controls.Add(pictureBox1);
			Name = "RobotListView";
			Size = new Size(200, 70);
			((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private PictureBox pictureBox1;
		private Label lbl_Name;
	}
}
