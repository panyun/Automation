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
                self.Out = res.ElementPath;
                return self;
            }
            self.Value = true;
            self.Out = request.ElementPath;
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
    public static class UtilsComponent
    {
        public static ELTask<WinParserCallValue> CompleteTrigger = ELTask<WinParserCallValue>.Create();
        /// <summary>
        /// 异步调用
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="ELNodeHandlerException"></exception>
        public static async ELTask<IResponse> Exec(IRequest request)
        {
            var res = await DispatcherHelper.ExecInspectAsync(async (inspect, parser) =>
                        {
                            IResponse res = new ResponseBase();
                            try
                            {
                                res = await RequestManager.StartAsync(request);
                                if (res.Error != 0)
                                    throw new ELNodeHandlerException(res.Message);
                                return res;

                            }
                            catch (Exception ex)
                            {
                                throw new ELNodeHandlerException(ex.Message);
                            }
                        });
            return res;
        }
        /// <summary>
        /// 获取对象值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propName"></param>
        /// <returns></returns>
        /// <exception cref="ELNodeHandlerException"></exception>
        public static string GetPropertyVal(object obj, string propName)
        {
            string val = string.Empty;
            var property = obj.GetType().GetProperty(propName, BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);
            if (property == default) throw new ELNodeHandlerException("未找到对应属性！");
            val = property.GetValue(obj) + "";
            return val;
        }
    }
    public class WinParserCallValue
    {
        public WinParserCallValue(bool isPass, IResponse value, string? msg = default)
        {
            this.IsPass = isPass;
            this.Value = value;
            ExMsg = msg;
        }
        public bool IsPass { get; set; }
        public string ExMsg { get; set; }
        public IResponse Value { get; set; }
    }
}

