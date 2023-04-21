using Automation.Inspect;
using Automation.Parser;
using EL.Async;

namespace EL.Robot.Component
{
    /// <summary>
    /// 节点高亮
    /// </summary>
    public class InputActionComponent : BaseComponent
    {
        public override async ELTask<INodeContent> Main(INodeContent self)
        {
            await base.Main(self);
            DisplayName = self.CurrentNode.Name;
            object element = default;
            try
            {
                element = self.CurrentNode.GetParamterValue("ElementPath");
            }
            catch (Exception ex)
            {
                throw new ELNodeHandlerException($"获取属性[ElementPath]为空！");
            }
            var inputTxt = self.CurrentNode.GetParamterValue("InputTxt") + "";
            var inputtypeStr = self.CurrentNode.GetParamterValue("inputtype") + "";
            Enum.TryParse(inputtypeStr, out InputType inputtype);
            bool.TryParse(self.CurrentNode.GetParamterValue("isclear") + "", out var isclear);
            int.TryParse(self.CurrentNode.GetParamterValue("TimeOut") + "", out var timeOut);
            InputActionRequest request = new()
            {
                InputTxt = inputTxt,
                InputType = inputtype,
                IsClear = isclear,
                TimeOut = timeOut
            };
            if (element is ElementPath elementPath)
            {
                request.ElementPath = elementPath;
                await UtilsComponent.Exec(request);
            }
            else if (element is ElementUIA ele)
            {
                InputActionSystem.UIAMain(request, new List<ElementUIA>() { ele });
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

