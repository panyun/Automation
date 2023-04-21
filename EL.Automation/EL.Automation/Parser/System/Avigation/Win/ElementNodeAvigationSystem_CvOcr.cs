using Automation.Inspect;
using EL.Capturing;
using OpenCvSharp.Extensions;
using OpenCvSharp;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using NPOI.SS.Formula.Functions;
using NPOI.HSSF.Record.CF;
using EL;
using OpenCvSharp.XFeatures2D;

namespace Automation.Parser
{
    public class ElementNodeAvigationSystem_CvOcr : IAvigation
    {
        public IEnumerable<Element> TryAvigation(ElementPath elementPath, int timeOut)
        {
            //var window = (ElementUIA)new ElementNodeAvigationSystem().TryAvigation(elementPath, timeOut).FirstOrDefault();
            //if (window == default) return default;
            //window.SetForeground();
            float matchingThreshold = 0.8f;
            double minval, maxval;
            OpenCvSharp.Point minloc, maxloc;
            var mat = GetMat();
            Mat matColor = mat.CvtColor(ColorConversionCodes.BGR2GRAY);
            var info = elementPath.PathNode.CurrentElementVcOcr;
            if (info == default) return default;
            var img = Mat.FromImageData(Convert.FromBase64String(info.MatchImg), ImreadModes.Grayscale);
            Mat matching = new Mat();
            var index = (int)Enum.GetValues(typeof(TemplateMatchModes)).Cast<TemplateMatchModes>().Max();
            while (index > -1)
            {
                Cv2.MatchTemplate(matColor, img, matching, (TemplateMatchModes)index);
                Cv2.MinMaxLoc(matching, out minval, out maxval, out minloc, out maxloc, null);

                if (index == (int)TemplateMatchModes.SqDiffNormed && minval < 0.3)
                {
                    info.BoundingRectangle = new Rectangle(minloc.X, minloc.Y, img.Width, img.Height);
                    return new List<Element>() { info };
                }
                if (index == (int)TemplateMatchModes.CCorrNormed && maxval > matchingThreshold)
                {
                    info.BoundingRectangle = new Rectangle(maxloc.X, maxloc.Y, img.Width, img.Height);
                    return new List<Element>() { info };
                }
                if (index == (int)TemplateMatchModes.CCoeffNormed && maxval > matchingThreshold)
                {
                    info.BoundingRectangle = new Rectangle(maxloc.X, maxloc.Y, img.Width, img.Height);
                    return new List<Element>() { info };
                }
                index -= 2;
            }
            throw new ParserException("通过节点元素路径未找到目标元素！");
        }


        public static Mat GetMat()
        {
            var cutCom = Boot.GetComponent<CutComponent>();
            var img = cutCom.CutBitmap();
            return img.ToMat();
        }
        private static byte[] BitmapToByte(Bitmap bitmap)
        {
            // 1.先将BitMap转成内存流
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            ms.Seek(0, System.IO.SeekOrigin.Begin);
            // 2.再将内存流转成byte[]并返回
            byte[] bytes = new byte[ms.Length];
            ms.Read(bytes, 0, bytes.Length);
            ms.Dispose();
            return bytes;
        }
    }
}
