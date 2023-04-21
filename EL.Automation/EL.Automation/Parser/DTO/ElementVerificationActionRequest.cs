using Automation.Inspect;
using EL;
using EL.UIA;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Automation.Parser
{
    /// <summary>
    /// 节点验证
    /// </summary>
    public class ElementVerificationActionRequest : RequestBase
    {

        public LightProperty LightProperty { get; set; } = new LightProperty()
        {
            Count = 3
        };
        /// <summary>
        /// 修改节点
        /// </summary>
        public List<ElementEdit> ElementEditNodes { get; set; }
        [JsonIgnore]
        public bool IsEdit
        {
            get
            {
                return ElementEditNodes.Exists(x => x.IsEdit);
            }
        }
    }

    public class ElementEdit
    {
        public string Id { get; set; }
        [JsonIgnore]
        public bool IsEdit => ElementPropertys.Exists(x => x.IsEdit);
        public bool IsSimilarity { get; set; }
        [JsonIgnore]
        public bool IsChecked => GetProperty(nameof(IsChecked)).GetValueBool();
        [JsonIgnore]
        public string Name => GetProperty(nameof(Name)).GetValueStr();
        [JsonIgnore]
        public int Index => GetProperty(nameof(Index)).GetValueInt();
        [JsonIgnore]
        public int ControlType => GetProperty(nameof(ControlType)).GetValueInt();

        public List<ElementProperty> ElementPropertys { get; set; }
        public ElementProperty GetProperty(string key)
        {
            return ElementPropertys.FirstOrDefault(x => x.Key.ToLower() == key.ToLower());
        }

    }
    public class ElementProperty
    {
        public ElementProperty(string name, string expression, object value, bool isActive = true)
        {
            this.Key = name;
            this.Expression = expression;
            this.Value = value;
            IsActive = isActive;
            this.DefalutRuntimeId = GetRuntimeId(Key, Value, isActive);
        }
        public string Key { get; set; }
        public string Expression { get; set; }
        public object Value { get; set; }
        /// <summary>
        /// 是否选用此项
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// 默认值
        /// </summary>
        public string DefalutRuntimeId { get; set; }
        [JsonIgnore]
        public bool IsEdit => DefalutRuntimeId != GetRuntimeId(Key, Value, IsActive);
        public int GetValueInt()
        {
            int.TryParse(Expression, out var value);
            return value;
        }
        public string GetValueStr()
        {
            if (Value == null)
                return "";
            return Value.ToString().EscapeDataString();
        }
        public bool GetValueBool()
        {
            Boolean.TryParse(Expression, out var value);
            return value;
        }
        public static string GetRuntimeId(string key, object value, bool isActive)
        {
            string json = default;
            if (value is string && int.TryParse(value + "", out var val))
            {
                json = JsonHelper.ToJson(new { key, value = val, isActive });
                return ElemenetSystem.GetCompareId(json);
            }
            json = JsonHelper.ToJson(new { key, value, isActive });
            return ElemenetSystem.GetCompareId(json);
        }
        public bool IsEqual(string value)
        {
            var properRuntimeId = ElementProperty.GetRuntimeId(Key, Value, true);
            var currentRuntimeId = ElementProperty.GetRuntimeId(Key, value, true);
            return properRuntimeId == currentRuntimeId ? true : Matches(value);
        }
        public bool Matches(string value)
        {
            try
            {
                var expression = RegexString(Value + "");
                if (string.IsNullOrWhiteSpace(expression))
                {
                    //Log.Trace($"value:[{value}] -- ex:[{expression}] rtn:[{false}] ");
                    return false;
                }
                var rtn = Regex.IsMatch(value, expression);
                //Log.Trace($"value:[{value}] -- ex:[{expression}] rtn:[{rtn}] ");
                return rtn;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        public string RegexString(string value)
        {
            StringBuilder sb = new StringBuilder(value);

            //sb.Replace("\\", "\\\\");

            //sb.Replace("^", "\\^");

            //sb.Replace("$", "\\$");

            //sb.Replace("+", "\\+");

            //sb.Replace("{", "\\{");

            //sb.Replace("}", "\\}");

            //sb.Replace(".", "\\.");

            //sb.Replace("[", "\\[");

            //sb.Replace("]", "\\]");

            //sb.Replace("(", "\\(");

            //sb.Replace(")", "\\)");
            //实现正则表达式对DOS通配符的转换
            sb.Replace("*", ".*");

            sb.Replace("?", ".?");

            //sb.Append("$");
            return sb.ToString();
        }
    }

    public class ElementVerificationActionResponse : IResponse
    {
        public int Error { get; set; }
        public string Message { get; set; }
        public int RpcId { get; set; }
        public string StackTrace { get; set; }
        public ElementPath ElementPath { get; set; }
    }
}
