using Automation.Inspect;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EL.UIA.ControlTypeConverter;

namespace EL.DeepLearning
{
    public class EWindow
    {
        public EWindow()
        {
            ID = GenerateTimestamp();
        }
        public string ID { get; set; }
        public string Name { get; set; }
        public string ClassName { get; set; }
        public string Role { get; set; }
        public string Text { get; set; }
        //public string Path { get; set; }
        public string Value { get; set; }
        public bool Enable { get; set; } = true;

        public Rectangle Rectangle { get; set; }

        public Rectangle RootRectangle { get; set; } = Rectangle.Empty;
        public Rectangle RelativeRectangle
        {
            get
            {
                if (RootRectangle == Rectangle.Empty)
                {
                    RootRectangle = Rectangle;
                }
                return new Rectangle(Rectangle.X - RootRectangle.X, Rectangle.Y - RootRectangle.Y, Rectangle.Width, Rectangle.Height);
            }
        }
        [JsonConverter(typeof(StringEnumConverter))]
        public ElementType ProgramType { get; set; } = ElementType.UIAUI;
        [JsonConverter(typeof(StringEnumConverter))]

        public ControlType ControlType { get; set; }
        public string LocalizedControlType { get; set; }
        public List<EWindow> Child { get; set; }
        public string Remarks { get; set; }
        private string GenerateTimestamp()
        {
            return new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds().ToString();
        }
        public List<EWindow> GetEWindows(EWindow subWindow = null)
        {
            var windows = new List<EWindow>();
            List<EWindow> child = subWindow == null ? Child : subWindow.Child;
            EWindow window1 = subWindow == null ? this : subWindow;
            windows.Add(window1);
            if (child?.Any() == true)
            {
                foreach (EWindow window in child)
                {
                    windows.AddRange(GetEWindows(window));
                }
            }
            return windows;
        }
    }
}
