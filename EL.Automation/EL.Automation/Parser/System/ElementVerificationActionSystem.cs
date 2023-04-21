using Automation.Inspect;
using EL;
using EL.Async;
using EL.Overlay;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automation.Parser
{
    public static class ElementVerificationActionSystem
    {
        public static ElementPath Main(this ElementVerificationActionRequest self)
        {
            var selfCopy = self.Copy();
            selfCopy.ElementPath.ElementEditNodes = selfCopy.ElementEditNodes;
            IEnumerable<Element> elements;
            if (self.ElementPath.AvigationType == AvigationType.ConsineSimilarity)
            {
                GenerateCosineSimilarActionRequest request = new GenerateCosineSimilarActionRequest()
                {
                    ElementPath = self.ElementPath,
                    CosineValue = self.ElementPath.CosineValue,
                    LightProperty = self.LightProperty,
                    TimeOut = self.TimeOut
                };
                (ElementPath elementPath, int count, elements) = GenerateCosineSimilarActionSystem.Main(request);
            }
            else
            {
                elements = self.AvigationElement();
                self.LightProperty.LightHighMany(elements.ToArray());
            }
            var element = elements.FirstOrDefault();
            if (elements == null || !elements.Any() || element == default) throw new ParserException("无法定位到目标元素！");
            var inspect = Boot.GetComponent<InspectComponent>();
            var winFormInspectComponent = inspect.GetComponent<WinFormInspectComponent>();
            var winpath = winFormInspectComponent.GetComponent<WinPathComponent>();
            selfCopy.ElementPath.Img = winpath.GetCapBase64(element.BoundingRectangle);
            return selfCopy.ElementPath;
        }
        public static ElementVerificationActionRequest Copy(this ElementVerificationActionRequest self)
        {
            var json = JsonHelper.ToJson(self);
            return JsonHelper.FromJson<ElementVerificationActionRequest>(json);
        }

    }
}
