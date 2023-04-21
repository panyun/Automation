using Automation.Inspect;
using EL;
using EL.Async;

namespace Automation.Parser
{
    public static class GenerateHtmlActionSystem
    {
        public static async ELTask<string> Main(this GenerateHtmlActionRequest self)
        {
            var playwrightInspect = Boot.GetComponent<InspectComponent>().GetComponent<PlaywrightInspectComponent>();
    
            var doc = await playwrightInspect.CurrentPage.QuerySelectorAsync("//body");
            var html = await doc.InnerHTMLAsync();
            return html;
        }
    }
}
