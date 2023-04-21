using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace EL.CollectWebPages.Common
{
    public static class Win32Helper
    {
        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hPos, int x, int y, int cx, int cy, uint nflags);
        public static void SetTop(IntPtr handel, bool top = true)
        {
            if (top)
            {
                IntPtr HWND_TOPMOST = new IntPtr(-1);
                SetWindowPos(handel, HWND_TOPMOST, 0, 0, 0, 0, 0x0001 | 0x0002);
            }
            else
            {
                IntPtr HWND_TOPMOST = new IntPtr(-2);
                SetWindowPos(handel, HWND_TOPMOST, 0, 0, 0, 0, 0x0001 | 0x0002);
            }
        }
    }
}
