﻿namespace ShimoExport
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
			this.panel1 = new System.Windows.Forms.Panel();
			this.btnExport = new System.Windows.Forms.Button();
			this.txtLog = new System.Windows.Forms.TextBox();
			this.txtCookies = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.timerGetText = new System.Windows.Forms.Timer(this.components);
			this.lnkHelp = new System.Windows.Forms.LinkLabel();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.Color.White;
			this.panel1.Controls.Add(this.lnkHelp);
			this.panel1.Controls.Add(this.btnExport);
			this.panel1.Controls.Add(this.txtLog);
			this.panel1.Controls.Add(this.txtCookies);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Font = new System.Drawing.Font("微软雅黑", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(1215, 874);
			this.panel1.TabIndex = 0;
			// 
			// btnExport
			// 
			this.btnExport.Enabled = false;
			this.btnExport.Location = new System.Drawing.Point(414, 280);
			this.btnExport.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
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
			this.txtLog.Location = new System.Drawing.Point(0, 388);
			this.txtLog.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.txtLog.Multiline = true;
			this.txtLog.Name = "txtLog";
			this.txtLog.ReadOnly = true;
			this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtLog.Size = new System.Drawing.Size(1215, 486);
			this.txtLog.TabIndex = 4;
			// 
			// txtCookies
			// 
			this.txtCookies.Dock = System.Windows.Forms.DockStyle.Top;
			this.txtCookies.Location = new System.Drawing.Point(0, 118);
			this.txtCookies.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
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
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1215, 874);
			this.Controls.Add(this.panel1);
			this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "MainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "石墨文档批量导出工具 by 木鱼 v1.1 Build 20220531";
			this.TopMost = true;
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
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
	}
}

