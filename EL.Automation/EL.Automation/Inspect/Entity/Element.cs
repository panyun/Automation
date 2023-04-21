using EL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EL.UIA.ControlTypeConverter;

namespace Automation.Inspect
{
    public abstract class Element
    {
        public string Id { get; set; } 
        [JsonIgnore]
        public object NotSupportedValue => WinFormInspectComponent.Instance.UIAFactory.ReservedNotSupportedValue;
        public abstract string Name { get; set; }
        public abstract int ControlType { get; set; }
        public abstract string Role { get; set; }
        public abstract string ControlTypeName { get;  }
        public abstract Rectangle BoundingRectangle { get; set; }
        public abstract Point ClickablePoint { get; }
        public bool TryElementWin(out ElementUIA element)
        {
            if (this is ElementUIA)
            {
                element = this as ElementUIA;
                return true;
            }
            element = default;
            return false;
        }
    }
}
