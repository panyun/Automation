using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace EL.WindowsAPI
{
    public static class DllTest
    {
        [DllImport(@"Dll1.dll")]
        public static extern int c_add(int a, int b);  
        [DllImport(@"Dll1.dll")]
        public static extern int c_sub(int a, int b);
        [DllImport(@"Dll1.dll")]
        public static extern int c_abs(int a);
    }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public static class Gdi32
    {
        public const int SRCCOPY = 0x00CC0020;
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr bmp);

        [DllImport("gdi32.dll")]
        public static extern bool BitBlt(IntPtr hdcDest, int xDest, int yDest, int wDest, int hDest, IntPtr hdcSource, int xSrc, int ySrc, CopyPixelOperation rop);

        [DllImport("gdi32.dll")]
        public static extern bool SetStretchBltMode(IntPtr hdc, StretchMode iStretchMode);

        [DllImport("gdi32.dll")]
        public static extern bool StretchBlt(
            IntPtr hdcDest, int nXOriginDest, int nYOriginDest, int nWidthDest, int nHeightDest,
            IntPtr hdcSrc, int nXOriginSrc, int nYOriginSrc, int nWidthSrc, int nHeightSrc,
            TernaryRasterOperations dwRop);

        [DllImport("gdi32.dll")]
        public static extern IntPtr DeleteDC(IntPtr hDc);

        [DllImport("gdi32.dll")]
        public static extern IntPtr DeleteObject(IntPtr hDc);
        [DllImport("gdi32.dll")]
        public static extern bool BitBlt(
        IntPtr hObject, int nXDest, int nYDest, int nWidth,
       int nHeight, IntPtr hObjSource, int nXSrc, int nYSrc,
        TernaryRasterOperations dwRop);
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
