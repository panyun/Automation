using Automation.Inspect;
using EL.Robot.Component.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL.Robot.Component
{
    public static class VariableSystem
    {
        public static readonly int TimeOut = 10000;
        public static ValueInfo InputVariable(string defalutValue = "")
        => new()
        {
            DisplayName = "录入值",
            Value = defalutValue,
            ActionType = ValueActionType.Input,
            Types = new List<Type>() { typeof(string) }
        };
        public static ValueInfo SelectVariable(params Type[] types) => new()
        {
            DisplayName = "选择值",
            Value = "",
            ActionType = ValueActionType.RequestList,
            Types = new List<Type>() { typeof(string) },
            Action = new SelectVariableRequest()
            {
                ComponentName = "SelectVariable",
                Types = types.ToList(),
            },
        };
        public static ValueInfo UpVariable(string defalutValue = "UpNode") => new()
        {
            DisplayName = "上一条结果",
            Value = defalutValue,
            Types = new List<Type>() { typeof(string) },
            ActionType = ValueActionType.Value,
        };
        public static ValueInfo CatchVariable() => new()
        {
            DisplayName = "捕获值",
            Value = "",
            ActionType = ValueActionType.RequestValue,
            Types = new List<Type>() { typeof(ElementPath) },
            Action = new CommponetRequest()
            {
                ComponentName = nameof(CatchElementComponent)
            }
        };
        public static ValueInfo OutParameterNameValue(string defalutValue = "") => new ValueInfo()
        {
            DisplayName = "输出变量",
            Value = defalutValue,
            ActionType = ValueActionType.Input,
        };
        public static List<ValueInfo> OutParameterNameValues(string defalutValue = "") => new() {
            OutParameterNameValue(defalutValue)
         };
        public static List<ValueInfo> InputOrSelectOrUpValues() => new() { InputVariable(), SelectVariable(typeof(string)), UpVariable() };
        //上一条Node
        public static Node UpNode { get; set; }
        public static ValueInfo SetValue_Clone(this ValueInfo self, object val)
        {
            var info = JsonHelper.FromJson<ValueInfo>(JsonHelper.ToJson(self));
            info.Value = val;
            return info;
        }
    }
}
