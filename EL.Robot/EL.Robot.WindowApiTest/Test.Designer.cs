namespace EL.Robot.WindowApiTest
{
	partial class Test
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
			btn_catch = new Button();
			button1 = new Button();
			button2 = new Button();
			button3 = new Button();
			SuspendLayout();
			// 
			// btn_catch
			// 
			btn_catch.Location = new Point(39, 49);
			btn_catch.Name = "btn_catch";
			btn_catch.Size = new Size(75, 23);
			btn_catch.TabIndex = 0;
			btn_catch.Text = "捕获";
			btn_catch.UseVisualStyleBackColor = true;
			btn_catch.Click += btn_catch_Click;
			// 
			// button1
			// 
			button1.Location = new Point(154, 47);
			button1.Margin = new Padding(2, 2, 2, 2);
			button1.Name = "button1";
			button1.Size = new Size(75, 25);
			button1.TabIndex = 1;
			button1.Text = "执行流程";
			button1.UseVisualStyleBackColor = true;
			button1.Click += button1_Click;
			// 
			// button2
			// 
			button2.Location = new Point(280, 49);
			button2.Margin = new Padding(2, 2, 2, 2);
			button2.Name = "button2";
			button2.Size = new Size(75, 25);
			button2.TabIndex = 2;
			button2.Text = "执行组件";
			button2.UseVisualStyleBackColor = true;
			button2.Click += button2_Click;
			// 
			// button3
			// 
			button3.Location = new Point(404, 48);
			button3.Margin = new Padding(2, 2, 2, 2);
			button3.Name = "button3";
			button3.Size = new Size(75, 25);
			button3.TabIndex = 3;
			button3.Text = "模拟执行";
			button3.UseVisualStyleBackColor = true;
			button3.Click += button3_Click;
			// 
			// Test
			// 
			AutoScaleDimensions = new SizeF(7F, 17F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(800, 450);
			Controls.Add(button3);
			Controls.Add(button2);
			Controls.Add(button1);
			Controls.Add(btn_catch);
			Name = "Test";
			Text = "Test";
			ResumeLayout(false);
		}

		#endregion

		private Button btn_catch;
		private Button button1;
		private Button button2;
		private Button button3;
	}
}