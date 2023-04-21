using Automation.Inspect;
using EL;
using EL.Capturing;
using EL.Overlay;
using NUnit.Framework;
using System.IO;
using System.Linq;
using System.Reflection;
using Interop.UIAutomationClient;
using Automation.Parser;
using EL.Basic.Component.Clipboard;
using System.Text.RegularExpressions;
using OpenCvSharp;
using System;
using OpenCvSharp.Extensions;

namespace Inspect.Test
{
    public class PathComponentTest
    {
        [SetUp]
        public void Setup()
        {
            //创建
            //Boot.App = new AppMananger();
            //Boot.SetLog(new FileLogger());
            ////加载程序集
            //Boot.App.EventSystem.Add(typeof(WinFormInspectComponent).Assembly);
            //Boot.App.EventSystem.Add(typeof(FormOverLayComponent).Assembly);
            //Boot.AddComponent<InspectComponent>();
            //Boot.AddComponent<ParserComponent>();

        }
        [Test]
        public void Xpath()
        {
            var component = Boot.GetComponent<WinFormInspectComponent>();
            var path = component.GetComponent<WinPathComponent>();
            var node = path.GetNodes(component.UIAFactory.ElementFromPoint(new tagPOINT() { x = 200, y = 200 }));
            var str = path.GetXPath(node);
            Assert.IsNotNull(str);
        } 
        [Test]
        public void OpenVc()
        {
            Mat src = Cv2.ImRead(@"D:\Work Space\c-automation\EL.Bin\WinFromInspect\x64\Debug\net47\VcOcr\1680861000760.jpg");
            Mat gray = new();
            Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);
            CascadeClassifier faceCascade = new CascadeClassifier("haarcascade_frontalface_default.xml");
            Rect[] faces = faceCascade.DetectMultiScale(gray, 1.1, 4);
            var path = Path.Combine(@"D:\Work Space\c-automation\EL.Bin\WinFromInspect\x64\Debug\net47\VcOcr\", "VcOcr", $"1_{Guid.NewGuid() + ""}.jpg");
            src.ToBitmap().Save(path);
            path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "VcOcr", $"2_{Guid.NewGuid() + ""}.jpg");
            src.ToBitmap().Save(path);
            path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "VcOcr", $"3_{Guid.NewGuid() + ""}.jpg");
            gray.ToBitmap().Save(path);
            //path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "VcOcr", $"4_{Guid.NewGuid() + ""}.jpg");
            //element1.ToBitmap().Save(path);
            //path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "VcOcr", $"5_{Guid.NewGuid() + ""}.jpg");
            //element2.ToBitmap().Save(path);
            //path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "VcOcr", $"6_{Guid.NewGuid() + ""}.jpg");
            //dilation.ToBitmap().Save(path);
            //path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "VcOcr", $"7_{Guid.NewGuid() + ""}.jpg");
            //erosion.ToBitmap().Save(path);
            //path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "VcOcr", $"8_{Guid.NewGuid() + ""}.jpg");
            //dilation2.ToBitmap().Save(path);
        }


        [Test]
        public void IEXpath()
        {
            var component = Boot.GetComponent<IEInspectComponent>();
            if (!component.IsIE()) return;
            var element = component.GetElement();
            var path = component.GetXPath(element);
            var img = component.GetCap(element);
            img.Bitmap.Save("test.png", System.Drawing.Imaging.ImageFormat.Bmp);
            Assert.IsNotNull(path);
            ///HTML/BODY/DIV[1]/main[1]/OL[1]/LI[1]/DIV[1]/DIV[1]/DIV[1]/H2[1]/A[1]
            ///HTML/BODY/DIV[1]/main[1]/OL[1]/LI[1]/DIV[1]/DIV[1]/DIV[1]/H2[1]/A[1]
            ///
            ///HTML/BODY/DIV[4]/DIV[1]/DIV[2]/DIV[1]/DIV[3]/DIV[1]/FORM[3]/INPUT[1]
            ///HTML/BODY/DIV[4]/DIV[1]/DIV[2]/DIV[1]/DIV[3]/DIV[1]/FORM[3]/INPUT[1]
        }
        [Test]
        public void IsIE()
        {
            var component = Boot.GetComponent<IEInspectComponent>();
            var isIE = component.IsIE();
            //Assert.IsTrue(isIe);
        }
        [Test]

        public void GetJson()
        {
            var component = Boot.GetComponent<WinFormInspectComponent>();
            var path = component.GetComponent<WinPathComponent>();
            var uiaElement = component.UIAFactory.ElementFromPoint(new tagPOINT() { x = 200, y = 200 });
            var element = component.GetAllParentNode(uiaElement);
            var json = path.GetJson(element);
        }
        [Test]
        public void GetPath()
        {
            string path = @"test////es\e//";
            var path1 = Regex.Replace(path, @"\\", @"/");
            var path2 = Regex.Replace(path1, "/+", @"/");
            //var inspect = Boot.GetComponent<InspectComponent>();
            //var parser = Boot.GetComponent<ParserComponent>();
            //var winFormInspect = inspect.GetComponent<WinFormInspectComponent>();
            //var element = winFormInspect.ElementFromPoint();
            //var path = winFormInspect.GetComponent<WinPathComponent>();
            //var info = path.GetPathInfo(element.NativeElement);
            //var json = JsonHelper.ToJson(info);
            //var elePath = JsonHelper.FromJson<ElementPath>(json);
            //var eleParser = parser.Start(elePath, default);
        }
    }

}