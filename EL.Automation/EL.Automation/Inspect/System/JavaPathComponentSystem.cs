//using Automation.Parser;
using EL;
using EL.Capturing;
using EL.WindowsAPI;
using System.Drawing;
using System.Drawing.Imaging;
using Interop.UIAutomationClient;
using WindowsAccessBridgeInterop;
using Automation.Parser;

namespace Automation.Inspect
{
    public enum ElementType
    {
        JABUI,
        UIAUI,
        MSAAUI,
        PlaywrightUI,
        VcOcr
    }

    public static class JavaPathComponentSystem
    {

        public static ElementNode GetNodes(this JavaPathComponent self, AccessibleNode element)
        {
            if (!self.ElementNodes.TryGetValue(element, out ElementNode node))
            {
                var inspect = self.GetParent<JavaFormInspectComponent>();
                node = inspect.GetAllParentNode(element);
                self.ElementNodes.Add(element, node);
            }
            return node;
        }
        public static ElementPath GetPathInfo(this JavaPathComponent self, AccessibleNode element)
        {
            var node = self.GetNodes(element);
            var ele = node.CurrentElementJava as ElementJAB;
            ElementPath pathInfo = new ElementPath();
            pathInfo.Path = node.ParentPath;
            pathInfo.PathNode = node;
            pathInfo.ProcessName = String.Empty;
            pathInfo.BoundingRectangle = node.CurrentElementJava.BoundingRectangle;
            pathInfo.MainWindowTitle = node.CurrentElementJava.MainWindowTitle;
            pathInfo.NativeWindowTitle = pathInfo.MainWindowTitle;
            pathInfo.Name = node.CurrentElementJava.Name;
            pathInfo.ControlType = node.CurrentElementJava.ControlTypeName.ToString();
            pathInfo.Img = self.GetCapBase64(pathInfo.BoundingRectangle);
            pathInfo.ElementEditNodes = GetElementNodes(node);
            pathInfo.Value = String.Empty;
            pathInfo.ElementType = ElementType.JABUI;
            return pathInfo;
        }
        public static List<ElementEdit> GetElementNodes(ElementNode elementNode)
        {
            var nodes = elementNode.GetParentNode();
            nodes.Reverse();
            var edits = nodes.Select(x =>
            {
                return new ElementEdit()
                {
                    Id = x.Id,
                    ElementPropertys = new List<ElementProperty>()
                      {
                        new ElementProperty(nameof(ElementEdit.IsChecked),"true",true),
                         new ElementProperty(nameof(x.CurrentElementJava.Name),x.CurrentElementJava.Name,x.CurrentElementJava.Name),
                               new ElementProperty(nameof(x.Index),x.Index+"",x.Index),
                      }
                };
            }).ToList();
            return edits;
        }
        public static string GetPathJson(this JavaPathComponent self, AccessibleNode element)
        {
            var path = self.GetPathInfo(element);
            return JsonHelper.ToJson(path);
        }
        public static CaptureImage GetCap(this JavaPathComponent self, Rectangle rec)
        {
            var widthMax = User32.GetSystemMetrics(SystemMetric.SM_CXSCREEN);
            var heightMax = User32.GetSystemMetrics(SystemMetric.SM_CYSCREEN);
            rec.X = Math.Max(0, rec.X);
            rec.Y = Math.Max(0, rec.Y);
            return CaptureComponent.Instance.Rectangle(rec);
        }
        public static string GetCapBase64(this JavaPathComponent self, Rectangle rec)
        {
            return self.ImgToBase64String(self.GetCap(rec).Bitmap);
        }

        private static string ImgToBase64String(this JavaPathComponent self, Bitmap bmp)
        {
            string result = "";
            if (bmp == null) return result;
            EncoderParameters ep = new EncoderParameters(1);
            ep.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 50);//设置压缩的比例1-100
            ImageCodecInfo[] arrayICI = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo jpegICIinfo = arrayICI.FirstOrDefault(t => t.FormatID == ImageFormat.Png.Guid);
            using (MemoryStream memoryStream = new MemoryStream())
            {
                bmp.Save(memoryStream, jpegICIinfo, ep);
                byte[] array = new byte[memoryStream.Length];
                memoryStream.Position = 0L;
                memoryStream.Read(array, 0, (int)memoryStream.Length);
                memoryStream.Close();
                result = "data:image/png;base64," + Convert.ToBase64String(array);
            }
            return result;
        }
        /// <summary>
        /// 获取xpath路径
        /// </summary>
        /// <param name="self"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public static string GetXPath(this JavaPathComponent self, ElementNode node)
        {
            if (node.Parent == null)
                return "";
            var currentControlName = $"{node.CurrentElementWin.ControlTypeName}";
            if (node.Index > 0)
            {
                currentControlName += $"[{node.Index}]"; //查找序列
            }
            return $"{GetXPath(self, node.Parent)}/{currentControlName}";
        }
        /// <summary>
        /// 获取node的json信息
        /// </summary>
        /// <param name="self"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public static string GetJson(this JavaPathComponent self, ElementNode node)
        {
            var inspect = self.GetParent<WinFormInspectComponent>();
            return JsonHelper.ToJson(node);
        }
    }

}

