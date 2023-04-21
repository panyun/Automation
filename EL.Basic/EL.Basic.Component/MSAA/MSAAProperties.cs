using Interop.UIAutomationClient;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace EL.MSAA
{
    public class MSAAProperties
    {
        private IAccessible accessible;

        private string _name;
        private string _role;
        private string _state;
        private string _value;
        private string _description;
        private int _childCount;
        private Rectangle _loaction;
        private int cId;
        public IUIAutomationElement ElementUIA { get; set; }
        public List<int> ChildIndexs { get; set; }
        public string Name { get { return this._name; } }
        public string Role { get { return this._role; } }
        public string State { get { return this._state; } }
        public string Value { get { return this._value; } }
        public string Description { get { return this._description; } }
        public Rectangle BoundingRectangle { get { return this._loaction; } }
        public IAccessible Accessible { get { return this.accessible; } }
        public int ChildCount
        {
            get
            {
                try { this._childCount = this.accessible.accChildCount; }
                catch { this._childCount = 0; }
                return this._childCount;
            }
        }
        //构造函数
        public MSAAProperties(IAccessible accessible) : this(accessible, 0) { }
        //构造函数
        public MSAAProperties(IAccessible accessible, int childId)
        {
            if (accessible == null)
                throw new ArgumentNullException();
            if (accessible != null)
                this.accessible = accessible;
            cId = childId;
            this.GetProperties();
        }
        public void FillProperties()
        {
            try { this._name = this.accessible.get_accName(cId); }
            catch { }
            try { this._value = this.accessible.get_accValue(cId); }
            catch { }
            try { this._description = this.accessible.get_accDescription(cId); }
            catch { }
            try
            {
                uint role = Convert.ToUInt32(this.accessible.get_accRole(cId));
                this._role = GetRoleText(role);
            }
            catch { }
            try
            {
                uint state = Convert.ToUInt32(this.accessible.get_accState(cId));
                this._state = this._state = GetStateText(state);
            }
            catch { }
        }
        //获取元素属性
        private void GetProperties()
        {
            try
            {
                int pxLeft, pyTop, pcxWidth, pcyHeight;
                this.accessible.accLocation(out pxLeft, out pyTop, out pcxWidth, out pcyHeight, cId);
                this._loaction = new Rectangle(pxLeft, pyTop, pcxWidth, pcyHeight);
            }
            catch { }
        }
        //获取控件名称
        private static string GetRoleText(uint role)
        {
            StringBuilder lpszRole = new StringBuilder(1024);
            GetRoleText(role, lpszRole, 1024);
            return lpszRole.ToString();
        }
        private const uint STATE_SYSTEM_ALERT_HIGH = 0x10000000;
        private const uint STATE_SYSTEM_ALERT_LOW = 0x04000000;
        private const uint STATE_SYSTEM_ALERT_MEDIUM = 0x08000000;
        private const uint STATE_SYSTEM_ANIMATED = 0x00004000;
        private const uint STATE_SYSTEM_BUSY = 0x00000800;
        private const uint STATE_SYSTEM_CHECKED = 0x00000010;
        private const uint STATE_SYSTEM_COLLAPSED = 0x00000400;
        private const uint STATE_SYSTEM_DEFAULT = 0x00000100;
        private const uint STATE_SYSTEM_EXPANDED = 0x00000200;
        private const uint STATE_SYSTEM_EXTSELECTABLE = 0x02000000;
        private const uint STATE_SYSTEM_FLOATING = 0x00001000;
        private const uint STATE_SYSTEM_FOCUSABLE = 0x00100000;
        private const uint STATE_SYSTEM_FOCUSED = 0x00000004;
        private const uint STATE_SYSTEM_HASPOPUP = 0x40000000;
        private const uint STATE_SYSTEM_HOTTRACKED = 0x00000080;
        private const uint STATE_SYSTEM_INVISIBLE = 0x00008000;
        private const uint STATE_SYSTEM_LINKED = 0x00400000;
        private const uint STATE_SYSTEM_MARQUEED = 0x00002000;
        private const uint STATE_SYSTEM_MIXED = 0x00000020;
        private const uint STATE_SYSTEM_MOVEABLE = 0x00040000;
        private const uint STATE_SYSTEM_MULTISELECTABLE = 0x01000000;
        private const uint STATE_SYSTEM_NORMAL = 0x00000000;
        private const uint STATE_SYSTEM_OFFSCREEN = 0x00010000;
        private const uint STATE_SYSTEM_PRESSED = 0x00000008;
        private const uint STATE_SYSTEM_READONLY = 0x00000040;
        private const uint STATE_SYSTEM_SELECTABLE = 0x00200000;
        private const uint STATE_SYSTEM_SELECTED = 0x00000002;
        private const uint STATE_SYSTEM_SELFVOICING = 0x00080000;
        private const uint STATE_SYSTEM_SIZEABLE = 0x00020000;
        private const uint STATE_SYSTEM_TRAVERSED = 0x00800000;
        private const uint STATE_SYSTEM_UNAVAILABLE = 0x00000001;
        private const uint STATE_SYSTEM_VALID = 0x1FFFFFFF;
        //获取状态文本
        private static string GetStateText(uint state)
        {
            StringBuilder focusableStateText = new StringBuilder(1024);
            StringBuilder sizeableStateText = new StringBuilder(1024);
            StringBuilder moveableStateText = new StringBuilder(1024);
            StringBuilder invisibleStateText = new StringBuilder(1024);
            StringBuilder unavailableStateText = new StringBuilder(1024);
            StringBuilder hasPopupStateText = new StringBuilder(1024);

            if (state == (STATE_SYSTEM_FOCUSABLE | STATE_SYSTEM_SIZEABLE | STATE_SYSTEM_MOVEABLE))
            {
                GetStateText(STATE_SYSTEM_FOCUSABLE, focusableStateText, 1024);
                GetStateText(STATE_SYSTEM_SIZEABLE, sizeableStateText, 1024);
                GetStateText(STATE_SYSTEM_MOVEABLE, moveableStateText, 1024);
                return focusableStateText + "," + sizeableStateText + "," + moveableStateText;
            }
            if (state == (STATE_SYSTEM_FOCUSABLE | STATE_SYSTEM_INVISIBLE))
            {
                GetStateText(STATE_SYSTEM_FOCUSABLE, focusableStateText, 1024);
                GetStateText(STATE_SYSTEM_INVISIBLE, invisibleStateText, 1024);
                return focusableStateText + "," + invisibleStateText;
            }
            if (state == (STATE_SYSTEM_FOCUSABLE | STATE_SYSTEM_UNAVAILABLE))
            {
                GetStateText(STATE_SYSTEM_FOCUSABLE, focusableStateText, 1024);
                GetStateText(STATE_SYSTEM_UNAVAILABLE, unavailableStateText, 1024);
                return focusableStateText + "," + unavailableStateText;
            }
            if (state == (STATE_SYSTEM_HASPOPUP | STATE_SYSTEM_UNAVAILABLE))
            {
                GetStateText(STATE_SYSTEM_HASPOPUP, hasPopupStateText, 1024);
                GetStateText(STATE_SYSTEM_UNAVAILABLE, unavailableStateText, 1024);
                return hasPopupStateText + "," + unavailableStateText;
            }
            var stateText = new StringBuilder((int)1024);
            GetStateText(state, stateText, 1024);
            return stateText.ToString();
        }

        #region API
        [DllImport("oleacc.dll")]
        private static extern uint GetRoleText(
            uint dwRole,
            StringBuilder lpszRole,
            uint cchRoleMax
        );
        [DllImport("oleacc.dll")]
        private static extern uint GetStateText(
            uint dwStateBit,
            StringBuilder lpszStateBit,
            uint cchStateBitMax
        );
        #endregion
    }
}
