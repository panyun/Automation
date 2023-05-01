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
			components = new System.ComponentModel.Container();
			pl_bottom = new Panel();
			notifyIcon1 = new NotifyIcon(components);
			pl_winTop = new Panel();
			btn_close = new Button();
			pl_content = new Panel();
			panel3 = new Panel();
			panel5 = new Panel();
			btn_save = new Button();
			pl_components = new Panel();
			btn_debug = new Button();
			txt_exp = new RichTextBox();
			btn_run = new Button();
			lbl_preExec = new Button();
			pl_cmd = new Panel();
			pl_Category = new Panel();
			button3 = new Button();
			button2 = new Button();
			btn_send = new Button();
			pl_view = new Panel();
			panel4 = new Panel();
			lbl_name = new Label();
			button1 = new Button();
			panel2 = new Panel();
			flp_robotList = new FlowLayoutPanel();
			panel1 = new Panel();
			btn_add = new Button();
			textBox1 = new TextBox();
			panel6 = new Panel();
			button9 = new Button();
			button8 = new Button();
			button7 = new Button();
			button6 = new Button();
			button5 = new Button();
			pl_winTop.SuspendLayout();
			pl_content.SuspendLayout();
			panel3.SuspendLayout();
			pl_Category.SuspendLayout();
			panel4.SuspendLayout();
			panel2.SuspendLayout();
			panel1.SuspendLayout();
			panel6.SuspendLayout();
			SuspendLayout();
			// 
			// pl_bottom
			// 
			pl_bottom.Dock = DockStyle.Bottom;
			pl_bottom.Location = new Point(0, 653);
			pl_bottom.Name = "pl_bottom";
			pl_bottom.Size = new Size(1105, 152);
			pl_bottom.TabIndex = 1;
			// 
			// notifyIcon1
			// 
			notifyIcon1.Text = "notifyIcon1";
			notifyIcon1.Visible = true;
			// 
			// pl_winTop
			// 
			pl_winTop.Controls.Add(btn_close);
			pl_winTop.Dock = DockStyle.Top;
			pl_winTop.Location = new Point(0, 0);
			pl_winTop.Name = "pl_winTop";
			pl_winTop.Size = new Size(1105, 26);
			pl_winTop.TabIndex = 2;
			// 
			// btn_close
			// 
			btn_close.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			btn_close.Location = new Point(1063, -3);
			btn_close.Name = "btn_close";
			btn_close.Size = new Size(42, 23);
			btn_close.TabIndex = 0;
			btn_close.Text = "关闭";
			btn_close.UseVisualStyleBackColor = true;
			// 
			// pl_content
			// 
			pl_content.Controls.Add(panel3);
			pl_content.Controls.Add(button1);
			pl_content.Controls.Add(panel2);
			pl_content.Controls.Add(panel6);
			pl_content.Dock = DockStyle.Fill;
			pl_content.Location = new Point(0, 26);
			pl_content.Name = "pl_content";
			pl_content.Size = new Size(1105, 627);
			pl_content.TabIndex = 3;
			// 
			// panel3
			// 
			panel3.BackColor = Color.WhiteSmoke;
			panel3.Controls.Add(panel5);
			panel3.Controls.Add(btn_save);
			panel3.Controls.Add(pl_components);
			panel3.Controls.Add(btn_debug);
			panel3.Controls.Add(txt_exp);
			panel3.Controls.Add(btn_run);
			panel3.Controls.Add(lbl_preExec);
			panel3.Controls.Add(pl_cmd);
			panel3.Controls.Add(pl_Category);
			panel3.Controls.Add(btn_send);
			panel3.Controls.Add(pl_view);
			panel3.Controls.Add(panel4);
			panel3.Dock = DockStyle.Fill;
			panel3.Location = new Point(281, 0);
			panel3.Name = "panel3";
			panel3.Size = new Size(824, 627);
			panel3.TabIndex = 9;
			// 
			// panel5
			// 
			panel5.BackColor = Color.WhiteSmoke;
			panel5.Dock = DockStyle.Bottom;
			panel5.Location = new Point(0, 617);
			panel5.Name = "panel5";
			panel5.Size = new Size(824, 10);
			panel5.TabIndex = 18;
			// 
			// btn_save
			// 
			btn_save.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
			btn_save.Location = new Point(182, 592);
			btn_save.Name = "btn_save";
			btn_save.Size = new Size(75, 23);
			btn_save.TabIndex = 4;
			btn_save.Text = "刷新";
			btn_save.UseVisualStyleBackColor = true;
			// 
			// pl_components
			// 
			pl_components.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			pl_components.BorderStyle = BorderStyle.FixedSingle;
			pl_components.Location = new Point(20, 358);
			pl_components.Name = "pl_components";
			pl_components.Size = new Size(782, 61);
			pl_components.TabIndex = 12;
			// 
			// btn_debug
			// 
			btn_debug.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
			btn_debug.Location = new Point(101, 590);
			btn_debug.Name = "btn_debug";
			btn_debug.Size = new Size(75, 23);
			btn_debug.TabIndex = 2;
			btn_debug.Text = "调试";
			btn_debug.UseVisualStyleBackColor = true;
			// 
			// txt_exp
			// 
			txt_exp.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			txt_exp.Location = new Point(20, 508);
			txt_exp.Name = "txt_exp";
			txt_exp.ReadOnly = true;
			txt_exp.Size = new Size(782, 81);
			txt_exp.TabIndex = 17;
			txt_exp.Text = "";
			// 
			// btn_run
			// 
			btn_run.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
			btn_run.Location = new Point(20, 590);
			btn_run.Name = "btn_run";
			btn_run.Size = new Size(75, 23);
			btn_run.TabIndex = 1;
			btn_run.Text = "运行";
			btn_run.UseVisualStyleBackColor = true;
			// 
			// lbl_preExec
			// 
			lbl_preExec.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			lbl_preExec.BackColor = Color.FromArgb(233, 233, 233);
			lbl_preExec.Location = new Point(637, 593);
			lbl_preExec.Name = "lbl_preExec";
			lbl_preExec.Size = new Size(75, 23);
			lbl_preExec.TabIndex = 16;
			lbl_preExec.Text = "预览";
			lbl_preExec.UseVisualStyleBackColor = false;
			// 
			// pl_cmd
			// 
			pl_cmd.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			pl_cmd.BackColor = Color.Gainsboro;
			pl_cmd.Location = new Point(20, 466);
			pl_cmd.Name = "pl_cmd";
			pl_cmd.Size = new Size(782, 39);
			pl_cmd.TabIndex = 15;
			// 
			// pl_Category
			// 
			pl_Category.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			pl_Category.BackColor = Color.Gainsboro;
			pl_Category.Controls.Add(button3);
			pl_Category.Controls.Add(button2);
			pl_Category.Location = new Point(20, 425);
			pl_Category.Name = "pl_Category";
			pl_Category.Size = new Size(782, 39);
			pl_Category.TabIndex = 11;
			// 
			// button3
			// 
			button3.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			button3.Location = new Point(689, 9);
			button3.Name = "button3";
			button3.Size = new Size(42, 23);
			button3.TabIndex = 1;
			button3.Text = "计划";
			button3.UseVisualStyleBackColor = true;
			// 
			// button2
			// 
			button2.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			button2.Location = new Point(737, 10);
			button2.Name = "button2";
			button2.Size = new Size(42, 23);
			button2.TabIndex = 0;
			button2.Text = "历史";
			button2.UseVisualStyleBackColor = true;
			// 
			// btn_send
			// 
			btn_send.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			btn_send.BackColor = Color.FromArgb(233, 233, 233);
			btn_send.Location = new Point(727, 593);
			btn_send.Name = "btn_send";
			btn_send.Size = new Size(75, 23);
			btn_send.TabIndex = 10;
			btn_send.Text = "发送";
			btn_send.UseVisualStyleBackColor = false;
			// 
			// pl_view
			// 
			pl_view.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			pl_view.Location = new Point(0, 45);
			pl_view.Margin = new Padding(0);
			pl_view.Name = "pl_view";
			pl_view.Size = new Size(824, 374);
			pl_view.TabIndex = 21;
			// 
			// panel4
			// 
			panel4.BackColor = Color.WhiteSmoke;
			panel4.Controls.Add(lbl_name);
			panel4.Dock = DockStyle.Top;
			panel4.Location = new Point(0, 0);
			panel4.Name = "panel4";
			panel4.Size = new Size(824, 45);
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
			// button1
			// 
			button1.Location = new Point(760, 488);
			button1.Name = "button1";
			button1.Size = new Size(75, 23);
			button1.TabIndex = 8;
			button1.Text = "发送";
			button1.UseVisualStyleBackColor = true;
			// 
			// panel2
			// 
			panel2.BackColor = Color.FromArgb(236, 233, 231);
			panel2.Controls.Add(flp_robotList);
			panel2.Controls.Add(panel1);
			panel2.Dock = DockStyle.Left;
			panel2.Location = new Point(80, 0);
			panel2.Name = "panel2";
			panel2.Size = new Size(201, 627);
			panel2.TabIndex = 7;
			// 
			// flp_robotList
			// 
			flp_robotList.Dock = DockStyle.Fill;
			flp_robotList.Location = new Point(0, 45);
			flp_robotList.Name = "flp_robotList";
			flp_robotList.Size = new Size(201, 582);
			flp_robotList.TabIndex = 2;
			// 
			// panel1
			// 
			panel1.Controls.Add(btn_add);
			panel1.Controls.Add(textBox1);
			panel1.Dock = DockStyle.Top;
			panel1.Location = new Point(0, 0);
			panel1.Name = "panel1";
			panel1.Size = new Size(201, 45);
			panel1.TabIndex = 3;
			// 
			// btn_add
			// 
			btn_add.Location = new Point(138, 15);
			btn_add.Name = "btn_add";
			btn_add.Size = new Size(58, 23);
			btn_add.TabIndex = 1;
			btn_add.Text = "添加";
			btn_add.UseVisualStyleBackColor = true;
			// 
			// textBox1
			// 
			textBox1.Location = new Point(5, 15);
			textBox1.Name = "textBox1";
			textBox1.Size = new Size(127, 23);
			textBox1.TabIndex = 0;
			// 
			// panel6
			// 
			panel6.BackColor = Color.FromArgb(46, 46, 46);
			panel6.Controls.Add(button9);
			panel6.Controls.Add(button8);
			panel6.Controls.Add(button7);
			panel6.Controls.Add(button6);
			panel6.Controls.Add(button5);
			panel6.Dock = DockStyle.Left;
			panel6.Location = new Point(0, 0);
			panel6.Name = "panel6";
			panel6.Size = new Size(80, 627);
			panel6.TabIndex = 6;
			// 
			// button9
			// 
			button9.Location = new Point(3, 585);
			button9.Name = "button9";
			button9.Size = new Size(60, 23);
			button9.TabIndex = 4;
			button9.Text = "配置";
			button9.UseVisualStyleBackColor = true;
			// 
			// button8
			// 
			button8.Location = new Point(5, 130);
			button8.Name = "button8";
			button8.Size = new Size(60, 23);
			button8.TabIndex = 3;
			button8.Text = "教程";
			button8.UseVisualStyleBackColor = true;
			// 
			// button7
			// 
			button7.Location = new Point(4, 90);
			button7.Name = "button7";
			button7.Size = new Size(60, 23);
			button7.TabIndex = 2;
			button7.Text = "设备";
			button7.UseVisualStyleBackColor = true;
			// 
			// button6
			// 
			button6.Location = new Point(3, 50);
			button6.Name = "button6";
			button6.Size = new Size(60, 23);
			button6.TabIndex = 1;
			button6.Text = "市场";
			button6.UseVisualStyleBackColor = true;
			// 
			// button5
			// 
			button5.Location = new Point(3, 15);
			button5.Name = "button5";
			button5.Size = new Size(60, 23);
			button5.TabIndex = 0;
			button5.Text = "机器人";
			button5.UseVisualStyleBackColor = true;
			// 
			// IndexForm
			// 
			AutoScaleDimensions = new SizeF(7F, 17F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(1105, 805);
			ControlBox = false;
			Controls.Add(pl_content);
			Controls.Add(pl_winTop);
			Controls.Add(pl_bottom);
			FormBorderStyle = FormBorderStyle.None;
			Name = "IndexForm";
			ShowIcon = false;
			ShowInTaskbar = false;
			Text = "Form1";
			pl_winTop.ResumeLayout(false);
			pl_content.ResumeLayout(false);
			panel3.ResumeLayout(false);
			pl_Category.ResumeLayout(false);
			panel4.ResumeLayout(false);
			panel4.PerformLayout();
			panel2.ResumeLayout(false);
			panel1.ResumeLayout(false);
			panel1.PerformLayout();
			panel6.ResumeLayout(false);
			ResumeLayout(false);
		}

		#endregion
		private ComboBox lbl_key;
		private Panel pl_bottom;
		private NotifyIcon notifyIcon1;
		private Panel pl_winTop;
		private Button btn_close;
		private Panel pl_content;
		private Panel panel3;
		private Panel panel5;
		private Button btn_save;
		private Panel pl_components;
		private Button btn_debug;
		private RichTextBox txt_exp;
		private Button btn_run;
		private Button lbl_preExec;
		private Panel pl_cmd;
		private Panel pl_Category;
		private Button button3;
		private Button button2;
		private Button btn_send;
		private Panel pl_view;
		private Panel panel4;
		private Label lbl_name;
		private Button button1;
		private Panel panel2;
		private FlowLayoutPanel flp_robotList;
		private Button btn_add;
		private TextBox textBox1;
		private Panel panel6;
		private Button button9;
		private Button button8;
		private Button button7;
		private Button button6;
		private Button button5;
		private Panel panel1;
	}
}