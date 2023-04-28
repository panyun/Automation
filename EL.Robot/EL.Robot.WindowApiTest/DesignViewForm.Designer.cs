namespace EL.Robot.WindowApiTest
{
    partial class DesignViewForm
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
            this.pl_Content = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // pl_Content
            // 
            this.pl_Content.AutoScroll = true;
            this.pl_Content.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pl_Content.Location = new System.Drawing.Point(0, 0);
            this.pl_Content.Name = "pl_Content";
            this.pl_Content.Size = new System.Drawing.Size(600, 400);
            this.pl_Content.TabIndex = 0;
            // 
            // DesignViewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pl_Content);
            this.Name = "DesignViewForm";
            this.Size = new System.Drawing.Size(600, 400);
            this.ResumeLayout(false);

        }

        #endregion

        private FlowLayoutPanel pl_Content;
    }
}
