using Automation.Inspect;
using EL;
using EL.Async;
using Interop.UIAutomationClient;
using Microsoft.Playwright;
using System.CodeDom;
using System.Runtime.CompilerServices;

namespace Automation.Parser
{
    public static class ElementActionSystem
    {
        public static async ELTask<ElementPath> Main(this ElementActionRequest self)
        {

            if (self.ElementType == ElementType.UIAUI)
            {
                var elements = self.AvigationElement();
                var element = elements.FirstOrDefault();
                return self.Main((ElementUIA)element);
            }
            if (self.ElementType == ElementType.JABUI)
            {
                var elements = self.AvigationElement();
                var element = elements.FirstOrDefault();
                return self.Main((ElementJAB)element);
            }
            if (self.ElementType == ElementType.MSAAUI)
            {
                var elements = self.AvigationElement();
                var element = elements.FirstOrDefault();
                return self.Main((ElementMSAA)element);
            }
            if (self.ElementType == ElementType.PlaywrightUI)
            {
                var elements = self.AvigationElement();
                var element = elements.FirstOrDefault();
                return await self.Main(self.ElementPath.PathNode.CurrentElementPlaywright);
            }
            if (self.ElementType == ElementType.VcOcr)
            {
                return await self.Main(self.ElementPath.PathNode.CurrentElementVcOcr);
            }
            throw new ParserException("未找到目标元素！");
        }
        public static async ELTask<ElementPath> Main(this ElementActionRequest self, ElementVcOcr win)
        {
            try
            {
                var elements = self.AvigationElement();
                var element = (ElementVcOcr)elements.FirstOrDefault();
                self.LightProperty.LightHighMany(default, element.BoundingRectangle);
                return self.ElementPath;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return self.ElementPath;
            }

        }

        public static async ELTask<ElementPath> Main(this ElementActionRequest self, ElementPlaywright win)
        {
            try
            {
                var playwrightInspect = Boot.GetComponent<InspectComponent>().GetComponent<PlaywrightInspectComponent>();
                var inspect = Boot.GetComponent<InspectComponent>();
                var winfromInspect = inspect.GetComponent<WinFormInspectComponent>();
                var pathComponent = winfromInspect.GetComponent<WinPathComponent>();
                var obj = playwrightInspect.FindLocatorByPath(win);
                var playwright = self.ElementPath.PathNode.CurrentElementPlaywright;
                var elementHtml = await playwrightInspect.GetElementByPath(playwright.XPath);
                playwright.Update(elementHtml);
                playwright.SetForeground();
                elementHtml = await playwrightInspect.GetElementByPath(playwright.XPath);
                playwright.Update(elementHtml);
                self.LightProperty.LightHighMany(default, playwright.GetScreenBoundingRectangle());
                return self.ElementPath;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return self.ElementPath;
            }

        }

        public static ElementPath Main(this ElementActionRequest self, ElementMSAA win)
        {
            var inspect = Boot.GetComponent<InspectComponent>();
            var winfromInspect = inspect.GetComponent<WinFormInspectComponent>();
            var pathComponent = winfromInspect.GetComponent<WinPathComponent>();
            var elementPath = pathComponent.GetPathInfo(win);
            elementPath.PathNode = self.ElementPath.PathNode;
            self.LightProperty.LightHighMany(win);
            return elementPath;
        }
        public static ElementPath Main(this ElementActionRequest self, ElementJAB java)
        {
            var inspect = Boot.GetComponent<InspectComponent>();
            var winfromInspect = inspect.GetComponent<JavaFormInspectComponent>();
            var pathComponent = winfromInspect.GetComponent<JavaPathComponent>();
            var elementPath = pathComponent.GetPathInfo(java.AccessibleNode);
            self.LightProperty.LightHighMany(java);
            return elementPath;
        }
        public static ElementPath Main(this ElementActionRequest self, ElementUIA win)
        {
            var inspect = Boot.GetComponent<InspectComponent>();
            var winfromInspect = inspect.GetComponent<WinFormInspectComponent>();
            var pathComponent = winfromInspect.GetComponent<WinPathComponent>();
            var elementPath = pathComponent.GetPathInfo_ByRuntime(win.NativeElement, self.ElementPath);
            if (self?.ElementPath?.PathNode != default)
                elementPath.PathNode = self.ElementPath.PathNode;
            self?.LightProperty.LightHighMany(win);
            return elementPath;
        }
    }
}
