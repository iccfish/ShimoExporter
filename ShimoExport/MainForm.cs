using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace ShimoExport
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();

			txtCookies.TextChanged += (sender, args) => btnExport.Enabled = txtCookies.TextLength > 0;
			btnExport.Click += BtnExport_Click;
			timerGetText.Start();
		}

		void Log(string txt)
		{
			txt += "\r\n";
			Invoke(new Action(() =>
			{
				txtLog.AppendText(txt);
				txtLog.SelectionStart = txtLog.TextLength;
				txtLog.ScrollToCaret();
			}));
		}

		private async void BtnExport_Click(object sender, EventArgs e)
		{
			btnExport.Enabled = false;

			var worker = new Exporter(txtCookies.Text);
			worker.Message += (o, args) => Invoke(new Action(() => Log(args.Data)));

			await worker.ExportAsync();
			btnExport.Enabled = true;
			Process.Start("explorer.exe", $"\"{Exporter.Root}\"");
		}

		private void TimerGetText_Tick(object sender, EventArgs e)
		{
			try
			{
				if (!Clipboard.ContainsText())
					return;

				var txt = Clipboard.GetText();
				if (txt.Contains("shimo_sid"))
				{
					txtCookies.Text = txt;
					btnExport.Enabled = true;
					Log("已检测到复制的登录信息，请点击“开始导出”。");
					timerGetText.Stop();
				}
			}
			catch (Exception ex)
			{

			}
		}

		private void lnkHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("https://oss.secon.cn/soft/shimo_exporter/usage.jpg");
		}
	}
}
