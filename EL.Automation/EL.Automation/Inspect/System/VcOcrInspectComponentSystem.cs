using Automation.Parser;
using EL;
using EL.Async;
using EL.Capturing;
using EL.Http;
using EL.Input;
using EL.WindowsAPI;
using Newtonsoft.Json.Linq;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Diagnostics;
using System.Drawing;

namespace Automation.Inspect
{
    public class VcOcrInspectComponent : Entity
    {
        public ElementIns currentIns { get; set; }
        public System.Drawing.Point currentPoint { get; set; }
        public ElementCvInfo currentWindow { get; set; }
        public string OrcImgUrl
        {
            get
            {
                return "http://192.168.0.107:10200/ocr_img";
            }
        }
    }
    public class VcOcrInspectComponentAwake : AwakeSystem<VcOcrInspectComponent>
    {
        public override void Awake(VcOcrInspectComponent self)
        {

        }
    }
    public static class VcOcrInspectComponentSystem
    {
        public static async ELTask<ElementIns> ElementFromPoint(this VcOcrInspectComponent self)
        {
            var img = self.GetMat();
            self.currentWindow = new ElementCvInfo()
            {
                WindowCv = self.RecognizeText(img)
            };
            System.Drawing.Point point = new(Mouse.Position.X, Mouse.Position.Y);
            if (self.currentIns != null && self.currentPoint != default && self.currentPoint.X == point.X && self.currentPoint.Y == point.Y)
                return self.currentIns;
            ElementCv cv = default;
            foreach (var item in self.currentWindow.WindowCv.Childs)
            {
                if (!item.Rectangle.Contains(point)) continue;
                var are = item.Rectangle.Width * item.Rectangle.Height;
                if (cv == default)
                {
                    cv = item;
                    continue;
                }
                if ((cv.Rectangle.Width * cv.Rectangle.Height) > are)
                {
                    cv = item;
                    continue;
                }
            }
            if (cv == default) return default;
            var bytes = self.GetCap(cv.Rectangle);
            self.currentWindow.Img = bytes;
            self.currentWindow.Rect = cv.Rectangle;
            self.currentPoint = point;
            self.currentIns = new ElementIns(self.currentWindow);
            return self.currentIns;
        }
        public static Mat GetMat(this VcOcrInspectComponent self)
        {
            
            //var img = CaptureComponent.Instance.Screen();
            var cutCom = Boot.GetComponent<CutComponent>();
            var img = cutCom.CutBitmap();
            var mat = img.ToMat();
            Mat matColor = mat.CvtColor(ColorConversionCodes.BGR2GRAY);
            return matColor;
        }
        public static byte[] GetCap(this VcOcrInspectComponent self, Rectangle rec)
        {
            rec.X = Math.Max(0, rec.X);
            rec.Y = Math.Max(0, rec.Y);
            var img = CaptureComponent.Instance.Rectangle(rec);
            var imgbyte = BitmapToByte(img.Bitmap);
            return imgbyte;
        }
        /// <summary>
        /// 将BitMap转换成bytes数组
        /// </summary>
        /// <param name="bitmap">要转换的图像</param>
        /// <returns></returns>
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
        /// <summary>
        /// 文本处理
        /// </summary>
        /// <param name="self"></param>
        /// <param name="src"></param>
        /// <returns></returns>
        public static Mat PreprocessText(this VcOcrInspectComponent self, Mat src)
        {
            Mat dilation2 = new Mat();
            //ImreadModes.Grayscale
            //读取灰度图

            //1.Sobel算子，x方向求梯度
            Mat sobel = new();
            Cv2.Sobel(src, sobel, MatType.CV_8U, 1, 0, 3);

            //2.二值化
            Mat binary = new();
            Cv2.Threshold(sobel, binary, 0, 255, ThresholdTypes.Otsu | ThresholdTypes.Binary);
            //3. 膨胀和腐蚀操作的核函数
            Mat element1 = new();
            Mat element2 = new();
            OpenCvSharp.Size size1 = new(16, 6);
            OpenCvSharp.Size size2 = new(12, 3);
            element1 = Cv2.GetStructuringElement(MorphShapes.Rect, size1);
            element2 = Cv2.GetStructuringElement(MorphShapes.Rect, size2);
            //4. 膨胀一次，让轮廓突出
            Mat dilation = new();
            Cv2.Dilate(binary, dilation, element2, null, 1);
            //5. 腐蚀一次，去掉细节，如表格线等。注意这里去掉的是竖直的线
            Mat erosion = new();
            Cv2.Erode(dilation, erosion, element1);
            //6. 再次膨胀，让轮廓明显一些
            Cv2.Dilate(erosion, dilation2, element2, null, 2);
            //var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "VcOcr", $"1_{Guid.NewGuid() + ""}.jpg");
            //src.ToBitmap().Save(path);
            //path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "VcOcr", $"2_{Guid.NewGuid() + ""}.jpg");
            //sobel.ToBitmap().Save(path);
            //path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "VcOcr", $"3_{Guid.NewGuid() + ""}.jpg");
            //binary.ToBitmap().Save(path);
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
            return dilation2;
        }
        public static WindowCv RecognizeText(this VcOcrInspectComponent self, Mat matImg)
        {
            WindowCv c = new WindowCv();
            // 读取图像 
            using (Mat image = self.PreprocessText(matImg))
            {
                OpenCvSharp.Point[][] contours;
                HierarchyIndex[] hierarchly;
                // 寻找轮廓 
                OpenCvSharp.Rect biggestContourRect = new OpenCvSharp.Rect();
                Cv2.FindContours(image, out contours, out hierarchly, RetrievalModes.External, ContourApproximationModes.ApproxSimple);
                foreach (OpenCvSharp.Point[] contour in contours)
                {
                    biggestContourRect = Cv2.BoundingRect(contour);
                    c.Childs.Add(new ElementCv()
                    {
                        Rectangle = new Rectangle(biggestContourRect.X - 3, biggestContourRect.Y - 5, biggestContourRect.Width + 6, biggestContourRect.Height + 10)
                    });
                }
            }
            Debug.WriteLine("count:" + c.Childs.Count + "");
            return c;
        }
        /// <summary>
        /// 文本处理
        /// </summary>
        /// <param name="self"></param>
        /// <param name="src"></param>
        /// <returns></returns>
        public static Mat PreprocessButtion(this VcOcrInspectComponent self, Mat src)
        {
            Mat dilation2 = new Mat();
            //ImreadModes.Grayscale
            //读取灰度图

            //1.Sobel算子，x方向求梯度
            Mat sobel = new();
            Cv2.Sobel(src, sobel, MatType.CV_8U, 1, 0, 3);

            //2.二值化
            Mat binary = new();
            Cv2.Threshold(sobel, binary, 0, 255, ThresholdTypes.Otsu | ThresholdTypes.Binary);
            //3. 膨胀和腐蚀操作的核函数
            Mat element1 = new();
            Mat element2 = new();
            OpenCvSharp.Size size1 = new(16, 6);
            OpenCvSharp.Size size2 = new(12, 3);
            element1 = Cv2.GetStructuringElement(MorphShapes.Rect, size1);
            element2 = Cv2.GetStructuringElement(MorphShapes.Rect, size2);
            //4. 膨胀一次，让轮廓突出
            Mat dilation = new();
            Cv2.Dilate(binary, dilation, element2, null, 1);
            //5. 腐蚀一次，去掉细节，如表格线等。注意这里去掉的是竖直的线
            Mat erosion = new();
            Cv2.Erode(dilation, erosion, element1);
            //6. 再次膨胀，让轮廓明显一些
            Cv2.Dilate(erosion, dilation2, element2, null, 2);
            return dilation2;
        }
        public static WindowCv RecognizeButtion(this VcOcrInspectComponent self, Mat matImg)
        {
            WindowCv c = new WindowCv();
            // 读取图像 
            using (Mat image = self.PreprocessButtion(matImg))
            {
                OpenCvSharp.Point[][] contours;
                HierarchyIndex[] hierarchly;
                // 寻找轮廓 
                Rect biggestContourRect = new OpenCvSharp.Rect();
                Cv2.FindContours(image, out contours, out hierarchly, RetrievalModes.External, ContourApproximationModes.ApproxSimple);
                foreach (OpenCvSharp.Point[] contour in contours)
                {
                    biggestContourRect = Cv2.BoundingRect(contour);
                    if (biggestContourRect.Width > 16 && biggestContourRect.Height > 16) //如果矩形的宽高满足条件，则认为是按钮
                    {
                        c.Childs.Add(new ElementCv()
                        {
                            Rectangle = new Rectangle(biggestContourRect.X, biggestContourRect.Y, biggestContourRect.Width, biggestContourRect.Height)
                        });
                    }
                }
            }
            Debug.WriteLine("count:" + c.Childs.Count + "");
            return c;
        }
        public static async ELTask<string> FindTextByRegion(this VcOcrInspectComponent self, Bitmap bitmap)
        {
            var httpComponent = Boot.GetComponent<HttpComponent>();
            var jobj = await httpComponent.PostImg<JObject>(self.OrcImgUrl, bitmap, "");
            if (jobj == null || jobj["texts"] == null) return default;
            var msg = string.Empty;
            for (int i = 0; i < jobj["texts"].Count(); i++)
                msg += new Msg(jobj["texts"][i] + "");
            return msg;
        }
    }
}
public class WindowCv
{
    public string Title { get; set; }
    public string Id { get; set; }
    public List<ElementCv> Childs = new List<ElementCv>();
}
public class ElementCv
{

    public int Type { get; set; }
    public string Text { get; set; }
    public System.Drawing.Rectangle Rectangle { get; set; }
    public System.Drawing.Point StartPoint { get; set; }
}
