using EL.UIA;
using Interop.UIAutomationClient;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EL.UIA.ControlTypeConverter;

namespace EL.CollectWebPages.Common
{
    public static class RectangleHelper
    {
        public static Rectangle ToRectangle(this IUIAutomationElement element)
        {
            Rectangle rectangle = Rectangle.Empty;
            if (element != null)
            {
                try
                {
                    rectangle = ValueConverter.ToRectangle(element.GetCurrentPropertyValue(UIA_PropertyIds.UIA_BoundingRectanglePropertyId));
                }
                catch (Exception)
                {
                }
            }
            return rectangle;
        }
        public static ControlType GetControlType(this IUIAutomationElement element)
        {
            return ToControlType(element.CurrentControlType);
        }
        public static string GetClassName(this IUIAutomationElement element)
        {
            return element?.CurrentClassName;
        }
        public static string GetAutomationId(this IUIAutomationElement element)
        {   
            return element?.CurrentAutomationId;
        }
    }
}
