namespace ShimoExport
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Net;
	using System.Text.RegularExpressions;
	using System.Threading.Tasks;

	using FSLib.Extension;
	using FSLib.Network.Http;

	using Newtonsoft.Json;

	internal class Exporter
	{
		private HttpClient _client;

		public Exporter(string cookies) { Cookies = cookies; }

		/// <summary>
		/// 导出根目录
		/// </summary>
		public static readonly string Root;

		static Exporter()
		{
			Root = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "已导出的文件");
		}

		/// <summary>
		/// Cookies
		/// </summary>
		public string Cookies { get; }

		public event EventHandler<GeneralEventArgs<string>> Message;

		void LogN(string txt, bool ts = true) => Log(txt, ts);

		void Log(string txt, bool ts = true)
		{
			if (ts)
				OnMessage($"[{DateTime.Now:HH:mm:ss}] {txt}");
			else OnMessage(txt);
		}

		void InitHttpClient()
		{
			_client = new HttpClient();
			_client.HttpHandler.BaseUri = new Uri("https://shimo.im/");
			_client.Setting.Accept = "application/vnd.shimo.v2+json";
			_client.Setting.Headers.Add("X-PUSH-CLIENT-ID", "fb7b36bf-ba12-4bf5-b7fa-7306513b2e17");
			_client.Setting.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/78.0.3882.0 Safari/537.36";
			_client.ImportCookies(Cookies, new Uri("https://shimo.im/"));
		}

		private List<File> rootData;

		/// <summary>
		/// 执行导出
		/// </summary>
		/// <returns></returns>
		public async Task ExportAsync()
		{
			InitHttpClient();

			LogN("开始导出...");
			await SyncContentAsync("", null);
			//记录数据
			System.IO.File.WriteAllText(Path.Combine(Root, "shimo.json"), JsonConvert.SerializeObject(rootData));

			LogN("导出操作已经全部完成");

		}

		/// <summary>
		/// 同步指定目录
		/// </summary>
		/// <param name="currentPath"></param>
		/// <param name="parent"></param>
		/// <returns></returns>
		private async Task SyncContentAsync(string currentPath, File parent)
		{
			var pid = parent?.Guid;
			LogN($"正在加载 石墨文档\\{parent?.Name} ...");

			try
			{
				var items = await LoadFilesAsync(pid);
				if (parent == null)
					rootData = items;
				else parent.SubItems = items;

				for (var i = 0; i < items.Count; i++)
				{
					var item = items[i];
					var subPath = Path.Combine(currentPath, item.Name);

					try
					{
						if (item.IsShortcut)
							continue; //跳过快捷方式
						if (item.Type == "folder")
						{
							await SyncContentAsync(subPath, item);
						}
						else
						{
							var type = item.Type;

							if (type == "board" || type == "form")
							{
								LogN($"警告：【{subPath}】类型为 {(type == "board" ? "白板" : type == "form" ? "表单" : "")}，不支持导出！");
								continue;
							}

							var ext = type.Contains("doc") ? "docx" : type.Contains("sheet") ? "xlsx" : type == "mindmap" ? "xmind" : throw new Exception("未知的文档类型，请联系木鱼: " + type);
							var targetPath = subPath + "." + ext;
							var phyTargetPath = Path.Combine(Root, targetPath);

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

							try
							{

								LogN($"正在导出 {targetPath}...");
								var data = await ExportAsync(item.Guid, item.Name, ext, false);
								Directory.CreateDirectory(Path.GetDirectoryName(phyTargetPath));
								System.IO.File.WriteAllBytes(phyTargetPath, data);
							}
							catch (NeedRetryException ex)
							{
								LogN($"操作过于频繁，需要等待一段时间后重试({ex.Timeout}毫秒)");
								await Task.Delay(ex.Timeout);
								i--;
								continue;
							}

							LogN("导出成功.");
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

		private async Task<byte[]> ExportAsync(string fileid, string name, string type, bool isAsync)
		{
			var downloadUrl = "";
			if (type == "xmind")
			{
				var mindCtx = _client.Create(HttpMethod.Get, $"/lizard-api/files/{fileid}?contentUrl=true", "desktop", result: new { contentUrl = "" });
				mindCtx.Send();

				if (!mindCtx.IsValid())
					throw new Exception("无法加载脑图地址信息，响应内容为：" + mindCtx.ResponseContent?.RawStringResult);

				downloadUrl = mindCtx.Result.contentUrl;
			}
			else
			{
				var url = $"/lizard-api/files/{fileid}/export?type={type}&file={fileid}&returnJson=1&name={System.Web.HttpUtility.UrlEncode(name)}";

				var ctx = _client.Create(HttpMethod.Get, url, $"docs/{fileid}", result: new { redirectUrl = "", data = new { taskId = "" }, status = 0, downloadUrl = "" });
				await ctx.SendAsync();
				if (!ctx.IsValid())
				{
					var message = ctx.ResponseContent?.RawStringResult;
					if (message.Contains("频繁"))
					{
						var timeout = Regex.Match(message, @"(\d+)\s*秒", RegexOptions.IgnoreCase).GetGroupValue(1).ToInt32();
						if (timeout == 0) timeout = 5000;
						else timeout *= 1000;

						throw new NeedRetryException(timeout);
					}
					throw new Exception("导出操作失败，响应内容为：" + ctx.ResponseContent?.RawStringResult);
				}

				if (!isAsync)
					downloadUrl = ctx.Result.redirectUrl;
				else
				{
					//检测进度
					while (true)
					{
						await Task.Delay(1000);

						var waitCtx = _client.Create(HttpMethod.Get,
							$"/lizard-api/files/{fileid}/export/progress?taskId={ctx.Result.data.taskId}",
							$"docs/{fileid}",
							result: new
							{
								data = new { tips = "", downloadUrl = "", progress = 0 },
								status = 0
							});
						await waitCtx.SendAsync();
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

			var downloadCtx = _client.Create<byte[]>(HttpMethod.Get, downloadUrl);
			await downloadCtx.SendAsync();

			if (!downloadCtx.IsValid())
			{
				throw new Exception($"文件数据下载失败，错误：{downloadCtx.GetExceptionMessage(downloadCtx.ResponseContent?.RawStringResult.DefaultForEmpty("无法下载文件"))}");
			}

			if (downloadCtx.Status == HttpStatusCode.Found)
			{
				downloadCtx = _client.Create<byte[]>(HttpMethod.Get, downloadCtx.WebResponse.GetResponseHeader("Location"));
				await downloadCtx.SendAsync();

				if (!downloadCtx.IsValid())
				{
					throw new Exception($"文件数据下载失败，错误：{downloadCtx.GetExceptionMessage(downloadCtx.ResponseContent?.RawStringResult.DefaultForEmpty("无法下载文件"))}");
				}
			}

			return downloadCtx.Result;
		}

		private async Task<List<File>> LoadFilesAsync(string pid)
		{
			var url = "lizard-api/files?collaboratorCount=true&contentUrl=true";
			if (!pid.IsNullOrEmpty())
			{
				url += "&folder=" + pid;
			}

			var ctx = _client.Create<List<File>>(HttpMethod.Get, url, "desktop");
			ctx.Send();

			if (!ctx.IsValid())
				throw new Exception("无法加载文件列表，响应内容为：" + ctx.ResponseContent?.RawStringResult);

			return ctx.Result;
		}

		protected virtual void OnMessage(string msg) => Message?.Invoke(this, new GeneralEventArgs<string>(msg));
	}
}
