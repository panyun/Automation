using Automation.Inspect;
using EL;
using EL.Capturing;
using EL.WindowsAPI;

namespace Automation.Com
{

    /// <summary>
    /// 窗口句柄截图
    /// </summary>
    public class InspectCaptureServer
    {

        public InspectCaptureServer()
        {
            //创建
            Boot.App = new AppMananger();
            //加载程序集
            Boot.App.EventSystem.Add(typeof(WinFormInspectComponent).Assembly);
            var inspect = Boot.AddComponent<WinFormInspectComponent>();
            inspect.AddComponent<CaptureComponent>();
            //var log = new FileLogger();
            //log.SetLevel(LogLevel.Error);
            //Boot.SetLog(log);
            //Log.Info($"--test");
        }


        /// <summary>
        /// 通过鼠标函数截图
        /// </summary>
        /// <returns></returns>
        public CaptureInfoResponse GetCaptureInfo()
        {
            //获取鼠标的坐标
            User32.GetCursorPos(out var point);
            return GetCaptureInfo(point.X, point.Y);
        }

        /// <summary>
        /// 通过坐标 xy查找element并截图
        /// </summary>
        /// <param name="x">鼠标对应的x坐标</param>
        /// <param name="y">鼠标对应的y坐标</param>
        /// <returns></returns>
        public CaptureInfoResponse GetCaptureInfo(int x, int y)
        {
            CaptureInfoResponse response = new CaptureInfoResponse();
            try
            {
                Log.Info($"--Start  GetCaptureInfo   X: {x}  Y:{y}");
                var inspect = Boot.GetComponent<WinFormInspectComponent>();
                var capture = inspect.GetComponent<CaptureComponent>();
                var element = inspect.UIAFactory.ElementFromPoint(new Interop.UIAutomationClient.tagPOINT() { x = x, y = y });
                element = inspect.GetNativeWindowHandle(element);

                var img = capture.ElementEx(element);
                var filePath = CaptureServerHelper.CaptureSave(img.Bitmap,$"{IdGenerater.Instance.GenerateId()}");
                response = new CaptureInfoResponse()
                {
                    TitleName = element.CurrentName,
                    X = element.CurrentBoundingRectangle.left,
                    Y = element.CurrentBoundingRectangle.top,
                    FileName = filePath,
                    ClassName = element.CurrentClassName,
                    CurrIntptr = element.CurrentNativeWindowHandle,
                    ProcessId = element.CurrentProcessId
                };
                Log.Info($"--End    GetCaptureInfo  \r\n Info: {JsonHelper.ToJson(response)}");
                return response;
            }
            catch (Exception ex)
            {
                response.SetError(ex.Message, 1);
                Log.Info($"--End    GetCaptureInfo  \r\n Ex: {ex.Message} \r\n Trace: {ex.StackTrace}");
            }
            return response;
        }


        /// <summary>
        /// 通过名称和className查找节点并截图
        /// </summary>
        /// <param name="titleName">窗口名称</param>
        /// <param name="className">窗口对应的Class名称</param>
        /// <returns></returns>
        public CaptureInfoResponse GetCaptureInfo(string titleName, string className)
        {
            CaptureInfoResponse response = new CaptureInfoResponse();
            try
            {
                Log.Info($"--Start  GetCaptureInfo   titleName: {titleName}  className:{className}");
                var inspect = Boot.GetComponent<WinFormInspectComponent>();
                var capture = inspect.GetComponent<CaptureComponent>();
                var element = CaptureServerHelper.FindWindow(titleName, className);
                if (element == null)
                {
                    Log.Error($"通过titleName和className查找窗口失败！titleName:{titleName},className:{className}");
                    response.SetError($"通过titleName和className查找窗口失败！titleName:{titleName},className:{className}", 1);
                    return response;
                }
                element = inspect.GetNativeWindowHandle(element);
                var img = capture.ElementEx(element);
                var filePath = CaptureServerHelper.CaptureSave(img.Bitmap,$"{IdGenerater.Instance.GenerateId()}");
                response = new CaptureInfoResponse()
                {
                    TitleName = element.CurrentName,
                    X = element.CurrentBoundingRectangle.left,
                    Y = element.CurrentBoundingRectangle.top,
                    FileName = filePath,
                    ClassName = element.CurrentClassName,
                    CurrIntptr = element.CurrentNativeWindowHandle,
                    ProcessId = element.CurrentProcessId
                };
                Log.Info($"--End    GetCaptureInfo  \r\n Info: {JsonHelper.ToJson(response)}");
                return response;
            }
            catch (Exception ex)
            {
                response.SetError(ex.Message, 1);
                Log.Info($"--End    GetCaptureInfo  \r\n Ex: {ex.Message} \r\n Trace: {ex.StackTrace}");
            }
            return response;
        }

        /// <summary>
        /// 设置截图文件夹路径
        /// </summary>
        /// <param name="path">截图文件夹的相对路径../ 或 cap</param>
        /// <returns></returns>
        public Response SetCapturePath(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                Param.CapturesFilePath = path;
                Log.Info("设置文件路径：" + path);
            }

            return default(Response);
        }

        /// <summary>
        /// 设置日记文件夹路径
        /// </summary>
        /// <param name="path">日记文件夹的相对路径../log 或 log</param>
        /// <returns></returns>
        public Response SetLogPath(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                Boot.SetLog(new FileLogger(path));
                Log.Info("设置日记路径：" + path);
            }
            return default(Response);
        }

    }
}
