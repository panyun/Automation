using EL;
using EL.Async;
using EL.Input;
using EL.WindowsAPI;
using HtmlAgilityPack;
using Interop.UIAutomationClient;
using Microsoft.Playwright;
using System.Diagnostics;
using System.Drawing;
using System.Text.Json;
using Log = EL.Log;

namespace Automation.Inspect
{
    public class PlaywrightInspectComponentAwake : AwakeSystem<PlaywrightInspectComponent>
    {
        public override void Awake(PlaywrightInspectComponent self)
        {
            try
            {
                //创建对象
                self.CreatePlaywrightContext(Parser.BrowserType.Msedge).Coroutine();
                self.CreatePlaywrightContext(Parser.BrowserType.Chromium).Coroutine();
            }
            catch (Exception)
            {

            }
        }
    }
    public static class PlaywrightInspectComponentSystem
    {
        public static async ELTask CreatePlaywrightContext(this PlaywrightInspectComponent self, Parser.BrowserType browserType)
        {
            PlaywrightContext context = new()
            {
                Id = IdGenerater.Instance.GenerateId()
            };
            try
            {
                context.Playwright = await Playwright.CreateAsync();
                if (browserType == Parser.BrowserType.Msedge)
                {
                    context.Browser = await context.Playwright.Chromium.ConnectOverCDPAsync("http://127.0.0.1:9223");
                }
                if (browserType == Parser.BrowserType.Chromium)
                {
                    context.Browser = await context.Playwright.Chromium.ConnectOverCDPAsync("http://127.0.0.1:9222");
                }
                context.BrowserType = browserType;
                self.PlaywrightContexts.Add(context.Id, context);
            }
            catch (Exception ex)
            {
                Log.Error("初始化浏览器对象失败！");
                Log.Error(ex);
            }

        }
        public static (ILocator, IPage) FindLocatorByPath(this PlaywrightInspectComponent self, ElementPlaywright ele)
        {
            IPage currentPage = default;
            try
            {

                if (currentPage == default && !string.IsNullOrEmpty(ele.WindowUrl))
                {
                    if (self.CurrentPage != default &&
           !string.IsNullOrEmpty(self.WindowURL) && (self.WindowURL.Contains(ele.WindowUrl) || ele.WindowUrl.Contains(self.WindowURL)))
                        currentPage = self.CurrentPage;
                }
                if (!string.IsNullOrEmpty(ele.WindowTitle) && currentPage == default)
                {
                    if (self.CurrentPage != default &&
          !string.IsNullOrEmpty(self.WindowTitle) && (self.WindowTitle.Contains(ele.WindowTitle) || ele.WindowTitle.Contains(self.WindowTitle)))
                        currentPage = self.CurrentPage;
                }
            }
            catch (Exception)
            {

            }
            if (currentPage == default && !string.IsNullOrEmpty(ele.WindowUrl))
            {
                foreach (var page in self.Pages)
                {
                    if (page.Url.Contains(ele.WindowUrl) || ele.WindowUrl.Contains(page.Url))
                    {
                        currentPage = page;
                        break;
                    }
                }
            }
            if (!string.IsNullOrEmpty(ele.WindowTitle) && currentPage == default)
            {
                foreach (var page in self.Pages)
                {
                    var title = page.TitleAsync().Result;
                    if (ele.WindowTitle.Contains(title) || title.Contains(ele.WindowTitle))
                    {
                        currentPage = page;
                        break;
                    }
                }
            }
            if (currentPage == default)
                throw new Exception("未找到浏览器的操作实例的url:" + ele.WindowUrl);
            var l = currentPage.Locator(ele.PXPath).First;
            if (self.CurrentPage != currentPage)
            {
                self.CurrentPage = currentPage;
                //await self.CurrentPage?.EvaluateAsync(@"window.location.reload()");
            }
            return (l, currentPage);
        }
        public static async ELTask<IPage> RefreshPage(this PlaywrightInspectComponent self, string titleTemp)
        {
            await ELTask.CompletedTask;
            try
            {
                if (self.CurrentPage != default &&
            !string.IsNullOrEmpty(self.WindowTitle) && (self.WindowTitle.Contains(titleTemp) || titleTemp.Contains(self.WindowTitle)))
                    return self.CurrentPage;
            }
            catch (Exception)
            {

            }

            foreach (var page in self.Pages)
            {
                try
                {
                    var temp = await page.TitleAsync();
                    var visibilityState = await page.EvaluateAsync<string>("document.visibilityState");
                    var url = page.Url;
                    if (temp.Contains(titleTemp) || titleTemp.Contains(temp))
                    {
                        self.CurrentPage = page;
                        return page;
                    }
                }
                catch (Exception ex)
                {

                }
            }
            return default;
        }
        public static async ELTask<ElementIns> ElementFromPoint(this PlaywrightInspectComponent self)
        {
            #region MyRegion
            await ELTask.CompletedTask;
            var intPter = User32.GetForegroundWindow();
            var temp = intPter.GetWindowTitle();
            uint processId = default;
            User32.GetWindowThreadProcessId(intPter, out processId);
            if (processId <= 0) return default;
            var process = Process.GetProcessById((int)processId);
            if (!process.ProcessName.Contains("chrome") && !process.ProcessName.Contains("msedge"))
                return default;
            #region MyRegion
            //var winFromInspect = Boot.GetComponent<InspectComponent>().GetComponent<WinFormInspectComponent>();
            //winFromInspect.UIAFactory.ElementFromHandle(intPter);

            //var ele = winFromInspect.ElementFromPoint();
            //if (ele == default) return default;
            //var window = ele.GetNativeWindowHandle();
            //if (window == default) return default;
            //string temp = window.CurrentName;
            //Rectangle rec;
            //rec = window.GetRectangle();
            //Point point = new(rec.Left, rec.Top);
            //if (window?.CurrentProcessId == default)
            //    return default;
            //var process = Process.GetProcessById(window.CurrentProcessId);
            //if (!process.ProcessName.Contains("chrome") && !process.ProcessName.Contains("msedge"))
            //return default;
            #endregion

            await self.RefreshPage(temp);
            if (self.CurrentPage == default)
                return default;
            Point tempPoint = self.PointOffsetChrome;
            Parser.BrowserType bType = Parser.BrowserType.Chromium;
            if (process.ProcessName.Contains("chrome"))
                tempPoint = self.PointOffsetChrome;
            if (process.ProcessName.Contains("msedge"))
                tempPoint = self.PointOffsetMsedge;
            var htmlPoint = new Point(Mouse.Position.X - tempPoint.X, Mouse.Position.Y - tempPoint.Y);
            var elementHtmlPath = await self.GetElementByPoint(self.CurrentPage, htmlPoint);
            if (elementHtmlPath == default)
                return default;
            ElementPlaywright elementPlaywright = new(elementHtmlPath, self.WindowURL, self.WindowTitle, bType);
            if (elementPlaywright != default && !string.IsNullOrEmpty(elementPlaywright.PXPath))
                return new ElementIns(elementPlaywright);
            return default;
            #endregion
        }

        public static async Task<string> GetXpath(this PlaywrightInspectComponent self, Point pointTemp)
        {
            string xpath = string.Empty;
            try
            {
                Point point = new Point(Mouse.Position.X, Mouse.Position.Y);
                var x = point.X - pointTemp.X;
                var y = point.Y - pointTemp.Y;
                xpath = await self.CurrentPage.EvaluateAsync<string>(@"([x,y]) =>
               {
                    var el = document.elementFromPoint(x,y);
                    for (var xpath = ''; el && el.nodeType == 1; el = el.parentNode) 
                    { 
                        var sibling = el.parentNode.firstElementChild;
                        var idx = 0; 
                        while (sibling && sibling.nodeType == 1 && sibling !== el) 
                        {
                            idx =idx+1; 
                            sibling = sibling.previousElementSibling; 
                         } 
                        if (idx > 0)
                        {
                            xpath = '/' + el.tagName.toLowerCase() + '[' + idx + ']' + xpath; 
                        }else{
                            xpath = '/' + el.tagName.toLowerCase() + xpath;
                        }
                      }
                     return xpath; 
                }",
                new[] { x, y });
                return xpath;
            }
            catch (Exception ex)
            {
            }

            return default;
        }

        static async Task<ElementHtmlPath> GetElementByPoint(this PlaywrightInspectComponent self, IPage page, Point point)
        {
            try
            {
                var elementHtmlPath = await page.EvaluateAsync<ElementHtmlPath>(@"([x,y]) => {
                    var element = document.elementFromPoint(x,y);
                    var userAgent= navigator.userAgent;
                     var rect = element.getBoundingClientRect();
                    var tagName = element.tagName.toLowerCase();
                    var value = element.value || element.textContent;
                    var attributes = {};
                      for (var i = 0; i < element.attributes.length; i++) {
                        var attribute = element.attributes[i];
                        attributes[attribute.name] = attribute.value;
                      }
                  var xpath='';
                  var idXpath='';
                // Otherwise, traverse the DOM to construct the XPath
                xpath = '';
                while (element && element.nodeType === Node.ELEMENT_NODE) {
                    var elementTagName = element.tagName.toLowerCase();
                    var siblingIndex = 1;
                    var sibling = element.previousElementSibling;
                    while (sibling) {
                        if (sibling.nodeType === Node.ELEMENT_NODE && sibling.tagName.toLowerCase() === elementTagName) {
                                siblingIndex++;
                        }
                        sibling = sibling.previousElementSibling;
                        }
                    if (element && element.id && idXpath==''){
                        var prefix= `//*[@id=""${element.id}""]`;
                        idXpath =  prefix + xpath;
                    }
                    if(siblingIndex>1){
                        var prefix = elementTagName + '[' + siblingIndex + ']';
                        xpath = '/' + prefix + xpath;
                    }else{
                        var prefix = elementTagName;
                        xpath = '/' + prefix + xpath;
                    }
                    element = element.parentNode;
                }
               return {
                    Y: rect.top,
                    X: rect.left,
                    Width: rect.width,
                    Height: rect.height,
                    Xpath: xpath,
                    IdXpath:idXpath,
                    Role:tagName,
                    Name:value,
                    Attributes:attributes,
                    UserAgent:userAgent
                  };
                }",
                 new[] { point.X, point.Y });
                return elementHtmlPath;
            }
            catch (Exception ex)
            {
            }
            return default;
        }
        public static async ELTask<ElementHtmlPath> GetElementByPath(this PlaywrightInspectComponent self, string xpath)
        {
            await ELTask.CompletedTask;
            try
            {
                var elementHtmlPath = await self.CurrentPage.EvaluateAsync<ElementHtmlPath>(@"(xpath) => {
                  var element = document.evaluate(xpath,document).iterateNext();
                 var userAgent= navigator.userAgent;
                    var  rect = element.getBoundingClientRect();;
                    var tagName = element.tagName.toLowerCase();
                    var value = element.value || element.textContent;
                    var attributes = {};
                      for (var i = 0; i < element.attributes.length; i++) {
                        var attribute = element.attributes[i];
                        attributes[attribute.name] = attribute.value;
                      }
                  var xpath='';
                  var idXpath='';
                // Otherwise, traverse the DOM to construct the XPath
                xpath = '';
                while (element && element.nodeType === Node.ELEMENT_NODE) {
                    var elementTagName = element.tagName.toLowerCase();
                    var siblingIndex = 1;
                    var sibling = element.previousElementSibling;
                    while (sibling) {
                        if (sibling.nodeType === Node.ELEMENT_NODE && sibling.tagName.toLowerCase() === elementTagName) {
                                siblingIndex++;
                        }
                        sibling = sibling.previousElementSibling;
                        }
                    if (element && element.id && idXpath==''){
                        var prefix= `//*[@id=""${element.id}""]`;
                        idXpath =  prefix + xpath;
                    }
                    if(siblingIndex>1){
                        var prefix = elementTagName + '[' + siblingIndex + ']';
                        xpath = '/' + prefix + xpath;
                    }else{
                        var prefix = elementTagName;
                        xpath = '/' + prefix + xpath;
                    }
                    element = element.parentNode;
                }
               return {
                    Y: rect.top,
                    X: rect.left,
                    Width: rect.width,
                    Height: rect.height,
                    Xpath: xpath,
                    IdXpath:idXpath,
                    Role:tagName,
                    Name:value,
                    Attributes:attributes,
                    UserAgent:userAgent
                  };
                }", xpath);
                return elementHtmlPath;
            }
            catch (Exception ex)
            {
            }
            return default;
        }
        static async Task<ElementHtmlPath> GetXpath2(this PlaywrightInspectComponent self, Point point)
        {
            try
            {
                var elementHtmlPath = await self.CurrentPage.EvaluateAsync<ElementHtmlPath>(@"([x,y]) => {
                  var element = document.elementFromPoint(x,y);
                     var rect = element.getBoundingClientRect();
                  var xpath='';
                  var idXpath='';
                  if (element && element.id) {
                    // If the element has an ID, use it to construct the XPath
                    idXpath= `//*[@id=""${element.id}""]`;
                  } else {
                    // Otherwise, traverse the DOM to construct the XPath
                    xpath = '';
                    while (element && element.nodeType === Node.ELEMENT_NODE) {
                        var elementTagName = element.tagName.toLowerCase();
                        var siblingIndex = 1;
                        var sibling = element.previousElementSibling;
                        while (sibling) {
                            if (sibling.nodeType === Node.ELEMENT_NODE && sibling.tagName.toLowerCase() === elementTagName) {
                                  siblingIndex++;
                            }
                            sibling = sibling.previousElementSibling;
                         }
                        if (element && element.id && idXpath==''){
                            var prefix= `//*[@id=""${element.id}""]`;
                            idXpath =  prefix + xpath;
                        }
                        if(siblingIndex>1){
                            var prefix = elementTagName + '[' + siblingIndex + ']';
                            xpath = '/' + prefix + xpath;
                        }else{
                            var prefix = elementTagName;
                            xpath = '/' + prefix + xpath;
                        }
                        element = element.parentNode;
                    }
                  }
                  return {
                    Y: rect.top,
                    X: rect.left,
                    Width: rect.width,
                    Height: rect.height,
                    Xpath: xpath
                  };
                }",
                 new[] { point.X, point.Y });
                return elementHtmlPath;
            }
            catch (Exception ex)
            {
            }
            return default;
        }
        class ElementHtml
        {
            public string Name { get; set; }
            public string Role { get; set; }
            public Dictionary<string, string> Attributes { get; set; }
        }
        static async Task<ElementHtml> GetElement(this PlaywrightInspectComponent self, Point point)
        {
            try
            {
                var jsFunction = @"([x, y]) => {
                    const element = document.elementFromPoint(x, y);
                    var tagName = element.tagName.toLowerCase();
                    var value = element.value || element.textContent;
                        return { 
                            text: value,
                            tag: tagName,
                            attributes: Array.from(element.attributes).map(attr =>
                            {
                                return { name: attr.name, value: attr.value }; 
                            })
                        }; 
                    }";
                // Execute the JavaScript function in the browser and retrieve the element information
                var elementInfo = await self.CurrentPage.EvaluateAsync<JsonElement>(jsFunction, new[] { point.X, point.Y });
                ElementHtml elementHtml = new ElementHtml();
                elementHtml.Name = elementInfo.GetProperty("text").GetString() ?? default;
                elementHtml.Role = elementInfo.GetProperty("tag").GetString() ?? default;
                elementHtml.Attributes = new Dictionary<string, string>();
                var attributes = elementInfo.GetProperty("attributes").EnumerateArray();
                while (attributes.MoveNext())
                {
                    var name = attributes.Current.GetProperty("name").GetString();
                    if (!string.IsNullOrEmpty(name))
                        elementHtml.Attributes.Add(name, attributes.Current.GetProperty("value").GetString());
                }
                return elementHtml;
            }
            catch (Exception)
            {

            }
            return default;
        }
        static async Task<ElementHtml> GetElement(this PlaywrightInspectComponent self, string xpath)
        {
            try
            {
                var jsFunction = @"(xpath) => {
                  const element =  document.evaluate(xpath,document).iterateNext();
                    var tagName = element.tagName.toLowerCase();
                    var value = element.value || element.textContent;
                    if(element==null) 
                    return null;
                        return { 
                            text: value,
                            tag: tagName,
                            attributes: Array.from(element.attributes).map(attr =>
                            {
                                return { name: attr.name, value: attr.value }; 
                            })
                        }; 
                    }";
                // Execute the JavaScript function in the browser and retrieve the element information
                var elementInfo = await self.CurrentPage.EvaluateAsync<JsonElement>(jsFunction, xpath);
                ElementHtml elementHtml = new ElementHtml();
                elementHtml.Name = elementInfo.GetProperty("text").GetString() ?? default;
                elementHtml.Role = elementInfo.GetProperty("tag").GetString() ?? default;
                elementHtml.Attributes = new Dictionary<string, string>();
                var attributes = elementInfo.GetProperty("attributes").EnumerateArray();
                while (attributes.MoveNext())
                {
                    var name = attributes.Current.GetProperty("name").GetString();
                    if (!string.IsNullOrEmpty(name))
                        elementHtml.Attributes.Add(name, attributes.Current.GetProperty("value").GetString());
                }
                return elementHtml;
            }
            catch (Exception)
            {

            }
            return default;
        }
        //public static async ELTask<ElementIns> ElementFromPointEx(this PlaywrightInspectComponent self)
        //{
        //    await ELTask.CompletedTask;
        //    var winFromInspect = Boot.GetComponent<InspectComponent>().GetComponent<WinFormInspectComponent>();
        //    var ele = winFromInspect.ElementFromPoint();
        //    string temp = string.Empty;
        //    //var windowintPtr = ele.CurrentNativeWindowHandle;
        //    //if (windowintPtr != IntPtr.Zero)
        //    //    User32.GetWindowText(windowintPtr, temp, int.MaxValue);
        //    if (string.IsNullOrEmpty(temp))
        //    {
        //        var window = ele.GetNativeWindowHandle();
        //        if (window?.CurrentProcessId == default)
        //            return default;
        //        var process = Process.GetProcessById(window.CurrentProcessId);
        //        if (!process.ProcessName.Contains("chrome"))
        //            return default;
        //        if (window == null)
        //            return default;
        //        temp = window.CurrentName;
        //    }
        //    if (temp != self.Title)
        //        await self.RefreshPageAsync(temp);
        //    if (self.HtmlDoc == default) return default;
        //    if (self.CurrentPage == default) return default;
        //    var property = winFromInspect.GetALLText(ele);
        //    var p3 = ele.GetCurrentPropertyValue(UIA_PropertyIds.UIA_HelpTextPropertyId) + "";
        //    isBreak = false;
        //    string[] filters = new string[] { property.Item1, property.Item2, property.Item3, p3 };
        //    var nodes = self.ElementNodes.Where(x =>
        //     {
        //         var txt = x.GetDirectInnerText();
        //         var val = x.GetAttributeValue("href", default(string));
        //         var help = x.GetAttributeValue("placeholder", default(string));
        //         if (!string.IsNullOrEmpty(txt) ||
        //         !string.IsNullOrEmpty(val) ||
        //         !string.IsNullOrEmpty(help))
        //         {
        //             if (!string.IsNullOrEmpty(txt) && !filters.Contains(txt))
        //                 return false;
        //             if (!string.IsNullOrEmpty(val) && !filters.Contains(val))
        //                 return false;
        //             //if (!string.IsNullOrEmpty(help) && !filters.Contains(help))
        //             //    return false;
        //             return true;
        //         }
        //         return false;
        //     }).ToList();
        //    if (nodes.Count > 0)
        //    {
        //        foreach (var node in nodes)
        //        {
        //            var xpath = "xpath=" + node.XPath;
        //            var l = self.CurrentPage.Locator(xpath);
        //            l.HighlightAsync();
        //        }
        //        var rec = ele.GetCurrentPropertyValue(UIA_PropertyIds.UIA_BoundingRectanglePropertyId);
        //        var rectangle = ValueConverter.ToRectangle(rec);

        //        return new ElementIns(default, rectangle);
        //    }
        //    return default;
        //}
        public static double StringSimilarity(string str1, string str2)
        {
            if (string.IsNullOrEmpty(str1) || string.IsNullOrEmpty(str2))
            {
                return 0;
            }
            int len1 = str1.Length;
            int len2 = str2.Length;
            int[,] c = new int[len1 + 1, len2 + 1];

            for (int i = 0; i <= len1; i++)
            {
                c[i, 0] = i;
            }

            for (int j = 0; j <= len2; j++)
            {
                c[0, j] = j;
            }

            for (int i = 1; i <= len1; i++)
            {
                for (int j = 1; j <= len2; j++)
                {
                    if (str1[i - 1] == str2[j - 1])
                    {
                        c[i, j] = c[i - 1, j - 1];
                    }
                    else
                    {
                        c[i, j] = Math.Min(c[i - 1, j], Math.Min(c[i, j - 1], c[i - 1, j - 1])) + 1;
                    }
                }
            }

            return 1 - (double)c[len1, len2] / Math.Max(len1, len2);
        }
        static bool isBreak = false;
        static string[] filter = new string[] { "script" };
        public static List<HtmlNode> FindNodes(this PlaywrightInspectComponent self, HtmlNode node)
        {
            List<HtmlNode> nodes = new();
            if (!filter.Contains(node.Name))
                nodes.Add(node);
            foreach (var item in node.ChildNodes)
                nodes.AddRange(self.FindNodes(item));
            return nodes;
        }

    }
    public static class ControlTypeConverter
    {
        public static AriaRole ToAriaRoleType(object nativeControlType)
        {
            switch ((int)nativeControlType)
            {
                case UIA_ControlTypeIds.UIA_ButtonControlTypeId:
                    return AriaRole.Button;
                case UIA_ControlTypeIds.UIA_CalendarControlTypeId:
                    return AriaRole.None;
                case UIA_ControlTypeIds.UIA_CheckBoxControlTypeId:
                    return AriaRole.Checkbox;
                case UIA_ControlTypeIds.UIA_ComboBoxControlTypeId:
                    return AriaRole.Combobox;
                case UIA_ControlTypeIds.UIA_CustomControlTypeId:
                    return AriaRole.None;
                case UIA_ControlTypeIds.UIA_DataGridControlTypeId:
                    return AriaRole.Grid;
                case UIA_ControlTypeIds.UIA_DataItemControlTypeId:
                    return AriaRole.Gridcell;
                case UIA_ControlTypeIds.UIA_DocumentControlTypeId:
                    return AriaRole.Document;
                case UIA_ControlTypeIds.UIA_EditControlTypeId:
                    return AriaRole.None;
                case UIA_ControlTypeIds.UIA_GroupControlTypeId:
                    return AriaRole.Group;
                case UIA_ControlTypeIds.UIA_HeaderControlTypeId:
                    return AriaRole.Heading;
                case UIA_ControlTypeIds.UIA_HeaderItemControlTypeId:
                    return AriaRole.Menuitem;
                case UIA_ControlTypeIds.UIA_HyperlinkControlTypeId:
                    return AriaRole.Link;
                case UIA_ControlTypeIds.UIA_ImageControlTypeId:
                    return AriaRole.Img;
                case UIA_ControlTypeIds.UIA_ListControlTypeId:
                    return AriaRole.List;
                case UIA_ControlTypeIds.UIA_ListItemControlTypeId:
                    return AriaRole.Listitem;
                case UIA_ControlTypeIds.UIA_MenuBarControlTypeId:
                    return AriaRole.Menubar;
                case UIA_ControlTypeIds.UIA_MenuControlTypeId:
                    return AriaRole.Menu;
                case UIA_ControlTypeIds.UIA_MenuItemControlTypeId:
                    return AriaRole.Menuitem;
                case UIA_ControlTypeIds.UIA_PaneControlTypeId:
                    return AriaRole.Tabpanel;
                case UIA_ControlTypeIds.UIA_ProgressBarControlTypeId:
                    return AriaRole.Progressbar;
                case UIA_ControlTypeIds.UIA_RadioButtonControlTypeId:
                    return AriaRole.Radio;
                case UIA_ControlTypeIds.UIA_ScrollBarControlTypeId:
                    return AriaRole.Scrollbar;
                case UIA_ControlTypeIds.UIA_SemanticZoomControlTypeId:
                    return AriaRole.None;
                case UIA_ControlTypeIds.UIA_SeparatorControlTypeId:
                    return AriaRole.Separator;
                case UIA_ControlTypeIds.UIA_SliderControlTypeId:
                    return AriaRole.Slider;
                case UIA_ControlTypeIds.UIA_SpinnerControlTypeId:
                    return AriaRole.Spinbutton;
                case UIA_ControlTypeIds.UIA_SplitButtonControlTypeId:
                    return AriaRole.None;
                case UIA_ControlTypeIds.UIA_StatusBarControlTypeId:
                    return AriaRole.Status;
                case UIA_ControlTypeIds.UIA_TabControlTypeId:
                    return AriaRole.Tab;
                case UIA_ControlTypeIds.UIA_TabItemControlTypeId:
                    return AriaRole.Tablist;
                case UIA_ControlTypeIds.UIA_TableControlTypeId:
                    return AriaRole.Table;
                case UIA_ControlTypeIds.UIA_TextControlTypeId:
                    return AriaRole.Textbox;
                case UIA_ControlTypeIds.UIA_ThumbControlTypeId:
                    return AriaRole.None;
                case UIA_ControlTypeIds.UIA_TitleBarControlTypeId:
                    return AriaRole.None;
                case UIA_ControlTypeIds.UIA_ToolBarControlTypeId:
                    return AriaRole.Toolbar;
                case UIA_ControlTypeIds.UIA_ToolTipControlTypeId:
                    return AriaRole.Tooltip;
                case UIA_ControlTypeIds.UIA_TreeControlTypeId:
                    return AriaRole.Tree;
                case UIA_ControlTypeIds.UIA_TreeItemControlTypeId:
                    return AriaRole.Treeitem;
                case UIA_ControlTypeIds.UIA_WindowControlTypeId:
                    return AriaRole.None;
                case UIA_ControlTypeIds.UIA_AppBarControlTypeId:
                    return AriaRole.None;
                default:
                    throw new NotSupportedException();
            }
        }
    }

}

