using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ShimoExport
{
	using System.Diagnostics;
	using System.IO;
	using System.Threading;
	using System.Threading.Tasks;

	using FSLib.Network.Http;

	using Newtonsoft.Json;

	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();

			txtCookies.TextChanged += (sender, args) => btnExport.Enabled = txtCookies.TextLength > 0;
			btnExport.Click += BtnExport_Click;
			timerGetText.Start();
		}

		void Log(string txt, bool ts = true)
		{
			Invoke(new Action(() =>
			{
				if (ts)
					txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {txt}");
				else txtLog.AppendText(txt);
				txtLog.SelectionStart = txtLog.TextLength;
				txtLog.ScrollToCaret();
			}));
		}

		void LogN(string txt, bool ts = true) => Log(txt + "\r\n", ts);

		private void BtnExport_Click(object sender, EventArgs e)
		{
			btnExport.Enabled = false;
			Task.Factory.StartNew(Run);
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
					LogN("已检测到复制的登录信息，请点击“开始导出”。");
					timerGetText.Stop();
				}
			}
			catch (Exception ex)
			{
				
			}
		}

		void Run()
		{
			LogN("开始导出...");

			var root = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "已导出的文件");
			var cookies = txtCookies.Text;

			var client = new HttpClient();
			client.HttpHandler.BaseUri = new Uri("https://shimo.im/");
			client.Setting.Accept = "application/vnd.shimo.v2+json";
			client.Setting.Headers.Add("X-PUSH-CLIENT-ID", "fb7b36bf-ba12-4bf5-b7fa-7306513b2e17");
			client.Setting.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/78.0.3882.0 Safari/537.36";


			client.ImportCookies(cookies, new Uri("https://shimo.im/"));

			List<File> LoadFiles(string pid)
			{
				var url = "lizard-api/files?collaboratorCount=true&contentUrl=true";
				if (!pid.IsNullOrEmpty())
				{
					url += "&folder=" + pid;
				}
				var ctx = client.Create<List<File>>(HttpMethod.Get, url, "desktop");
				ctx.Send();

				if (!ctx.IsValid())
					throw new Exception("无法加载文件列表，响应内容为：" + ctx.ResponseContent?.RawStringResult);

				return ctx.Result;
			}

			byte[] Export(string fileid, string name, string type, bool isAsync)
			{
				var downloadUrl = "";
				if (type == "xmind")
				{
					var mindCtx = client.Create(HttpMethod.Get, $"/lizard-api/files/{fileid}?contentUrl=true", "desktop", result: new { contentUrl = "" });
					mindCtx.Send();

					if (!mindCtx.IsValid())
						throw new Exception("无法加载脑图地址信息，响应内容为：" + mindCtx.ResponseContent?.RawStringResult);

					downloadUrl = mindCtx.Result.contentUrl;
				}
				else
				{
					var url = $"/lizard-api/files/{fileid}/export?type={type}&file={fileid}&returnJson=1&name={System.Web.HttpUtility.UrlEncode(name)}";

					var ctx = client.Create(HttpMethod.Get, url, $"docs/{fileid}", result: new { redirectUrl = "", data = new { taskId = "" }, status = 0 });
					ctx.Send();
					if (!ctx.IsValid())
						throw new Exception("导出操作失败，响应内容为：" + ctx.ResponseContent?.RawStringResult);

					if (!isAsync)
						downloadUrl = ctx.Result.redirectUrl;
					else
					{
						//检测进度
						while (true)
						{
							Thread.Sleep(1000);

							var waitCtx = client.Create(HttpMethod.Get, $"/lizard-api/files/{fileid}/export/progress?taskId={ctx.Result.data.taskId}", $"docs/{fileid}", result: new
							{
								data = new { tips = "", downloadUrl = "", progress = 0 },
								status = 0
							});
							waitCtx.Send();
							if (!waitCtx.IsValid())
								throw new Exception("导出进度查询失败，响应内容为：" + waitCtx.ResponseContent?.RawStringResult);

							if (waitCtx.Result.data.progress == 100)
							{
								downloadUrl = waitCtx.Result.data.downloadUrl;
								break;
							}
						}
					}
				}

				try
				{
					return client.GetDataResult(downloadUrl);
				}
				catch (Exception ex)
				{
					throw new Exception($"文件数据下载失败，错误：{ex.Message}");
				}
			}

			List<File> rootData = null;
			void Sync(string currentPath, File parent)
			{
				var pid = parent?.Guid;
				LogN($"正在加载 石墨文档\\{parent?.Name} ...");

				try
				{
					var items = LoadFiles(pid);
					if (parent == null)
						rootData = items;
					else parent.SubItems = items;
					foreach (var item in items)
					{
						var subPath = Path.Combine(currentPath, item.Name);

						try
						{
							//if (item.IsShortcut)
							//	continue; //跳过快捷方式
							if (item.Type == "folder")
							{
								Sync(subPath, item);
							}
							else
							{

								var type = item.Type;

								if (type == "board" || type == "form")
								{
									LogN($"警告：【{subPath}】类型为 {(type == "board" ? "白板" : type == "form" ? "表单" : "")}，不支持导出！");
									continue;
								}

								var ext = type.Contains("doc") ? "docx" : type.Contains("sheet") ? "xlsx" : type == "mindmap" ? "xmind" : throw new Exception("Unknown type: " + type);
								var targetPath = subPath + "." + ext;
								var phyTargetPath = Path.Combine(root, targetPath);

								if (System.IO.File.Exists(phyTargetPath))
								{
									var fileInfo = new FileInfo(phyTargetPath);

									if (fileInfo.CreationTime < item.UpdatedAt)
									{
										LogN($"文件 {targetPath} 已存在，但是已过期，需要更新");
									}
									else
									{
										LogN($"文件 {targetPath} 已存在且为最新，将跳过");
										continue;
									}
								}

								Log($"正在导出 {targetPath}...");
								var data = Export(item.Guid, item.Name, ext, false);
								Directory.CreateDirectory(Path.GetDirectoryName(phyTargetPath));
								System.IO.File.WriteAllBytes(phyTargetPath, data);
								LogN("导出成功.", false);
							}
						}
						catch (Exception ex)
						{
							LogN($"导出文件 {item.Name} 失败：{ex.Message}");
						}
					}
				}
				catch (Exception ex)
				{
					LogN($"导出目录 {parent?.Name ?? "根目录"} 失败：{ex.Message}");
				}
			}
			Sync("", null);

			System.IO.File.WriteAllText(Path.Combine(root, "shimo.json"), JsonConvert.SerializeObject(rootData));

			LogN("导出操作已经全部完成");

			this.Invoke(() => { btnExport.Enabled = true; });
			Process.Start("explorer.exe", $"\"{root}\"");
		}
	}

	class File
	{
		public DateTime CreatedAt { get; set; }
		public string Guid { get; set; }
		public int Id { get; set; }
		public string InviteCode { get; set; }
		public bool IsFolder { get; set; }
		public string Type { get; set; }
		public string Name { get; set; }
		public string Url { get; set; }
		public User User { get; set; }
		public List<File> SubItems { get; set; }
		public DateTime UpdatedAt { get; set; }
		public bool IsShortcut { get; set; }
		public string ContentUrl { get; set; }
	}

	class User
	{
		public string Avatar { get; set; }
		public string Email { get; set; }
		public string Name { get; set; }
		public int Id { get; set; }
	}
}
