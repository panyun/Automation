namespace EL.Robot.WindowApiTest
{
	partial class LogsViewForm
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
			tabControl1 = new TabControl();
			tb_FlowRunLog = new TabPage();
			rtb_rowLogs = new RichTextBox();
			tb_FlowDesignLog = new TabPage();
			rtb_designLogs = new RichTextBox();
			tb_ParamterInfo = new TabPage();
			lv_paramters = new ListView();
			tabControl1.SuspendLayout();
			tb_FlowRunLog.SuspendLayout();
			tb_FlowDesignLog.SuspendLayout();
			tb_ParamterInfo.SuspendLayout();
			SuspendLayout();
			// 
			// tabControl1
			// 
			tabControl1.Controls.Add(tb_FlowRunLog);
			tabControl1.Controls.Add(tb_FlowDesignLog);
			tabControl1.Controls.Add(tb_ParamterInfo);
			tabControl1.Dock = DockStyle.Fill;
			tabControl1.Location = new Point(0, 0);
			tabControl1.Name = "tabControl1";
			tabControl1.SelectedIndex = 0;
			tabControl1.Size = new Size(983, 175);
			tabControl1.TabIndex = 4;
			// 
			// tb_FlowRunLog
			// 
			tb_FlowRunLog.Controls.Add(rtb_rowLogs);
			tb_FlowRunLog.Location = new Point(4, 26);
			tb_FlowRunLog.Name = "tb_FlowRunLog";
			tb_FlowRunLog.Padding = new Padding(3);
			tb_FlowRunLog.Size = new Size(975, 145);
			tb_FlowRunLog.TabIndex = 0;
			tb_FlowRunLog.Text = "运行日记";
			tb_FlowRunLog.UseVisualStyleBackColor = true;
			// 
			// rtb_rowLogs
			// 
			rtb_rowLogs.Dock = DockStyle.Fill;
			rtb_rowLogs.Location = new Point(3, 3);
			rtb_rowLogs.Name = "rtb_rowLogs";
			rtb_rowLogs.Size = new Size(969, 139);
			rtb_rowLogs.TabIndex = 1;
			rtb_rowLogs.Text = "";
			// 
			// tb_FlowDesignLog
			// 
			tb_FlowDesignLog.Controls.Add(rtb_designLogs);
			tb_FlowDesignLog.Location = new Point(4, 26);
			tb_FlowDesignLog.Name = "tb_FlowDesignLog";
			tb_FlowDesignLog.Padding = new Padding(3);
			tb_FlowDesignLog.Size = new Size(975, 145);
			tb_FlowDesignLog.TabIndex = 1;
			tb_FlowDesignLog.Text = "操作记录";
			tb_FlowDesignLog.UseVisualStyleBackColor = true;
			// 
			// rtb_designLogs
			// 
			rtb_designLogs.Dock = DockStyle.Fill;
			rtb_designLogs.Location = new Point(3, 3);
			rtb_designLogs.Name = "rtb_designLogs";
			rtb_designLogs.Size = new Size(969, 139);
			rtb_designLogs.TabIndex = 2;
			rtb_designLogs.Text = "";
			// 
			// tb_ParamterInfo
			// 
			tb_ParamterInfo.Controls.Add(lv_paramters);
			tb_ParamterInfo.Location = new Point(4, 26);
			tb_ParamterInfo.Name = "tb_ParamterInfo";
			tb_ParamterInfo.Size = new Size(975, 145);
			tb_ParamterInfo.TabIndex = 2;
			tb_ParamterInfo.Text = "参数信息";
			tb_ParamterInfo.UseVisualStyleBackColor = true;
			// 
			// lv_paramters
			// 
			lv_paramters.Dock = DockStyle.Fill;
			lv_paramters.Location = new Point(0, 0);
			lv_paramters.Name = "lv_paramters";
			lv_paramters.Size = new Size(975, 145);
			lv_paramters.TabIndex = 0;
			lv_paramters.UseCompatibleStateImageBehavior = false;
			// 
			// LogsViewForm
			// 
			AutoScaleDimensions = new SizeF(7F, 17F);
			AutoScaleMode = AutoScaleMode.Font;
			Controls.Add(tabControl1);
			Name = "LogsViewForm";
			Size = new Size(983, 175);
			tabControl1.ResumeLayout(false);
			tb_FlowRunLog.ResumeLayout(false);
			tb_FlowDesignLog.ResumeLayout(false);
			tb_ParamterInfo.ResumeLayout(false);
			ResumeLayout(false);
		}

		#endregion

		private TabControl tabControl1;
		private TabPage tb_FlowRunLog;
		private RichTextBox rtb_rowLogs;
		private TabPage tb_FlowDesignLog;
		private RichTextBox rtb_designLogs;
		private TabPage tb_ParamterInfo;
		private ListView lv_paramters;
	}
}
