using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace ShimoExport
{
	using System.Collections.Generic;
	using System.Threading.Tasks;

	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();

			txtCookies.TextChanged += (sender, args) => btnExport.Enabled = txtCookies.TextLength > 0;
			btnExport.Click += BtnExport_Click;
			timerGetText.Start();

			Text = "石墨文档批量导出工具 by 木鱼 v3.7 Build 20220601";
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

		private Exporter _exporter;

		private async void BtnExport_Click(object sender, EventArgs e)
		{
			btnExport.Enabled = false;

			_exporter = new Exporter(txtCookies.Text) { IncludeShare = chkIncludeShare.Checked };
			_exporter.Message += (o, args) => Invoke(new Action(() => Log(args.Data)));
			//_exporter.NotSupportTypeFound += (o, args) => Invoke(new Action(() => OnNotSupportFound(args.Data1, args.Data2)));

			UpdateProgressAsync();
			await _exporter.ExportAsync();

			btnExport.Enabled = true;

			MessageBox.Show(this, $"导出完成。\n\n导出成功 {_exporter.Succeed} 个文档，失败 {_exporter.Failed} 个文档，跳过了 {_exporter.Skipped} 个文档。{(_exporter.Failed > 0 ? "\n\n失败的文档已经保存在下载的目录下的“!!!!未能下载的文档.txt”文件中。" : "")}", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
			if (_exporter.Succeed + _exporter.Skipped > 0) Process.Start("explorer.exe", $"\"{_exporter.Root}\"");

			_exporter = null;
		}

		async Task UpdateProgressAsync()
		{
			int? sleepTime = null;

			while (_exporter != null)
			{
				lblFailed.Text = _exporter.Failed.ToString("N0");
				lblSucc.Text = _exporter.Succeed.ToString("N0");
				lblSkipped.Text = _exporter.Skipped.ToString("N0");

				var st = _exporter.SleepTime;
				if (st == null)
				{

					pWait.Visible = false;
					sleepTime = null;
				}
				else
				{
					pWait.Visible = true;

					if (sleepTime == null)
					{
						sleepTime = st.Value;
						pgWait.Maximum = st.Value;
					}

					pgWait.Value = st.Value;
					lblWait.Text = (st.Value / 1000.0).ToString("N1") + "s";
				}

				await Task.Delay(100);
			}
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

		private HashSet<string> _notifiedTypes = new HashSet<string>();

		void OnNotSupportFound(string type, string subType)
		{
			if (_notifiedTypes.Contains(type + subType))
				return;

			_notifiedTypes.SafeAdd(type + subType);
			MessageBox.Show(this, $"暂不支持导出{type}/{subType}类型的文件，如需要支持，请联系木鱼。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}
	}
}
