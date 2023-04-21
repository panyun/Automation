using EL;
using EL.Async;
using EL.MSAA;
using Interop.UIAutomationClient;
using Microsoft.Playwright;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Cms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Automation.Inspect
{
    public class ElementPlaywright : Element
    {
        public ElementPlaywright()
        {

        }
        public ElementPlaywright(ElementHtmlPath ele, string url, string windowTitle, Parser.BrowserType browserType)
        {
            var playwrightInspect = Boot.GetComponent<InspectComponent>().GetComponent<PlaywrightInspectComponent>();
            this.PXPath = "xpath=" + ele.Xpath;
            XPath = ele.Xpath;
            this.Name = ele.Name;
            this.Role = ele.Role;
            this.BoundingRectangle = new Rectangle(ele.X, ele.Y, ele.Width, ele.Height);
            this.BoundingRectangleOffsetChrome =
            new Rectangle(ele.X + playwrightInspect.PointOffsetChrome.X, ele.Y + playwrightInspect.PointOffsetChrome.Y, ele.Width, ele.Height);
            this.BoundingRectangleOffsetMsedge =
          new Rectangle(ele.X + playwrightInspect.PointOffsetMsedge.X, ele.Y + playwrightInspect.PointOffsetMsedge.Y, ele.Width, ele.Height);
            Attributes = ele.Attributes;
            WindowUrl = url;
            WindowTitle = windowTitle;
            UserAgent = ele.UserAgent;
            IdXpath = ele.IdXpath;
        }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> Attributes { get; set; }
        public string PXPath { get; set; }
        public string XPath { get; set; }
        public string IdXpath { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public override string Name { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public override int ControlType { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public override string Role { get; set; }
        [JsonIgnore]
        public override string ControlTypeName => Role;
        [JsonIgnore]
        public string Value { get; set; }
        [JsonIgnore]
        public string Text { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string WindowUrl { get; set; }
        public string WindowTitle { get; set; }
        public string UserAgent { get; set; }
        public Parser.BrowserType BrowserType
        {
            get
            {
                if (UserAgent.IndexOf("Edg") > -1)
                    return Parser.BrowserType.Msedge;
                if (UserAgent.IndexOf("Chrome") > -1)
                    return Parser.BrowserType.Chromium;
                return Parser.BrowserType.Msedge;
            }
        }
        public override Rectangle BoundingRectangle { get; set; }
        public Rectangle BoundingRectangleOffsetChrome { get; set; }
        public Rectangle BoundingRectangleOffsetMsedge { get; set; }

        [JsonIgnore]
        public override Point ClickablePoint
        {
            get
            {
                //TODO WJF JAVA窗口最小化时，程序恢复显示时，取坐标为负数，原因不明
                var point = new Point(BoundingRectangle.Left + BoundingRectangle.Width / 2, BoundingRectangle.Top + BoundingRectangle.Height / 2);
                return point;
            }
        }

    }
    public class ElementHtmlPath
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Rectangle Rectangle { get; set; }
        public string Xpath { get; set; }
        public string IdXpath { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public Dictionary<string, string> Attributes { get; set; }
        public string UserAgent { get; set; }
    }

    public static class ElementPlaywrightSystem
    {
        public static void Update(this ElementPlaywright self, ElementHtmlPath ele)
        {
            if (ele == default) return;
            var playwrightInspect = Boot.GetComponent<InspectComponent>().GetComponent<PlaywrightInspectComponent>();
            self.PXPath = "xpath=" + ele.Xpath;
            self.XPath = ele.Xpath;
            self.Name = ele.Name;
            self.Role = ele.Role;
            self.BoundingRectangle = new Rectangle(ele.X, ele.Y, ele.Width, ele.Height);
            self.BoundingRectangleOffsetChrome =
            new Rectangle(ele.X + playwrightInspect.PointOffsetChrome.X, ele.Y + playwrightInspect.PointOffsetChrome.Y, ele.Width, ele.Height);
            self.BoundingRectangleOffsetMsedge =
       new Rectangle(ele.X + playwrightInspect.PointOffsetMsedge.X, ele.Y + playwrightInspect.PointOffsetMsedge.Y, ele.Width, ele.Height);
            self.Attributes = ele.Attributes;
            self.IdXpath = ele.IdXpath;
            self.UserAgent = ele.UserAgent;
        }
        public static Rectangle GetScreenBoundingRectangle(this ElementPlaywright self)
        {
            if (self.BrowserType == Parser.BrowserType.Chromium) return self.BoundingRectangleOffsetChrome;
            return self.BoundingRectangleOffsetMsedge;
        }
        
    }
}
