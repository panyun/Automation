namespace EL.ComandControl
{
    partial class Form1
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
            this.txt_Cmd = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.lbl_error = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txt_Cmd
            // 
            this.txt_Cmd.Location = new System.Drawing.Point(10, 8);
            this.txt_Cmd.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txt_Cmd.Multiline = true;
            this.txt_Cmd.Name = "txt_Cmd";
            this.txt_Cmd.Size = new System.Drawing.Size(440, 76);
            this.txt_Cmd.TabIndex = 0;
            this.txt_Cmd.Text = "打开 WeChat.exe";
            this.txt_Cmd.TextChanged += new System.EventHandler(this.txt_Cmd_TextChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(163, 88);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(119, 27);
            this.button1.TabIndex = 1;
            this.button1.Text = "执行";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // lbl_error
            // 
            this.lbl_error.AutoSize = true;
            this.lbl_error.Location = new System.Drawing.Point(10, 128);
            this.lbl_error.Name = "lbl_error";
            this.lbl_error.Size = new System.Drawing.Size(0, 12);
            this.lbl_error.TabIndex = 2;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(464, 160);
            this.Controls.Add(this.lbl_error);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txt_Cmd);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TextBox txt_Cmd;
        private Button button1;
        private Label lbl_error;
    }
}