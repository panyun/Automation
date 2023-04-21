using Automation.Inspect;
using EL;
using EL.Async;
using EL.Basic.Component.Clipboard;
using EL.Hook;
using EL.Input;
using EL.WindowsAPI;
using Interop.UIAutomationClient;
using Microsoft.Playwright;
using NPOI.Util;
using WindowsAccessBridgeInterop;
using static EL.UIA.ControlTypeConverter;

namespace Automation.Parser
{
    public static class InputActionSystem
    {
        public static async ELTask Main(this InputActionRequest self)
        {
            if (self.ElementType == ElementType.UIAUI)
            {
                self.UIAMain();
                return;
            }
            if (self.ElementType == ElementType.JABUI)
            {
                self.JABMain();
                return;
            }
            if (self.ElementType == ElementType.MSAAUI)
            {
                self.MSAAMain();
                return;
            }
            if (self.ElementType == ElementType.PlaywrightUI)
            {
                await self.PlaywrightAMain();
                return;
            }
            if (self.ElementType == ElementType.VcOcr)
            {
                self.VcOcrMain();
                return;
            }
        }
        public static async ELTask PlaywrightAMain(this InputActionRequest self)
        {

            var playwrightInspect = Boot.GetComponent<InspectComponent>().GetComponent<PlaywrightInspectComponent>();
            var ele = self.ElementPath.PathNode.CurrentElementPlaywright;
            ele.SetForeground();
            var obj = playwrightInspect.FindLocatorByPath(ele);
            switch (self.InputType)
            {
                case InputType.Keyboard:
                    ele.Focus();
                    await obj.Item2.Keyboard.InsertTextAsync(self.InputTxt);
                    break;
                case InputType.ElementInput:
                    await obj.Item1.FillAsync(self.InputTxt);
                    break;
                case InputType.Paste:
                    ele.SetText_Copy(self.InputTxt);
                    break;
                default:
                    break;
            }

        }
        public static async void Focus(this ElementPlaywright self)
        {
            var playwrightInspect = Boot.GetComponent<InspectComponent>().GetComponent<PlaywrightInspectComponent>();
            await playwrightInspect.CurrentPage.Mouse.ClickAsync(self.ClickablePoint.X, self.ClickablePoint.Y, new MouseClickOptions()
            {
                Button = Microsoft.Playwright.MouseButton.Left,
                ClickCount = 2,
                Delay = 50
            });
        }
        public static void VcOcrMain(this InputActionRequest self)
        {
            var elements = self.AvigationElement().Cast<ElementVcOcr>().ToList();
            Log.Trace($"elements Length :{elements.Count()} ");
            foreach (var element in elements)
            {
                element.Focus();
                if (self.IsClear)
                {
                    Wait.UntilInputIsProcessed();
                    KeyboardSimulator.SimulateStandardShortcut(StandardShortcut.SelectAll);
                    Keyboard.Press(VirtualKeyShort.BACK);
                    Wait.UntilInputIsProcessed();
                }
                if (string.IsNullOrEmpty(self.InputTxt)) return;
                switch (self.InputType)
                {
                    case InputType.Keyboard:
                        element.SetText_ByKeyboard(self.InputTxt);
                        break;
                    case InputType.ElementInput:
                        throw new ParserException("当前节点不支持事件点击！");
                    case InputType.Paste:
                        element.SetText_Copy(self.InputTxt);
                        break;
                    default:
                        break;
                }
            }
        }
        public static void MSAAMain(this InputActionRequest self)
        {
            var elements = self.AvigationElement().Cast<ElementMSAA>().ToList();
            Log.Trace($"elements Length :{elements.Count()} ");
            foreach (var element in elements)
            {
                element.Focus();
                if (self.IsClear)
                {
                    Wait.UntilInputIsProcessed();
                    KeyboardSimulator.SimulateStandardShortcut(StandardShortcut.SelectAll);
                    Keyboard.Press(VirtualKeyShort.BACK);
                    Wait.UntilInputIsProcessed();
                }
                if (string.IsNullOrEmpty(self.InputTxt)) return;
                switch (self.InputType)
                {
                    case InputType.Keyboard:
                        element.SetText_ByKeyboard(self.InputTxt);
                        break;
                    case InputType.ElementInput:
                        throw new ParserException("当前节点不支持事件点击！");
                    case InputType.Paste:
                        element.SetText_Copy(self.InputTxt);
                        break;
                    default:
                        break;
                }
            }
        }
        public static void JABMain(this InputActionRequest self)
        {
            var elements = Avigation.Create(self.ElementPath).TryAvigation(self.ElementPath, self.TimeOut).Cast<ElementJAB>();
            var inspect = Boot.GetComponent<InspectComponent>();
            var winInspect = inspect.GetComponent<WinFormInspectComponent>();
            Log.Trace($"elements Length :{elements.Count()} ");
            foreach (var element in elements)
            {
                element.Focus();
                if (self.IsClear)
                {
                    element.SetText_Copy("");
                    KeyboardSimulator.SimulateStandardShortcut(StandardShortcut.SelectAll);
                    Keyboard.Press(VirtualKeyShort.BACK);
                    Wait.UntilInputIsProcessed(TimeSpan.FromMilliseconds(20));
                }
                if (string.IsNullOrEmpty(self.InputTxt)) return;
                switch (self.InputType)
                {
                    case InputType.Keyboard:
                        element.SetText_Copy(self.InputTxt);
                        //element.SetText_ByKeyboard(self.InputAction.InputTxt);
                        break;
                    case InputType.ElementInput:
                        element.SetText_ElementInput(self.InputTxt);
                        break;
                    case InputType.Paste:
                        element.SetText_Copy(self.InputTxt);
                        break;
                    default:
                        break;
                }
            }
        }
        public static void UIAMain(this InputActionRequest self)
        {
            var elements = self.AvigationElement().Cast<ElementUIA>().ToList();
            self.UIAMain(elements);
        }
        public static void UIAMain(this InputActionRequest self, List<ElementUIA> elementWins)
        {
            Log.Trace($"elements Length :{elementWins.Count()} ");
            foreach (var element in elementWins)
            {
                element.Focus();
                if (!(element.ControlTypeName == ControlType.ComboBox.ToString() || element.ControlTypeName == ControlType.ListItem.ToString()))
                {
                    if (self.IsClear)
                    {
                        Wait.UntilInputIsProcessed();
                        KeyboardSimulator.SimulateStandardShortcut(StandardShortcut.SelectAll);
                        Keyboard.Press(VirtualKeyShort.BACK);
                        Wait.UntilInputIsProcessed();
                    }
                }
                if (string.IsNullOrEmpty(self.InputTxt)) return;
                switch (self.InputType)
                {
                    case InputType.Keyboard:
                        element.SetText_ByKeyboard(self.InputTxt);
                        break;
                    case InputType.ElementInput:
                        element.SetText_ElementInput(self.InputTxt);
                        break;
                    case InputType.Paste:
                        element.SetText_ByCopy(self.InputTxt);
                        break;
                    default:
                        break;
                }
            }
        }

        #region win
        public static void SetText_ElementInput(this ElementUIA self, string value)
        {
            if (self == null) return;
            //self.Focus();
            var valuePattern = (IUIAutomationValuePattern)self.NativeElement.GetCurrentPattern(UIA_PatternIds.UIA_ValuePatternId);
            if (valuePattern != null)
                valuePattern.SetValue(value);
            //Keyboard.Type(VirtualKeyShort.ENTER);
        }

        public static void SetText_ByKeyboard(this ElementUIA self, string value)
        {
            try
            {
                Wait.UntilInputIsProcessed(TimeSpan.FromMilliseconds(20));
                if (String.IsNullOrEmpty(value)) return;
                var lines = value.Replace("\r\n", "\n").Split('\n');
                Keyboard.Type(lines[0]);
                foreach (var line in lines.Skip(1))
                {
                    Keyboard.Type(VirtualKeyShort.RETURN);
                    Keyboard.Type(line);
                }
                Wait.UntilInputIsProcessed();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }
        public static string GetText_Copy(this ElementUIA self)
        {
            Wait.UntilInputIsProcessed(TimeSpan.FromMilliseconds(20));
            KeyboardSimulator.SimulateStandardShortcut(StandardShortcut.SelectAll);
            KeyboardSimulator.SimulateStandardShortcut(StandardShortcut.Copy);
            var inspect = Boot.GetComponent<InspectComponent>();
            var clipboard = inspect.GetComponent<ClipboardComponent>();
            Wait.UntilInputIsProcessed(TimeSpan.FromMilliseconds(50));
            string value = clipboard.GetFromClipboard();
            Log.Trace($"value:{value}");
            Wait.UntilInputIsProcessed(TimeSpan.FromMilliseconds(50));
            clipboard.EmptyClipboard();
            return value;
        }
        public static void SetText_ByCopy(this ElementUIA self, string value)
        {
            if (String.IsNullOrEmpty(value)) return;
            var inspect = Boot.GetComponent<InspectComponent>();
            var clipboard = inspect.GetComponent<ClipboardComponent>();
            clipboard.CopyToClipboard(value);
            Wait.UntilInputIsProcessed(TimeSpan.FromMilliseconds(50));

            KeyboardSimulator.SimulateStandardShortcut(StandardShortcut.Paste);
            Wait.UntilInputIsProcessed(TimeSpan.FromMilliseconds(50));
            clipboard.EmptyClipboard();
        }
        #endregion
        #region java
        public static void SetText_Copy(this ElementPlaywright self, string value)
        {
            var inspect = Boot.GetComponent<InspectComponent>();
            var clipboard = inspect.GetComponent<ClipboardComponent>();
            clipboard.CopyToClipboard(value);
            self.Focus();
            Wait.UntilInputIsProcessed(TimeSpan.FromMilliseconds(50));
            KeyboardSimulator.SimulateStandardShortcut(StandardShortcut.Paste);
            Wait.UntilInputIsProcessed(TimeSpan.FromMilliseconds(50));
            clipboard.EmptyClipboard();
            Wait.UntilInputIsProcessed(TimeSpan.FromMilliseconds(50));
        }
        public static void SetText_Copy(this Element self, string value)
        {
            var inspect = Boot.GetComponent<InspectComponent>();
            var clipboard = inspect.GetComponent<ClipboardComponent>();
            clipboard.CopyToClipboard(value);
            self.Focus();
            Wait.UntilInputIsProcessed(TimeSpan.FromMilliseconds(40));
            KeyboardSimulator.SimulateStandardShortcut(StandardShortcut.Paste);
            Wait.UntilInputIsProcessed(TimeSpan.FromMilliseconds(40));
            clipboard.EmptyClipboard();
            Wait.UntilInputIsProcessed(TimeSpan.FromMilliseconds(40));
        }
        public static void SetText_ElementInput(this ElementJAB self, string value)
        {
            var t2 = self.AccessibleNode.AccessBridge.Functions.SetTextContents(self.AccessibleNode.JvmId, ((AccessibleContextNode)self.AccessibleNode).AccessibleContextHandle, value);
        }
        public static void SetText_ByKeyboard(this Element self, string value)
        {
            try
            {
                self.Focus();
                Wait.UntilInputIsProcessed(TimeSpan.FromMilliseconds(50));
                if (String.IsNullOrEmpty(value)) return;
                var lines = value.Replace("\r\n", "\n").Split('\n');
                Keyboard.Type(lines[0]);
                foreach (var line in lines.Skip(1))
                {
                    Keyboard.Type(VirtualKeyShort.RETURN);
                    Keyboard.Type(line);
                }
                Wait.UntilInputIsProcessed();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }
        public static string GetText_Copy(this ElementJAB self)
        {
            self.Focus();
            Wait.UntilInputIsProcessed(TimeSpan.FromMilliseconds(50));
            KeyboardSimulator.SimulateStandardShortcut(StandardShortcut.SelectAll);
            KeyboardSimulator.SimulateStandardShortcut(StandardShortcut.Copy);
            Wait.UntilInputIsProcessed(TimeSpan.FromMilliseconds(50));
            var inspect = Boot.GetComponent<InspectComponent>();
            var clipboard = inspect.GetComponent<ClipboardComponent>();
            Wait.UntilInputIsProcessed(TimeSpan.FromMilliseconds(50));
            string value = clipboard.GetFromClipboard();
            Log.Trace($"value:{value}");
            clipboard.EmptyClipboard();
            Wait.UntilInputIsProcessed(TimeSpan.FromMilliseconds(50));
            return value;
        }

        #endregion

    }
}
