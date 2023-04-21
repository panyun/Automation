namespace WinFromInspect
{
    partial class Inspect_form
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
            this.btn_ElementCatch = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.txt_ProgramType = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txt_ProcessName = new System.Windows.Forms.TextBox();
            this.txt_wh = new System.Windows.Forms.TextBox();
            this.txt_XY = new System.Windows.Forms.TextBox();
            this.txt_Path = new System.Windows.Forms.TextBox();
            this.txt_Value = new System.Windows.Forms.TextBox();
            this.txt_Name = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txt_Title = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.NativeWindowTitle = new System.Windows.Forms.Label();
            this.txt_NativeWindowTitle = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btn_Parser = new System.Windows.Forms.Button();
            this.cb_Click = new System.Windows.Forms.ComboBox();
            this.txt_Content = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_ElementCatch
            // 
            this.btn_ElementCatch.Location = new System.Drawing.Point(51, 12);
            this.btn_ElementCatch.Name = "btn_ElementCatch";
            this.btn_ElementCatch.Size = new System.Drawing.Size(137, 23);
            this.btn_ElementCatch.TabIndex = 0;
            this.btn_ElementCatch.Text = "鼠标捕捉";
            this.btn_ElementCatch.UseVisualStyleBackColor = true;
            this.btn_ElementCatch.Click += new System.EventHandler(this.btn_ElementCatch_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.tableLayoutPanel1.Controls.Add(this.txt_ProgramType, 1, 8);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 8);
            this.tableLayoutPanel1.Controls.Add(this.txt_ProcessName, 1, 6);
            this.tableLayoutPanel1.Controls.Add(this.txt_wh, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.txt_XY, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.txt_Path, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.txt_Value, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.txt_Name, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label12, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.label10, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.label8, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.label6, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.txt_Title, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.NativeWindowTitle, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this.txt_NativeWindowTitle, 1, 7);
            this.tableLayoutPanel1.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(408, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 9;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.474159F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.477044F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.477044F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20.09329F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.477044F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.477044F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.477044F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.5429F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.50443F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(487, 312);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // txt_ProgramType
            // 
            this.txt_ProgramType.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.txt_ProgramType.Location = new System.Drawing.Point(149, 286);
            this.txt_ProgramType.Name = "txt_ProgramType";
            this.txt_ProgramType.Size = new System.Drawing.Size(335, 23);
            this.txt_ProgramType.TabIndex = 24;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label3.Location = new System.Drawing.Point(3, 295);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(140, 17);
            this.label3.TabIndex = 23;
            this.label3.Text = "ProgramType:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txt_ProcessName
            // 
            this.txt_ProcessName.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.txt_ProcessName.Location = new System.Drawing.Point(149, 210);
            this.txt_ProcessName.Name = "txt_ProcessName";
            this.txt_ProcessName.Size = new System.Drawing.Size(335, 23);
            this.txt_ProcessName.TabIndex = 20;
            // 
            // txt_wh
            // 
            this.txt_wh.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.txt_wh.Location = new System.Drawing.Point(149, 181);
            this.txt_wh.Name = "txt_wh";
            this.txt_wh.Size = new System.Drawing.Size(335, 23);
            this.txt_wh.TabIndex = 19;
            // 
            // txt_XY
            // 
            this.txt_XY.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.txt_XY.Location = new System.Drawing.Point(149, 152);
            this.txt_XY.Name = "txt_XY";
            this.txt_XY.Size = new System.Drawing.Size(335, 23);
            this.txt_XY.TabIndex = 18;
            // 
            // txt_Path
            // 
            this.txt_Path.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.txt_Path.Location = new System.Drawing.Point(149, 90);
            this.txt_Path.Multiline = true;
            this.txt_Path.Name = "txt_Path";
            this.txt_Path.Size = new System.Drawing.Size(335, 56);
            this.txt_Path.TabIndex = 17;
            // 
            // txt_Value
            // 
            this.txt_Value.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.txt_Value.Location = new System.Drawing.Point(149, 61);
            this.txt_Value.Name = "txt_Value";
            this.txt_Value.Size = new System.Drawing.Size(335, 23);
            this.txt_Value.TabIndex = 16;
            // 
            // txt_Name
            // 
            this.txt_Name.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.txt_Name.Location = new System.Drawing.Point(149, 32);
            this.txt_Name.Name = "txt_Name";
            this.txt_Name.Size = new System.Drawing.Size(335, 23);
            this.txt_Name.TabIndex = 15;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label12.Location = new System.Drawing.Point(3, 219);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(140, 17);
            this.label12.TabIndex = 12;
            this.label12.Text = "ProcessName";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label10.Location = new System.Drawing.Point(3, 190);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(140, 17);
            this.label10.TabIndex = 10;
            this.label10.Text = "Width\\Height";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label8.Location = new System.Drawing.Point(3, 161);
            this.label8.Name = "label8";
            this.label8.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label8.Size = new System.Drawing.Size(140, 17);
            this.label8.TabIndex = 8;
            this.label8.Text = "X\\Y";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label6.Location = new System.Drawing.Point(3, 132);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(140, 17);
            this.label6.TabIndex = 6;
            this.label6.Text = "Path";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label4.Location = new System.Drawing.Point(3, 70);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(140, 17);
            this.label4.TabIndex = 4;
            this.label4.Text = "Value";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label2.Location = new System.Drawing.Point(3, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(140, 17);
            this.label2.TabIndex = 2;
            this.label2.Text = "Name";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txt_Title
            // 
            this.txt_Title.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.txt_Title.Location = new System.Drawing.Point(149, 3);
            this.txt_Title.Name = "txt_Title";
            this.txt_Title.Size = new System.Drawing.Size(335, 23);
            this.txt_Title.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label1.Location = new System.Drawing.Point(3, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(140, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "MainWindowTitle";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // NativeWindowTitle
            // 
            this.NativeWindowTitle.AutoSize = true;
            this.NativeWindowTitle.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.NativeWindowTitle.Location = new System.Drawing.Point(3, 255);
            this.NativeWindowTitle.Name = "NativeWindowTitle";
            this.NativeWindowTitle.Size = new System.Drawing.Size(140, 17);
            this.NativeWindowTitle.TabIndex = 21;
            this.NativeWindowTitle.Text = "NativeWindowTitle";
            this.NativeWindowTitle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txt_NativeWindowTitle
            // 
            this.txt_NativeWindowTitle.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.txt_NativeWindowTitle.Location = new System.Drawing.Point(149, 246);
            this.txt_NativeWindowTitle.Name = "txt_NativeWindowTitle";
            this.txt_NativeWindowTitle.Size = new System.Drawing.Size(335, 23);
            this.txt_NativeWindowTitle.TabIndex = 22;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(408, 318);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(487, 273);
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // btn_Parser
            // 
            this.btn_Parser.Location = new System.Drawing.Point(204, 12);
            this.btn_Parser.Name = "btn_Parser";
            this.btn_Parser.Size = new System.Drawing.Size(137, 23);
            this.btn_Parser.TabIndex = 3;
            this.btn_Parser.Text = "解析执行";
            this.btn_Parser.UseVisualStyleBackColor = true;
            this.btn_Parser.Click += new System.EventHandler(this.btn_Parser_Click);
            // 
            // cb_Click
            // 
            this.cb_Click.FormattingEnabled = true;
            this.cb_Click.Items.AddRange(new object[] {
            "单击",
            "双击",
            "右键点击"});
            this.cb_Click.Location = new System.Drawing.Point(51, 67);
            this.cb_Click.Name = "cb_Click";
            this.cb_Click.Size = new System.Drawing.Size(290, 25);
            this.cb_Click.TabIndex = 4;
            this.cb_Click.SelectedIndexChanged += new System.EventHandler(this.cb_Click_SelectedIndexChanged);
            // 
            // txt_Content
            // 
            this.txt_Content.Location = new System.Drawing.Point(54, 120);
            this.txt_Content.Multiline = true;
            this.txt_Content.Name = "txt_Content";
            this.txt_Content.Size = new System.Drawing.Size(287, 220);
            this.txt_Content.TabIndex = 5;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(50, 47);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 17);
            this.label5.TabIndex = 6;
            this.label5.Text = "点击类型";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(54, 100);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(56, 17);
            this.label7.TabIndex = 7;
            this.label7.Text = "输入内容";
            // 
            // Inspect_form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(898, 594);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txt_Content);
            this.Controls.Add(this.cb_Click);
            this.Controls.Add(this.btn_Parser);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.btn_ElementCatch);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(914, 633);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(914, 633);
            this.Name = "Inspect_form";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "探测器1.0Beat";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button btn_ElementCatch;
        private TableLayoutPanel tableLayoutPanel1;
        private TextBox txt_ProcessName;
        private TextBox txt_wh;
        private TextBox txt_XY;
        private TextBox txt_Path;
        private TextBox txt_Value;
        private TextBox txt_Name;
        private Label label12;
        private Label label10;
        private Label label8;
        private Label label6;
        private Label label4;
        private Label label2;
        private TextBox txt_Title;
        private Label label1;
        private PictureBox pictureBox1;
        private Label NativeWindowTitle;
        private TextBox txt_NativeWindowTitle;
        private TextBox txt_ProgramType;
        private Label label3;
        private Button btn_Parser;
        private ComboBox cb_Click;
        private TextBox txt_Content;
        private Label label5;
        private Label label7;
    }
}