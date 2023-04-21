using EL;
using EL.UIA;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using System.Drawing;

namespace Automation.Inspect
{
    public class ElementVcOcr : Element
    {
        public ElementVcOcr()
        {
           
        }
        public ElementVcOcr(byte[] img, Rectangle rectangle)
        {
            MatchImg =  Convert.ToBase64String(img);
            BoundingRectangle = rectangle;
        }
        /// <summary>
        /// 
        /// </summary>
        public string MatchImg { get; set; }
        public override string Name { get; set; }
        public override int ControlType { get; set; }
        public override string Role { get; set; }
        public override string ControlTypeName { get; }
        public override Rectangle BoundingRectangle { get; set; }
        [JsonIgnore]
        public override Point ClickablePoint
        {
            get
            {
                var point = new Point(BoundingRectangle.Left + BoundingRectangle.Width / 2, BoundingRectangle.Top + BoundingRectangle.Height / 2);
                return point;
            }
        }
    }
    public static class ElementVcOcrSystem
    {
        public static ElementVcOcr Convert(this ElementCvInfo self)
        {
            if (self == default) return default;
            return new ElementVcOcr(self.Img, self.Rect);
        }
    }
    public class ElementCvInfo
    {
        public ElementCvInfo()
        {

        }
        [JsonIgnore]
        public Rectangle Rect { get; set; }
        [JsonIgnore]
        public byte[] Img { get; set; }
        [JsonIgnore]
        public WindowCv WindowCv { get; set; }
    }
}
