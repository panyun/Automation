using EL.Async;
using EL.MSAA;
using Interop.UIAutomationClient;
using Microsoft.Playwright;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automation.Inspect
{
    public class ElementMSAA : Element
    {
        public ElementMSAA(MSAAProperties mSAAProperties)
        {
            this.MSAAProperties = mSAAProperties;
            this.Name = mSAAProperties.Name;
            this.Text = mSAAProperties.Description;
            this.Value = mSAAProperties.Value;
            this.Role = mSAAProperties.Role;
            this.BoundingRectangle = mSAAProperties.BoundingRectangle;
        }
        public MSAAProperties MSAAProperties { get; set; }
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
        public override Rectangle BoundingRectangle { get; set; }
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
    public static class ElementMSAASystem
    {
        public static ElementMSAA Convert(this MSAAProperties self)
        {
            self.FillProperties();
            return new ElementMSAA(self);
        }
        public static ElementIns ConvertElementInspect
          (this MSAAProperties self)
        {
            return new ElementIns(self);
        }
    }
}
