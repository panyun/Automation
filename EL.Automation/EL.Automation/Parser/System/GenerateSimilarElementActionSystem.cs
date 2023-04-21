using Automation.Inspect;
using Automation.Parser;

namespace Automation.Parser
{
    public static class GenerateSimilarElementActionSystem
    {
        public static (ElementPath, int) Main(this GenerateSimilarElementActionRequest self)
        {
            var firstElement = self.ElementPath;
            var lastElement = self.LastElementPath;
            var elementPath = Analyse(firstElement, lastElement);
            var elements = self.AvigationElement();
            self.LightProperty.LightHighMany(elements.ToArray());
            return (elementPath, elements.Count());
            //高亮显示
        }
        /// <summary>
        /// 获取至少2个待查模板元素，通过2个待查模板元素的DOM路径，提取出路径的差异
        ///点，进而定位2个待查模板元素的最小公共DOM层级，即该层DOM开始分叉相似元素，最后通
        ///过差异路径点后续的相同路径，即可获取到全部的相似元素。
        /// </summary>
        /// <param name="basePath"></param>
        /// <param name="elementPath"></param>
        /// <returns></returns>
        public static ElementPath Analyse(ElementPath basePath, ElementPath elementPath)
        {
            //节点目录树不一致
            var baseNodes = basePath.PathNode.GetParentNode().OrderBy(x => x.LevelIndex).ToList();
            var nodes = elementPath.PathNode.GetParentNode().OrderBy(x => x.LevelIndex).ToList();
            if (baseNodes.Count != nodes.Count)
                throw new ParserException("节点目录树不一致");
            ElementNode differenceNode = default;
            for (int i = 0; i < baseNodes.Count; i++)
            {
                var baseNode = baseNodes[i];
                var node = nodes[i];
                var baseCompareId = baseNode.GetCompareValue(nameof(ElementExpand.CompareId));
                var compareId = node.GetCompareValue(nameof(ElementExpand.CompareId));
                if (baseCompareId != compareId)
                {
                    differenceNode = baseNode;
                    break;
                }
            }
            if (differenceNode == null)
                differenceNode = baseNodes[baseNodes.Count - 1];
            var editNodes = basePath.ElementEditNodes.Find(x => x.Id == differenceNode.Id);
            if (editNodes == null) throw new ParserException("程序异常！");
            editNodes.IsSimilarity = true;
            return basePath;
        }

    }
}
