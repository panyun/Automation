using Automation.Inspect;
using EL;
using EL.Async;
using EL.Input;
using EL.WindowsAPI;
using Interop.UIAutomationClient;
using NPOI.SS.Formula.Functions;
using System;
using System.Data;
using System.Diagnostics;
using Log = EL.Log;

namespace Automation.Parser
{
    public static class ParserComponentSystem
    {
        public static async ELTask MouseAction(this ParserComponent self, string param)
        {
            var json = param.Substring(5);
            var request = JsonHelper.FromJson<MouseActionRequest>(json);
            await self.MouseAction(request);
        }
        public static async ELTask MouseAction(this ParserComponent self, MouseActionRequest request)
        {
            try
            {
                if (request.ElementPath == null) throw new ParserException("传入参数的元素节点信息不正确！");
                await request.Main();
            }
            catch (ParserException ex)
            {
                var obj = new MouseActionResponse().GetFail(ex);
                self.SetResult(obj);
                return;
            }
            catch (Exception ex)
            {
                var obj = new MouseActionResponse().GetFail(ex);
                self.SetResult(obj);
                return;
            }
            var res = new MouseActionResponse();
            self.SetResult(res);
        }
        public static void InputAction(this ParserComponent self, string param)
        {
            var inspect = Boot.GetComponent<InspectComponent>();
            Log.Trace($"InputAction Start");
            var json = param.Substring(5);
            Log.Trace($"InputAction FromJson");
            var request = JsonHelper.FromJson<InputActionRequest>(json);
            self.InputAction(request);
        }
        public static async ELTask InputAction(this ParserComponent self, InputActionRequest request)
        {
            var inspect = Boot.GetComponent<InspectComponent>();
            try
            {

                if (request.ElementPath == null)
                    throw new ParserException("传入参数的元素节点信息不正确！");
                Log.Trace($"InputAction exec");
                await request.Main();
                Log.Trace($"InputAction end");
            }
            catch (ParserException ex)
            {
                var obj = new InputActionResponse().GetFail(ex);
                self.SetResult(obj);
                return;
            }
            catch (Exception ex)
            {
                var obj = new InputActionResponse().GetFail(ex);
                self.SetResult(obj);
                return;
            }
            var res = new InputActionResponse();
            self.SetResult(res);
        }
        public static void ElementPropertyAction(this ParserComponent self, string param)
        {
            ElementPath path = null;
            var inspect = Boot.GetComponent<InspectComponent>();
            Log.Trace($"ElementPropertyAction Start");
            var json = param.Substring(5);
            Log.Trace($"ElementPropertyAction FromJson");
            var request = JsonHelper.FromJson<ElementPropertyActionRequest>(json);
            self.ElementPropertyAction(request);
        }
        public static void ElementPropertyAction(this ParserComponent self, ElementPropertyActionRequest request)
        {
            ElementPath path = null;
            var inspect = Boot.GetComponent<InspectComponent>();
            try
            {
                if (request.ElementPath == null)
                    throw new ParserException("传入参数的元素节点信息不正确！");
                Log.Trace($"ElementPropertyAction exec");
                path = request.Main();
                Log.Trace($"ElementPropertyAction end");
            }
            catch (ParserException ex)
            {
                var obj = new ElementPropertyActionResponse().GetFail(ex);
                self.SetResult(obj);
                return;
            }
            catch (Exception ex)
            {
                var obj = new ElementPropertyActionResponse().GetFail(ex);
                self.SetResult(obj);
                return;
            }
            ElementPropertyActionResponse elementActionResponse = new ElementPropertyActionResponse();
            elementActionResponse.ElementPath = path;
            self.SetResult(elementActionResponse);
        }
        public static void ElementAction(this ParserComponent self, string param)
        {
            ElementPath path = null;
            var inspect = Boot.GetComponent<InspectComponent>();
            Log.Trace($"ElementAction Start");
            var json = param.Substring(5);
            Log.Trace($"ElementAction FromJson");
            var request = JsonHelper.FromJson<ElementActionRequest>(json);
            self.ElementAction(request);
        }
        public static async void ElementAction(this ParserComponent self, ElementActionRequest request)
        {
            ElementPath path = null;
            var inspect = Boot.GetComponent<InspectComponent>();
            try
            {
                if (request.ElementPath == null)
                    throw new ParserException("传入参数的元素节点信息不正确！");
                Log.Trace($"ElementAction exec");
                path = await request.Main();
                Log.Trace($"ElementAction end");
            }
            catch (ParserException ex)
            {
                var obj = new ElementActionResponse().GetFail(ex);
                self.SetResult(obj);
                return;
            }
            catch (Exception ex)
            {
                var obj = new ElementActionResponse().GetFail(ex);
                self.SetResult(obj);
                return;
            }
            ElementActionResponse elementActionResponse = new ElementActionResponse();
            elementActionResponse.ElementPath = path;
            self.SetResult(elementActionResponse);
        }
        public static void SimilarElementAction(this ParserComponent self, string param)
        {
            List<ElementUIA> elements = null;
            var inspect = Boot.GetComponent<InspectComponent>();
            SimilarElementActionRequest request = default;
            Log.Trace($"SimilarElementAction Start");
            var json = param.Substring(5);
            Log.Trace($"SimilarElementAction FromJson");
            request = JsonHelper.FromJson<SimilarElementActionRequest>(json);
            self.SimilarElementAction(request);
        }
        public static void SimilarElementAction(this ParserComponent self, SimilarElementActionRequest request)
        {
            List<ElementUIA> elements = null;
            List<ElementPath> elementPaths = new List<ElementPath>();
            var inspect = Boot.GetComponent<InspectComponent>();
            var winfromInspect = inspect.GetComponent<WinFormInspectComponent>();
            var pathComponent = winfromInspect.GetComponent<WinPathComponent>();
            try
            {
                if (request.ElementPath == null)
                    throw new ParserException("传入参数的元素节点信息不正确！");
                Log.Trace($"SimilarElementAction exec");
                elements = request.Main().Cast<ElementUIA>().ToList();
                if (request.IsElementPath)
                {
                    foreach (var item in elements)
                    {

                        var runtimePath = pathComponent.GetPathInfo_ByRuntime(item.NativeElement, request.ElementPath);
                        elementPaths.Add(runtimePath);
                    }
                }
                Log.Trace($"SimilarElementAction end");
            }
            catch (ParserException ex)
            {
                var obj = new SimilarElementActionResponse().GetFail(ex);
                self.SetResult(obj);
                return;
            }
            catch (Exception ex)
            {
                var obj = new SimilarElementActionResponse().GetFail(ex);
                self.SetResult(obj);
                return;
            }
            SimilarElementActionResponse elementActionResponse = new SimilarElementActionResponse
            {
                Elements = elements,
                ElementPaths = elementPaths
            };
            self.SetResult(elementActionResponse);
        }

        public static void GenerateCosineSimilarAction(this ParserComponent self, string param)
        {
            ElementPath elementPath = default;
            var inspect = Boot.GetComponent<InspectComponent>();
            GenerateCosineSimilarActionRequest request = default;
            int count = default;
            Log.Trace($"SimilarElementAction Start");
            var json = param.Substring(5);
            Log.Trace($"SimilarElementAction FromJson");
            request = JsonHelper.FromJson<GenerateCosineSimilarActionRequest>(json);
            self.GenerateCosineSimilarAction(request);
        }
        public static void GenerateCosineSimilarAction(this ParserComponent self, GenerateCosineSimilarActionRequest request)
        {
            ElementPath elementPath = default;
            var inspect = Boot.GetComponent<InspectComponent>();
            int count = default;
            try
            {

                if (request.ElementPath == null)
                    throw new ParserException("传入参数的元素节点信息不正确！");
                Log.Trace($"SimilarElementAction exec");
                (elementPath, count, var elements) = request.Main();
                if (count == 0 || elements == default || elements.Count == 0)
                    throw new ParserException("相似元素生成失败！");
                Log.Trace($"SimilarElementAction end");
            }
            catch (ParserException ex)
            {
                var obj = new GenerateCosineSimilarActionResponse().GetFail(ex);
                self.SetResult(obj);
                return;
            }
            catch (Exception ex)
            {
                var obj = new GenerateCosineSimilarActionResponse().GetFail(ex);
                self.SetResult(obj);
                return;
            }
            GenerateCosineSimilarActionResponse elementActionResponse = new GenerateCosineSimilarActionResponse
            {
                ElementPath = elementPath,
                Count = count
            };
            self.SetResult(elementActionResponse);
        }
        public static void CosineSimilarElementAction(this ParserComponent self, string param)
        {
            ElementPath elementPath = default;
            var inspect = Boot.GetComponent<InspectComponent>();
            CosineSimilarElementActionRequest request = default;
            int count = default;
            Log.Trace($"SimilarElementAction Start");
            var json = param.Substring(5);
            Log.Trace($"SimilarElementAction FromJson");
            request = JsonHelper.FromJson<CosineSimilarElementActionRequest>(json);
            self.CosineSimilarElementAction(request);
        }
        public static void CosineSimilarElementAction(this ParserComponent self, CosineSimilarElementActionRequest request)
        {
            List<ElementUIA> elements = null;
            List<ElementPath> elementPaths = new List<ElementPath>();
            var inspect = Boot.GetComponent<InspectComponent>();
            var winfromInspect = inspect.GetComponent<WinFormInspectComponent>();
            var pathComponent = winfromInspect.GetComponent<WinPathComponent>();
            try
            {
                if (request.ElementPath == null)
                    throw new ParserException("传入参数的元素节点信息不正确！");
                Log.Trace($"SimilarElementAction exec");
                elements = request.Main().Cast<ElementUIA>().ToList();
                if (request.IsElementPath)
                {
                    foreach (var item in elements)
                    {
                        var runtimePath = pathComponent.GetPathInfo_ByRuntime(item.NativeElement, request.ElementPath);
                        elementPaths.Add(runtimePath);
                    }
                }
                Log.Trace($"SimilarElementAction end");
            }
            catch (ParserException ex)
            {
                var obj = new CosineSimilarElementActionResponse().GetFail(ex);
                self.SetResult(obj);
                return;
            }
            catch (Exception ex)
            {
                var obj = new CosineSimilarElementActionResponse().GetFail(ex);
                self.SetResult(obj);
                return;
            }
            CosineSimilarElementActionResponse elementActionResponse = new()
            {
                Elements = elements,
                ElementPaths = elementPaths
            };
            self.SetResult(elementActionResponse);
        }
        public static void ElementVerificationAction(this ParserComponent self, string param)
        {
            ElementPath elementPath = null;
            var inspect = Boot.GetComponent<InspectComponent>();
            Log.Trace($"InputAction Start");
            var json = param.Substring(5);
            Log.Trace($"InputAction FromJson");
            var request = JsonHelper.FromJson<ElementVerificationActionRequest>(json);
            self.ElementVerificationAction(request);
        }
        public static void ElementVerificationAction(this ParserComponent self, ElementVerificationActionRequest request)
        {
            ElementPath elementPath = null;

            try
            {
                if (request.ElementPath == null)
                    throw new ParserException("传入参数的元素节点信息不正确！");
                Log.Trace($"InputAction exec");
                elementPath = request.Main();
                if (elementPath == null) throw new ParserException("未定位到目标元素！");
                Log.Trace($"InputAction end");
            }
            catch (ParserException ex)
            {
                var obj = new ElementVerificationActionResponse().GetFail(ex);
                self.SetResult(obj);
                return;
            }
            catch (Exception ex)
            {
                var obj = new ElementVerificationActionResponse().GetFail(ex);
                self.SetResult(obj);
            }
            ElementVerificationActionResponse elementVerificationActionResponse = new ElementVerificationActionResponse();
            elementVerificationActionResponse.ElementPath = elementPath;
            self.SetResult(elementVerificationActionResponse);
        }

        public static void SetResult(this ParserComponent self, IResponse response)
        {
            if (!InspectRequestManager.FinishAction.IsCompleted)
                InspectRequestManager.FinishAction.SetResult(response);
            var inspect = Boot.GetComponent<InspectComponent>();
        }
        public static void StartMsgAction(this ParserComponent self, string param)
        {
            var inspect = Boot.GetComponent<InspectComponent>();
            Log.Trace($"IdentifyMsgAction Start");
            var json = param.Substring(5);
            Log.Trace($"IdentifyMsgAction FromJson");
            var request = JsonHelper.FromJson<StartMsgActionRequest>(json);
            self.StartMsgAction(request);
        }
        public static StartMsgActionResponse StartMsgAction(this ParserComponent self, StartMsgActionRequest request)
        {
            CatchWXIdentifyMsgAction current = default;
            var inspect = Boot.GetComponent<InspectComponent>();
            try
            {
                if (request.ElementPath == null)
                    throw new ParserException("传入参数的元素节点信息不正确！");
                Log.Trace($"InputAction exec");
                current = request.Main();
                Log.Trace($"InputAction end");
            }
            catch (ParserException ex)
            {
                var obj = new StartMsgActionResponse().GetFail(ex);
                self.SetResult(obj);
                return (StartMsgActionResponse)obj;
            }
            catch (Exception ex)
            {
                var obj = new StartMsgActionResponse().GetFail(ex);
                self.SetResult(obj);
                return (StartMsgActionResponse)obj;
            }
            StartMsgActionResponse elementVerificationActionResponse = new()
            {
                TaskId = current.Id
            };
            self.SetResult(elementVerificationActionResponse);
            return elementVerificationActionResponse;
        }
        public static void GenerateSimilarElementAction(this ParserComponent self, string param)
        {
            ElementPath path = null;
            var inspect = Boot.GetComponent<InspectComponent>();
            int count = default;
            Log.Trace($"GenerateSimilarElementRequest Start");
            var json = param.Substring(5);
            Log.Trace($"GenerateSimilarElementRequest FromJson");
            var request = JsonHelper.FromJson<GenerateSimilarElementActionRequest>(json);
            self.GenerateSimilarElementAction(request);
        }
        public static void GenerateSimilarElementAction(this ParserComponent self, GenerateSimilarElementActionRequest request)
        {
            ElementPath path = null;
            int count = default;
            try
            {
                if (request.ElementPath == null || request.LastElementPath == null)
                    throw new ParserException($"{nameof(request.ElementPath)}传入参数的元素节点信息不正确！");
                Log.Trace($"GenerateSimilarElementRequest exec");
                (path, count) = request.Main();
                if (count == default)
                    throw new ParserException("相似元素生成失败！");
                Log.Trace($"GenerateSimilarElementRequest end");
            }
            catch (ParserException ex)
            {
                var obj = new ElementActionResponse().GetFail(ex);
                self.SetResult(obj);
                return;
            }
            catch (Exception ex)
            {
                var obj = new ElementActionResponse().GetFail(ex);
                self.SetResult(obj);
                return;
            }
            GenerateSimilarElementActionResponse generateSimilarElementResponse = new()
            {
                ElementPath = path,
                Count = count,
            };
            self.SetResult(generateSimilarElementResponse);
        }

        public static void GenerateTableAction(this ParserComponent self, GenerateTableActionRequest request)
        {
            DataTable dataTable = default;
            try
            {

                Log.Trace($"GenerateSimilarElementRequest exec");
                dataTable = request.Main();
                Log.Trace($"GenerateSimilarElementRequest end");
            }
            catch (ParserException ex)
            {
                var obj = new GenerateTableActionResponse().GetFail(ex);
                self.SetResult(obj);
                return;
            }
            catch (Exception ex)
            {
                var obj = new GenerateTableActionResponse().GetFail(ex);
                self.SetResult(obj);
                return;
            }
            var json = DataBaseComponentSystem.ConvertJson(dataTable);
            GenerateTableActionResponse generateSimilarElementResponse = new()
            {
                DataTable = dataTable,
                DataTableJson = json
            };
            self.SetResult(generateSimilarElementResponse);
        }
        public static async ELTask GenerateHtmlAction(this ParserComponent self, GenerateHtmlActionRequest request)
        {
            var html = string.Empty;
            try
            {

                Log.Trace($"GenerateSimilarElementRequest exec");
                html = await request.Main();
                Log.Trace($"GenerateSimilarElementRequest end");
            }
            catch (ParserException ex)
            {
                var obj = new GenerateExcelDataActionResponse().GetFail(ex);
                self.SetResult(obj);
                return;
            }
            catch (Exception ex)
            {
                var obj = new GenerateExcelDataActionResponse().GetFail(ex);
                self.SetResult(obj);
                return;
            }

            GenerateHtmlActionResponse generateHtmlActionResponse = new()
            {
                Html = html
            };
            self.SetResult(generateHtmlActionResponse);
        }
        public static async ELTask HighlightAction(this ParserComponent self, HighlightActionRequest request)
        {
            int count = default;
            try
            {

                Log.Trace($"GenerateSimilarElementRequest exec");
                count = await request.Main();
                Log.Trace($"GenerateSimilarElementRequest end");
            }
            catch (ParserException ex)
            {
                var obj = new GenerateExcelDataActionResponse().GetFail(ex);
                self.SetResult(obj);
                return;
            }
            catch (Exception ex)
            {
                var obj = new GenerateExcelDataActionResponse().GetFail(ex);
                self.SetResult(obj);
                return;
            }

            HighlightActionResponse generateHtmlActionResponse = new()
            {
                Count = count
            };
            self.SetResult(generateHtmlActionResponse);
        }
        public static async ELTask OpenBrowserAction(this ParserComponent self, OpenBrowserActionRequest request)
        {
            try
            {
                Log.Trace($"GenerateSimilarElementRequest exec");
                request.Main();
                Log.Trace($"GenerateSimilarElementRequest end");
            }
            catch (ParserException ex)
            {
                var obj = new GenerateExcelDataActionResponse().GetFail(ex);
                self.SetResult(obj);
                return;
            }
            catch (Exception ex)
            {
                var obj = new GenerateExcelDataActionResponse().GetFail(ex);
                self.SetResult(obj);
                return;
            }
            OpenBrowserActionResponse generateHtmlActionResponse = new();
            self.SetResult(generateHtmlActionResponse);
        }

        public static async ELTask ChildsElementAction(this ParserComponent self, ChildsElementActionRequest request)
        {
            List<ElementPath> elementPaths = new();
            var inspect = Boot.GetComponent<InspectComponent>();
            var winfromInspect = inspect.GetComponent<WinFormInspectComponent>();
            var pathComponent = winfromInspect.GetComponent<WinPathComponent>();
            List<ElementUIA> elements;
            try
            {
                if (request.ElementPath == null)
                    throw new ParserException("传入参数的元素节点信息不正确！");
                Log.Trace($"ChildsElementAction exec");
                elements = (await request.Main()).Cast<ElementUIA>().ToList();
                if (request.IsElementPath)
                {
                    foreach (var item in elements)
                    {
                        var runtimePath = pathComponent.GetPathInfo_ByRuntime(item.NativeElement, request.ElementPath);
                        elementPaths.Add(runtimePath);
                    }
                }
                Log.Trace($"ChildsElementAction end");
            }
            catch (ParserException ex)
            {
                var obj = new CosineSimilarElementActionResponse().GetFail(ex);
                self.SetResult(obj);
                return;
            }
            catch (Exception ex)
            {
                var obj = new CosineSimilarElementActionResponse().GetFail(ex);
                self.SetResult(obj);
                return;
            }
            CosineSimilarElementActionResponse elementActionResponse = new()
            {
                Elements = elements,
                ElementPaths = elementPaths
            };
            self.SetResult(elementActionResponse);
        }

        public static async ELTask ParentElementAction(this ParserComponent self, ParentElementActionRequest request)
        {
            List<ElementUIA> elements = null;
            List<ElementPath> elementPaths = new List<ElementPath>();
            var inspect = Boot.GetComponent<InspectComponent>();
            var winfromInspect = inspect.GetComponent<WinFormInspectComponent>();
            var pathComponent = winfromInspect.GetComponent<WinPathComponent>();
            try
            {
                if (request.ElementPath == null)
                    throw new ParserException("传入参数的元素节点信息不正确！");
                Log.Trace($"ParentElementAction exec");
                var path = await request.Main();
                elementPaths.Add(path);
                Log.Trace($"ParentElementAction end");
            }
            catch (ParserException ex)
            {
                var obj = new CosineSimilarElementActionResponse().GetFail(ex);
                self.SetResult(obj);
                return;
            }
            catch (Exception ex)
            {
                var obj = new CosineSimilarElementActionResponse().GetFail(ex);
                self.SetResult(obj);
                return;
            }
            CosineSimilarElementActionResponse elementActionResponse = new()
            {
                Elements = elementPaths.Select(x => x.PathNode.CurrentElementWin).ToList(),
                ElementPaths = elementPaths
            };
            self.SetResult(elementActionResponse);
        }
        public static void GenerateExcelDataAction(this ParserComponent self, GenerateExcelDataActionRequest request)
        {
            DataTable dataTable = default;
            try
            {

                Log.Trace($"GenerateSimilarElementRequest exec");
                dataTable = request.Main();
                Log.Trace($"GenerateSimilarElementRequest end");
            }
            catch (ParserException ex)
            {
                var obj = new GenerateExcelDataActionResponse().GetFail(ex);
                self.SetResult(obj);
                return;
            }
            catch (Exception ex)
            {
                var obj = new GenerateExcelDataActionResponse().GetFail(ex);
                self.SetResult(obj);
                return;
            }
            var json = DataBaseComponentSystem.ConvertJson(dataTable);
            GenerateExcelDataActionResponse generateSimilarElementResponse = new()
            {
                DataTable = dataTable,
                DataTableJson = json
            };
            self.SetResult(generateSimilarElementResponse);
        }

        public static void Focus(this ElementUIA self)
        {
            if (self.NativeElement.CurrentNativeWindowHandle != default)
            {
                var windowHandle = self.NativeElement.CurrentNativeWindowHandle;
                if (windowHandle != Win32Constants.FALSE)
                {
                    uint windowThreadId = User32.GetWindowThreadProcessId(windowHandle, out _);
                    uint currentThreadId = Kernel32.GetCurrentThreadId();

                    // attach window to the calling thread's message queue
                    User32.AttachThreadInput(currentThreadId, windowThreadId, true);
                    User32.SetFocus(windowHandle);
                    // detach the window from the calling thread's message queue
                    User32.AttachThreadInput(currentThreadId, windowThreadId, false);

                    Wait.UntilResponsive(self.NativeElement.CurrentNativeWindowHandle);
                    return;
                }
            }
            // Fallback to the UIA Version
            self.NativeElement.SetFocus();
        }
        /// <summary>
        /// Brings a window to the foreground.
        /// </summary>
        public static void SetForeground(this ElementUIA self)
        {
            if (self == default) return;
            self.NativeElement?.SetForeground();
        }
        public static void SetForeground(this IUIAutomationElement self)
        {
            if (self == default) return;
            var inspect = Boot.GetComponent<WinFormInspectComponent>();
            var processInfo = inspect.GetProcessInfo(self);
            IUIAutomationElement ele = self;
            if (self.CurrentNativeWindowHandle == default)
            {
                ele = self.GetNativeWindowHandle();
            }
            if (ele.CurrentNativeWindowHandle != default)
            {
                if (ele.CurrentNativeWindowHandle != Win32Constants.FALSE)
                {
                    ele.CurrentNativeWindowHandle.SetForeground();
                    return;
                }
            }
            else
            {
                User32.SetForegroundWindow(processInfo.MainWindowHandle);
                Wait.UntilResponsive(processInfo.MainWindowHandle);
            }
        }
        public static void SetForeground(this IntPtr intPtr)
        {
            User32.SetForegroundWindow(intPtr);
            Wait.UntilResponsive(intPtr);
        }
        public static void SetForeground(this ElementJAB self)
        {
            if (self.NativeWindowHandle != default)
            {
                if (self.NativeWindowHandle != Win32Constants.FALSE)
                {
                    self.NativeWindowHandle.SetForeground();
                    return;
                }
            }
        }
        public static void SetForeground(this ElementPlaywright self)
        {
            Process[] process;
            if (self.BrowserType == BrowserType.Chromium)
            {
                process = Process.GetProcessesByName("chrome");
                if (process != null && process.Length > 0)
                {
                    foreach (var item in process)
                    {
                        item.MainWindowHandle.SetForeground();
                        item.MainWindowHandle.MaximizedWindow();
                        Wait.UntilResponsive(item.MainWindowHandle);
                    }
                }
            }
            if (self.BrowserType == BrowserType.Msedge)
            {
                process = Process.GetProcessesByName("msedge");
                if (process != null && process.Length > 0)
                {
                    foreach (var item in process)
                    {
                        if (string.IsNullOrEmpty(item.MainWindowTitle)) continue;
                        if (item.MainWindowTitle.Contains(self.WindowTitle) || self.WindowTitle.Contains(item.MainWindowTitle))
                        {
                            item.MainWindowHandle.SetForeground();
                            item.MainWindowHandle.MaximizedWindow();
                            Wait.UntilResponsive(item.MainWindowHandle);
                        }
                    }
                }
            }
        }
    }
}
