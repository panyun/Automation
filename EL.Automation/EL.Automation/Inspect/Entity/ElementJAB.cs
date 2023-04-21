using Interop.UIAutomationClient;
using Newtonsoft.Json;
using System.Drawing;
using WindowsAccessBridgeInterop;

namespace Automation.Inspect
{
    public class ElementJAB : Element
    {
        [JsonIgnore]
        public PropertyList Properties { get; set; }
        [JsonIgnore]
        public AccessibleNode AccessibleNode { get; }
        public ElementJAB()
        {

        }
        public ElementJAB(AccessibleNode accessibleNode) : this(accessibleNode, IntPtr.Zero, string.Empty)
        {
        }
        public ElementJAB(AccessibleNode accessibleNode, IntPtr nativeWindowHandle, string mainWindowTitle)
        {
            AccessibleNode = accessibleNode;
            Properties = AccessibleNode.GetProperties(PropertyOptions.AccessibleContextInfo | PropertyOptions.ObjectDepth);
            MainWindowTitle = mainWindowTitle;
            Name = GetValue<string>("name");
            Role = GetValue<string>("role");
            ControlType = default;
            NativeWindowHandle = nativeWindowHandle;
            ControlTypeName = Role;
            BoundingRectangle = AccessibleNode.GetScreenRectangle() ?? new Rectangle();
            AccessibleNode.Refresh();
            var temp = AccessibleNode.GetScreenRectangle();
            if (temp != null)
                BoundingRectangle = temp.Value;
        }
        public IntPtr NativeWindowHandle { get; set; }
        public string MainWindowTitle { get; set; }
        public override string Name
        {
            get; set;
        }
        public override string Role
        {
            get; set;
        }
        public override int ControlType { get; set; }
        public override string ControlTypeName { get; }
        [JsonIgnore]
        public override Rectangle BoundingRectangle
        {
            get; set;
        }
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
        public T GetValue<T>(string name)
        {
            var pro = Properties.FirstOrDefault(x => x.Name.ToLower() == name.ToLower());
            if (pro == null) return default(T);
            return (T)pro.Value;
        }
        public void FillValue()
        {

        }
    }
    public static class ElementJABSystem
    {
        public static ElementIns ConvertElementInspect
          (this ElementJAB self)
        {
            return new ElementIns(self);
        }

    }
}
