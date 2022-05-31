namespace ShimoExport
{
	partial class MainForm
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

		#region Windows 窗体设计器生成的代码

		/// <summary>
		/// 设计器支持所需的方法 - 不要修改
		/// 使用代码编辑器修改此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.panel1 = new System.Windows.Forms.Panel();
			this.chkIncludeShare = new System.Windows.Forms.CheckBox();
			this.pWait = new System.Windows.Forms.Panel();
			this.lblWait = new System.Windows.Forms.Label();
			this.pgWait = new System.Windows.Forms.ProgressBar();
			this.lblSkipped = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.lblFailed = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.lblSucc = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.lnkHelp = new System.Windows.Forms.LinkLabel();
			this.btnExport = new System.Windows.Forms.Button();
			this.txtLog = new System.Windows.Forms.TextBox();
			this.txtCookies = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.timerGetText = new System.Windows.Forms.Timer(this.components);
			this.panel1.SuspendLayout();
			this.pWait.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.Color.White;
			this.panel1.Controls.Add(this.chkIncludeShare);
			this.panel1.Controls.Add(this.pWait);
			this.panel1.Controls.Add(this.lblSkipped);
			this.panel1.Controls.Add(this.label4);
			this.panel1.Controls.Add(this.lblFailed);
			this.panel1.Controls.Add(this.label3);
			this.panel1.Controls.Add(this.lblSucc);
			this.panel1.Controls.Add(this.label2);
			this.panel1.Controls.Add(this.lnkHelp);
			this.panel1.Controls.Add(this.btnExport);
			this.panel1.Controls.Add(this.txtLog);
			this.panel1.Controls.Add(this.txtCookies);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Font = new System.Drawing.Font("微软雅黑", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Margin = new System.Windows.Forms.Padding(4);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(1215, 874);
			this.panel1.TabIndex = 0;
			// 
			// chkIncludeShare
			// 
			this.chkIncludeShare.AutoSize = true;
			this.chkIncludeShare.Checked = true;
			this.chkIncludeShare.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkIncludeShare.Location = new System.Drawing.Point(795, 336);
			this.chkIncludeShare.Name = "chkIncludeShare";
			this.chkIncludeShare.Size = new System.Drawing.Size(293, 40);
			this.chkIncludeShare.TabIndex = 9;
			this.chkIncludeShare.Text = "包括别人共享给我的";
			this.chkIncludeShare.UseVisualStyleBackColor = true;
			// 
			// pWait
			// 
			this.pWait.Controls.Add(this.lblWait);
			this.pWait.Controls.Add(this.pgWait);
			this.pWait.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pWait.Location = new System.Drawing.Point(0, 459);
			this.pWait.Name = "pWait";
			this.pWait.Size = new System.Drawing.Size(1215, 46);
			this.pWait.TabIndex = 8;
			this.pWait.Visible = false;
			// 
			// lblWait
			// 
			this.lblWait.AutoSize = true;
			this.lblWait.Location = new System.Drawing.Point(1092, 6);
			this.lblWait.Name = "lblWait";
			this.lblWait.Size = new System.Drawing.Size(39, 36);
			this.lblWait.TabIndex = 1;
			this.lblWait.Text = "--";
			// 
			// pgWait
			// 
			this.pgWait.Location = new System.Drawing.Point(12, 7);
			this.pgWait.Name = "pgWait";
			this.pgWait.Size = new System.Drawing.Size(1074, 36);
			this.pgWait.TabIndex = 0;
			// 
			// lblSkipped
			// 
			this.lblSkipped.AutoSize = true;
			this.lblSkipped.ForeColor = System.Drawing.Color.DimGray;
			this.lblSkipped.Location = new System.Drawing.Point(814, 406);
			this.lblSkipped.Name = "lblSkipped";
			this.lblSkipped.Size = new System.Drawing.Size(39, 36);
			this.lblSkipped.TabIndex = 7;
			this.lblSkipped.Text = "--";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.ForeColor = System.Drawing.Color.DimGray;
			this.label4.Location = new System.Drawing.Point(691, 406);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(127, 36);
			this.label4.TabIndex = 7;
			this.label4.Text = "已跳过：";
			// 
			// lblFailed
			// 
			this.lblFailed.AutoSize = true;
			this.lblFailed.ForeColor = System.Drawing.Color.Crimson;
			this.lblFailed.Location = new System.Drawing.Point(473, 406);
			this.lblFailed.Name = "lblFailed";
			this.lblFailed.Size = new System.Drawing.Size(39, 36);
			this.lblFailed.TabIndex = 7;
			this.lblFailed.Text = "--";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.ForeColor = System.Drawing.Color.Crimson;
			this.label3.Location = new System.Drawing.Point(312, 406);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(155, 36);
			this.label3.TabIndex = 7;
			this.label3.Text = "导出失败：";
			// 
			// lblSucc
			// 
			this.lblSucc.AutoSize = true;
			this.lblSucc.ForeColor = System.Drawing.Color.SeaGreen;
			this.lblSucc.Location = new System.Drawing.Point(163, 406);
			this.lblSucc.Name = "lblSucc";
			this.lblSucc.Size = new System.Drawing.Size(39, 36);
			this.lblSucc.TabIndex = 7;
			this.lblSucc.Text = "--";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.ForeColor = System.Drawing.Color.SeaGreen;
			this.label2.Location = new System.Drawing.Point(12, 406);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(155, 36);
			this.label2.TabIndex = 7;
			this.label2.Text = "导出成功：";
			// 
			// lnkHelp
			// 
			this.lnkHelp.AutoSize = true;
			this.lnkHelp.Location = new System.Drawing.Point(3, 280);
			this.lnkHelp.Name = "lnkHelp";
			this.lnkHelp.Size = new System.Drawing.Size(127, 36);
			this.lnkHelp.TabIndex = 6;
			this.lnkHelp.TabStop = true;
			this.lnkHelp.Text = "使用说明";
			this.lnkHelp.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkHelp_LinkClicked);
			// 
			// btnExport
			// 
			this.btnExport.Enabled = false;
			this.btnExport.Location = new System.Drawing.Point(414, 280);
			this.btnExport.Margin = new System.Windows.Forms.Padding(4);
			this.btnExport.Name = "btnExport";
			this.btnExport.Size = new System.Drawing.Size(362, 96);
			this.btnExport.TabIndex = 5;
			this.btnExport.Text = "开始导出";
			this.btnExport.UseVisualStyleBackColor = true;
			// 
			// txtLog
			// 
			this.txtLog.BackColor = System.Drawing.Color.White;
			this.txtLog.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.txtLog.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.txtLog.Location = new System.Drawing.Point(0, 505);
			this.txtLog.Margin = new System.Windows.Forms.Padding(4);
			this.txtLog.Multiline = true;
			this.txtLog.Name = "txtLog";
			this.txtLog.ReadOnly = true;
			this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtLog.Size = new System.Drawing.Size(1215, 369);
			this.txtLog.TabIndex = 4;
			// 
			// txtCookies
			// 
			this.txtCookies.Dock = System.Windows.Forms.DockStyle.Top;
			this.txtCookies.Location = new System.Drawing.Point(0, 118);
			this.txtCookies.Margin = new System.Windows.Forms.Padding(4);
			this.txtCookies.Multiline = true;
			this.txtCookies.Name = "txtCookies";
			this.txtCookies.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtCookies.Size = new System.Drawing.Size(1215, 151);
			this.txtCookies.TabIndex = 2;
			this.txtCookies.Text = "shimo_sid=";
			// 
			// label1
			// 
			this.label1.Dock = System.Windows.Forms.DockStyle.Top;
			this.label1.ForeColor = System.Drawing.Color.ForestGreen;
			this.label1.Location = new System.Drawing.Point(0, 0);
			this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(1215, 118);
			this.label1.TabIndex = 3;
			this.label1.Text = "请使用Chrome浏览器访问石墨，并按照视频教程复制相关的Cookies信息到下框中。实在不会用且石墨文档数量很多想一键导出，或使用中遇到问题，请联系作者木鱼，企" +
	"微见 💕";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// timerGetText
			// 
			this.timerGetText.Interval = 500;
			this.timerGetText.Tick += new System.EventHandler(this.TimerGetText_Tick);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1215, 874);
			this.Controls.Add(this.panel1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Margin = new System.Windows.Forms.Padding(4);
			this.MaximizeBox = false;
			this.Name = "MainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.pWait.ResumeLayout(false);
			this.pWait.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button btnExport;
		private System.Windows.Forms.TextBox txtLog;
		private System.Windows.Forms.TextBox txtCookies;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Timer timerGetText;
		private System.Windows.Forms.LinkLabel lnkHelp;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label lblSucc;
		private System.Windows.Forms.Label lblSkipped;
		private System.Windows.Forms.Label lblFailed;
		private System.Windows.Forms.Panel pWait;
		private System.Windows.Forms.ProgressBar pgWait;
		private System.Windows.Forms.Label lblWait;
		private System.Windows.Forms.CheckBox chkIncludeShare;
	}
}

