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
        public static readonly ValueInfo InputVariable = new()
        {
            DisplayName = "输入值",
            Value = "",
            AcationType = ValueActionType.Input,
        };
        public static readonly ValueInfo SelectVariable = new()
        {
            DisplayName = "选择值",
            Value = "",
            AcationType = ValueActionType.RequestList,
            Action = new SelectVariableRequest()
            {
                ComponentName = "SelectVariable",
                Types = new List<Type>() { typeof(string) }
            },
        };
        public static readonly ValueInfo UpVariable = new()
        {
            DisplayName = "上一条结果",
            Value = "upout",
            AcationType = ValueActionType.Value,
        };
        public static readonly List<ValueInfo> OutParameterNameValues = new() {
            new ValueInfo()
            {
                DisplayName = "变量名称",
                Value = "",
                AcationType = ValueActionType.Input,
            }
         };
        public static readonly List<ValueInfo> InputOrSelectOrUpValues = new() {
                        InputVariable,
                        SelectVariable,
                        UpVariable
         };
        //上一条Node
        public static Node UpNode { get; set; }
    }
}
