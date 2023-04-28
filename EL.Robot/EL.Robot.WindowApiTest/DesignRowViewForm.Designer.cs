namespace EL.Robot.WindowApiTest.bin
{
    partial class DesignRowViewForm
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
            this.lbl_Index = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.pl_Cmd = new System.Windows.Forms.Panel();
            this.lbl_DisplayCmd = new System.Windows.Forms.Label();
            this.lbl_Name = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pic_block = new System.Windows.Forms.PictureBox();
            this.pl_Cmd.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_block)).BeginInit();
            this.SuspendLayout();
            // 
            // lbl_Index
            // 
            this.lbl_Index.AutoSize = true;
            this.lbl_Index.Location = new System.Drawing.Point(6, 20);
            this.lbl_Index.Name = "lbl_Index";
            this.lbl_Index.Size = new System.Drawing.Size(15, 17);
            this.lbl_Index.TabIndex = 0;
            this.lbl_Index.Text = "1";
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label1.Location = new System.Drawing.Point(32, -7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(10, 77);
            this.label1.TabIndex = 1;
            this.label1.Text = "label1";
            // 
            // pl_Cmd
            // 
            this.pl_Cmd.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.pl_Cmd.BackColor = System.Drawing.Color.LightSteelBlue;
            this.pl_Cmd.Controls.Add(this.pic_block);
            this.pl_Cmd.Controls.Add(this.lbl_DisplayCmd);
            this.pl_Cmd.Controls.Add(this.lbl_Name);
            this.pl_Cmd.Controls.Add(this.pictureBox1);
            this.pl_Cmd.Location = new System.Drawing.Point(57, 7);
            this.pl_Cmd.Margin = new System.Windows.Forms.Padding(10);
            this.pl_Cmd.Name = "pl_Cmd";
            this.pl_Cmd.Size = new System.Drawing.Size(543, 44);
            this.pl_Cmd.TabIndex = 2;
            // 
            // lbl_DisplayCmd
            // 
            this.lbl_DisplayCmd.AutoSize = true;
            this.lbl_DisplayCmd.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lbl_DisplayCmd.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.lbl_DisplayCmd.Location = new System.Drawing.Point(102, 22);
            this.lbl_DisplayCmd.Name = "lbl_DisplayCmd";
            this.lbl_DisplayCmd.Size = new System.Drawing.Size(121, 20);
            this.lbl_DisplayCmd.TabIndex = 2;
            this.lbl_DisplayCmd.Text = "这是一个鼠标点击";
            // 
            // lbl_Name
            // 
            this.lbl_Name.AutoSize = true;
            this.lbl_Name.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lbl_Name.Location = new System.Drawing.Point(101, 1);
            this.lbl_Name.Name = "lbl_Name";
            this.lbl_Name.Size = new System.Drawing.Size(74, 21);
            this.lbl_Name.TabIndex = 1;
            this.lbl_Name.Text = "鼠标点击";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Gold;
            this.pictureBox1.Location = new System.Drawing.Point(37, 4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(48, 37);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // pic_block
            // 
            this.pic_block.Image = global::EL.Robot.WindowApiTest.Properties.Resources.Hide;
            this.pic_block.Location = new System.Drawing.Point(4, 11);
            this.pic_block.Name = "pic_block";
            this.pic_block.Size = new System.Drawing.Size(28, 21);
            this.pic_block.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pic_block.TabIndex = 3;
            this.pic_block.TabStop = false;
            // 
            // DesignRowViewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pl_Cmd);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lbl_Index);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "DesignRowViewForm";
            this.Size = new System.Drawing.Size(600, 59);
            this.pl_Cmd.ResumeLayout(false);
            this.pl_Cmd.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_block)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label lbl_Index;
        private Label label1;
        private Panel pl_Cmd;
        private Label lbl_DisplayCmd;
        private Label lbl_Name;
        private PictureBox pictureBox1;
        private PictureBox pic_block;
    }
}
