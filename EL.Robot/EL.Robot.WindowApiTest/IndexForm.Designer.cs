namespace EL.Robot.WindowApiTest
{
	partial class IndexForm
	{
		/// <summary>
		///  Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		///  Clean up any resources being used.
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
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			panel1 = new Panel();
			button6 = new Button();
			button5 = new Button();
			panel2 = new Panel();
			btn_add = new Button();
			textBox1 = new TextBox();
			button1 = new Button();
			panel3 = new Panel();
			lbl_exec = new Button();
			pl_cmd = new Panel();
			pl_key = new Panel();
			pl_components = new Panel();
			pl_Category = new Panel();
			button2 = new Button();
			panel4 = new Panel();
			lbl_name = new Label();
			button4 = new Button();
			btn_run = new Button();
			button3 = new Button();
			txt_msg = new TextBox();
			txt_exp = new TextBox();
			panel1.SuspendLayout();
			panel2.SuspendLayout();
			panel3.SuspendLayout();
			panel4.SuspendLayout();
			SuspendLayout();
			// 
			// panel1
			// 
			panel1.BackColor = Color.FromArgb(46, 46, 46);
			panel1.Controls.Add(button6);
			panel1.Controls.Add(button5);
			panel1.Dock = DockStyle.Left;
			panel1.Location = new Point(0, 0);
			panel1.Name = "panel1";
			panel1.Size = new Size(66, 561);
			panel1.TabIndex = 1;
			// 
			// button6
			// 
			button6.Location = new Point(3, 117);
			button6.Name = "button6";
			button6.Size = new Size(60, 23);
			button6.TabIndex = 1;
			button6.Text = "市场";
			button6.UseVisualStyleBackColor = true;
			// 
			// button5
			// 
			button5.Location = new Point(0, 67);
			button5.Name = "button5";
			button5.Size = new Size(60, 23);
			button5.TabIndex = 0;
			button5.Text = "机器人";
			button5.UseVisualStyleBackColor = true;
			// 
			// panel2
			// 
			panel2.BackColor = Color.FromArgb(236, 233, 231);
			panel2.Controls.Add(btn_add);
			panel2.Controls.Add(textBox1);
			panel2.Dock = DockStyle.Left;
			panel2.Location = new Point(66, 0);
			panel2.Name = "panel2";
			panel2.Size = new Size(200, 561);
			panel2.TabIndex = 2;
			// 
			// btn_add
			// 
			btn_add.Location = new Point(139, 12);
			btn_add.Name = "btn_add";
			btn_add.Size = new Size(58, 23);
			btn_add.TabIndex = 1;
			btn_add.Text = "添加";
			btn_add.UseVisualStyleBackColor = true;
			btn_add.Click += btn_add_Click;
			// 
			// textBox1
			// 
			textBox1.Location = new Point(6, 12);
			textBox1.Name = "textBox1";
			textBox1.Size = new Size(127, 23);
			textBox1.TabIndex = 0;
			// 
			// button1
			// 
			button1.Location = new Point(760, 511);
			button1.Name = "button1";
			button1.Size = new Size(75, 23);
			button1.TabIndex = 4;
			button1.Text = "发送";
			button1.UseVisualStyleBackColor = true;
			// 
			// panel3
			// 
			panel3.BackColor = Color.WhiteSmoke;
			panel3.Controls.Add(lbl_exec);
			panel3.Controls.Add(pl_cmd);
			panel3.Controls.Add(pl_key);
			panel3.Controls.Add(pl_components);
			panel3.Controls.Add(pl_Category);
			panel3.Controls.Add(button2);
			panel3.Controls.Add(panel4);
			panel3.Controls.Add(txt_msg);
			panel3.Controls.Add(txt_exp);
			panel3.Dock = DockStyle.Fill;
			panel3.Location = new Point(266, 0);
			panel3.Name = "panel3";
			panel3.Size = new Size(618, 561);
			panel3.TabIndex = 5;
			// 
			// lbl_exec
			// 
			lbl_exec.BackColor = Color.FromArgb(233, 233, 233);
			lbl_exec.Location = new Point(451, 524);
			lbl_exec.Name = "lbl_exec";
			lbl_exec.Size = new Size(75, 23);
			lbl_exec.TabIndex = 16;
			lbl_exec.Text = "预览";
			lbl_exec.UseVisualStyleBackColor = false;
			lbl_exec.Click += lbl_exec_Click;
			// 
			// pl_cmd
			// 
			pl_cmd.BackColor = Color.Gainsboro;
			pl_cmd.Location = new Point(20, 371);
			pl_cmd.Name = "pl_cmd";
			pl_cmd.Size = new Size(596, 53);
			pl_cmd.TabIndex = 15;
			// 
			// pl_key
			// 
			pl_key.BackColor = Color.Gainsboro;
			pl_key.Location = new Point(23, 451);
			pl_key.Name = "pl_key";
			pl_key.Size = new Size(576, 38);
			pl_key.TabIndex = 14;
			// 
			// pl_components
			// 
			pl_components.BorderStyle = BorderStyle.FixedSingle;
			pl_components.Location = new Point(23, 259);
			pl_components.Name = "pl_components";
			pl_components.Size = new Size(576, 61);
			pl_components.TabIndex = 12;
			// 
			// pl_Category
			// 
			pl_Category.BackColor = Color.Gainsboro;
			pl_Category.Location = new Point(20, 326);
			pl_Category.Name = "pl_Category";
			pl_Category.Size = new Size(596, 39);
			pl_Category.TabIndex = 11;
			// 
			// button2
			// 
			button2.BackColor = Color.FromArgb(233, 233, 233);
			button2.Location = new Point(541, 524);
			button2.Name = "button2";
			button2.Size = new Size(75, 23);
			button2.TabIndex = 10;
			button2.Text = "发送";
			button2.UseVisualStyleBackColor = false;
			button2.Click += button2_Click;
			// 
			// panel4
			// 
			panel4.BackColor = Color.WhiteSmoke;
			panel4.Controls.Add(lbl_name);
			panel4.Controls.Add(button4);
			panel4.Controls.Add(btn_run);
			panel4.Controls.Add(button3);
			panel4.Dock = DockStyle.Top;
			panel4.Location = new Point(0, 0);
			panel4.Name = "panel4";
			panel4.Size = new Size(618, 45);
			panel4.TabIndex = 9;
			// 
			// lbl_name
			// 
			lbl_name.AutoSize = true;
			lbl_name.Font = new Font("隶书", 21.75F, FontStyle.Regular, GraphicsUnit.Point);
			lbl_name.Location = new Point(25, 12);
			lbl_name.Name = "lbl_name";
			lbl_name.Size = new Size(73, 29);
			lbl_name.TabIndex = 3;
			lbl_name.Text = "名字";
			// 
			// button4
			// 
			button4.Location = new Point(442, 12);
			button4.Name = "button4";
			button4.Size = new Size(75, 23);
			button4.TabIndex = 2;
			button4.Text = "调试";
			button4.UseVisualStyleBackColor = true;
			// 
			// btn_run
			// 
			btn_run.Location = new Point(354, 12);
			btn_run.Name = "btn_run";
			btn_run.Size = new Size(75, 23);
			btn_run.TabIndex = 1;
			btn_run.Text = "运行";
			btn_run.UseVisualStyleBackColor = true;
			btn_run.Click += btn_run_Click;
			// 
			// button3
			// 
			button3.Location = new Point(526, 12);
			button3.Name = "button3";
			button3.Size = new Size(75, 23);
			button3.TabIndex = 0;
			button3.Text = "关闭";
			button3.UseVisualStyleBackColor = true;
			// 
			// txt_msg
			// 
			txt_msg.BackColor = Color.WhiteSmoke;
			txt_msg.BorderStyle = BorderStyle.FixedSingle;
			txt_msg.Location = new Point(20, 51);
			txt_msg.Multiline = true;
			txt_msg.Name = "txt_msg";
			txt_msg.Size = new Size(596, 269);
			txt_msg.TabIndex = 8;
			// 
			// txt_exp
			// 
			txt_exp.BackColor = Color.WhiteSmoke;
			txt_exp.BorderStyle = BorderStyle.FixedSingle;
			txt_exp.Enabled = false;
			txt_exp.Location = new Point(20, 430);
			txt_exp.Multiline = true;
			txt_exp.Name = "txt_exp";
			txt_exp.Size = new Size(596, 85);
			txt_exp.TabIndex = 7;
			txt_exp.TextChanged += txt_exp_TextChanged;
			// 
			// IndexForm
			// 
			AutoScaleDimensions = new SizeF(7F, 17F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(884, 561);
			ControlBox = false;
			Controls.Add(panel3);
			Controls.Add(button1);
			Controls.Add(panel2);
			Controls.Add(panel1);
			FormBorderStyle = FormBorderStyle.None;
			Name = "IndexForm";
			ShowIcon = false;
			ShowInTaskbar = false;
			Text = "Form1";
			panel1.ResumeLayout(false);
			panel2.ResumeLayout(false);
			panel2.PerformLayout();
			panel3.ResumeLayout(false);
			panel3.PerformLayout();
			panel4.ResumeLayout(false);
			panel4.PerformLayout();
			ResumeLayout(false);
		}

		#endregion
		private Panel panel1;
		private Panel panel2;
		private Button button1;
		private Panel panel3;
		private Button button2;
		private Panel panel4;
		private TextBox txt_msg;
		private TextBox txt_exp;
		private Panel pl_Category;
		private Button button3;
		private Panel pl_components;
		private ComboBox lbl_key;
		private Panel pl_key;
		private Panel pl_cmd;
		private Button lbl_exec;
		private Button btn_add;
		private TextBox textBox1;
		private Button button4;
		private Button btn_run;
		private Button button6;
		private Button button5;
		private Label lbl_name;
	}
}