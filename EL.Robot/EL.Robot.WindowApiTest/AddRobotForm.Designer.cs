namespace EL.Robot.WindowApiTest
{
	partial class AddRobotForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			button1 = new Button();
			button2 = new Button();
			label1 = new Label();
			textBox1 = new TextBox();
			label2 = new Label();
			pictureBox1 = new PictureBox();
			button3 = new Button();
			((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
			SuspendLayout();
			// 
			// button1
			// 
			button1.Location = new Point(141, 143);
			button1.Name = "button1";
			button1.Size = new Size(75, 23);
			button1.TabIndex = 0;
			button1.Text = "取消";
			button1.UseVisualStyleBackColor = true;
			button1.Click += button1_Click;
			// 
			// button2
			// 
			button2.Location = new Point(231, 143);
			button2.Name = "button2";
			button2.Size = new Size(75, 23);
			button2.TabIndex = 1;
			button2.Text = "确定";
			button2.UseVisualStyleBackColor = true;
			button2.Click += button2_Click;
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new Point(43, 25);
			label1.Name = "label1";
			label1.Size = new Size(80, 17);
			label1.TabIndex = 2;
			label1.Text = "机器人名字：";
			// 
			// textBox1
			// 
			textBox1.Location = new Point(141, 22);
			textBox1.Name = "textBox1";
			textBox1.Size = new Size(165, 23);
			textBox1.TabIndex = 3;
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new Point(43, 84);
			label2.Name = "label2";
			label2.Size = new Size(80, 17);
			label2.TabIndex = 4;
			label2.Text = "机器人头像：";
			// 
			// pictureBox1
			// 
			pictureBox1.Location = new Point(141, 64);
			pictureBox1.Name = "pictureBox1";
			pictureBox1.Size = new Size(77, 49);
			pictureBox1.TabIndex = 5;
			pictureBox1.TabStop = false;
			// 
			// button3
			// 
			button3.Location = new Point(231, 78);
			button3.Name = "button3";
			button3.Size = new Size(75, 23);
			button3.TabIndex = 6;
			button3.Text = "上传";
			button3.UseVisualStyleBackColor = true;
			// 
			// AddRobotForm
			// 
			AutoScaleDimensions = new SizeF(7F, 17F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(328, 178);
			Controls.Add(button3);
			Controls.Add(pictureBox1);
			Controls.Add(label2);
			Controls.Add(textBox1);
			Controls.Add(label1);
			Controls.Add(button2);
			Controls.Add(button1);
			FormBorderStyle = FormBorderStyle.None;
			Name = "AddRobotForm";
			Text = "AddRobotForm";
			((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private Button button1;
		private Button button2;
		private Label label1;
		private TextBox textBox1;
		private Label label2;
		private PictureBox pictureBox1;
		private Button button3;
	}
}