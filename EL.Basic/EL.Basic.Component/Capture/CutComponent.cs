using EL.WindowsAPI;
using NAudio.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL.Capturing
{
    public class CutComponent : Entity
    {
    }
    public static class CutComponentSystem
    {
        /// <summary>
        /// 截取完整屏幕图片
        /// </summary>
        /// <returns></returns>
        private static Image GetDestopImage()
        {
            var vScreenWidth = User32.GetSystemMetrics(SystemMetric.SM_CXVIRTUALSCREEN);
            var vScreenHeight = User32.GetSystemMetrics(SystemMetric.SM_CYVIRTUALSCREEN);
            var vScreenLeft = User32.GetSystemMetrics(SystemMetric.SM_XVIRTUALSCREEN);
            var vScreenTop = User32.GetSystemMetrics(SystemMetric.SM_YVIRTUALSCREEN);
            Bitmap bmp = new(
                vScreenWidth, vScreenHeight, PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(bmp);

            IntPtr gHdc = g.GetHdc();
            IntPtr deskHandle = User32.GetDesktopWindow();

            IntPtr dHdc = User32.GetDC(deskHandle);
            Gdi32.BitBlt(
                gHdc,
            0,
                0,
                vScreenWidth,
                vScreenHeight,
                dHdc,
                0,
                0,
                TernaryRasterOperations.SRCCOPY);
            User32.ReleaseDC(deskHandle, dHdc);
            g.ReleaseHdc(gHdc);
            return bmp;
        }
        //截取整个桌面
        public static Image Cut(this CutComponent self)
        {
            var vScreenWidth = User32.GetSystemMetrics(SystemMetric.SM_CXVIRTUALSCREEN);
            var vScreenHeight = User32.GetSystemMetrics(SystemMetric.SM_CYVIRTUALSCREEN);
            var vScreenLeft = User32.GetSystemMetrics(SystemMetric.SM_XVIRTUALSCREEN);
            var vScreenTop = User32.GetSystemMetrics(SystemMetric.SM_YVIRTUALSCREEN);
            int iWidth = vScreenWidth;
            int iHeight = vScreenHeight;
            Image myImage = new Bitmap(iWidth, iHeight);
            Graphics.FromImage(myImage).CopyFromScreen(new System.Drawing.Point(vScreenLeft, vScreenTop), new System.Drawing.Point(0, 0), new System.Drawing.Size(iWidth, iHeight));
            return myImage;
        }
        public static Bitmap CutBitmap(this CutComponent self)
        {
            var vScreenWidth = User32.GetSystemMetrics(SystemMetric.SM_CXVIRTUALSCREEN);
            var vScreenHeight = User32.GetSystemMetrics(SystemMetric.SM_CYVIRTUALSCREEN);
            var vScreenLeft = User32.GetSystemMetrics(SystemMetric.SM_XVIRTUALSCREEN);
            var vScreenTop = User32.GetSystemMetrics(SystemMetric.SM_YVIRTUALSCREEN);
            int iWidth = vScreenWidth;
            int iHeight = vScreenHeight;
            Bitmap myImage = new Bitmap(iWidth, iHeight);
            Graphics.FromImage(myImage).CopyFromScreen(new System.Drawing.Point(vScreenLeft, vScreenTop), new System.Drawing.Point(0, 0), new System.Drawing.Size(iWidth, iHeight));
            return myImage;
        }
        //截取一个Rectangle.
        public static Image Cut(this CutComponent self, Rectangle rect)
        {
            if (rect.IsEmpty) return null;
            Rectangle rc = rect;
            int iWidth = rc.Width;
            int iHeight = rc.Height;
            Image myImage = new Bitmap(iWidth, iHeight);
            Graphics.FromImage(myImage).CopyFromScreen(rc.Location, new System.Drawing.Point(0, 0), new System.Drawing.Size(iWidth, iHeight));
            return myImage;
        }
        //截取 x, y 点 weight, height
        public static Image Cut(this CutComponent self, int X, int Y, int Width, int Height)
        {
            Rectangle rc = new Rectangle(X, Y, Width, Height);
            int iWidth = rc.Width;
            int iHeight = rc.Height;
            Image myImage = new Bitmap(iWidth, iHeight);
            Graphics.FromImage(myImage).CopyFromScreen(rc.Location, new System.Drawing.Point(0, 0), new System.Drawing.Size(iWidth, iHeight));
            return myImage;
        }
    }

}
