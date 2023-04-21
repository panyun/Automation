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
			panel2 = new Panel();
			button1 = new Button();
			panel3 = new Panel();
			pl_key = new Panel();
			pl_components = new Panel();
			pl_Category = new Panel();
			button2 = new Button();
			panel4 = new Panel();
			button3 = new Button();
			textBox2 = new TextBox();
			txt_exp = new TextBox();
			panel3.SuspendLayout();
			panel4.SuspendLayout();
			SuspendLayout();
			// 
			// panel1
			// 
			panel1.BackColor = Color.FromArgb(46, 46, 46);
			panel1.Dock = DockStyle.Left;
			panel1.Location = new Point(0, 0);
			panel1.Name = "panel1";
			panel1.Size = new Size(56, 561);
			panel1.TabIndex = 1;
			// 
			// panel2
			// 
			panel2.BackColor = Color.FromArgb(236, 233, 231);
			panel2.Dock = DockStyle.Left;
			panel2.Location = new Point(56, 0);
			panel2.Name = "panel2";
			panel2.Size = new Size(200, 561);
			panel2.TabIndex = 2;
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
			panel3.Controls.Add(pl_key);
			panel3.Controls.Add(pl_components);
			panel3.Controls.Add(pl_Category);
			panel3.Controls.Add(button2);
			panel3.Controls.Add(panel4);
			panel3.Controls.Add(textBox2);
			panel3.Controls.Add(txt_exp);
			panel3.Dock = DockStyle.Fill;
			panel3.Location = new Point(256, 0);
			panel3.Name = "panel3";
			panel3.Size = new Size(628, 561);
			panel3.TabIndex = 5;
			// 
			// pl_key
			// 
			pl_key.BackColor = Color.Gainsboro;
			pl_key.Location = new Point(23, 405);
			pl_key.Name = "pl_key";
			pl_key.Size = new Size(513, 38);
			pl_key.TabIndex = 14;
			// 
			// pl_components
			// 
			pl_components.BorderStyle = BorderStyle.FixedSingle;
			pl_components.Location = new Point(23, 289);
			pl_components.Name = "pl_components";
			pl_components.Size = new Size(512, 47);
			pl_components.TabIndex = 12;
			// 
			// pl_Category
			// 
			pl_Category.BackColor = Color.Gainsboro;
			pl_Category.Location = new Point(20, 341);
			pl_Category.Name = "pl_Category";
			pl_Category.Size = new Size(596, 42);
			pl_Category.TabIndex = 11;
			// 
			// button2
			// 
			button2.BackColor = Color.FromArgb(233, 233, 233);
			button2.Location = new Point(541, 511);
			button2.Name = "button2";
			button2.Size = new Size(75, 23);
			button2.TabIndex = 10;
			button2.Text = "发送";
			button2.UseVisualStyleBackColor = false;
			// 
			// panel4
			// 
			panel4.BackColor = Color.WhiteSmoke;
			panel4.Controls.Add(button3);
			panel4.Dock = DockStyle.Top;
			panel4.Location = new Point(0, 0);
			panel4.Name = "panel4";
			panel4.Size = new Size(628, 45);
			panel4.TabIndex = 9;
			// 
			// button3
			// 
			button3.Location = new Point(524, 12);
			button3.Name = "button3";
			button3.Size = new Size(75, 23);
			button3.TabIndex = 0;
			button3.Text = "button3";
			button3.UseVisualStyleBackColor = true;
			// 
			// textBox2
			// 
			textBox2.BackColor = Color.WhiteSmoke;
			textBox2.BorderStyle = BorderStyle.FixedSingle;
			textBox2.Location = new Point(20, 51);
			textBox2.Multiline = true;
			textBox2.Name = "textBox2";
			textBox2.Size = new Size(596, 285);
			textBox2.TabIndex = 8;
			// 
			// txt_exp
			// 
			txt_exp.BackColor = Color.WhiteSmoke;
			txt_exp.BorderStyle = BorderStyle.FixedSingle;
			txt_exp.Location = new Point(20, 389);
			txt_exp.Multiline = true;
			txt_exp.Name = "txt_exp";
			txt_exp.Size = new Size(596, 116);
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
			panel3.ResumeLayout(false);
			panel3.PerformLayout();
			panel4.ResumeLayout(false);
			ResumeLayout(false);
		}

		#endregion
		private Panel panel1;
		private Panel panel2;
		private Button button1;
		private Panel panel3;
		private Button button2;
		private Panel panel4;
		private TextBox textBox2;
		private TextBox txt_exp;
		private Panel pl_Category;
		private Button button3;
		private Panel pl_components;
		private ComboBox lbl_key;
		private Panel pl_key;
	}
}