using Microsoft.Playwright;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinTest
{
    public class PlaywrightTest
    {
        [SetUp]
        public void Setup()
        {

        }
        [Test]
        public static async Task Test1()
        {
            // Connect to an existing browser instance
            var playwright = await Playwright.CreateAsync();
            //var t =await playwright.APIRequest.NewContextAsync(new APIRequestNewContextOptions()
            //{
            //    BaseURL = "https://www.bing.com/"
            //});

            var browser = await playwright.Chromium.ConnectOverCDPAsync("http://localhost:9222");
            //var browser1 = await playwright.Chromium.ConnectAsync("http://localhost:9222");
            // Create a new page
            var contexts = browser.Contexts;
            IPage page = default;
            foreach (var item in contexts)
            {
                foreach (var item1 in item.Pages)
                {
                    //var l = page.GetByTitle("");
                    //page.IsVisibleAsync();
                    page = item1;

                    //await page.GotoAsync("https://www.bing.com/");
                }
            }

            var l = page.Locator("xpath=/html/body/div[2]/main/ol/li[2]/div/h2/a", default);
            var c = await l.CountAsync();
            await l.ClickAsync();
            //var c = await page.QuerySelectorAllAsync("/html/body/div/div[1]/nav[1]/ul[1]/li[5]/a");
            //var bound = await l.BoundingBoxAsync();
            //var c = await l.CountAsync();
            //await l.HighlightAsync();
            // Navigate to a URL
            //await page.GotoAsync("https://www.example.com/");
            // Wait for the page to load
            // Take a screenshot of the page
            //await page.ScreenshotAsync(new PageScreenshotOptions { Path = "example.png" });
        }
        [Test]
        public async Task StartPlay()
        {

            //const browserServer = await chromium.launchServer();
            //const wsEndpoint = browserServer.wsEndpoint();
            using var playwright = await Playwright.CreateAsync();
            //await using var browser = await playwright.Chromium.AttachAsync("<browser websocket url>");
            //var browser = await playwright.Chromium.AttachAsync("<browser websocket url>");
            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false,
                Channel = "msedge",
                SlowMo = 100,
                Devtools = true,

                //Proxy = new Proxy()
                //{
                //    Server = "123.123.123.123:80"
                //}
            });
            //var browser = await playwright.Chromium.ConnectOverCDPAsync("http://localhost:9222");
            var context = await browser.NewContextAsync();
            var contexts = browser.Contexts;

            var page = await context.NewPageAsync();

            //await page.GotoAsync("https://cn.bing.com/");
            await page.PauseAsync();
            await page.GetByRole(AriaRole.Searchbox, new() { Name = "输入搜索词" }).FillAsync("git");

            //await page.GetByRole(AriaRole.Searchbox, new() { Name = "输入搜索词" }).PressAsync("Enter");


        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task Started()
        {
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Firefox.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false,
                SlowMo = 1000
            });
            var context = await browser.NewContextAsync();
            // Open new page
            var page = await context.NewPageAsync();
            // Go to https://cn.bing.com/
            await page.GotoAsync("https://cn.bing.com/");
            // Click [aria-label="Enter\ your\ search\ term"]
            await page.Locator("[aria-label=\"Enter\\your\\search\\term\"]").ClickAsync();
            // Click .dimmer
            await page.Locator(".dimmer").ClickAsync();
            // Click [aria-label="Enter\ your\ search\ term"]
            await page.Locator("[aria-label=\"Enter\\ your\\ search\\ term\"]").ClickAsync();
            // Fill [aria-label="Enter\ your\ search\ term"]
            await page.Locator("[aria-label=\"Enter\\ your\\ search\\ term\"]").FillAsync("博客园");
            // Click input[name="go"]
            await page.Locator("input[name=\"go\"]").ClickAsync();
            // Assert.AreEqual("https://cn.bing.com/search?q=%E5%8D%9A%E5%AE%A2%E5%9B%AD&go=%E6%90%9C%E7%B4%A2&qs=ds&form=QBRE", page.Url);
            // Click a:has-text("博客园 - 开发者的网上家园") >> nth=0
            var page1 = await page.RunAndWaitForPopupAsync(async () =>
            {
                await page.Locator("a:has-text(\"博客园 - 开发者的网上家园\")").First.ClickAsync();
            });
            // Click text=WPF优秀组件推荐之MahApps
            var page2 = await page1.RunAndWaitForPopupAsync(async () =>
            {
                await page1.Locator("text=WPF优秀组件推荐之MahApps").ClickAsync();
            });
        }
    }
}
