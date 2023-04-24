using EL.Async;
using System.CodeDom.Compiler;
using System.Text;

namespace EL.Robot.Component
{
    public class IFComponent : BaseComponent
    {
        public string Expression { get; set; }
        public IFComponent()
        {
            Config.Category = Category.流程控制;
        }
        public override Config GetConfig()
        {
            if (Config.IsInit) return Config;
            Config.ButtonDisplayName = "条件分支";
            Config.Parameters = new List<Parameter>()
            {
                new Parameter()
                {
                    Key = nameof(Expression),
                    DisplayName = "条件表达式",
                    Value = "",
                    Title = "判断分支为真则执行",
                    Type = new List<Type>(){ typeof(string) },
                    IsInput = true,
                    Values = new List <ValueInfo>() {
                        VariableSystem.InputVariable,
                        VariableSystem.SelectVariable
                    },
                },
            };
            return base.GetConfig();
        }
        public override async ELTask<INodeContent> Main(INodeContent self)
        {
            var exrp = self.CurrentNode.GetParamterValueExrp(nameof(Expression)) + "";
            try
            {
                self.Value = Evaluation(exrp);
            }
            catch (Exception)
            {
                throw new ELNodeHandlerException($"表达式不正确！{exrp}");
            }
            self.Out = self.Value;
            return self;
        }


        public bool Evaluation(string expression)
        {
            ICodeCompiler comp = new Microsoft.CSharp.CSharpCodeProvider().CreateCompiler();
            CompilerParameters parms = new CompilerParameters();
            parms.GenerateInMemory = true;//是否在内存中生成space
            StringBuilder code = new StringBuilder();
            code.Append("using System; \n");
            code.Append("      public class Expression    {\n");
            code.Append("   public object Evaluation()        { ");
            code.AppendFormat($"  return (bool)({expression});");
            code.Append("}\n");
            code.Append("} ");
            CompilerResults cr = comp.CompileAssemblyFromSource(parms, code.ToString());
            System.Reflection.Assembly a = cr.CompiledAssembly;
            object _Compiled = a.CreateInstance("Expression");
            System.Reflection.MethodInfo mi = _Compiled.GetType().GetMethod("Evaluation");
            return (bool)mi.Invoke(_Compiled, null);
        }

    }
}
#region MyRegion
//if ((string)self.CurrentNode.GetParamterValue("ifType") == "1" || self.CurrentNode.GetParamterValue("ifType") == null)
//{
//    Dictionary<string, string> judgmentConditions = new Dictionary<string, string>()
//                {
//                    {"等于","=="},
//                    {"大于",">"},
//                    {"大于等于",">="},
//                    {"不等于","!="},
//                    {"小于","<"},
//                    {"小于等于","<="},

//                };
//    var conditionGroup = self.CurrentNode.GetParamterValue("conditionGroup");
//    var conditions = JsonHelper.FromJson<List<Expression>>(JsonHelper.ToJson(conditionGroup));
//    string operator1 = null;
//    if ((string)self.CurrentNode.GetParamterValue("ifRelation") == "1")
//    {
//        operator1 = "&";
//    }
//    else if ((string)self.CurrentNode.GetParamterValue("ifRelation") == "2")
//    {
//        operator1 = "||";
//    }

//    for (int i = 0; i < conditions.Count; i++)
//    {
//        string lo = conditions[i].lo;
//        string op = conditions[i].op;
//        string ro = conditions[i].ro;
//        if (op == "包含")
//        {
//            exrp += $"(\"{lo}\".Contains(\"{ro}\")){operator1}";
//        }
//        else if (op == "不包含")
//        {
//            exrp += $"(!\"{lo}\".Contains(\"{ro}\")){operator1}";
//        }
//        else
//        {
//            judgmentConditions.TryGetValue(op, out string value);
//            exrp += $"{lo}{value}{ro}{operator1}";
//        }
//    }
//    if (conditions.Count == 1)
//        exrp = exrp.TrimEnd(operator1.ToCharArray()[0]);
//}
//else if ((string)self.CurrentNode.GetParamterValue("ifType") == "2")
//{
//    exrp = (string)self.CurrentNode.GetParamterValue("ifExpression");
//};
//bool is_true = false;
//try
//{
//    is_true = Evaluation(exrp);
//}
//catch (Exception ex)
//{
//    throw new ELNodeHandlerException("表达式错误！");
//}
#endregion