using Automation;
using Automation.Inspect;
using Automation.Parser;
using EL.Async;
using EL.Robot.Core;
using System.Diagnostics;
using System.Drawing;

namespace EL.Robot.Component
{
    /// <summary>
    /// 节点高亮
    /// </summary>
    public class SimilarElementActionComponent : BaseComponent
    {
        public override async ELTask<INodeContent> Main(INodeContent self)
        {
            await base.Main(self);
            DisplayName = self.CurrentNode.Name;
            var elementPathStr = self.CurrentNode.GetParamterValue("content");
            if (string.IsNullOrWhiteSpace(elementPathStr + ""))
                throw new ELNodeHandlerException("ElementPath 为空!");
            var capturetype = self.CurrentNode.GetParamterInt("capturetype");
            if (capturetype == 1)
            {
                CosineSimilarElementActionRequest request = new()
                {
                    LightProperty = new LightProperty()
                    {
                        ColorName = nameof(Color.Red),
                        Count = 3,
                        Time = 500
                    },
                    TimeOut = self.CurrentNode.GetBaseProperty().Timeout,
                    ElementPath = JsonHelper.FromJson<ElementPath>(elementPathStr + "")
                };
                var response = (CosineSimilarElementActionResponse)await UtilsComponent.Exec(request);
                self.Out = response.Elements;
                self.Value = true;
                return self;
            }
            if (capturetype == 2)
            {
                SimilarElementActionRequest request = new()
                {
                    LightProperty = new LightProperty()
                    {
                        ColorName = nameof(Color.Red),
                        Count = 3,
                        Time = 100
                    },
                    TimeOut = self.CurrentNode.GetBaseProperty().Timeout,
                    ElementPath = JsonHelper.FromJson<ElementPath>(elementPathStr + "")
                };
                var response = (SimilarElementActionResponse)await UtilsComponent.Exec(request);
                self.Out = response.Elements;
                self.Value = true;
                return self;
            }
            throw new ELNodeHandlerException("相似元素组件异常！");
        }
    }
}

