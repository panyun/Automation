using EL.WindowsAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace EL.Basic.Component.Clipboard
{
    public class ClipboardComponent : Entity
    {
        #region Win32 Interface
        [DllImport("user32.dll")]
        public static extern bool EmptyClipboard();

        [DllImport("user32.dll", SetLastError = true)]
        public extern static IntPtr SetClipboardData(uint format, IntPtr handle);
        [DllImport("user32.dll")]
        public static extern IntPtr GetClipboardData(uint uFormat);
        [DllImport("user32.dll")]
        public static extern bool IsClipboardFormatAvailable(uint format);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool OpenClipboard(IntPtr hWndNewOwner);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool CloseClipboard();
        [DllImport("kernel32.dll")]
        public static extern IntPtr GlobalLock(IntPtr hMem);
        [DllImport("kernel32.dll")]
        public static extern bool GlobalUnlock(IntPtr hMem);
        public const uint CF_UNICODETEXT = 13;
        #endregion
    }
    public static class ClipboardComponentSystem
    {
        /// <summary>
        /// 设置剪贴版
        /// </summary>
        /// <param name="self"></param>
        /// <param name="text"></param>
        public static bool CopyToClipboard(this ClipboardComponent self, string text, uint id = ClipboardComponent.CF_UNICODETEXT)
        {
            WriteResultFile(text);
            if (ClipboardComponent.OpenClipboard(IntPtr.Zero))
            {
                ClipboardComponent.EmptyClipboard();
                IntPtr hmem = Marshal.StringToHGlobalUni(text);
                var ptr = ClipboardComponent.GlobalLock(hmem);
                ClipboardComponent.GlobalUnlock(ptr);
                ClipboardComponent.SetClipboardData(id, ptr);
                ClipboardComponent.CloseClipboard();
                return true;
            }
            return false;
        }
        /// <summary>
        /// 创建一个数据库文件。 这只是创建一个零字节的文件，
        /// SQLite正确打开文件后，它将变成数据库。 
        /// </summary>
        /// <param name="databaseFileName">需要创建的文件</param>
        public static void WriteResultFile(string msg, string filePath = "Result")
        {

            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);
            filePath += "/result.txt";
            byte[] myByte = Encoding.UTF8.GetBytes(msg);
            using (var file = File.Open(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                file.Write(myByte, 0, myByte.Count());
                file.Close();
            }
        }

        public static string GetFromClipboard(this ClipboardComponent self, uint id = ClipboardComponent.CF_UNICODETEXT)
        {
            if (!ClipboardComponent.IsClipboardFormatAvailable(id)) return null;
            if (!ClipboardComponent.OpenClipboard(IntPtr.Zero)) return null;

            string data = null;
            var hGlobal = ClipboardComponent.GetClipboardData(id);
            if (hGlobal != IntPtr.Zero)
            {
                var lpwcstr = ClipboardComponent.GlobalLock(hGlobal);
                if (lpwcstr != IntPtr.Zero)
                {
                    data = Marshal.PtrToStringAuto(lpwcstr);
                    ClipboardComponent.GlobalUnlock(lpwcstr);
                }
            }
            ClipboardComponent.CloseClipboard();
            return data;
        }
        public static bool EmptyClipboard(this ClipboardComponent self)
        {
            if (ClipboardComponent.OpenClipboard(IntPtr.Zero))
            {
                ClipboardComponent.EmptyClipboard();
                return true;
            }
            return false;
        }
    }
}
