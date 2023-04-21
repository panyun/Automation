using Automation.Inspect;
using EL;
using EL.Async;
using EL.Input;
using EL.WindowsAPI;
using Microsoft.Playwright;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Automation.Parser
{
    public static class OpenBrowserActionSystem
    {

        public static async void Main(this OpenBrowserActionRequest self)
        {
            try
            {
                switch (self.BrowserType)
                {
                    case BrowserType.Chromium:
                        self.OpenBrowserChrome();
                        break;
                    case BrowserType.Msedge:
                        self.OpenBrowserEdge();
                        break;
                    case BrowserType.Firefox:
                        break;
                    case BrowserType.Webkit:
                        break;
                    default:
                        break;
                }
                var playwrightInspect = Boot.GetComponent<InspectComponent>().GetComponent<PlaywrightInspectComponent>();
                await playwrightInspect.CreatePlaywrightContext(self.BrowserType);
                return;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            await self.OpenBrowser();
        }
        public static void OpenBrowserEdge(this OpenBrowserActionRequest self)
        {
            Process edgeProcess = new Process();
            // Set the program name and arguments
            edgeProcess.StartInfo.FileName = "msedge.exe";
            //edgeProcess.StartInfo.Arguments = "--remote-debugging-port=9222 --user-data-dir=remote-debug-profile" + $" {self.Url}";
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Edge");
            edgeProcess.StartInfo.Arguments = $"--remote-debugging-port=9223 --user-data-dir={path} {self.Url}";
            // Start the process
            edgeProcess.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
            edgeProcess.Start();

        }
        public static void MaximizedWindow(this IntPtr window)
        {
            //var isIconic = User32.IsIconic(window);
            //if (isIconic)
            User32.ShowWindow(window, ShowWindowTypes.SW_MAXIMIZE);
        }
        public static void OpenBrowserChrome(this OpenBrowserActionRequest self)
        {
            Process edgeProcess = new Process();
            // Set the program name and arguments
            edgeProcess.StartInfo.FileName = "chrome.exe";
            edgeProcess.StartInfo.Arguments = "--remote-debugging-port=9222" + $" {self.Url}";
            edgeProcess.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
            // Start the process
            edgeProcess.Start();

        }
        #region MyRegion
        public static async ELTask OpenBrowser(this OpenBrowserActionRequest self)
        {
            var id = IdGenerater.Instance.GenerateId();
            PlaywrightContext playwrightContext = new()
            {
                Playwright = await Playwright.CreateAsync(),
                Id = id,
            };
            IBrowserType browserType = default;
            var options = new BrowserTypeLaunchOptions
            {
                Headless = false,
                SlowMo = 1000
            };
            switch (self.BrowserType)
            {
                case BrowserType.Chromium:
                    options.Channel = "chrome";
                    browserType = playwrightContext.Playwright.Chromium;
                    break;
                case BrowserType.Msedge:
                    options.Channel = "msedge";
                    browserType = playwrightContext.Playwright.Chromium;
                    break;
                case BrowserType.Firefox:
                    browserType = playwrightContext.Playwright.Firefox;
                    break;
                case BrowserType.Webkit:
                    browserType = playwrightContext.Playwright.Webkit;
                    break;
                default:
                    break;
            }

            playwrightContext.Browser = await browserType.LaunchAsync(options);
            var context = await playwrightContext.Browser.NewContextAsync();
            // Open new page
            var page = await context.NewPageAsync();
            // Go to https://cn.bing.com/
            bool IsValidUrl(string url)
            {
                Regex regex = new Regex(@"^(http|https)://([\w-]+\.)+[\w-]+(/[\w-./?%&=]*)?$");
                return regex.IsMatch(url);
            }
            if (!IsValidUrl(self.Url))
            {
                await playwrightContext.Browser.CloseAsync();
                throw new ParserException("URL地址不正确！");
            }
            var playwrightInspect = Boot.GetComponent<InspectComponent>().GetComponent<PlaywrightInspectComponent>();
            playwrightInspect.PlaywrightContexts.Add(id, playwrightContext);
            //playwrightContext.BrowserContext.Close += (x, y) =>
            //{
            //    playwrightInspect.PlaywrightContexts.Remove(id);
            //};
            //playwrightContext.Browser.Disconnected += (x, y) =>
            //{
            //    playwrightInspect.PlaywrightContexts.Remove(id);
            //};
            //playwrightContext.BrowserContext.Page += (x, y) =>
            //{
            //    playwrightContext.CurrentPage = y;
            //    playwrightContext.BrowserContext = (IBrowserContext)x;
            //    playwrightContext.CurrentPage.Close += (x, y) =>
            //    {

            //    };
            //};
            await page.GotoAsync(self.Url);

        }
        #endregion






    }
}
