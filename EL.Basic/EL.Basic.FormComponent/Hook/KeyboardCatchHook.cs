using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EL.Hook
{
    /// <summary>  
    /// 键盘钩子类
    /// </summary>  
    public class KeyboardCatchHook : GlobalHook
    {

        #region Events

        public event KeyEventHandler KeyDown;
        public event KeyEventHandler CatchEvent;
        public event KeyPressEventHandler KeyPress;

        #endregion

        #region Constructor

        public KeyboardCatchHook()
        {

            _hookType = WH_KEYBOARD_LL;

        }

        #endregion

        #region Methods

        protected override int HookCallbackProcedure(int nCode, int wParam, IntPtr lParam)
        {
            if ( CatchEvent != default && wParam == WM_KEYUP)
            {
                // Is Control being held down?  
                bool control = ((GetKeyState(VK_LCONTROL) & 0x80) != 0) ||
                               ((GetKeyState(VK_RCONTROL) & 0x80) != 0);
                // Is Shift being held down?  
                bool shift = ((GetKeyState(VK_LSHIFT) & 0x80) != 0) ||
                             ((GetKeyState(VK_RSHIFT) & 0x80) != 0);
                // Is Alt being held down?  
                bool alt = ((GetKeyState(VK_LALT) & 0x80) != 0) ||
                           ((GetKeyState(VK_RALT) & 0x80) != 0);
                if (control || shift || alt ) {
                    bool capslock = (GetKeyState(VK_CAPITAL) != 0);
                    KeyboardHookStruct keyboardHookStruct =
                        (KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyboardHookStruct));
                    KeyEventArgs e = new KeyEventArgs(
                       (Keys)(
                           keyboardHookStruct.vkCode |
                           (control ? (int)Keys.Control : 0) |
                           (shift ? (int)Keys.Shift : 0) |
                           (alt ? (int)Keys.Alt : 0) |
                           (capslock ? (int)Keys.CapsLock : 0)
                           ));
                    CatchEvent(this, e);
                    return -1;
                }            
            }
            return CallNextHookEx(_handleToHook, nCode, wParam, lParam);
        }
        public override void Start()
        {
            base.Start();
        }
        #endregion
    }
}
