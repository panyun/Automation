using Automation.Inspect;
using SimMetrics.Net.API;
using SimMetrics.Net.Utilities;

namespace Automation.Parser
{
    public static class GenerateCosineSimilarActionSystem
    {
        public static (ElementPath, int, List<Element> wins) Main(this GenerateCosineSimilarActionRequest self)
        {
            //1.获取base控件对象
            self.ElementPath.CosineValue = -1;
            var baseElement = (ElementUIA)self.AvigationElement().FirstOrDefault();
            var name = baseElement.NativeElement.CurrentName;
            var elementPath = new ElementPropertyActionRequest().Main(baseElement);
            //2.获取相似度值
            elementPath.CosineValue = self.CosineValue;
            self.ElementPath = elementPath;
            //3.获取要筛选的相似度对象树
            var elements = self.AvigationElement();
            var wins = Analyse(baseElement, elements, elementPath.CosineValue);
            if (wins == null || wins.Count == 0) throw new ParserException("未找到元素节点");
            self.LightProperty.LightHighMany(wins.ToArray());
            return (elementPath, wins.Count, wins);
        }
        public static List<Element> Analyse(Element baseElement, IEnumerable<Element> elementWins, double consineValue)
        {
            List<Element> eles = new();
            foreach (var element in elementWins)
            {
                var vec = GetVector((ElementUIA)baseElement, (ElementUIA)element);
                if (CosineValue(vec, consineValue))
                    eles.Add(element);
            }
            return eles;
        }
        public static List<Element> Analyse1(this GenerateCosineSimilarActionRequest self, Element baseElement, IEnumerable<Element> elementWins)
        {
            var baseContent = GetContent((ElementUIA)baseElement);
            List<Element> eles = new() { baseElement };
            SimHashAnalyser simHashAnalyser = new();
            foreach (var element in elementWins)
            {
                var content = GetContent((ElementUIA)element);
                if (simHashAnalyser.GetLikenessValue(baseContent, content) >= self.CosineValue)
                    eles.Add(element);
            }
            return eles;
        }
        public static string GetContent(ElementUIA element)
        {
            return $"{element.Name}|{element.ControlTypeName}|{element.Role}|{element.Value}|{element.Text}|{element.ClassName}";
        }
        public static List<int> GetVector(ElementUIA baseElement, ElementUIA element)
        {
            if (element == default)
            {
                return new()
            {
                0,0,0,0,0,0,0
            };
            }
            List<int> vector = new()
            {
                baseElement.Name == element.Name?1:0,
                baseElement.ControlTypeName == element.ControlTypeName?1:0,
                baseElement.Role == element.Role?1:0,
                baseElement.Value == element.Value?1:0,
                baseElement.Text == element.Text?1:0,
                baseElement.ClassName == element.ClassName?1:0,
               Math.Abs( (baseElement.BoundingRectangle.Height*baseElement.BoundingRectangle.Width) -( element.BoundingRectangle.Height*element.BoundingRectangle.Width))<=(baseElement.BoundingRectangle.Height*baseElement.BoundingRectangle.Width)/5?1:0
            };
            return vector;
        }
        public static bool CosineValue(List<int> values, double cosineValue)
        {
            List<int> baseValue = new()
            {
                1,1,1,1,1,1,1
            };
            double val1 = 0;
            double val2 = 0;
            double val3 = 0;
            for (int i = 0; i < baseValue.Count; i++)
            {
                val1 += baseValue[i] * values[i];
                val2 += Math.Pow(baseValue[i], 2);
                val3 += Math.Pow(values[i], 2);
            }
            var temp = val1 / (Math.Sqrt(val2) * Math.Sqrt(val3));
            return temp >= cosineValue;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class SimHashAnalyser
    {
        #region Constants and Fields

        private const int HashSize = 32;

        #endregion

        #region Public Methods and Operators

        public float GetLikenessValue(string needle, string haystack)
        {
            var needleSimHash = this.DoCalculateSimHash(needle);
            var hayStackSimHash = this.DoCalculateSimHash(haystack);
            return (HashSize - GetHammingDistance(needleSimHash, hayStackSimHash)) / (float)HashSize;
        }

        #endregion

        #region Methods

        private static IEnumerable<int> DoHashTokens(IEnumerable<string> tokens)
        {
            var hashedTokens = new List<int>();
            foreach (string token in tokens)
            {
                hashedTokens.Add(token.GetHashCode());
            }
            return hashedTokens;
        }

        private static int GetHammingDistance(int firstValue, int secondValue)
        {
            var hammingBits = firstValue ^ secondValue;
            var hammingValue = 0;
            for (int i = 0; i < 32; i++)
            {
                if (IsBitSet(hammingBits, i))
                {
                    hammingValue += 1;
                }
            }
            return hammingValue;
        }

        private static bool IsBitSet(int b, int pos)
        {
            return (b & (1 << pos)) != 0;
        }

        private int DoCalculateSimHash(string input)
        {
            ITokeniser tokeniser = new TokeniserQGram2();
            var hashedtokens = DoHashTokens(tokeniser.Tokenize(input));
            var vector = new int[HashSize];
            for (var i = 0; i < HashSize; i++)
            {
                vector[i] = 0;
            }

            foreach (var value in hashedtokens)
            {
                for (var j = 0; j < HashSize; j++)
                {
                    if (IsBitSet(value, j))
                    {
                        vector[j] += 1;
                    }
                    else
                    {
                        vector[j] -= 1;
                    }
                }
            }

            var fingerprint = 0;
            for (var i = 0; i < HashSize; i++)
            {
                if (vector[i] > 0)
                {
                    fingerprint += 1 << i;
                }
            }
            return fingerprint;
        }

        #endregion
    }
}
