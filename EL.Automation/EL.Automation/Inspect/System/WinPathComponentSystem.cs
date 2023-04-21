using Automation.Parser;
using EL;
using EL.Capturing;
using EL.WindowsAPI;
using Interop.UIAutomationClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Automation.Inspect
{
	public static class WinPathComponentSystem
	{
		public static ElementNode GetNodes(this WinPathComponent self, IUIAutomationElement element)
		{
			//Stopwatch RequestTime = new Stopwatch();
			//RequestTime.Start();
			//var id = element.GetRuntimeId();
			//var val = String.Concat(id.Select(x => x + ""));
			//var code = val.GetHashCode();
			//if (!self.ElementNodes.TryGetValue(code, out ElementNode node))
			//{
			//    var inspect = self.GetParent<WinFormInspectComponent>();
			//    node = inspect.GetAllParentNode(element);
			//    self.ElementNodes.Add(code, node);
			//}
			//RequestTime.Stop();
			//var time = RequestTime.ElapsedTicks;
			var inspect = self.GetParent<WinFormInspectComponent>();
			return inspect.GetAllParentNode(element);
		}
		public static ElementPath GetPathInfo(this WinPathComponent self, IUIAutomationElement element)
		{
			var node = self.GetNodes(element);
			if (node == default) return default;
			ElementPath pathInfo = new()
			{
				Id = IdGenerater.Instance.GenerateId()
			};
			var inspect = self.GetParent<WinFormInspectComponent>();
			var ele = node.CurrentElementWin;
			var elementWin = node.CurrentElementWin.NativeElement.Convert();
			node.CurrentElementWin = elementWin;
			var processInfo = inspect.GetProcessInfo(ele.NativeElement);
			pathInfo.Path = node.ParentPath;
			pathInfo.PathNode = node;
			pathInfo.Index = node.Index;
			pathInfo.ProcessName = processInfo.ProcessName;
			pathInfo.BoundingRectangle = elementWin.BoundingRectangle;
			pathInfo.MainWindowTitle = processInfo.MainWindowTitle;
			pathInfo.Name = elementWin.Name;
			pathInfo.ControlType = elementWin.ControlTypeName.ToString();
			pathInfo.Img = self.GetCapBase64(pathInfo.BoundingRectangle);
			pathInfo.ElementEditNodes = GetElementNodes(node);
			pathInfo.NativeWindowTitle = elementWin.GetNativeMainTitle();
			pathInfo.Value = elementWin.Value;
			pathInfo.Text = elementWin.Text;
			pathInfo.ElementType = ElementType.UIAUI;
			// node.GenerateCompareChildrenId();
			return pathInfo;
		}
		public static ElementPath GetPathInfo(this WinPathComponent self, ElementMSAA element)
		{
			var node = self.GetNodes(element.MSAAProperties.ElementUIA);
			node.ChildIndexs = element.MSAAProperties.ChildIndexs;
			if (node == default) return default;
			ElementPath pathInfo = new()
			{
				Id = IdGenerater.Instance.GenerateId()
			};
			var inspect = self.GetParent<WinFormInspectComponent>();
			var ele = node.CurrentElementWin;
			var elementWin = node.CurrentElementWin.NativeElement.Convert();
			node.CurrentElementWin = elementWin;
			var processInfo = inspect.GetProcessInfo(ele.NativeElement);
			pathInfo.Path = node.ParentPath;
			pathInfo.PathNode = node;
			pathInfo.Index = node.Index;
			pathInfo.ProcessName = processInfo.ProcessName;
			pathInfo.BoundingRectangle = element.BoundingRectangle;
			pathInfo.MainWindowTitle = processInfo.MainWindowTitle;
			pathInfo.Name = element.Name;
			pathInfo.ControlType = element.ControlTypeName;
			pathInfo.Img = self.GetCapBase64(element.BoundingRectangle);
			pathInfo.ElementEditNodes = GetElementNodes(node);
			pathInfo.NativeWindowTitle = elementWin.GetNativeMainTitle();
			pathInfo.Value = element.Value;
			pathInfo.Text = element.Text;
			pathInfo.ElementType = ElementType.MSAAUI;
			return pathInfo;
		}
		public static ElementPath GetPathInfo(this WinPathComponent self, ElementPlaywright element)
		{
			//var node = self.GetNodes(element.ElementUIA);
			var node = new ElementNode();
			if (node == default) return default;
			ElementPath pathInfo = new()
			{
				Id = IdGenerater.Instance.GenerateId()
			};
			var inspect = self.GetParent<WinFormInspectComponent>();
			node.CurrentElementPlaywright = element;
			//var processInfo = inspect.GetProcessInfo(ele.NativeElement);
			pathInfo.Path = element.XPath;
			pathInfo.PathNode = node;
			pathInfo.Index = node.Index;
			//pathInfo.ProcessName = processInfo.ProcessName;
			pathInfo.BoundingRectangle = element.BoundingRectangle;
			pathInfo.MainWindowTitle = element.WindowTitle;
			pathInfo.Name = element.Name;
			pathInfo.ControlType = element.Role;
			pathInfo.Img = self.GetCapBase64(element.GetScreenBoundingRectangle());
			pathInfo.NativeWindowTitle = element.WindowTitle;
			pathInfo.Value = element.Value;
			pathInfo.Text = element.Text;
			pathInfo.ElementType = ElementType.PlaywrightUI;
			// node.GenerateCompareChildrenId();
			return pathInfo;
		}
		public static async Task<ElementPath> GetPathInfo(this WinPathComponent self, ElementCvInfo ele)
		{
			var inspect = Boot.GetComponent<InspectComponent>();
			var node = new ElementNode();
			if (node == default) return default;
			ElementPath pathInfo = new()
			{
				Id = IdGenerater.Instance.GenerateId()
			};
			var element = ele.Convert();
			node.CurrentElementVcOcr = element;
			pathInfo.PathNode = node;
			pathInfo.Index = node.Index;
			pathInfo.BoundingRectangle = element.BoundingRectangle;
			pathInfo.Name = "";
			pathInfo.ControlType = element.Role;
			pathInfo.Img = self.GetCapBase64(element.BoundingRectangle);
			pathInfo.ElementType = ElementType.VcOcr;
			pathInfo.Path = "ocr";
			pathInfo.ElementEditNodes = new List<ElementEdit>();
			return pathInfo;
		}

		public static ElementPath GetPathInfo_ByRuntime(this WinPathComponent self, IUIAutomationElement element, ElementPath elePath)
		{
			ElementPath pathInfo = new()
			{
				Id = IdGenerater.Instance.GenerateId()
			};
			var inspect = self.GetParent<WinFormInspectComponent>();
			var elementWin = element.Convert();
			var processInfo = inspect.GetProcessInfo(elementWin.NativeElement);
			pathInfo.ProcessName = processInfo.ProcessName;
			pathInfo.BoundingRectangle = elementWin.BoundingRectangle;
			pathInfo.MainWindowTitle = processInfo.MainWindowTitle;
			pathInfo.Name = elementWin.Name;
			pathInfo.ControlType = elementWin.ControlTypeName.ToString();
			pathInfo.Img = self.GetCapBase64(pathInfo.BoundingRectangle);
			pathInfo.NativeWindowTitle = elementWin.NativeMainTitle;
			pathInfo.Value = elementWin.Value;
			pathInfo.Text = elementWin.Text;
			var handler = element.CurrentNativeWindowHandle;
			if (handler == default)
				handler = element.GetNativeWindowHandle().CurrentNativeWindowHandle;
			var id = element.GetRuntimeId();
			pathInfo.ElementType = ElementType.UIAUI;
			pathInfo.PathNode = new ElementNode();
			pathInfo.PathNode.PathRuntime = new ElementNodeRuntime()
			{
				Handle = handler,
				RuntimeId = id,
				ControlType = element.CurrentControlType,
				Role = element.CurrentAriaRole
			};
			return pathInfo;
		}
		public static ElementPath GetPathInfo_ByPRuntime(this WinPathComponent self, IUIAutomationElement element, ElementPath elePath)
		{
			ElementPath pathInfo = new()
			{
				Id = IdGenerater.Instance.GenerateId()
			};
			var inspect = self.GetParent<WinFormInspectComponent>();
			var elementPlaywright = elePath.PathNode.CurrentElementPlaywright;
			var processInfo = inspect.GetProcessInfo(element);
			pathInfo.ProcessName = processInfo.ProcessName;
			pathInfo.BoundingRectangle = elementPlaywright.BoundingRectangle;
			pathInfo.MainWindowTitle = processInfo.MainWindowTitle;
			pathInfo.Name = elementPlaywright.Name;
			pathInfo.ControlType = elementPlaywright.ControlTypeName.ToString();
			pathInfo.Img = self.GetCapBase64(pathInfo.BoundingRectangle);
			pathInfo.NativeWindowTitle = elementPlaywright.WindowTitle;
			pathInfo.Value = elementPlaywright.Value;
			pathInfo.Text = elementPlaywright.Text;
			var handler = element.CurrentNativeWindowHandle;
			if (handler == default)
				handler = element.GetNativeWindowHandle().CurrentNativeWindowHandle;
			var id = element.GetRuntimeId();
			pathInfo.ElementType = ElementType.UIAUI;
			pathInfo.PathNode = elePath.PathNode;
			pathInfo.PathNode.PathRuntime = new ElementNodeRuntime()
			{
				Handle = handler,
				RuntimeId = id,
				ControlType = element.CurrentControlType,
				Role = element.CurrentAriaRole
			};
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
						new ElementProperty(nameof(x.CurrentElementWin.Name),x.CurrentElementWin.Name,x.CurrentElementWin.Name),
						new ElementProperty(nameof(x.Index),x.Index+"",x.Index),
						new ElementProperty(nameof(x.CurrentElementWin.ControlType),x.CurrentElementWin.ControlType.ToString(),x.CurrentElementWin.ControlType)
					  }
				  };
			  }).ToList();
			return edits;
		}
		public static string GetPathJson(this WinPathComponent self, IUIAutomationElement element)
		{
			var path = self.GetPathInfo(element);
			return JsonHelper.ToJson(path);
		}
		public static CaptureImage GetCap(this WinPathComponent self, Rectangle rec)
		{
			var widthMax = User32.GetSystemMetrics(SystemMetric.SM_CXSCREEN);
			var heightMax = User32.GetSystemMetrics(SystemMetric.SM_CYSCREEN);
			rec.X = Math.Max(0, rec.X);
			rec.Y = Math.Max(0, rec.Y);
			return CaptureComponent.Instance.Rectangle(rec);
		}
		public static string GetCapBase64(this WinPathComponent self, Rectangle rec)
		{
			rec = new Rectangle(rec.X - 5, rec.Y - 5, rec.Width + 10, rec.Height + 10);
			return self.ImgToBase64String(self.GetCap(rec).Bitmap);
		}
		public static Image Base64ToImage(string imgStr, Size size)
		{
			String base64 = imgStr.Substring(imgStr.IndexOf(",") + 1);      //将‘，’以前的多余字符串删除
			byte[] arr = Convert.FromBase64String(base64);//将纯净资源Base64转换成等效的8位无符号整形数组
			System.IO.MemoryStream ms = new System.IO.MemoryStream(arr);//转换成无法调整大小的MemoryStream对象
			var bitmap = new System.Drawing.Bitmap(ms);//将MemoryStream对象转换成Bitmap对象
			return ResizeImage(bitmap, size);
		}

		public static Bitmap ResizeImage(Bitmap mg, Size newSize)
		{
			double ratio;//压缩比
			int myWidth;
			int myHeight;
			int x = 0;
			int y = 0;
			if ((mg.Width / Convert.ToDouble(newSize.Width)) > (mg.Height / Convert.ToDouble(newSize.Height)))
				ratio = Convert.ToDouble(mg.Width) / Convert.ToDouble(newSize.Width);
			else
				ratio = Convert.ToDouble(mg.Height) / Convert.ToDouble(newSize.Height);
			myHeight = (int)Math.Ceiling(mg.Height / ratio);
			myWidth = (int)Math.Ceiling(mg.Width / ratio);
			Bitmap bp = new Bitmap(newSize.Width, newSize.Height);
			x = (newSize.Width - myWidth) / 2;
			y = (newSize.Height - myHeight) / 2;
			System.Drawing.Graphics g = Graphics.FromImage(bp);
			g.SmoothingMode = SmoothingMode.HighQuality;
			g.InterpolationMode = InterpolationMode.HighQualityBicubic;
			g.PixelOffsetMode = PixelOffsetMode.HighQuality;
			Rectangle rect = new Rectangle(x, y, myWidth, myHeight);
			g.DrawImage(mg, rect, 0, 0, mg.Width, mg.Height, GraphicsUnit.Pixel);
			return bp;
		}
		private static Image ResizeImage1(Bitmap imgToResize, Size size)
		{
			//获取图片宽度
			int sourceWidth = imgToResize.Width;
			//获取图片高度
			int sourceHeight = imgToResize.Height;

			float nPercent = 0;
			float nPercentW = 0;
			float nPercentH = 0;
			//计算宽度的缩放比例
			nPercentW = ((float)size.Width / (float)sourceWidth);
			//计算高度的缩放比例
			nPercentH = ((float)size.Height / (float)sourceHeight);

			if (nPercentH < nPercentW)
				nPercent = nPercentH;
			else
				nPercent = nPercentW;
			//期望的宽度
			int destWidth = (int)(sourceWidth * nPercent);
			//期望的高度
			int destHeight = (int)(sourceHeight * nPercent);

			Bitmap b = new Bitmap(sourceWidth, sourceHeight);
			Graphics g = Graphics.FromImage((System.Drawing.Image)b);
			//g.InterpolationMode = InterpolationMode.HighQualityBicubic;
			//绘制图像
			g.DrawImage(imgToResize, 0, 0, sourceWidth, sourceHeight);
			g.Dispose();
			return (System.Drawing.Image)b;
		}
		private static string ImgToBase64String(this WinPathComponent self, Bitmap bmp)
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
		public static string GetXPath(this WinPathComponent self, ElementNode node)
		{
			if (node.Parent == null)
				return "";
			var currentControlName = $"{node.CurrentElementWin.ControlTypeName}";
			currentControlName += $"[{node.Index}]"; //查找序列
			return $"{GetXPath(self, node.Parent)}/{currentControlName}";
		}
		/// <summary>
		/// 获取node的json信息
		/// </summary>
		/// <param name="self"></param>
		/// <param name="node"></param>
		/// <returns></returns>
		public static string GetJson(this WinPathComponent self, ElementNode node)
		{
			var inspect = self.GetParent<WinFormInspectComponent>();
			return JsonHelper.ToJson(node);
		}
	}

}

