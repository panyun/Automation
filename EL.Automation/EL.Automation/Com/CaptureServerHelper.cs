using Automation.Inspect;
using EL;
using EL.Capturing;
using EL.UIA;
using EL.WindowsAPI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interop.UIAutomationClient;

namespace Automation.Com
{
    public static class CaptureServerHelper
    {
        /// <summary>
        /// 保存截图文件
        /// </summary>
        /// <param name="bitmap">截图文件流</param>
        /// <param name="fileName">文件信息</param>
        /// <returns></returns>
        public static string CaptureSave(Bitmap bitmap, string fileName)
        {
            if (!Directory.Exists(Param.CapturesFilePath))
                Directory.CreateDirectory(Param.CapturesFilePath);
            string filePath = string.Empty;
            try
            {
                filePath = $@"{Param.CapturesFilePath}/{fileName}.png";
                Log.Info($"filePath:{filePath}");
                using (var file = File.Open(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    file.Close();
                }
                File.Delete(filePath);
                bitmap.Save(filePath, ImageFormat.Png);

            }
            catch (Exception)
            {
                filePath = $@"{Param.CapturesFilePath}/{fileName}.Bmp";
                Log.Info($"filePath:{filePath}");
                using (var file = File.Open(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    file.Close();
                }
                File.Delete(filePath);
                bitmap.Save(filePath, ImageFormat.Bmp);
            }
            finally
            {
                bitmap.Dispose();
            }
            return filePath;
        }
        /// <summary>
        /// 程序置顶
        /// </summary>
        /// <param name="element"></param>
        public static void MostTop(IUIAutomationElement element)
        {
            //User32.SetWindowLong(element.CurrentNativeWindowHandle, WindowLongParam.GWL_EXSTYLE, WindowStyles.WS_EX_TOPMOST);
            User32.SetForegroundWindow(element.CurrentNativeWindowHandle);
        }
        /// <summary>
        /// 通过titleName和className查找窗口
        /// </summary>
        /// <param name="titleName">窗口Name</param>
        /// <param name="className">窗口对应的className</param>
        /// <returns></returns>
        public static IUIAutomationElement FindWindow(string titleName, string className)
        {
            IUIAutomationElement element = null;
            var inspect = Boot.GetComponent<WinFormInspectComponent>();
            var capture = inspect.GetComponent<CaptureComponent>();
            var condition1 = inspect.UIAFactory.CreatePropertyCondition(UIA_PropertyIds.UIA_NamePropertyId, titleName);
            var condition2 = inspect.UIAFactory.CreatePropertyCondition(UIA_PropertyIds.UIA_ClassNamePropertyId, className);
            var conditionAnd = inspect.UIAFactory.CreateAndCondition(condition1, condition2);
            var elements = inspect.RootElement.FindAll(TreeScope.TreeScope_Descendants, conditionAnd);
            //if (elements == null || elements.Length == 0)
            //{
            //    IUIAutomationCondition con = default;
            //    if (!string.IsNullOrWhiteSpace(className))
            //        con = inspect.UIAFactory.CreatePropertyCondition(UIA_PropertyIds.UIA_ClassNamePropertyId, className);
            //    else
            //        con = inspect.UIAFactory.CreatePropertyCondition(UIA_PropertyIds.UIA_NamePropertyId,
            //             titleName);
            //    elements = inspect.RootElement.FindAll(TreeScope.TreeScope_Descendants, con);
            //}
            if (elements == null || elements.Length < 0)
            {
                var intptrMain = User32.FindWindow(null, "企业微信");
                if (intptrMain == default)
                    return element;
                element = inspect.UIAFactory.ElementFromHandle(intptrMain);
                return element;
            }
            if (elements != null && elements.Length > 0)
                element = elements.GetElement(0);
            return element;
        }

    }
}
