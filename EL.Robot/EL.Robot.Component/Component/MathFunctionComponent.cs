using EL.Async;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace EL.Robot.Component.Component
{
    public static class CDll
    {
        [DllImport("/EL.Robot.CDll.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int c_abs(int value);
        [DllImport("/EL.Robot.CDll.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern long c_labs(long value);
        [DllImport("/EL.Robot.CDll.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern double c_dabs(double value);
        [DllImport("EL.Robot.CDll.dll")]
        public static extern double c_round(double value, int figure);
        [DllImport("EL.Robot.CDll.dll")]
        public static extern double c_ceil(double value);
        [DllImport("EL.Robot.CDll.dll")]
        public static extern double c_floor(double value);
        [DllImport("EL.Robot.CDll.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern double c_sqrt(double value);
        [DllImport("EL.Robot.CDll.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern double c_pow(double value, float p);
        [DllImport("EL.Robot.CDll.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern double c_percentage(double value);
        [DllImport("EL.Robot.CDll.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int c_random(int start, int end, int figure);
    }
    public class MathFunctionComponent : BaseComponent
    {
		public MathFunctionComponent()
		{
			Config.Category = Category.基础函数;
		}
		public override Config GetConfig()
		{
			if (Config.IsInit) return Config;
			Config.DisplayName = "数学";
			return base.GetConfig();
		}
		/// <summary>
		/// 求绝对值
		/// </summary>
		/// <param name="self"></param>
		/// <returns></returns>

		public override async ELTask<INodeContent> Main(INodeContent self)
        {
            await base.Main(self);
            //var currentValues = Math.Abs(Convert.ToDouble(self.CurrentNode.Value));
            object currentVal = default;
            var function = self.CurrentNode.GetParamterString("function").ToLower();
            switch (function)
            {
                case "absolutevalue": // 绝对值
                    var targetValue = self.CurrentNode.GetParamterDouble("targetvalue");
                    //currentVal = CDll.c_dabs(targetValue);
                    currentVal = Math.Abs(targetValue);
                    break;
                case "floattofixed": //四舍五入
                    var type = self.CurrentNode.GetParamterString("type");
                    targetValue = self.CurrentNode.GetParamterDouble("targetvalue");
                    switch (type)
                    {
                        case "round":
                            var figure = self.CurrentNode.GetParamterInt("figure");
                            //currentVal = CDll.c_round(targetValue, figure);
                            currentVal = Math.Round(targetValue, figure);
                            break;
                        case "ceil":
                            //currentVal = CDll.c_ceil(targetValue);
                            currentVal = Math.Ceiling(targetValue);
                            break;
                        case "floor":
                            //currentVal = CDll.c_floor(targetValue);
                            currentVal = Math.Floor(targetValue);
                            break;
                        default:
                            break;
                    }
                    break;
                case "simpleformula": //简单四则运算
                    var exp = self.CurrentNode.GetParamterValueExrp("formula") + "";
                    try
                    {
                        currentVal = Evaluation(exp);
                    }
                    catch (Exception)
                    {
                        throw new ELNodeHandlerException($"表达式不正确！{exp}");
                    }
                    break;
                case "convertpercent": //百分比
                    targetValue = self.CurrentNode.GetParamterDouble("targetvalue");
                    //currentVal = CDll.c_percentage(targetValue);
                    //currentVal = Math..c_percentage(targetValue);
                    currentVal = $"{targetValue * 100}%";
                    break;
                case "getrandom":
                    var max = self.CurrentNode.GetParamterInt("max");
                    var min = self.CurrentNode.GetParamterInt("min");
                    var randomFigure = self.CurrentNode.GetParamterInt("randomfigure");
                    //currentVal = CDll.c_random(min, max, randomFigure);
                    Random random = new Random((int)DateTime.Now.Ticks);
                    currentVal = random.Next(min, max);
                    //currentVal = (min, max, randomFigure);
                    break;
                case "cuberoot":
                    targetValue = self.CurrentNode.GetParamterDouble("targetvalue");
                    var power = (float)self.CurrentNode.GetParamterDouble("power");
                    var powerorroot = self.CurrentNode.GetParamterString("powerorroot");
                    switch (powerorroot)
                    {
                        case "power":
                            //currentVal = CDll.c_pow(targetValue, power);
                            currentVal = Math.Pow(targetValue, power);
                            break;
                        case "root":
                            //currentVal = CDll.c_sqrt(targetValue);
                            currentVal =Math.Sqrt(targetValue);
                            break;
                    }
                    break;
                default:
                    break;
            }
            self.Out = currentVal;
            self.Value = true;
            return self;

        }

        public double Evaluation(string expression)
        {
            ICodeCompiler comp = new Microsoft.CSharp.CSharpCodeProvider().CreateCompiler();
            CompilerParameters parms = new CompilerParameters();
            parms.GenerateInMemory = true;//是否在内存中生成space
            StringBuilder code = new StringBuilder();
            code.Append("using System; \n");
            code.Append("      public class Expression    {\n");
            code.Append("   public object Evaluation()        { ");
            code.AppendFormat($"  return (double)({expression});");
            code.Append("}\n");
            code.Append("} ");
            System.CodeDom.Compiler.CompilerResults cr = comp.CompileAssemblyFromSource(parms, code.ToString());
            System.Reflection.Assembly a = cr.CompiledAssembly;
            object _Compiled = a.CreateInstance("Expression");
            System.Reflection.MethodInfo mi = _Compiled.GetType().GetMethod("Evaluation");
            return (double)mi.Invoke(_Compiled, null);
        }
    }

}
