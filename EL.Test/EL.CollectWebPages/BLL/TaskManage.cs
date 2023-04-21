using Automation.Inspect;
using EL.CollectWebPages.Common;
using EL.CollectWebPages.Model;
using Interop.UIAutomationClient;
using Microsoft.Playwright;
using NPOI.SS.Formula.Functions;
using OfficeOpenXml.Sorting;
using Org.BouncyCastle.Tls.Crypto;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Threading.Tasks;

namespace EL.CollectWebPages.BLL
{
    public class TaskManage : IDisposable
    {
        public IPlaywright PlaywrightObject;
        public Action<string> ShowMessage { get; set; }
        public volatile bool StartServer = false;
        public string RootUrl;
        public string RootPath;
        public IBrowser browser;
        public string ExePath = @"C:\Program Files\Google\Chrome\Application\chrome.exe";
        public UrlConfigManage URLManage;
        public FileTaskManage FileTaskManage;
        public int UrlCount = int.MaxValue;
        public Process ExeProcess;
        public ConcurrentDictionary<string, DateTime> UrlList = new ConcurrentDictionary<string, DateTime>();
        public bool IsRun = false;
        public DateTime LastTime = DateTime.Now;
        public bool NeedStart = false;
        public ConfigInfo<List<UrlInfo>> UrlInfo;
        public UrlInfo Current;
        public ExcelInfo CurrentExcel;
        public TaskManage(string exePath, string rootPath, string rootUrl = "https://baike.baidu.com/")
        {
            if (!string.IsNullOrEmpty(exePath))
            {
                ExePath = exePath;
            }
            this.RootPath = rootPath;
            this.RootUrl = rootUrl;
            this.URLManage = new UrlConfigManage();
        }
        public TaskManage(string exePath, string rootPath, ConfigInfo<List<UrlInfo>> UrlInfo, int UrlCount)
        {
            if (!string.IsNullOrEmpty(exePath))
            {
                ExePath = exePath;
            }
            this.RootPath = rootPath;
            this.RootUrl = null;
            this.UrlInfo = UrlInfo;
            this.URLManage = new UrlConfigManage();
            this.UrlCount = UrlCount;
        }
        public TaskManage(string exePath, string rootPath, FileTaskManage FileTaskManage, int UrlCount)
        {
            if (!string.IsNullOrEmpty(exePath))
            {
                ExePath = exePath;
            }
            this.RootPath = rootPath;
            this.RootUrl = null;
            this.FileTaskManage = FileTaskManage;
            this.URLManage = new UrlConfigManage();
            this.UrlCount = UrlCount;
        }

        public string GetRootUrl()
        {
            if (UrlInfo != null)
            {
                if (Current == null || Current.Count >= this.UrlCount)
                {
                    Current = UrlInfo.CurrentConfig.Where(t => !(t.Count >= this.UrlCount)).FirstOrDefault();
                }
                return Current.Url;
            }
            else if (FileTaskManage != null)
            {
                if (CurrentExcel == null)
                {
                    CurrentExcel = FileTaskManage.GetTask().First();
                }
                else if (CurrentExcel.Count >= this.UrlCount || !string.IsNullOrEmpty(CurrentExcel.状态))
                {
                    CurrentExcel.完成实际时间 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    if (string.IsNullOrEmpty(CurrentExcel.状态))
                    {
                        CurrentExcel.SetValue(nameof(ExcelInfo.状态), "完成");
                    }
                    FileTaskManage.SaveTask(new List<ExcelInfo>() { CurrentExcel });
                    URLManage.Clear();
                    CurrentExcel = FileTaskManage.GetTask().First();
                }
                return CurrentExcel.GetUrl();
            }
            else
            {
                return RootUrl;
            }
        }
        public void UpdateRootUrl()
        {
            if (Current != null)
            {
                Current.Count++;
                UrlInfo.Save();
            }
            else if (CurrentExcel != null)
            {
                CurrentExcel.Count = this.URLManage.GetAllUrlTable(URLState.处理完毕).Count;
            }
        }
        public void StartView()
        {
            NeedStart = false;
            _ = Start();
        }
        public async Task Start()
        {
            try
            {
                Log("准备处理任务!");
                if (PlaywrightObject != null)
                {
                    return;
                }
                PlaywrightObject = await Playwright.CreateAsync();
                browser = await PlaywrightObject.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                {
                    ExecutablePath = ExePath,//@"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe",                                                                 
                    Headless = false,
                    Args = new[] { "--window-position=0,0" }
                });
                Log("启动目标游览器!");
                var page = await NewPage(browser);
                var id = ProcessHelper.GetWebProcessID();
                ExeProcess = Process.GetProcessById(id);
                //Win32Helper.SetTop(process.MainWindowHandle);
                StartServer = true;
                Log("创建处理任务!");
                await Task.Factory.StartNew(async () =>
                {
                    IsRun = true;
                    try
                    {
                        await WebTaskProcess(browser, page, ExeProcess);
                    }
                    catch (Exception ex)
                    {
                        Log(ex.ToString());
                    }
                    NeedStart = true;
                }, TaskCreationOptions.LongRunning);

                Log("开始处理任务!");
            }
            catch (Exception ex)
            {
                Log(ex.Message);
            }
        }
        public Rectangle GetSize()
        {
            return new Rectangle(0, 0, 1280, Screen.PrimaryScreen.WorkingArea.Height - 100);
        }
        private async Task<IPage> NewPage(IBrowser browser)
        {
            var page = await browser.NewPageAsync();
            var size = GetSize();
            await page.SetViewportSizeAsync(size.Width, size.Height);
            await WaitForResponseEndAsync();
            //await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            page.Request += (_, request) =>
            {
                UrlList.TryAdd(request.Url, DateTime.Now);
            };
            page.Response += (_, response) =>
            {
                UrlList.TryRemove(response.Url, out var _);
            };
            return page;
        }
        private async Task<UrlTable> ToUrl(UrlTable result, IPage page)
        {
            if (page.Url == result.URL)
            {
                return result;
            }
            while (result != null)
            {
                try
                {
                    await page.GotoAsync(result.URL);
                    return result;
                }
                catch (Exception)
                {
                    result.State = URLState.处理完毕;
                    result.EndTime = DateTime.Now;
                    URLManage.Update(result);
                    result = URLManage.GetFirstTask();
                }
                Thread.Sleep(3000);
            }
            return null;
        }
        private async Task ToHome(string url, IPage page, int times = 5)
        {
            Exception exception = null;
            for (int i = 0; i < times; i++)
            {
                try
                {
                    await page.GotoAsync(url);
                    return;
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
                Thread.Sleep(3000);
            }
            if (exception != null)
            {
                throw exception;
            }
        }
        private async Task WebTaskProcess(IBrowser browser, IPage page, Process process)
        {
            while (StartServer)
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                try
                {
                    var result = URLManage.GetFirstTask();
                    if (page.Url == "about:blank" || result == null)
                    {
                        var url = GetRootUrl();
                        await ToHome(url, page);
                    }
                    //获取当前所有链接
                    await GetAllUrl(page);
                    var imageCount = 0;
                    result = URLManage.GetFirstTask();
                    if (result != null)
                    {
                        Log("打开目标地址，并等待加载完毕!");
                        result = await ToUrl(result, page);
                        if (result == null)
                        {
                            throw new Exception("任务处理完毕，没有其他的需要处理的");
                        }
                        await WaitForResponseEndAsync();
                        //await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                        Log("目标地址加载完毕!");
                        result.State = URLState.处理中;
                        URLManage.Update(result);
                        CatchImageManage catchImageManage = new CatchImageManage(this.RootPath, result.ID);
                        imageCount = await GetAllImage(page, result.URL, catchImageManage, process);
                        if (imageCount == -999)
                        {
                            CurrentExcel.完成实际时间 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            CurrentExcel.SetValue(nameof(ExcelInfo.状态), "异常(爬虫)");
                            URLManage.Clear();
                            Log("发生了异常,爬虫");
                            //result.State = URLState.处理完毕;
                            //result.EndTime = DateTime.Now;
                            //URLManage.Update(result);
                            //var newPage = await NewPage(browser);
                            //await page.CloseAsync();
                            //page = newPage;
                        }
                        else
                        {
                            if (!StartServer)
                            {
                                break;
                            }
                            if (catchImageManage.CheckImageFile())
                            {
                                result.State = URLState.处理完毕;
                                result.EndTime = DateTime.Now;
                                URLManage.Update(result);
                            }
                        }
                    }
                    else
                    {
                        CurrentExcel.完成实际时间 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        CurrentExcel.SetValue(nameof(ExcelInfo.状态), "异常(无链接)");
                        URLManage.Clear();
                        Log("发生了异常,无连接!");
                    }
                    var resultTime = imageCount > 0 ? stopwatch.ElapsedMilliseconds / imageCount : 0;
                    Log($"采集耗时:{stopwatch.ElapsedMilliseconds}毫秒  图片:{imageCount} 平均单个图片耗时:{resultTime}", "Time");
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("事件无法调用任何订户")||ex.Message.Contains("Target page"))
                    {
                        throw ex;
                    }
                    CurrentExcel.完成实际时间 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    CurrentExcel.SetValue(nameof(ExcelInfo.状态), "异常");
                    URLManage.Clear();
                    Log("发生了异常!" + ex.Message + ex.StackTrace);
                }
                UpdateRootUrl();
                Clear();
                stopwatch.Stop();
            }

            browser?.CloseAsync();
            Log($"已停止任务");
        }
        public (bool success, IUIAutomationElement ElementFromHandle, IUIAutomationElement Root) GetElementFromHandle(Process process)
        {
            var inspect = Boot.GetComponent<InspectComponent>().GetComponent<WinFormInspectComponent>();
            var ElementFromHandle = inspect.UIAFactory.ElementFromHandle(process.MainWindowHandle);
            var Root = inspect.UIAFactory.GetRootElement();
            return (true, ElementFromHandle, Root);
        }

        private async Task<int> GetAllImage(IPage page, string url, CatchImageManage catchImageManage, Process process)
        {
            var count = 0;
            var imageCount = 0;
            (bool success, IUIAutomationElement ElementFromHandle, IUIAutomationElement Root) = GetElementFromHandle(process);
            try
            {
                while (StartServer)
                {
                    try
                    {
                        if (ElementFromHandle == null)
                        {

                        }
                        var times = new Dictionary<int, long>();
                        var stopwatch = Stopwatch.StartNew();
                        var IUIAutomationElement = CatchImageManage.GetUIAutomation(ElementFromHandle, Root);
                        stopwatch.Stop();
                        times.Add(0, stopwatch.ElapsedMilliseconds);
                        var url1 = new Uri(page.Url);
                        var url2 = new Uri(url);
                        if (page.Url != url && url1.Host != url2.Host)
                        {
                            Log("打开目标地址，并等待加载完毕!");

                            stopwatch = Stopwatch.StartNew();
                            await page.GotoAsync(url);
                            stopwatch.Stop();
                            times.Add(1, stopwatch.ElapsedMilliseconds);

                            stopwatch = Stopwatch.StartNew();
                            await WaitForResponseEndAsync();
                            stopwatch.Stop();
                            times.Add(2, stopwatch.ElapsedMilliseconds);
                            Log("目标地址加载完毕!");
                            var resultInfo = GetElementFromHandle(process);
                            IUIAutomationElement = CatchImageManage.GetUIAutomation(resultInfo.ElementFromHandle, resultInfo.Root);
                            count++;
                        }
                        if (count > 5)
                        {
                            Log("对方爬虫限制无法爬取!");
                            return -999;
                        }
                        stopwatch = Stopwatch.StartNew();
                        var result = catchImageManage.Process(IUIAutomationElement, process.ProcessName);
                        stopwatch.Stop();
                        times.Add(3, stopwatch.ElapsedMilliseconds);
                        if (result.success)
                        {
                            LastTime = DateTime.Now;
                            if (result.window.Child?.Any() == false)
                            {

                            }
                            var path = Path.Combine(catchImageManage.RootPath, $"{result.ProcessName}_{result.window.ID}.jpg");
                            stopwatch = Stopwatch.StartNew();
                            await page.ScreenshotAsync(new PageScreenshotOptions { Path = path, Type = ScreenshotType.Jpeg });
                            stopwatch.Stop();
                            times.Add(4, stopwatch.ElapsedMilliseconds);

                            stopwatch = Stopwatch.StartNew();
                            catchImageManage.Save(result.window, result.ProcessName);
                            stopwatch.Stop();
                            times.Add(5, stopwatch.ElapsedMilliseconds);
                            imageCount++;
                        }
                        stopwatch = Stopwatch.StartNew();
                        var state = await GetScrollState(page);
                        stopwatch.Stop();
                        times.Add(6, stopwatch.ElapsedMilliseconds);
                        if (state)
                        {
                            break;
                        }
                        stopwatch = Stopwatch.StartNew();
                        await ScrollNext(page);
                        stopwatch.Stop();
                        times.Add(7, stopwatch.ElapsedMilliseconds);

                        stopwatch = Stopwatch.StartNew();
                        await WaitForResponseEndAsync();
                        stopwatch.Stop();
                        times.Add(8, stopwatch.ElapsedMilliseconds);
                        Log("耗时:" + string.Join(" ", times.OrderBy(t => t.Key).Select(t => $"{t.Key}:{t.Value}")), "Time");
                        if (imageCount > 30)
                        {
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log($"异常:" + ex.Message + ex.StackTrace);
                    }
                }
                return imageCount;
            }
            finally
            {
                try
                {
                    Marshal.FinalReleaseComObject(ElementFromHandle);
                }
                catch (Exception)
                {
                }
                try
                {
                    Marshal.FinalReleaseComObject(Root);
                }
                catch (Exception)
                {
                }
            }
        }
        private async Task ScrollNext(IPage page)
        {
            await page.EvaluateAsync("window.scrollBy(0, window.innerHeight);");
        }
        private async Task<bool> GetScrollState(IPage page)
        {
            var scrollOffset = await page.EvaluateAsync<int>(@"() => {
return document.documentElement.scrollHeight - window.pageYOffset - window.innerHeight;
}");
            if (scrollOffset <= 0)
            {
                return true;
            }
            return false;
        }
        private async Task GetAllUrl(IPage page)
        {
            var currentTitle = await page.TitleAsync();
            var currentUrl = page.Url;
            var Domain = new Uri(GetRootUrl());
            if (currentUrl.Contains(Domain.Host))
            {
                var UrlTable = new UrlTable();
                UrlTable.AddTime = DateTime.Now;
                UrlTable.SetUrl(currentUrl);
                UrlTable.Title = currentTitle;
                URLManage.InsertOrUpdate(UrlTable);
            }
            HashSet<string> Urls = new HashSet<string>();
            try
            {
                var urlList = await page.EvaluateAsync<string>(@"() => {
var regex = /^(https?:\/\/[^ ]+)/;
var links = document.links;
var urls = [];
for (var i = 0; i < links.length; i++) {
  var href = links[i].href;
  var result = regex.exec(href);
  if (result) {
    urls.push(result[1]);
  }
}
return JSON.stringify(urls);
}");
                var result = urlList.ToObj<List<string>>();
                if (result?.Any() == true)
                {
                    foreach (var url in result)
                    {
                        var NewUrl = url;
                        if (url.EndsWith("/"))
                        {
                            NewUrl = url.Substring(0, url.Length - 1);
                        }
                        if (NewUrl.Contains(Domain.Host) && NewUrl != GetRootUrl() && !NewUrl.Contains("#"))
                        {
                            Urls.Add(url);
                        }
                    }
                }
                var urlTables = new List<UrlTable>();
                foreach (var url in Urls)
                {
                    var list = new UrlTable();
                    list.AddTime = DateTime.Now;
                    list.State = URLState.默认;
                    list.SetUrl(url);
                    urlTables.Add(list);
                }
                var number = URLManage.Insert(urlTables);
            }
            catch (Exception)
            {
            }
        }
        public async Task WaitForResponseEndAsync(int millisecondsTimeout = 3000)
        {
            SpinWait.SpinUntil(() => !IsAny(), millisecondsTimeout);
            await Task.CompletedTask;
        }
        public bool IsAny()
        {
            return UrlList.Where(t => (DateTime.Now - t.Value).TotalSeconds > 10).Any();
        }
        public void Clear()
        {
            var delete = UrlList.Where(t => (DateTime.Now - t.Value).TotalSeconds > 10);
            foreach (var item in delete)
            {
                UrlList.TryRemove(item.Key, out var _);
            }
        }
        public void Close()
        {
            IsRun = false;
            StartServer = false;
            PlaywrightObject = null;
            browser?.CloseAsync();
            try
            {
                ExeProcess?.Kill();
            }
            catch (Exception)
            {
            }
            Log($"开始停止任务");
        }
        private static string logFile = Path.Combine(AppContext.BaseDirectory, "log");
        public void Log(string data, string name = "")
        {
            if (!Directory.Exists(logFile))
            {
                Directory.CreateDirectory(logFile);
            }
            var filePath = Path.Combine(logFile, $"{name}log_{DateTime.Now.ToString("yyyyMMdd")}.txt");
            var key = UrlTable.GetMd5Value(filePath);
            Mutex mutex = new Mutex(false, string.Concat("Global/", key));
            try
            {
                mutex.WaitOne();
                using (StreamWriter sw = new StreamWriter(filePath, true))
                {
                    sw.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff")} {data}");
                }
                ShowMessage?.Invoke(data);
            }
            catch (Exception)
            {
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }
        public void Dispose()
        {

        }
    }
}
