namespace EL.Robot.WindowApiTest
{
	partial class DesignFlowRowViewForm
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
			lbl_Index = new Label();
			label1 = new Label();
			pl_Cmd = new Panel();
			pic_block = new PictureBox();
			lbl_DisplayCmd = new Label();
			pictureBox1 = new PictureBox();
			pl_Cmd.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)pic_block).BeginInit();
			((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
			SuspendLayout();
			// 
			// lbl_Index
			// 
			lbl_Index.Dock = DockStyle.Left;
			lbl_Index.Location = new Point(0, 4);
			lbl_Index.Name = "lbl_Index";
			lbl_Index.RightToLeft = RightToLeft.Yes;
			lbl_Index.Size = new Size(34, 25);
			lbl_Index.TabIndex = 0;
			lbl_Index.Text = "1";
			lbl_Index.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// label1
			// 
			label1.BorderStyle = BorderStyle.Fixed3D;
			label1.Location = new Point(39, -7);
			label1.Name = "label1";
			label1.Size = new Size(10, 50);
			label1.TabIndex = 1;
			label1.Text = "label1";
			// 
			// pl_Cmd
			// 
			pl_Cmd.BackColor = Color.LightSteelBlue;
			pl_Cmd.Controls.Add(pic_block);
			pl_Cmd.Controls.Add(lbl_DisplayCmd);
			pl_Cmd.Controls.Add(pictureBox1);
			pl_Cmd.Dock = DockStyle.Right;
			pl_Cmd.Location = new Point(67, 4);
			pl_Cmd.Margin = new Padding(0);
			pl_Cmd.Name = "pl_Cmd";
			pl_Cmd.Size = new Size(733, 25);
			pl_Cmd.TabIndex = 2;
			// 
			// pic_block
			// 
			pic_block.Image = Properties.Resources.Hide;
			pic_block.Location = new Point(3, 2);
			pic_block.Name = "pic_block";
			pic_block.Size = new Size(17, 19);
			pic_block.SizeMode = PictureBoxSizeMode.StretchImage;
			pic_block.TabIndex = 3;
			pic_block.TabStop = false;
			// 
			// lbl_DisplayCmd
			// 
			lbl_DisplayCmd.AutoSize = true;
			lbl_DisplayCmd.Font = new Font("Microsoft YaHei UI", 10.5F, FontStyle.Regular, GraphicsUnit.Point);
			lbl_DisplayCmd.ForeColor = Color.Black;
			lbl_DisplayCmd.Location = new Point(55, 1);
			lbl_DisplayCmd.Margin = new Padding(0);
			lbl_DisplayCmd.Name = "lbl_DisplayCmd";
			lbl_DisplayCmd.Size = new Size(121, 20);
			lbl_DisplayCmd.TabIndex = 2;
			lbl_DisplayCmd.Text = "这是一个鼠标点击";
			// 
			// pictureBox1
			// 
			pictureBox1.BackColor = Color.Gold;
			pictureBox1.Location = new Point(26, 3);
			pictureBox1.Name = "pictureBox1";
			pictureBox1.Size = new Size(26, 17);
			pictureBox1.TabIndex = 0;
			pictureBox1.TabStop = false;
			// 
			// DesignRowViewForm
			// 
			AutoScaleDimensions = new SizeF(7F, 17F);
			AutoScaleMode = AutoScaleMode.Font;
			Controls.Add(pl_Cmd);
			Controls.Add(label1);
			Controls.Add(lbl_Index);
			Margin = new Padding(0);
			Name = "DesignRowViewForm";
			Padding = new Padding(0, 4, 0, 4);
			Size = new Size(800, 33);
			pl_Cmd.ResumeLayout(false);
			pl_Cmd.PerformLayout();
			((System.ComponentModel.ISupportInitialize)pic_block).EndInit();
			((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
			ResumeLayout(false);
		}

		#endregion

		private Label lbl_Index;
		private Label label1;
		private Panel pl_Cmd;
		private Label lbl_DisplayCmd;
		private PictureBox pictureBox1;
		private PictureBox pic_block;
	}
}
