using Automation.Inspect;
using EL;
using EL.Async;
using EL.Input;
using EL.Overlay;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Interop.UIAutomationClient;

namespace Automation.Parser
{
    public static class CosineSimilarElementActionSystem
    {
        public static IEnumerable<Element> Main(this CosineSimilarElementActionRequest self)
        {
            if (self.ElementPath.AvigationType != AvigationType.ConsineSimilarity)
                throw new ParserException("元素信息非余弦元素路径！");
            var value = self.ElementPath.CosineValue;
            self.ElementPath.CosineValue = -1;
            var baseElement = (ElementUIA)self.AvigationElement().FirstOrDefault();
            var elementPath = new ElementPropertyActionRequest().Main(baseElement);
            //2.获取相似度值
            elementPath.CosineValue = value;
            self.ElementPath = elementPath;
            //3.获取要筛选的相似度对象树
            var elements = self.AvigationElement();
            var wins = GenerateCosineSimilarActionSystem.Analyse(baseElement, elements, elementPath.CosineValue);
            self.LightProperty.LightHighMany(wins.ToArray());
            return wins;
        }
    }
}
