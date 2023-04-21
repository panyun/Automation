using EL;
using EL.WindowsAPI;
using HtmlAgilityPack;
using Interop.UIAutomationClient;
using Microsoft.Playwright;
using System.Collections.Generic;
using System.Drawing;

namespace Automation.Inspect
{
    public class PlaywrightInspectComponent : Entity
    {

        public Dictionary<long, PlaywrightContext> PlaywrightContexts { get; set; } = new();
        public IPage CurrentPage
        {
            get; set;
        }
        private List<IPage> pages = new List<IPage>();
        public List<IPage> Pages
        {
            get
            {
                var tempIds = new List<long>();
                foreach (var context in PlaywrightContexts)
                {
                    if (context.Value.BrowserContexts.Count == 0)
                        tempIds.Add(context.Key);

                }
                tempIds.ForEach(x => PlaywrightContexts.Remove(x));
                pages.Clear();
                if (PlaywrightContexts?.Any() ?? false)
                {
                    foreach (var item in PlaywrightContexts)
                    {
                        var temps = item.Value.BrowserContexts.SelectMany(x => x.Pages).ToList() ?? default;
                        pages.AddRange(temps);
                    }
                }
                return pages;
            }
        }
        public string WindowURL
        {
            get
            {
                return CurrentPage.Url;
            }
        }
        public string WindowTitle
        {
            get
            {
                return CurrentPage.TitleAsync().Result;
            }
        }
        public Point PointOffsetMsedge
        {
            get
            {
                if (RectangleOffset != default)
                    return new Point(RectangleOffset.X+8, RectangleOffset.Y-8);
                return new Point();


            }
        }
        public Point PointOffsetChrome
        {
            get
            {
                if (RectangleOffset != default)
                    return new Point(RectangleOffset.X, RectangleOffset.Y);
                return new Point();
            }
        }
        public Rectangle RectangleOffset
        {
            get
            {
                var rectangle = CurrentPage.EvaluateAsync<Rectangle>(@"() => {
                            var    Width = window.outerWidth;
                            var   Height = window.outerHeight
                            var X = (typeof window,screenLeft == ""number"") ? window.screenLeft :window.screenX;
                            var Y = (typeof window.screenTop ==""number"") ? window.screenTop : window.screenY;
                            if(Y>0)
                            Y+= (window.outerHeight - window.innerHeight);
                            if(Y==0)
                                Y+= (window.outerHeight - window.innerHeight);
                            return {X,Y,Width,Height};
                        }").Result;
                return rectangle;
            }
        }
    }
    public class PlaywrightContext
    {

        public long Id { get; set; }
        public IPlaywright Playwright { get; set; }
        public IReadOnlyList<IBrowserContext> BrowserContexts
        {
            get
            {
                return Browser?.Contexts ?? default;
            }
        }
        public IBrowser Browser { get; set; }
        public Parser.BrowserType BrowserType { get; set; }
    }
}
