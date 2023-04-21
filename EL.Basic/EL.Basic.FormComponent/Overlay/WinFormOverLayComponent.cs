#if NETFRAMEWORK || NETCOREAPP
using EL.Async;
using EL.Hook;
using EL.Overlay;
using EL.WindowsAPI;
using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EL.Overlay
{
    public enum Mode
    {
        Auto,
        UIA2,
        Playwright,
        JAB,
        VCOcr,
        MSAA,
        None
    }
    /// <summary>
    /// 弹出组件
    /// </summary>
    public class FormOverLayComponent : Entity
    {
        public AutomationConfig Config = AutomationConfig.Instance;
        public ELTask<dynamic> ELTaskOverLay { get; set; }
        public OverlayRectangleForm Form { get; set; }
        public OverlayMsgForm MsgForm { get; set; }
        public static FormOverLayComponent Instance { get; set; }
        /// <summary>
        /// 完成通知
        /// </summary>
        public Action CompleteEvent { get; set; }
        /// <summary>
        /// 退出通知
        /// </summary>
        public Action ExitEvent { get; set; }
        /// <summary>
        /// mode修改通知
        /// </summary>
        public Action ModeEvent { get; set; }
        /// <summary>
        /// 截图事件
        /// </summary>
        public Action ScreenshotEvent { get; set; }
        public Keys ComplateHotKey
        {
            get
            {
                try
                {
                    var key = Config.ComplateHotKey;
                    if (string.IsNullOrWhiteSpace(key?.ToString()))
                        return Keys.Q;
                    return (Keys)Enum.Parse(typeof(Keys), key);
                }
                catch (Exception)
                {

                    return Keys.Q;
                }

            }
        }
        public Keys ExitHotKey
        {
            get
            {
                try
                {
                    var key = Config.ExitHotKey;
                    if (string.IsNullOrWhiteSpace(key?.ToString()))
                        return Keys.Escape;
                    return (Keys)Enum.Parse(typeof(Keys), key);
                }
                catch (Exception)
                {
                    return Keys.Escape;
                }

            }
        }
        public Keys ModeHotKey
        {
            get
            {
                try
                {
                    var key = Config.ModeHotKey;
                    if (string.IsNullOrWhiteSpace(key?.ToString()))
                        return Keys.W;
                    return (Keys)Enum.Parse(typeof(Keys), key);
                }
                catch (Exception)
                {
                    return Keys.W;
                }

            }
        }
        public Keys ScreenshotHotKey
        {
            get
            {
                try
                {
                    var key = Config.ScreenshotHotKey;
                    if (string.IsNullOrWhiteSpace(key?.ToString()))
                        return Keys.E;
                    return (Keys)Enum.Parse(typeof(Keys), key);
                }
                catch (Exception)
                {
                    return Keys.E;
                }

            }
        }
        public bool IsKeyboardHook => Config.IsKeyboardHook;
        public bool IsMouseHook => Config.IsMouseHook;
        public Keys FunctionKey
        {
            get
            {
                try
                {
                    var key = Config.FunctionKey;
                    if (key == null || string.IsNullOrWhiteSpace(key?.ToString()))
                        return Keys.Alt;
                    return (Keys)Enum.Parse(typeof(Keys), key);
                }
                catch (Exception)
                {
                    return Keys.Alt;
                }

            }
        }
        public int ComplateHotKeyId = 99999;
        public int ExitHotKeyId = 99998;
        public int ModeHotKeyId = 99997;
        public int ScreenshotHotKeyId = 99996;
        public KeyboardCatchHook KeyboardCatchHook { get; set; }
        public MouseCatchHook MouseCatchHook { get; set; }
        public bool IsCatchComplete = false;
        public bool IsCatchStart = false;
        /// <summary>
        /// 是否满足屏幕截图
        /// </summary>
        public bool IsScreenshot
        {
            get
            {
                return IsCatchStart && !IsCatchComplete && Mode == Mode.VCOcr;
            }
        }
        public Mode Mode { get; set; } = Mode.UIA2;
    }

    public class FormOverLayComponentAwake : AwakeSystem<FormOverLayComponent>
    {
        public override void Awake(FormOverLayComponent self)
        {
            HotkeyComponent hotKey;
            try
            {
                 hotKey = self.AddComponent<HotkeyComponent>();
            }
            catch (Exception ex)
            {

                throw;
            }
         
            FormOverLayComponent.Instance = self;
            self.Form = new OverlayRectangleForm();
            self.ELTaskOverLay = ELTask<dynamic>.Create();
            self.Form.Hide();
            self.MsgForm = new OverlayMsgForm();
            self.MsgForm.Hide();
            HotkeyComponent.UnregisterHotKey(self.Form.Handle, self.ComplateHotKeyId);
            HotkeyComponent.UnregisterHotKey(self.Form.Handle, self.ExitHotKeyId);
            HotkeyComponent.UnregisterHotKey(self.Form.Handle, self.ModeHotKeyId);
            HotkeyComponent.UnregisterHotKey(self.Form.Handle, self.ScreenshotHotKeyId);
            HotkeyModifiers hotkeyModifiers = default;
            if (self.FunctionKey == Keys.Control)
                hotkeyModifiers = HotkeyModifiers.MOD_CONTROL;
            else if (self.FunctionKey == Keys.Alt)
                hotkeyModifiers = HotkeyModifiers.MOD_ALT;
            else if (self.FunctionKey == Keys.Shift)
                hotkeyModifiers = HotkeyModifiers.MOD_SHIFT;
            hotKey.Regist(self.Form.Handle, hotkeyModifiers, self.ComplateHotKey, () => self.CompleteEvent?.Invoke(), self.ComplateHotKeyId);
            hotKey.Regist(self.Form.Handle, hotkeyModifiers, self.ExitHotKey, () => self.ExitEvent?.Invoke(), self.ExitHotKeyId);
            hotKey.Regist(self.Form.Handle, hotkeyModifiers, self.ModeHotKey, () => self.ModeEvent?.Invoke(), self.ModeHotKeyId);
            hotKey.Regist(self.Form.Handle, hotkeyModifiers, self.ScreenshotHotKey, () => self.ModeEvent?.Invoke(), self.ScreenshotHotKeyId);
            if (self.IsKeyboardHook)
            {
                self.KeyboardCatchHook = new KeyboardCatchHook();
                self.KeyboardCatchHook.CatchEvent += (x, y) =>
                {
                    bool isHot =
                    (self.FunctionKey == Keys.Control && y.Control) ||
                    (self.FunctionKey == Keys.Alt && y.Alt)
                    || (self.FunctionKey == Keys.Shift && y.Shift);

                    if (isHot && y.KeyCode == self.ComplateHotKey)
                    {
                        self.CompleteEvent?.Invoke();
                        return;
                    }
                    if (isHot && y.KeyCode == self.ExitHotKey)
                    {
                        self.ExitEvent?.Invoke();
                        return;
                    }
                    if (isHot && y.KeyCode == self.ExitHotKey)
                    {
                        Environment.Exit(0);
                        return;
                    }
                    if (y.KeyCode == self.ModeHotKey)
                    {
                        self.ModeEvent?.Invoke();
                        return;
                    }
                    return;
                };
            }
            if (self.IsMouseHook)
            {

                self.MouseHookInit();
            }
        }
    }
    public class FormOverLayComponentDestroy : DestroySystem<FormOverLayComponent>
    {
        public override void Destroy(FormOverLayComponent self)
        {
            self.Form.HideEx();
            var hotKey = self.AddComponent<HotkeyComponent>();
            FormOverLayComponent.Instance = self;
            HotkeyComponent.UnregisterHotKey(self.Form.Handle, self.ComplateHotKeyId);
            HotkeyComponent.UnregisterHotKey(self.Form.Handle, self.ExitHotKeyId);
        }
    }

    public static class FormOverLayComponentSystem
    {
        public static void MouseHookInit(this FormOverLayComponent self)
        {
            var formOver = self.GetComponent<FormOverLayComponent>();
            self.MouseCatchHook = new MouseCatchHook();
            self.MouseCatchHook.CatchEvent += () =>
            {
                self.IsCatchComplete = true;
            };
            Log.Trace("鼠标捕获监测开始---");
        }
        public static void KeyboardHookStart(this FormOverLayComponent self)
        {
            if (!self.IsKeyboardHook) return;
            self.KeyboardCatchHook.Start();
        }
        public static void MouseHookStart(this FormOverLayComponent self)
        {
            if (!self.IsMouseHook) return;
            self.MouseCatchHook.Start();
        }
        public static void KeyboardHookStop(this FormOverLayComponent self)
        {
            if (!self.IsKeyboardHook) return;
            self.KeyboardCatchHook.Stop();
        }
        public static void MouseHookStop(this FormOverLayComponent self)
        {
            if (!self.IsMouseHook) return;
            self.MouseCatchHook.Stop();
        }
        public static async ELTask Show(this FormOverLayComponent self, Color color)
        {
            var element = await self.ELTaskOverLay;
            var gdiColor = Color.FromArgb(color.A, color.R, color.G, color.B);
            self.Form.SetWindow(element, gdiColor);
            self.Form.Invoke(() =>
            {
                self.Form.TopMost = true;
                self.Form.Refresh();
            });
        }
        public static void FloatShow(this FormOverLayComponent self, string msg, int time = default)
        {
            self.MsgForm.Show();
            msg = msg ?? "打开界面探测器成功";
            if (!string.IsNullOrEmpty(msg) && self.MsgForm != null)
            {
                self.MsgForm.Invoke(() =>
                {
                    self.MsgForm.TopMost = true;
                    self.MsgForm.label1.Text = msg;
                    self.MsgForm.Refresh();
                });
            }
            if (time != default)
            {
                Task.Run(() =>
                {
                    Thread.Sleep(time);
                    self.MsgForm.Invoke(() =>
                    {
                        self.MsgForm.Hide();
                    });
                });
            }

        }

        public static async ELTask LightHigh(this FormOverLayComponent self, Color color, int count = 1, int time = 200, Action action = default)
        {
            var element = await self.ELTaskOverLay;
            var gdiColor = Color.FromArgb(color.A, color.R, color.G, color.B);
            self.Form.LightHigh(element, gdiColor);
            self.Form.Invoke(() =>
            {
                self.Form.TopMost = true;
                int i = 0;
                while (i < count)
                {
                    self.Form.Show();
                    self.Form.Refresh();
                    Thread.Sleep(time);

                    self.Form.Hide();
                    self.Form.Refresh();
                    Thread.Sleep(time);
                    i++;
                }
                self.Form.Clear();
                action?.Invoke();
            });
        }
        public static void LightHighShow(this FormOverLayComponent self, Color color, Rectangle rectangle)
        {
            var gdiColor = Color.FromArgb(color.A, color.R, color.G, color.B);
            self.Form.LightHigh(rectangle, gdiColor);
            self.Form.Invoke(() =>
            {
                self.Form.TopMost = true;
                self.Form.Show();
                self.Form.Refresh();
            });
        }
        public static void LightHighHide(this FormOverLayComponent self)
        {
            self.Form.Invoke(() =>
            {
                self.Form.TopMost = false;
                self.Form.Clear();
                self.Form.Hide();
                self.Form.Refresh();
            });
        }
        public static async ELTask LightHighMany(this FormOverLayComponent self, Color color, CancellationTokenSource cancellationTokenSource, Action action = default)
        {
            await ELTask.CompletedTask;
            try
            {
                var list = await self.ELTaskOverLay;
                var gdiColor = Color.FromArgb(color.A, color.R, color.G, color.B);
                self.Form.LightHighMany(list, gdiColor);
                self.Form.Invoke(() =>
                {
                    self.Form.TopMost = true;
                    self.Form.Show();
                    self.Form.Refresh();
                });
                _ = Task.Run(() =>
                    {
                        while (true)
                        {
                            if (cancellationTokenSource.IsCancellationRequested)
                            {
                                self.Form.Invoke(() =>
                                {
                                    self.Form.Clear();
                                    self.Form.Hide();
                                    self.Form.Refresh();
                                    action?.Invoke();
                                });

                                break;
                            }
                        }
                    });
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
        public static async ELTask LightHighMany(this FormOverLayComponent self, Color color, int count = 1, int time = 200, Action action = default)
        {
            try
            {
                var list = await self.ELTaskOverLay;
                var gdiColor = Color.FromArgb(color.A, color.R, color.G, color.B);
                self.Form.LightHighMany(list, gdiColor);
                self.Form.Invoke(() =>
                {
                    self.Form.TopMost = true;
                    int i = 0;
                    while (i < count)
                    {
                        self.Form.Show();
                        self.Form.Refresh();
                        Thread.Sleep(time);

                        self.Form.Hide();
                        self.Form.Refresh();
                        Thread.Sleep(time);
                        i++;
                    }
                    self.Form.Clear();
                    self.Form.Hide();
                    action?.Invoke();
                });
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public static async ELTask Draw(this FormOverLayComponent self, Color color)
        {
            var element = await self.ELTaskOverLay;
            var handler = element.CurrentNativeWindowHandle;
            Graphics graphics = Graphics.FromHwndInternal(IntPtr.Zero);
            User32.RedrawWindow(IntPtr.Zero, 0, IntPtr.Zero, 1 | 4 | 128);
            Pen myPen = new Pen(color, 2);
            var rec = element.BoundingRectangle;
            var rectangle = new Rectangle(rec.left, rec.top, rec.right - rec.left, rec.bottom - rec.top);
            graphics.DrawRectangle(myPen, rectangle);
            graphics.DrawString("F1：完成", new Font("宋体", 10, FontStyle.Bold), new SolidBrush(Color.Red),
                rec.left, rec.bottom + 5);
            graphics.Dispose();
        }
    }
}
//User32.InvalidateRect(mainHandle, IntPtr.Zero, true);
//User32.SendMessage(mainHandle, 0x000F, IntPtr.Zero, IntPtr.Zero);
//User32.RedrawWindow(mainHandle, 0, IntPtr.Zero, 1 | 4 | 128);
//User32.UpdateWindow(mainHandle);
//Graphics graphics1 = Graphics.FromHwndInternal(mainHandle);
//if (graphicsState != null)
//graphics1.Restore(graphicsState);
//graphics1.ResetClip();
//User32.InvalidateRect(IntPtr.Zero, IntPtr.Zero, true);
//User32.UpdateWindow(IntPtr.Zero);
//User32.SendMessage(IntPtr.Zero, 0x000F, IntPtr.Zero, IntPtr.Zero);
#endif