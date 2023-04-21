using Automation.Inspect;
using EL;
using EL.Basic.Component.Clipboard;
using EL.Capturing;
using NUnit.Framework;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using Interop.UIAutomationClient;
using WindowsAccessBridgeInterop;

namespace Inspect.Test
{
    public class WinFormInspectComponentTests
    {
        [SetUp]
        public void Setup()
        {
            //创建
            Boot.App = new AppMananger();
            //加载程序集
            Boot.App.EventSystem.Add(typeof(WinFormInspectComponent).Assembly);
            Boot.AddComponent<InspectComponent>();

        }
        [Test]
        public void Clip()
        {
            var inspect = Boot.GetComponent<InspectComponent>();
            var clip = inspect.GetComponent<ClipboardComponent>();
            clip.CopyToClipboard("test");
            var val = clip.GetFromClipboard();
        }
        [Test]
        public void GetChilds()
        {
            var cutCom = Boot.GetComponent<CutComponent>();
            var img = cutCom.Cut();
            img.Save("Captures/test.img", ImageFormat.Jpeg);
            //var component = Boot.GetComponent<WinFormInspectComponent>();
            //var childs = component.LoadChildren(component.RootElement);
            //Assert.IsNotNull(childs);
        }
        [Test]
        public void GetParentNode()
        {
            var component = Boot.GetComponent<WinFormInspectComponent>();
            var nodes = component.GetAllParentNode(component.UIAFactory.ElementFromPoint(new tagPOINT() { x = 200, y = 200 }));
            Assert.IsNotNull(nodes);
        }
        [Test]
        public void GetAllChildrenNode()
        {
            var component = Boot.GetComponent<WinFormInspectComponent>();
            var nodes = component.GetAllChildrenNode(component.RootElement, 2);
        }
        //[Test]
        //public void JavaElement()
        //{
        //    var inspect = Boot.GetComponent<InspectComponent>();
        //    var component = inspect.GetComponent<JavaFormInspectComponent>();
        //    //component.FromPoint();
        //}
        [Test]
        public void MSAAElement()
        {
            var inspect = Boot.GetComponent<InspectComponent>();
            var component = Boot.AddComponent<WinFormInspectComponent>();
            var element = component.ElementFromPoint_MSAA();
            //component.FromPoint();
        }
    }

}