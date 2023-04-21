using Automation;
using Automation.Inspect;
using Automation.Parser;
using EL.Async;
using System.Diagnostics;

namespace EL.Robot.Component
{
    /// <summary>
    /// 节点高亮
    /// </summary>
    public class MouseActionComponent : BaseComponent
    {
        public override async ELTask<INodeContent> Main(INodeContent self)
        {
            await base.Main(self);
            DisplayName = self.CurrentNode.Name;
            var element = self.CurrentNode.GetParamterValue("ElementPath");
            var actiontypeStr = self.CurrentNode.GetParamterValue("actiontype") + "";
            var clicktypeStr = self.CurrentNode.GetParamterValue("clicktype") + "";
            Enum.TryParse(actiontypeStr, true, out ActionType actionType);
            Enum.TryParse(clicktypeStr, true, out ClickType clickType);
            MouseActionRequest request = new()
            {
                ActionType = actionType,
                ClickType = clickType,
                LocationType = LocationType.Center,
                OffsetX = 0,
                OffsetY = 0,
                TimeOut = 10000
            };
            if (element is ElementPath)
            {
                var elementPath = element as ElementPath;
                if (elementPath == null)
                    throw new ELNodeHandlerException("elementPath is null!");
                request.ElementPath = elementPath;
                await UtilsComponent.Exec(request);
            }
            else if (element is ElementUIA ele)
            {
                MouseActionSystem.UIAMain(request, new List<ElementUIA>() { ele });
            }
            else
            {
                throw new ELNodeHandlerException("引用参数的类型不匹配！");
            }

            self.Value = true;
            return self;
        }
    }
}

