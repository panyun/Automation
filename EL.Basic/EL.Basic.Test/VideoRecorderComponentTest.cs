using EL.Capturing;
using EL.Video;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL.Basic.Test
{
    [TestFixture]
    public class VideoRecorderComponentTest
    {
        [SetUp]
        public void Setup()
        {
            //创建
            Boot.App = new AppMananger();
            //加载程序集
            //Boot.App.EventSystem.Add(typeof(VideoRecorderComponent).Assembly);
            Boot.AddComponent<VideoRecorderComponent>();
            Boot.AddComponent<CutComponent>();
        }
        [Test]
        public void VideoTest()
        {
            //var component = Boot.GetComponent<VideoRecorderComponent>();
            var cutCom = Boot.GetComponent<CutComponent>();
            var img = cutCom.Cut();
            img.Save("test.png", ImageFormat.Jpeg);
        }
    }
}
