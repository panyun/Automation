using Automation.Inspect;
using EL;
using EL.Async;
using EL.Overlay;
using Microsoft.Playwright;
using System.Drawing;

namespace Automation.Parser
{
    public static class HighlightActionSystem
    {
        public static Task Task;
        public static async ELTask<int> Main(this HighlightActionRequest self)
        {
            var playwrightInspect = Boot.GetComponent<InspectComponent>().GetComponent<PlaywrightInspectComponent>();
            try
            {
                //self.ElementPath.PathNode.CurrentElementWin.NativeElement.SetForeground();
       
                //var l = await playwrightInspect.CurrentPage.QuerySelectorAsync(self.XPath, default);
                var rec = self.ElementPath.PathNode.CurrentElementWin.NativeElement.GetNativeWindowHandle().CurrentBoundingRectangle;
                if (playwrightInspect.CurrentPage == default)
                    return default;
                var l = playwrightInspect.CurrentPage.Locator(self.XPath);
                var bound = await l.BoundingBoxAsync();
                var count = await l.CountAsync();
                _ = l.HighlightAsync();
                if (bound != default)
                {
                    self.LightProperty.CancellationTokenSource = new CancellationTokenSource();
                    self?.LightProperty.LightHighShow(new Rectangle(rec.left + (int)bound.X, rec.top + (int)bound.Y, (int)bound.Width, (int)bound.Height));
                }
                return count;
            }
            catch (Exception ex)
            {

            }
            return default;
        }
    }
}
