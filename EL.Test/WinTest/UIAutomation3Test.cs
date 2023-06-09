using EL.Async;
using Interop.UIAutomationClient;
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace WinTest
{
    public class UIAutomation3Test
    {
        [SetUp]
        public void Setup()
        {

        }

        /// <summary>
        /// 本测试执行  写入文本
        /// 打开 --> 获取句柄 --> 查找RichEdit控件 --> 查找ValuePattern --写入测试文本
        /// </summary>
        [Test]
        public void GetNotepadWriteAndRead()
        {
            //  1.打开记事本, 等待500毫秒
            var processStartInfo = new ProcessStartInfo("notepad.exe", null);
            var process = Process.Start(processStartInfo);
            Task.Delay(500).Wait();

            //  2.查找窗口句柄 得到Element
            var winHandle = process.MainWindowHandle;
            IUIAutomation uIAutomation = new CUIAutomation8();
            var element = uIAutomation.ElementFromHandle(winHandle);

            //  3.查找RichEdit编辑控件
            var btnCondition = uIAutomation.CreatePropertyCondition(UIA_PropertyIds.UIA_ControlTypePropertyId, UIA_ControlTypeIds.UIA_DocumentControlTypeId);
            var automationElement = AutomationElement.FromHandle(element.CurrentNativeWindowHandle);
            var walker = uIAutomation.ControlViewWalker;

            var elementDocument = element.FindFirst(Interop.UIAutomationClient.TreeScope.TreeScope_Children, btnCondition);
            var parent = TreeWalker.ControlViewWalker.GetParent(automationElement);
            var parentDoc = walker.GetParentElement(elementDocument);

            var elements = elementDocument.FindAll(Interop.UIAutomationClient.TreeScope.TreeScope_Parent, null);
            //  3.获取ValuePattern 并写入测试文本
            var txtInput = "这是一个测试程序执行的逻辑";

            IUIAutomationValuePattern valuePattern = (IUIAutomationValuePattern)elementDocument.GetCurrentPattern(ValuePattern.Pattern.Id);

            valuePattern.SetValue(txtInput);
        }
        [Test]
        public void GetAppWindow()
        {
            NodeTree noteTree = new NodeTree("5name", "name5");
            noteTree.ParentNote = new NodeTree("4name", "name4");
            noteTree.ParentNote.ParentNote = new NodeTree("3name", "name3");
            noteTree.ParentNote.ParentNote.ParentNote = new NodeTree("2name", "name2");
            var note = new NodeTree("1name", "name1");
            noteTree.ParentNote.ParentNote.ParentNote.ParentNote = note;
            var str = noteTree.ToString();
            var node = NodeTree.ConvertNote(str);
        }

        public class NodeTree : Object
        {
            public NodeTree(string name, string controlType)
            {
                this.Name = name;
                this.ControlType = controlType;
            }
            public NodeTree ParentNote { get; set; }
            public string Name { get; set; }
            public string ControlType { get; set; }
            public override string ToString()
            {
                var currentItemText = $"{this.ControlType}|{this.Name}";
                if (this.ParentNote == null)
                    return currentItemText;
                return $"{ParentNote.ToString()}/{currentItemText}";
            }
            public static NodeTree ConvertNote(string str)
            {
                var strs = str.Split('/');
                var cStr = strs[strs.Length - 1];
                NodeTree node = new NodeTree(cStr.Split('|')[0], cStr.Split('|')[1]);
                str = str.Replace(("/" + cStr), "");
                str = str.Replace((cStr), "");
                if (strs != null && str.Length != 0)
                    node.ParentNote = ConvertNote(str);
                return node;
            }
        }

    }
}