namespace ShimoExport
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
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
        ///     导出根目录
        /// </summary>
        public string Root { get; private set; }

        /// <summary>
        ///     Cookies
        /// </summary>
        public string Cookies { get; }

        public int Succeed { get; private set; }

        public int Failed { get; private set; }

        public int Skipped { get; private set; }

        public int? SleepTime { get; private set; }

        public bool IncludeShare { get; set; }

        public long Uid { get; set; }

        public string UserName { get; set; }

        public List<string> FailedList { get; private set; }

        public List<File> DataList { get; private set; }

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
            _client                     = new HttpClient();
            _client.HttpHandler.BaseUri = new Uri("https://shimo.im/");
            _client.Setting.Accept      = "application/vnd.shimo.v2+json";
            _client.Setting.Headers.Add("X-PUSH-CLIENT-ID", "fb7b36bf-ba12-4bf5-b7fa-7306513b2e17");
            _client.Setting.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/78.0.3882.0 Safari/537.36";
            _client.ImportCookies(Cookies, new Uri("https://shimo.im/"));
        }

        /// <summary>
        ///     执行导出
        /// </summary>
        /// <returns></returns>
        public async Task ExportAsync()
        {
            Succeed    = 0;
            Failed     = 0;
            Skipped    = 0;
            SleepTime  = null;
            FailedList = new List<string>();

            InitHttpClient();

            LogN("验证身份...");
            await CheckUserAsync();
            if (Uid == 0)
                return;

            LogN($"正在为 {UserName}(UID={Uid}) 导出...");
            Root = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), UserName);
            Directory.CreateDirectory(Root);

            LogN("开始导出空间...");
            var spaces = await ListSpacesAsync();
            foreach (var space in spaces)
            {
                await SyncContentAsync(space.Name, space);
            }

            LogN("开始导出桌面...");
            await SyncContentAsync("", null);

            if (FailedList.Count > 0)
            {
                System.IO.File.WriteAllLines(Path.Combine(Root, "!!!!未能下载的文档.txt"), FailedList);
            }

            System.IO.File.AppendAllText(Path.Combine(Root, "shimo.json"), JsonConvert.SerializeObject(DataList));

            LogN("正在更新文档日期信息...");
            await Task.Factory.StartNew(() => UpdateDirectoryInfo(null));

            LogN("导出操作已经全部完成");
        }

        void UpdateDirectoryInfo(string path)
        {
            path = path ?? Root;

            var dirs = Directory.GetDirectories(path);
            dirs.ForEach(UpdateDirectoryInfo);

            var minTime    = Directory.GetFiles(path).Select(s => (DateTime?)(new FileInfo(s).LastWriteTime)).Min();
            var minDirTime = Directory.GetDirectories(path).Select(s => (DateTime?)(new DirectoryInfo(s).LastWriteTime)).Min();
            var time       = new[] { minTime, minDirTime }.Min();

            new DirectoryInfo(path).LastWriteTime = time.Value;
        }

        async Task CheckUserAsync()
        {
            Uid      = 0L;
            UserName = null;

            var ctx = _client.Create(HttpMethod.Get, "lizard-api/users/me", "desktop", result: new { id = 0L, name = "" });
            await ctx.SendAsync();

            if (ctx.IsValid())
            {
                Uid      = ctx.Result.id;
                UserName = ctx.Result.name;
            }

            LogN(ctx.GetExceptionMessage("登录信息错误"));
        }

        /// <summary>
        /// </summary>
        public event EventHandler<GeneralEventArgs<string, string>> NotSupportTypeFound;

        /// <summary>
        ///     引发 <see cref="NotSupportTypeFound" /> 事件
        /// </summary>
        protected virtual void OnNotSupportTypeFound(string type, string name)
        {
            NotSupportTypeFound?.Invoke(this, new GeneralEventArgs<string, string>(type, name));
        }


        /// <summary>
        ///     同步指定目录
        /// </summary>
        /// <param name="currentPath"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        private async Task SyncContentAsync(string currentPath, File parent)
        {
            var pid = parent?.IsShortcut == true ? parent.ShortcutSource.Guid : parent?.Guid;
            LogN($"正在加载 石墨文档\\{parent?.Name} (GUID={pid}) ...");

            try
            {
                var items = await LoadFilesAsync(pid);
                if (parent == null)
                {
                    (DataList ?? (DataList = new List<File>())).AddRange(items);
                }
                else
                {
                    parent.SubItems.AddRange(items);
                }

                for (var i = 0; i < items.Count; i++)
                {
                    var item        = items[i];
                    var subPath     = Path.Combine(currentPath, IOUtility.RemoveInvalidFileNameChars(item.Name));
                    var itemType    = item.Type;
                    var itemSubType = item.SubType;
                    var guid        = item.Guid;

                    try
                    {
                        if (item.IsShortcut)
                        {
                            (itemType, itemSubType) = item.ShortcutSource.GetOriginalType();
                            guid                    = item.ShortcutSource.Guid;
                        }

                        if (itemType == "folder" || itemType == "space")
                        {
                            await SyncContentAsync(subPath, item);
                        }
                        else
                        {
                            var ext = "";

                            if (itemType == "board" || itemType == "form")
                            {
                                Failed++;
                                FailedList.Add($"{parent?.Name} (GUID={pid}) 白板或表单无法导出");
                                OnNotSupportTypeFound(itemType, itemType == "board" ? "白板" : "表单");
                                continue;
                            }

                            if (itemType == "presentation" || itemType == "slide")
                            {
                                ext = "pptx";
                            }
                            else if (itemType == "mosheet" || itemType == "sheet" || itemType == "table" || itemType == "spreadsheet")
                            {
                                ext = "xlsx";
                            }
                            else if (itemType == "newdoc" || itemType == "modoc")
                            {
                                ext = "docx";
                            }
                            else if (itemType == "mindmap")
                            {
                                ext = "xmind";
                            }
                            else if (!item.SubType.IsNullOrEmpty())
                            {
                                ext = item.SubType;
                            }
                            else
                            {
                                Failed++;
                                FailedList.Add($"{parent?.Name} (GUID={pid})");
                                OnNotSupportTypeFound(itemType, itemSubType.DefaultForEmpty("未知"));
                                continue;
                            }

                            var targetPath    = subPath.EndsWith(ext, StringComparison.OrdinalIgnoreCase) ? subPath : subPath + "." + ext;
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
                                    fileInfo.LastWriteTime = item.UpdatedAt;
                                    Skipped++;
                                    LogN($"文件 {targetPath} 已存在且为最新，将跳过");
                                    continue;
                                }
                            }

                            if (item.UserId != Uid && !IncludeShare)
                            {
                                LogN($"文件 {targetPath} 的原作者是 {item.User?.Name}(UID={item.UserId})，跳过下载");
                                Skipped++;
                                continue;
                            }

                            try
                            {
                                LogN($"正在导出 {targetPath}...");
                                var data = await ExportAsync(guid, item.Name, itemType, itemSubType, ext);
                                Directory.CreateDirectory(Path.GetDirectoryName(phyTargetPath));
                                System.IO.File.WriteAllBytes(phyTargetPath, data);

                                var fi = new FileInfo(phyTargetPath);
                                fi.LastWriteTime = item.UpdatedAt;
                            }
                            catch (NeedRetryException ex)
                            {
                                LogN($"石墨导出限制，需要等待一段时间后重试({ex.Timeout}毫秒)");

                                SleepTime = ex.Timeout;
                                while (SleepTime > 0)
                                {
                                    await Task.Delay(100);
                                    SleepTime -= 100;
                                }

                                SleepTime = null;
                                i--;
                                continue;
                            }

                            LogN("导出成功.");
                            Succeed++;
                        }
                    }
                    catch (Exception ex)
                    {
                        Failed++;
                        FailedList.Add($"{parent?.Name} (GUID={pid}) => {ex.Message}");
                        LogN($"导出文件 {item.Name} 失败：{ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Failed++;
                FailedList.Add($"{parent?.Name} (GUID={pid}) => {ex.Message}");
                LogN($"导出目录 {parent?.Name ?? "根目录"} 失败：{ex.Message}");
            }
        }

        async Task<List<File>> ListSpacesAsync()
        {
            var result = new List<File>();
            var urls   = new[] { "panda-api/file/spaces?orderBy=updatedAt", "panda-api/file/pinned_spaces" };

            foreach (var url in urls)
            {
                var ctx = _client.Create(HttpMethod.Get, url, "space", result: new { spaces = new List<Space>() });
                await ctx.SendAsync();

                if (!ctx.IsValid())
                    throw new Exception("无法加载空间列表，响应内容为：" + ctx.ResponseContent?.RawStringResult);

                if (ctx.Result.spaces != null)
                    result.AddRange(ctx.Result.spaces.Select(s => new File
                    {
                        Name      = s.Name,
                        Guid      = s.Guid,
                        UpdatedAt = DateTimeEx.FromJsTicks(s.UpdatedAt)
                    }));
            }

            return result;
        }

        private async Task<byte[]> ExportAsync(string fileid, string name, string type, string subType, string ext)
        {
            var downloadUrl = "";

            if (!subType.IsNullOrEmpty())
            {
                downloadUrl = $"/lizard-api/files/{fileid}/download";
            }
            else
            {
                var url = $"/lizard-api/files/{fileid}/export?type={ext}&file={fileid}&returnJson=1&name={System.Web.HttpUtility.UrlEncode(name)}&isAsync=0&timezoneOffset=-8";

                var ctx = _client.Create(HttpMethod.Get, url, $"docs/{fileid}", result: new { redirectUrl = "", data = new { taskId = "" }, status = 0, downloadUrl = "" });
                await ctx.SendAsync();
                if (!ctx.IsValid())
                {
                    var message = ctx.ResponseContent?.RawStringResult;
                    if (message.Contains("频繁"))
                    {
                        var timeout = Regex.Match(message, @"(\d+)\s*秒", RegexOptions.IgnoreCase).GetGroupValue(1).ToInt32();
                        if (timeout == 0) timeout =  5000;
                        else timeout              *= 1000;

                        throw new NeedRetryException(timeout);
                    }

                    throw new Exception("导出操作失败，响应内容为：" + ctx.ResponseContent?.RawStringResult);
                }

                var isAsync = !(ctx.Result.data?.taskId ?? "").IsNullOrEmpty();

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
                                data   = new { tips = "", downloadUrl = "", progress = 0 },
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

            var downloadCtx = _client.Create<byte[]>(HttpMethod.Get, downloadUrl, $"file/{fileid}");
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
            await ctx.SendAsync();

            if (!ctx.IsValid())
                throw new Exception("无法加载文件列表，响应内容为：" + ctx.ResponseContent?.RawStringResult);

            return ctx.Result;
        }

        protected virtual void OnMessage(string msg) => Message?.Invoke(this, new GeneralEventArgs<string>(msg));
    }
}
