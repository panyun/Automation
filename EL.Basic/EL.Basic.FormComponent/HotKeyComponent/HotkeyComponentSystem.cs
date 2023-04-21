using EL.Capturing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EL.Overlay
{
    public enum HotkeyModifiers
    {
        MOD_ALT = 0x1,
        MOD_CONTROL = 0x2,
        MOD_SHIFT = 0x4,
        MOD_WIN = 0x8
    }
    public static class HotkeyComponentSystem
    {
        /// <summary> 
        /// 注册快捷键 
        /// </summary> 
        /// <param name="hWnd">持有快捷键窗口的句柄</param> 
        /// <param name="fsModifiers">组合键</param> 
        /// <param name="vk">快捷键的虚拟键码</param> 
        /// <param name="callBack">回调函数</param> 
        public static void Regist(this HotkeyComponent self, IntPtr hWnd, HotkeyModifiers fsModifiers, Keys vk, HotkeyComponent.HotKeyCallBackHanlder callBack, int keyId)
        {
            HotkeyComponent.RegisterHotKey(hWnd, keyId, fsModifiers, vk);
            self.Keymap[keyId] = callBack;
        }

        /// <summary> 
        /// 注销快捷键 
        /// </summary> 
        /// <param name="hWnd">持有快捷键窗口的句柄</param> 
        /// <param name="callBack">回调函数</param> 
        public static void UnRegist(this HotkeyComponent self, IntPtr hWnd, int keyId)
        {
            HotkeyComponent.UnregisterHotKey(hWnd, keyId);
        }

        /// <summary> 
        /// 快捷键消息处理 
        /// </summary> 
        public static void ProcessHotKey(this HotkeyComponent self, Message m)
        {
            if (self == null)
                return;
            if (m.Msg == self.WM_HOTKEY)
            {
                int id = m.WParam.ToInt32();
                if (id == FormOverLayComponent.Instance.ComplateHotKeyId)
                {
                    FormOverLayComponent.Instance.CompleteEvent?.Invoke();
                }
                if (id == FormOverLayComponent.Instance.ExitHotKeyId)
                {
                    FormOverLayComponent.Instance.ExitEvent?.Invoke();
                }
                if (id == FormOverLayComponent.Instance.ModeHotKeyId)
                {
                    FormOverLayComponent.Instance.ModeEvent?.Invoke();
                }
                if (id == FormOverLayComponent.Instance.ScreenshotHotKeyId)
                {
                    FormOverLayComponent.Instance.ScreenshotEvent?.Invoke();
                }
            }
        }
    }
}
