using Automation.Inspect;
using EL;
using EL.MSAA;
using Interop.UIAutomationClient;
using System.Diagnostics;
using System.Drawing;

namespace Automation.Parser
{
    public class ElementNodeAvigationSystem_Msaa : IAvigation
    {
        public IEnumerable<Element> TryAvigation(ElementPath elementPath, int timeOut)
        {
            var elementWindows = Avigation.GetUIAUI(elementPath.AvigationType).TryAvigation(elementPath, default);
            var eleWindow = (ElementUIA)elementWindows.FirstOrDefault();
            var cIndexJson = JsonHelper.ToJson(elementPath.PathNode.ChildIndexs);
            var elementMSAA = ElementFromPoint_MSAA(eleWindow, JsonHelper.FromJson<List<int>>(cIndexJson));
            elementMSAA.MSAAProperties.ChildIndexs = elementPath.PathNode.ChildIndexs;
            return new List<Element>() { elementMSAA };
        }
        public static ElementMSAA ElementFromPoint_MSAA(ElementUIA ele, List<int> pathIndexs)
        {
            var eleWindow = ele.NativeElement.GetNativeWindowHandle();
            MSAAComponent mSAAComponent = new(eleWindow.CurrentNativeWindowHandle);
            var massElement = GetChild_MSAA(mSAAComponent, pathIndexs);
            massElement.ElementUIA = eleWindow;
            return massElement.Convert();
        }
        public static MSAAProperties GetChild_MSAA(MSAAComponent mSAAComponent, List<int> pathIndexs)
        {
            var childs = mSAAComponent.GetAccessibleChildren();
            if (childs == null || childs.Count == 0)
                return mSAAComponent.Properties;
            if (pathIndexs.Count > 0)
            {
                var index = pathIndexs.FirstOrDefault();
                pathIndexs.RemoveAt(0);
                return GetChild_MSAA(childs[index], pathIndexs);
            }
            return mSAAComponent.Properties;
        }
    }
}
