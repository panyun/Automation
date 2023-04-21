using Automation.Parser;
using EL.MSAA;
using EL.UIA;
using HtmlAgilityPack;
using Interop.UIAutomationClient;
using Microsoft.Playwright;
using Newtonsoft.Json;
using System.Drawing;
using static EL.UIA.ControlTypeConverter;

namespace Automation.Inspect
{
	/// <summary>
	/// 节点信息
	/// </summary>
	public class ElementUIA : Element
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		public ElementUIA()
		{

		}
		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="self"></param>
		public ElementUIA(IUIAutomationElement self)
		{
			NativeElement = self;
			FillValue();
		}
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public override string Name { get; set; }
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public override int ControlType { get; set; }
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string ClassName { get; set; }
		[JsonIgnore]
		public override string ControlTypeName
		{
			get
			{
				return ToControlType(ControlType) + "";
			}
		}
		[JsonIgnore]
		public string Value { get; set; }
		[JsonIgnore]
		public string Text { get; set; }
		[JsonIgnore]
		public string HelpText
		{
			get
			{
				if (NativeElement != default)
					return NativeElement.GetCurrentPropertyValue(UIA_PropertyIds.UIA_HelpTextPropertyId) + "";
				return default;
			}
		}
		public string Labeled
		{
			get
			{
				if (NativeElement != default)
					return NativeElement.GetCurrentPropertyValue(UIA_PropertyIds.UIA_LabeledByPropertyId) + "";
				return default;
			}
		}
		public string Described
		{
			get
			{
				if (NativeElement != default)
					return NativeElement.GetCurrentPropertyValue(UIA_PropertyIds.UIA_DescribedByPropertyId) + "";
				return default;
			}
		}

		[JsonIgnore]
		public string NativeMainTitle { get; set; }
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public override string Role { get; set; }
		[JsonIgnore]
		public IUIAutomationElement NativeElement { get; set; }
		[JsonIgnore]
		public override Rectangle BoundingRectangle
		{
			get; set;
		}
		[JsonIgnore]
		public override Point ClickablePoint
		{
			get
			{
				//TODO WJF JAVA窗口最小化时，程序恢复显示时，取坐标为负数，原因不明
				var point = new Point(BoundingRectangle.Left + BoundingRectangle.Width / 2, BoundingRectangle.Top + BoundingRectangle.Height / 2);
				return point;
			}
		}




		public void FillValue()
		{
			Name = NativeElement.CurrentName.EscapeDataString();
			ControlType = NativeElement.CurrentControlType;
			ClassName = NativeElement.CurrentClassName;
			Text = NativeElement.GetText();
			Value = NativeElement.GetValue();
			var rec = NativeElement.GetCurrentPropertyValue(UIA_PropertyIds.UIA_BoundingRectanglePropertyId);
			BoundingRectangle = ValueConverter.ToRectangle(rec);
			Role = NativeElement.CurrentAriaRole;
		}
	}

	public static class ElementWinSystem
	{
		public static string GetText(this IUIAutomationElement nativeElement)
		{
			try
			{
				var valuePattern1 = (IUIAutomationTextPattern)nativeElement.GetCurrentPattern(UIA_PatternIds.UIA_TextPatternId);

				var valuePattern2 = (IUIAutomationTextPattern2)nativeElement.GetCurrentPattern(UIA_PatternIds.UIA_TextPattern2Id);

				//var valuePattern3 = (IUIAutomationTextChildPattern)nativeElement.GetCurrentPattern(UIA_PatternIds.UIA_TextChildPatternId);
				var valuePattern4 = (IUIAutomationTextEditPattern)nativeElement.GetCurrentPattern(UIA_PatternIds.UIA_TextEditPatternId);
				if (valuePattern1 != null)
				{
					return valuePattern1.DocumentRange.GetText(int.MaxValue);
				}
				if (valuePattern2 != null)
				{
					return valuePattern2.DocumentRange.GetText(int.MaxValue);

				}

				if (valuePattern4 != null)
				{
					return valuePattern4.DocumentRange.GetText(int.MaxValue);
				}
			}
			catch (Exception)
			{

			}

			return string.Empty;
		}
		public static string GetNativeMainTitle(this ElementUIA self)
		{
			if (self.NativeElement == default) return default;
			try
			{
				self.NativeMainTitle = self.NativeElement.GetNativeWindowHandle().CurrentName.EscapeDataString();
			}
			catch (Exception)
			{
			}
			return self.NativeMainTitle;
		}
		public static dynamic Get(this IUIAutomationElement self)
		{
			return new
			{
				Name = self.CurrentName.EscapeDataString(),
				ControlType = self.CurrentControlType,
				ControlTypeName = ToControlType(self.CurrentControlType) + "",
				Role = self.CurrentAriaRole,
				Value = GetValue(self)
			};
		}
		public static string GetName(this IUIAutomationElement self) => self.CurrentName.EscapeDataString();
		public static string GetValue(this IUIAutomationElement self)
		{
			var valuePattern = (IUIAutomationValuePattern)self.GetCurrentPattern(UIA_PatternIds.UIA_ValuePatternId);
			if (valuePattern != null)
				return valuePattern.CurrentValue;
			return "";
		}
		public static ElementUIA Convert(this IUIAutomationElement self)
		{
			if (self == null)
				return default;
			try
			{
				return new ElementUIA(self);
			}
			catch (Exception)
			{
				return default;
			}
		}
		public static ElementUIA Convert_Json(this IUIAutomationElement self)
		{
			ElementUIA elementWin = new()
			{
				Name = self.CurrentName,
				ControlType = self.CurrentControlType,
				ClassName = self.CurrentClassName,
				Role = self.CurrentAriaRole,
				NativeElement = self
			};
			return elementWin;
		}
		public static Rectangle GetRectangle(this IUIAutomationElement self)
		{
			var rec = self.GetCurrentPropertyValue(UIA_PropertyIds.UIA_BoundingRectanglePropertyId);
			return ValueConverter.ToRectangle(rec);

		}

		public static Point GetClickablePoint(this IUIAutomationElement self)
		{
			var rec = self.GetCurrentPropertyValue(UIA_PropertyIds.UIA_BoundingRectanglePropertyId);
			var boundingRectangle = ValueConverter.ToRectangle(rec);
			var point = new Point(boundingRectangle.Left + boundingRectangle.Width / 2, boundingRectangle.Top + boundingRectangle.Height / 2);
			return point;
		}
		public static string GetControlTypeName(this IUIAutomationElement self)
		{
			return ToControlType(self.CurrentControlType) + "";
		}
		public static ElementIns ConvertElementInspect
			(this IUIAutomationElement self)
		{
			return new ElementIns(self);
		}


		public static IUIAutomationElement Convert(this ElementUIA element)
		{
			return element.NativeElement;
		}
		public static List<ElementUIA> Converts(this IUIAutomationElementArray self)
		{
			List<ElementUIA> list = new();
			for (int i = 0; i < self.Length; i++)
			{
				var entity = self.GetElement(i).Convert();
				list.Add(entity);
			}
			return list;
		}

	}

	public class ElementIns
	{
		public ElementIns(IUIAutomationElement uIAutomationElement)
		{
			ElementUIA = uIAutomationElement;
			var rec = ElementUIA.GetCurrentPropertyValue(UIA_PropertyIds.UIA_BoundingRectanglePropertyId);
			Rectangle = ValueConverter.ToRectangle(rec);
			ElementType = ElementType.UIAUI;
		}
		public ElementIns(ElementJAB element)
		{
			ElementJAB = element;
			Rectangle = ElementJAB.BoundingRectangle;
			ElementType = ElementType.JABUI;
		}
		public ElementIns(MSAAProperties element)
		{
			ElementMSAA = element;
			Rectangle = ElementMSAA.BoundingRectangle;
			ElementType = ElementType.MSAAUI;
		}
		public ElementIns(ElementPlaywright element)
		{
			ElementPlaywright = element;
			if (element.BrowserType == Parser.BrowserType.Chromium)
				Rectangle = element.BoundingRectangleOffsetChrome;
			else
				Rectangle = element.BoundingRectangleOffsetMsedge;
			ElementType = ElementType.PlaywrightUI;
		}
		public ElementIns(ElementCvInfo element)
		{
			ElementVcOcr = element;
			Rectangle = ElementVcOcr.Rect;
			ElementType = ElementType.VcOcr;
		}
		public ElementIns(Rectangle rec)
		{
			Rectangle = rec;
			ElementType = ElementType.VcOcr;
		}
		public Rectangle Rectangle
		{
			get; set;
		}

		public IUIAutomationElement ElementUIA { get; set; }
		public ElementJAB ElementJAB { get; set; }
		public MSAAProperties ElementMSAA { get; set; }
		public ElementPlaywright ElementPlaywright { get; set; }
		public ElementCvInfo ElementVcOcr { get; set; }
		public string ControlType
		{
			get
			{
				return GetControlType();
			}
		}
		public ElementType ElementType
		{
			get;
		}
		public int ProcessId
		{
			get
			{
				if (ElementType == ElementType.UIAUI)
					return ElementUIA.CurrentProcessId;
				return -1;
			}
		}
		public static bool Equal(ElementIns baseIns, ElementIns ins)
		{
			if (baseIns == default) return false;
			if (baseIns.ElementType != ins.ElementType) return false;
			if (baseIns.ElementType == ElementType.UIAUI)
				return Equal(baseIns.ElementUIA, ins.ElementUIA);
			if (baseIns.ElementType == ElementType.PlaywrightUI)
				return baseIns.ElementPlaywright.XPath == ins.ElementPlaywright.XPath;
			//if (baseIns.ElementType == ElementType.MSAAUI)
			//    return baseIns.ElementMSAA.Id == ins.ElementMSAA.Id;
			if (baseIns.ElementType == ElementType.JABUI)
				return baseIns.ElementJAB.Id == ins.ElementJAB.Id;
			//if (baseIns.ElementType == ElementType.VcOcr)
			//    return baseIns.ElementVcOcr.Rect == ins.ElementVcOcr.Rect;
			return false;
		}
		public string GetControlType()
		{
			try
			{
				if (this.ElementType == ElementType.UIAUI)
					return this.ElementUIA.GetControlTypeName();
				if (this.ElementType == ElementType.PlaywrightUI)
					return this.ElementPlaywright.ControlTypeName;
				if (this.ElementType == ElementType.MSAAUI)
					return this.ElementMSAA.Role;
				if (this.ElementType == ElementType.JABUI)
					return this.ElementJAB.ControlTypeName;
				if (this.ElementType == ElementType.VcOcr)
					return "图像";
				return "未识别";
			}
			catch (Exception)
			{
				return "未识别";
			}
		
		}
		public static bool Equal(IUIAutomationElement a, IUIAutomationElement b)
		{
			try
			{
				var aId = a.GetRuntimeId();
				var bId = b.GetRuntimeId();
				if (aId?.Length > 0 && bId?.Length > 0)
					return Enumerable.SequenceEqual(aId, bId);
				return false;
			}
			catch (Exception)
			{
				return false;
			}

		}
	}
}
