using Automation;
using Automation.Inspect;
using Automation.Parser;
using EL.Async;
using EL.Robot.Core;
using System.Drawing;
using System.Reflection;

namespace EL.Robot.Component
{
    /// <summary>
    /// 节点高亮
    /// </summary>
    public class ElementActionComponent : BaseComponent
    {
        public override async ELTask<INodeContent> Main(INodeContent self)
        {
            await base.Main(self);
            var elementPathStr = self.CurrentNode.GetParamterValue("content");
            if (string.IsNullOrWhiteSpace(elementPathStr + ""))
                throw new ELNodeHandlerException("ElementPath 为空!");
            ElementActionRequest request = new();
            request.LightProperty = new LightProperty()
            {
                ColorName = nameof(Color.Red),
                Count = 1,
                Time = 100
            };
            request.TimeOut = self.CurrentNode.GetBaseProperty().Timeout;
            if (elementPathStr is string elementPath)
            {
                request.ElementPath = JsonHelper.FromJson<ElementPath>(elementPath);
                //var json = $"{(int)RequestType.ElementActionRequest}{JsonHelper.ToJson(request)}";

                var res = (ElementActionResponse)await UtilsComponent.Exec(request);
                self.Value = res.Error == 0;
                self.Out = new ValueInfo() { Value = res.ElementPath };
                return self;
            }
            self.Value = true;
            self.Out = new ValueInfo() { Value = request.ElementPath };
            return self;
        }
        public static void Main(ElementUIA elementWin)
        {
            ElementActionRequest elementActionRequest = new ElementActionRequest
            {
                LightProperty = new LightProperty()
                {
                    ColorName = nameof(Color.Red),
                    Count = 1,
                    Time = 100
                }
            };
            ElementActionSystem.Main(elementActionRequest, elementWin);
        }
    }
  
}

