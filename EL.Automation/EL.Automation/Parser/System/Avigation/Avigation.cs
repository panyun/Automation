using Automation.Inspect;

namespace Automation.Parser
{
    public interface IAvigation
    {
        public IEnumerable<Element> TryAvigation(ElementPath elementPath, int timeOut);
    }
    /// <summary>
    /// 路径导航
    /// </summary>
    public static class Avigation
    {
        /// <summary>
        /// 路径解析器入口
        /// </summary>
        /// <param name="elementPath"></param>
        /// <returns></returns>
        public static IAvigation Create(ElementPath elementPath)
        {
            if (elementPath.ElementType == ElementType.UIAUI)
                return GetUIAUI(elementPath.AvigationType);
            if (elementPath.ElementType == ElementType.PlaywrightUI)
                return GetUIAUI(elementPath.AvigationType);
            if (elementPath.ElementType == ElementType.JABUI)
                return new ElementNodeAvigationSystem_Java();
            if (elementPath.ElementType == ElementType.MSAAUI)
                return new ElementNodeAvigationSystem_Msaa();
            if (elementPath.ElementType == ElementType.VcOcr)
                return new ElementNodeAvigationSystem_CvOcr();
            return new ElementNodeAvigationSystem();
        }
        public static IAvigation GetUIAUI(AvigationType avigationType)
        {
            switch (avigationType)
            {
                case AvigationType.None:
                    return new ElementNodeAvigationSystem();
                case AvigationType.Edit:
                    return new ElementNodeAvigationSystem_Edit();
                case AvigationType.Similarity:
                    return new ElementNodeAvigationSystem_Similarity();
                case AvigationType.ConsineSimilarity:
                    return new ElementNodeAvigationSystem_ConsineSimilarity();
                case AvigationType.Runtime:
                    return new ElementNodeAvigationSystem_Runtime();
                default:
                    break;
            }
            return new ElementNodeAvigationSystem();
        }
        public static IEnumerable<Element> AvigationElement(this RequestBase self)
        {
            IEnumerable<Element> elements;
            if (self.IsAvigationPath) // 是否动态查找路径
            {
                elements = Create(self.ElementPath).TryAvigation(self.ElementPath, self.TimeOut);
            }
            else
            {
                elements = new List<Element>() { self.ElementPath.PathNode.CurrentElementWin.NativeElement.Convert() };
            }
            return elements;
        }
    }
}
