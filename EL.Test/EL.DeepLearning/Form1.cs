using Automation;
using Automation.Inspect;
using EL.Async;
using EL.Capturing;
using EL.DeepLearning.Common;
using EL.Overlay;
using EL.UIA;
using Interop.UIAutomationClient;
using NPOI.OpenXmlFormats.Dml.Chart;
using NPOI.OpenXmlFormats.Dml.Diagram;
using NPOI.OpenXmlFormats.Vml;
using NPOI.SS.Formula.Functions;
using NPOI.XSSF.Streaming.Values;
using SixLabors.Fonts;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Windows.Automation;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using static EL.UIA.ControlTypeConverter;

namespace EL.DeepLearning;

public partial class Form1 : Form
{
    static IUIAutomationElement Root;
    IUIAutomationElement ElementFromHandle;
    static Process Process;
    int HotKeyID = 12345;
    public Form1()
    {
        InitializeComponent();
    }
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        HotkeyComponent.UnregisterHotKey(this.Handle, HotKeyID);
        var result = HotkeyComponent.RegisterHotKey(this.Handle, HotKeyID, HotkeyModifiers.MOD_CONTROL | HotkeyModifiers.MOD_ALT, Keys.None);

    }
    protected override void OnClosing(CancelEventArgs e)
    {
        base.OnClosing(e);
        HotkeyComponent.UnregisterHotKey(this.Handle, HotKeyID);
    }
    protected override void WndProc(ref Message m)
    {
        base.WndProc(ref m);
        const int WM_HOTKEY = 0x0312;
        if (m.Msg == WM_HOTKEY)
        {
            if (m.WParam.ToInt32() == HotKeyID)
            {
                Task.Run(() =>
                {
                    this.Invoke(new Action(() =>
                    {
                        button2_Click(null, null);
                    }));
                });
            }
        }
    }
    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        HotkeyComponent.UnregisterHotKey(this.Handle, HotKeyID);
    }
    private async void button1_ClickAsync(object sender, EventArgs e)
    {
        ElementFromHandle = null;
        Process = null;
		CatchUIRequest catchWindowRequest = new CatchUIRequest()
        {
            Msg = "打开界面探测器"
        };
        var catchWindowResponse = (CatchUIResponse)await InspectRequestManager.StartAsync(catchWindowRequest);
        var inspect = Boot.GetComponent<InspectComponent>().GetComponent<WinFormInspectComponent>();
        if (Root == null)
        {
            Root = inspect.UIAFactory.GetRootElement();
        }
        Process = Process.GetProcessById(catchWindowResponse.CurrentElement.ElementUIA.CurrentProcessId);
        ElementFromHandle = inspect.UIAFactory.ElementFromHandle(catchWindowResponse.CurrentElement.ElementUIA.GetNativeWindowHandle().CurrentNativeWindowHandle);
        if (catchWindowResponse.CurrentElement != null && catchWindowResponse.ElementPath != null && ElementFromHandle != null)
        {
            Update(ElementFromHandle, catchWindowResponse.ElementPath.ElementType, Process.MainWindowTitle, Process.ProcessName, Process.Id, catchWindowResponse.ElementPath.BoundingRectangle);
        }
        else
        {
            MessageBox.Show("没有获取到数据!");
        }
    }
    private void Update(IUIAutomationElement ElementFromHandle, ElementType ProgramType, string MainWindowTitle, string ProcessName, int ProcessId, Rectangle BoundingRectangle)
    {
        this.Invoke(new Action(() =>
        {
            listBox1.Items.Clear();
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add(nameof(Process.MainWindowTitle), MainWindowTitle);
            dic.Add(nameof(Process.ProcessName), ProcessName);
            dic.Add(nameof(ProgramType), ProgramType);
            dic.Add(nameof(ProcessId), ProcessId);
            dic.Add(nameof(ElementFromHandle.CurrentName), ElementFromHandle.CurrentName);
            dic.Add("BoundingRectangle", $"X:{BoundingRectangle.X} Y:{BoundingRectangle.Y} Width:{BoundingRectangle.Width} Height:{BoundingRectangle.Height}");

            var maxLength = dic.Keys.Max(t => t.Length);
            foreach (var item in dic)
            {
                listBox1.Items.Add($"{item.Key.PadRight(maxLength, ' ')} {item.Value}");
            }
        }));
    }
    private async void button2_Click(object sender, EventArgs e)
    {
        if (ElementFromHandle != null && Process != null)
        {
            this.Invoke(new Action(() =>
            {
                label2.Text = "采集中...";
            }));
            await Task.Run(() =>
            {

                var CurrentEle = GetRootWindow(ElementFromHandle);
                var isWeb = false;
                if (CurrentEle == null)
                {
                    CurrentEle = ElementFromHandle;
                }
                if (CurrentEle.GetClassName() == "Chrome_WidgetWin_1")
                {
                    isWeb = true;
                }
                CurrentEle = GetRealEle(CurrentEle, isWeb);
                var rec = CurrentEle.ToRectangle();
                //Win32Helper.SetTop(CurrentEle.CurrentNativeWindowHandle);
                var result = new CatchImageManage().Process(CurrentEle, Process.ProcessName);
                //Win32Helper.SetTop(CurrentEle.CurrentNativeWindowHandle, false);
                this.Invoke(new Action(() =>
                {
                    this.TopMost = true;
                    if (result.success)
                    {
                        CatchImageManage.Save(result.captureImage, result.window, result.ProcessName);
                        this.Invoke(new Action(() =>
                        {
                            label2.Text = "采集完毕";
                        }));
                    }
                    else
                    {
                        this.Invoke(new Action(() =>
                        {
                            label2.Text = "采集失败";
                        }));
                    }
                }));
            });
        }
        else
        {
            this.Invoke(new Action(() =>
            {
                label2.Text = "请先捕获数据";
            }));
        }
    }
    private static IUIAutomationElement GetRootWindow(IUIAutomationElement CurrentEle)
    {
        var type = EL.UIA.ControlTypeConverter.ToControlType(CurrentEle.CurrentControlType);
        if (type != EL.UIA.ControlTypeConverter.ControlType.Window)
        {
            var subEle = CurrentEle;
            var list = new List<IUIAutomationElement>();
            IUIAutomationElement before = null;
            var id = Root.CurrentProcessId;
            while (subEle != null)
            {
                subEle = WinFormInspectComponent.Instance.ControlViewWalker.GetParentElement(subEle);
                if (subEle != null)
                {
                    if (subEle.CurrentProcessId != id)
                    {
                        before = subEle;
                    }
                    var subType = EL.UIA.ControlTypeConverter.ToControlType(subEle.CurrentControlType);
                    if (subType == EL.UIA.ControlTypeConverter.ControlType.Window)
                    {
                        return subEle;
                    }
                }
                else
                {
                    return before;
                }
            }
        }
        return CurrentEle;
    }
    private static IUIAutomationElement GetRealEle(IUIAutomationElement CurrentEle, bool isWeb = false)
    {
        var reCurrentEle = CurrentEle;
        var inspect = Boot.GetComponent<InspectComponent>().GetComponent<WinFormInspectComponent>();

        if (isWeb)
        {
            var webChildren = CurrentEle.FindAll(TreeScope.TreeScope_Descendants, inspect.UIAFactory.CreatePropertyCondition(UIA_PropertyIds.UIA_ControlTypePropertyId, UIA_ControlTypeIds.UIA_DocumentControlTypeId));
            if (webChildren?.Length > 0)
            {
                for (int i = 0; i < webChildren.Length; i++)
                {
                    var subEle = webChildren.GetElement(i);
                    return subEle;
                    //var rec = subEle.ToRectangle();
                    //Show(rec);
                }
            }
        }
        var rect = CurrentEle.ToRectangle();
        var children = CurrentEle.FindAll(TreeScope.TreeScope_Children, inspect.UIAFactory.CreateTrueCondition());
        if (children?.Length > 0)
        {
            try
            {
                for (int i = 0; i < children.Length; i++)
                {
                    var subEle = children.GetElement(i);
                    try
                    {
                        var subRec = subEle.ToRectangle();
                        var vWidth = rect.Width - subRec.Width;
                        var vHeight = rect.Height - subRec.Height;

                        var v = Math.Abs(vWidth - vHeight);
                        if (v <= 3)
                        {
                            reCurrentEle = subEle;
                            break;
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            catch (Exception)
            {
            }
        }
        return reCurrentEle;
    }
    //private static IUIAutomationElement GetRealEle(IUIAutomationElement CurrentEle)
    //{
    //    var reCurrentEle = CurrentEle;
    //    var inspect = Boot.GetComponent<InspectComponent>().GetComponent<WinFormInspectComponent>();
    //    var rect = CurrentEle.ToRectangle();
    //    var children = CurrentEle.FindAll(TreeScope.TreeScope_Children, inspect.UIAFactory.CreateTrueCondition());
    //    if (children?.Length > 0)
    //    {
    //        try
    //        {
    //            for (int i = children.Length; i > -1; i--)
    //            {
    //                var subEle = children.GetElement(i - 1);
    //                try
    //                {
    //                    var subRec = subEle.ToRectangle();
    //                    var vWidth = rect.Width - subRec.Width;
    //                    var vHeight = rect.Height - subRec.Height;

    //                    var v = Math.Abs(vWidth - vHeight);
    //                    if (v <= 8)
    //                    {
    //                        reCurrentEle = subEle;
    //                        break;
    //                    }
    //                }
    //                catch (Exception)
    //                {
    //                }
    //            }
    //        }
    //        catch (Exception)
    //        {
    //        }
    //    }
    //    return reCurrentEle;
    //}
    public static void Show(Rectangle rectangle, int delay = 2000)
    {
        var formOver = Boot.GetComponent<InspectComponent>().GetComponent<FormOverLayComponent>();
        formOver.LightHighShow(System.Drawing.Color.Red, rectangle);
        Thread.Sleep(delay);
        formOver.LightHighHide();
    }

    private async void button4_Click_1(object sender, EventArgs e)
    {
        if (ElementFromHandle != null && Process != null)
        {
            this.Invoke(new Action(() =>
            {
                label2.Text = "采集中...";
            }));
            await Task.Run(() =>
            {
                //Win32Helper.SetTop(Process.MainWindowHandle);
                var result = new CatchImageManage().SmartProcess(ElementFromHandle, Process);
                //Win32Helper.SetTop(Process.MainWindowHandle, false);
                this.Invoke(new Action(() =>
                {
                    this.TopMost = true;
                    if (result)
                    {
                        this.Invoke(new Action(() =>
                        {
                            label2.Text = "采集完毕";
                        }));
                    }
                    else
                    {
                        this.Invoke(new Action(() =>
                        {
                            label2.Text = "采集失败";
                        }));
                    }
                }));
            });
        }
        else
        {
            this.Invoke(new Action(() =>
            {
                label2.Text = "请先捕获数据";
            }));
        }
    }

    private void button3_Click_1(object sender, EventArgs e)
    {
        new Form2().Show();
    }


    [DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
    public static extern IntPtr GetParent(IntPtr hWnd);

    public delegate bool EnumChildProc(IntPtr hwndChild, ref IntPtr lParam);

    [DllImport("User32.dll")]
    public static extern bool EnumChildWindows(IntPtr hWndParent, EnumChildProc lpEnumFunc, ref IntPtr lParam);

    private delegate bool EnumWindowsProc(IntPtr hWnd, int lParam);

    [DllImport("user32.dll")]
    private static extern bool EnumWindows(EnumWindowsProc enumFunc, int lParam);

    private delegate bool EnumThreadDelegate(IntPtr hWnd, IntPtr lParam);
    [DllImport("user32.dll")]
    private static extern bool EnumThreadWindows(uint dwThreadId, EnumThreadDelegate lpfn, IntPtr lParam);

    IUIAutomationElement element = null;
}

//List<IntPtr> windows = new List<IntPtr>(); // create a list to store window handles
//EnumWindows(delegate (IntPtr hWnd, int lParam) // enumerate all top-level windows
//{
//    uint windowProcessId;
//    GetWindowThreadProcessId(hWnd, out windowProcessId); // get window process ID

//    if (windowProcessId == Process.Id) // check if window belongs to current process
//    {
//        windows.Add(hWnd); // add window handle to list
//    }

//    return true; // continue enumeration
//}, 0);

//foreach (var win in windows)
//{
//    var element = inspect.UIAFactory.ElementFromHandle(win);
//    var rec = element.ToRectangle();
//    Show(rec);
//}

//GetWindowThreadProcessId(Process.MainWindowHandle, out var mainWindowThreadId); // get thread ID of main window


//EnumThreadWindows(mainWindowThreadId, delegate (IntPtr hWnd, IntPtr lParam) // enumerate all windows created by main thread
//{
//    windows.Add(hWnd);  // add window handle to list   
//    return true;  // continue enumeration  
//}, IntPtr.Zero);

//var inspect = Boot.GetComponent <InspectComponent>().GetComponent<WinFormInspectComponent>();
//var result = new CatchImageManage().Process(inspect.UIAFactory.GetRootElement(), "test");
//if (result.success)
//{
//    CatchImageManage.Save(result.captureImage, result.window, "test");
//}