using EL.WindowsAPI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Interop.UIAutomationClient;

namespace EL.Capturing
{
    public class CaptureComponentAwakeSystem : AwakeSystem<CaptureComponent>
    {
        public override void Awake(CaptureComponent self)
        {
            CaptureComponent.Instance = self;
        }
    }
    /// <summary>
    /// Provides methods to capture the screen, <see cref="AutomationElement"/>s or rectangles on them.
    /// </summary>
    public class CaptureComponent : Entity
    {
        public static CaptureComponent Instance { get; set; }
        public CaptureSettings CaptureSettings { get; set; }
        public CaptureImage CaptureImage { get; set; }
    }
    public static class CaptureComponentSystem
    {

        /// <summary>
        /// Captures the main (primary) screen.
        /// </summary>
        public static CaptureImage MainScreen(this CaptureComponent self)
        {
            var primaryScreenBounds = new Rectangle(
                0, 0,
                User32.GetSystemMetrics(SystemMetric.SM_CXSCREEN), User32.GetSystemMetrics(SystemMetric.SM_CYSCREEN));
            return self.Rectangle(primaryScreenBounds);
        }

        /// <summary>
        /// Captures the whole screen (all monitors).
        /// </summary>
        public static CaptureImage Screen(this CaptureComponent self, int screenIndex = -1)
        {
            Rectangle capturingRectangle;
            // Take the appropriate screen if requested
            if (screenIndex >= 0 && screenIndex < User32.GetSystemMetrics(SystemMetric.SM_CMONITORS))
            {
                var rectangle = self.GetBoundsByScreenIndex(screenIndex);
                capturingRectangle = rectangle;
            }
            else
            {
                // Use the entire desktop
                capturingRectangle = new Rectangle(
                    User32.GetSystemMetrics(SystemMetric.SM_XVIRTUALSCREEN), User32.GetSystemMetrics(SystemMetric.SM_YVIRTUALSCREEN),
                    User32.GetSystemMetrics(SystemMetric.SM_CXVIRTUALSCREEN), User32.GetSystemMetrics(SystemMetric.SM_CYVIRTUALSCREEN));
            }
            return self.Rectangle(capturingRectangle);
        }

        /// <summary>
        /// Captures all screens an element is on.
        /// </summary>
        public static CaptureImage ScreensWithElement(this CaptureComponent self, IUIAutomationElement element)
        {
            var rec = element.CurrentBoundingRectangle;
            var elementRectangle = new Rectangle(rec.left, rec.top, rec.right - rec.left, rec.bottom - rec.top);

            var intersectedScreenBounds = new List<Rectangle>();
            // Calculate which screens intersect with the element
            for (var screenIndex = 0; screenIndex < User32.GetSystemMetrics(SystemMetric.SM_CMONITORS); screenIndex++)
            {
                var screenRectangle = self.GetBoundsByScreenIndex(screenIndex);
                if (screenRectangle.IntersectsWith(elementRectangle))
                {
                    intersectedScreenBounds.Add(screenRectangle);
                }
            }
            if (intersectedScreenBounds.Count > 0)
            {
                var minX = intersectedScreenBounds.Min(x => x.Left);
                var maxX = intersectedScreenBounds.Max(x => x.Right);
                var minY = intersectedScreenBounds.Min(x => x.Top);
                var maxY = intersectedScreenBounds.Max(x => x.Bottom);
                var captureRect = new Rectangle(minX, minY, maxX - minX, maxY - minY);
                return self.Rectangle(captureRect);
            }
            // Fallback to the whole screen
            return self.Screen();
        }

        /// <summary>
        /// Captures an element and returns the image.
        /// </summary>
        public static CaptureImage Element(this CaptureComponent self, IUIAutomationElement element)
        {
            var rec = element.CurrentBoundingRectangle;
            var widthMax = User32.GetSystemMetrics(SystemMetric.SM_CXSCREEN);
            var heightMax = User32.GetSystemMetrics(SystemMetric.SM_CYSCREEN);
            rec.left = Math.Max(0, rec.left);
            rec.top = Math.Max(0, rec.top);
            rec.right = Math.Min(widthMax, rec.right);
            rec.bottom = Math.Min(heightMax, rec.bottom);
            var bounds = new Rectangle(rec.left, rec.top, rec.right - rec.left, rec.bottom - rec.top);
            return self.Rectangle(bounds);
        }

        /// <summary>
        /// Captures an element and returns the image. 无遮挡
        /// </summary>
        public static CaptureImage ElementEx(this CaptureComponent self, IUIAutomationElement element)
        {
            var rec = element.CurrentBoundingRectangle;
            var bounds = new Rectangle(rec.left, rec.top, rec.right - rec.left, rec.bottom - rec.top);

            return self.CaptureWindowToBitmap(element.CurrentNativeWindowHandle);
        }

        /// <summary>
        /// Captures a rectangle inside an element and returns the image.
        /// </summary>
        public static CaptureImage ElementRectangle(this CaptureComponent self, IUIAutomationElement element, Rectangle rectangle)
        {
            var rec = element.CurrentBoundingRectangle;
            var elementBounds = new Rectangle(rec.left, rec.top, rec.right - rec.left, rec.bottom - rec.top);
            // Calculate the rectangle that should be captured
            var capturingRectangle = new Rectangle(elementBounds.Left + rectangle.Left, elementBounds.Top + rectangle.Top, rectangle.Width, rectangle.Height);
            // Check if the element contains the rectangle that should be captured
            if (!elementBounds.Contains(capturingRectangle))
            {
                throw new Exception($"The given rectangle ({capturingRectangle}) is out of bounds of the element ({elementBounds}).");
            }
            return self.Rectangle(capturingRectangle);
        }

        /// <summary>
        /// Captures a specific area from the screen.
        /// </summary>
        public static CaptureImage Rectangle(this CaptureComponent self, Rectangle bounds)
        {

            // Calculate the size of the output rectangle
            var outputRectangle = CaptureComponentHelper.ScaleAccordingToSettings(bounds, self.CaptureSettings);

            Bitmap bmp;
            if (outputRectangle.Width == bounds.Width || outputRectangle.Height == bounds.Height)
            {
                // Capture directly without any resizing
                bmp = self.CaptureDesktopToBitmap(bounds.Width, bounds.Height, (dest, src) =>
                {
                    Gdi32.BitBlt(dest, outputRectangle.X, outputRectangle.Y, outputRectangle.Width, outputRectangle.Height, src, bounds.X, bounds.Y, CopyPixelOperation.SourceCopy | CopyPixelOperation.CaptureBlt);
                });
                //bmp.Save(@"d:\temp\test.png", ImageFormat.Png);
            }
            else
            {
                //  Capture with scaling
                bmp = self.CaptureDesktopToBitmap(outputRectangle.Width, outputRectangle.Height, (dest, src) =>
                {
                    Gdi32.SetStretchBltMode(dest, StretchMode.STRETCH_HALFTONE);
                    Gdi32.StretchBlt(dest, outputRectangle.X, outputRectangle.Y, outputRectangle.Width, outputRectangle.Height, src, bounds.X, bounds.Y, bounds.Width, bounds.Height, TernaryRasterOperations.SRCCOPY | TernaryRasterOperations.CAPTUREBLT);
                });
            }
            self.CaptureImage = new CaptureImage(bmp, bounds, self.CaptureSettings);
            return self.CaptureImage;
        }
       
        private static Rectangle GetBoundsByScreenIndex(this CaptureComponent self, int screenIndex)
        {
            var monitors = new List<MonitorInfo>();
            User32.EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, MonitorDelegate, IntPtr.Zero);
            var monitorRect = monitors[screenIndex].monitor;
            return new Rectangle(monitorRect.left, monitorRect.top, monitorRect.right - monitorRect.left, monitorRect.bottom - monitorRect.top);

            bool MonitorDelegate(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData)
            {
                var mi = new MonitorInfo();
                mi.size = (uint)Marshal.SizeOf(mi);
                var success = User32.GetMonitorInfo(hMonitor, ref mi);
                monitors.Add(mi);
                return true;
            }
        }

        private static Bitmap CaptureDesktopToBitmap(this CaptureComponent self, int width, int height, Action<IntPtr, IntPtr> action)
        {
            // Use P/Invoke because of: https://stackoverflow.com/a/3072580/1069200
            var hDesk = User32.GetDesktopWindow();
            var hSrc = User32.GetWindowDC(hDesk);
            var hDest = Gdi32.CreateCompatibleDC(hSrc);
            var hBmp = Gdi32.CreateCompatibleBitmap(hSrc, width, height);
            var hPrevBmp = Gdi32.SelectObject(hDest, hBmp);
            action(hDest, hSrc);
            var bmp = Image.FromHbitmap(hBmp);
            Gdi32.SelectObject(hDest, hPrevBmp);
            Gdi32.DeleteObject(hBmp);
            Gdi32.DeleteDC(hDest);
            User32.ReleaseDC(hDesk, hSrc);
            return bmp;
        }

        /// <summary>
        /// 指定窗口截图  无遮挡
        /// </summary>
        /// <param name="handle">窗口句柄. (在windows应用程序中, 从Handle属性获得)</param>
        /// <returns></returns>
        public static CaptureImage CaptureWindowToBitmap(this CaptureComponent self, IntPtr handle)
        {
            IntPtr hdcSrc = User32.GetWindowDC(handle);
            User32.RECT windowRect = new User32.RECT();
            User32.GetWindowRect(handle, ref windowRect);
            int width = windowRect.Right - windowRect.Left;
            int height = windowRect.Bottom - windowRect.Top;
            IntPtr hdcDest = Gdi32.CreateCompatibleDC(hdcSrc);
            IntPtr hBitmap = Gdi32.CreateCompatibleBitmap(hdcSrc, width, height);
            IntPtr hOld = Gdi32.SelectObject(hdcDest, hBitmap);
            Gdi32.BitBlt(hdcDest, 0, 0, width, height, hdcSrc, 0, 0, CopyPixelOperation.SourceCopy);
            Gdi32.SelectObject(hdcDest, hOld);
            Gdi32.DeleteDC(hdcDest);
            User32.ReleaseDC(handle, hdcSrc);
            Bitmap img = Image.FromHbitmap(hBitmap);
            Gdi32.DeleteObject(hBitmap);
            var bounds = new Rectangle(windowRect.Left, windowRect.Top, windowRect.Right - windowRect.Left, windowRect.Bottom - windowRect.Top);
            self.CaptureImage = new CaptureImage(img, bounds, self.CaptureSettings);
            return self.CaptureImage;
        }
    }
}
