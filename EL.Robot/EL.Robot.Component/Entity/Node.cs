using Automation.Inspect;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace EL.Robot.Component
{
	public class Node
	{
		[IgnoreDataMember]
		[BsonIgnore]
		[JsonIgnore]
		public Node DesignParent { get; set; }
		[JsonProperty(PropertyName = "id")]
		public long Id { get; set; }
		[IgnoreDataMember]
		[BsonIgnore]
		[JsonIgnore]
		public bool IsEnd
		{
			get
			{
				if (ComponentName == "BlockEndComponent") return true;
				return false;
			}
		}
		public bool IsView { get; set; } = true;
		public bool IsBlock { get; set; } = false;
		public Node LinkNode { get; set; }
		[IgnoreDataMember]
		[BsonIgnore]
		[JsonIgnore]
		public bool IsNew { get; set; }
		[JsonProperty(PropertyName = "name")]
		/// <summary>
		/// 名称
		/// </summary>
		public string Name { get; set; }
		[JsonProperty(PropertyName = "componentname")]
		/// <summary>
		/// Aciont
		/// </summary>
		public string ComponentName { get; set; }
		[JsonProperty(PropertyName = "annotation")]
		/// <summary>
		/// 备注
		/// </summary>
		public string Annotation { get; set; }
		[JsonProperty(PropertyName = "ignore")]
		/// <summary>
		/// 忽略
		/// </summary>
		public bool Ignore { get; set; }
		[JsonProperty(PropertyName = "debug")]
		/// <summary>
		/// 断点
		/// </summary>
		public bool Debug { get; set; }
		[JsonProperty(PropertyName = "islock")]
		/// <summary>
		/// 是否锁定
		/// </summary>
		public bool IsLock { get; set; }
		/// <summary>
		/// 输出参数名字
		/// </summary>
		public string OutParameterName { get; set; }
		private List<Parameter> parameters = new List<Parameter>();
		/// <summary>
		/// 字典
		/// </summary>
		public List<Parameter> Parameters
		{
			get; set;
		} = new List<Parameter>();

		public string DisplayExp
		{
			get
			{
				string str = string.Empty;
				if (ComponentName == nameof(CommentComponent))
				{
					var comment = Parameters.FirstOrDefault(x => x.Key == nameof(CommentComponent.Comment));
					str = $" {Name}//{comment?.Value?.Value ?? "--注释内容空--"}";
				}
				else
				{
					str = $" {Name}->>";
					foreach (Parameter parameter in Parameters)
					{
						if (parameter != null)
							str += parameter.DisplayExp;
					}

				}

				return str.Length > 80 ? str.Substring(0, 80) : str;
			}
		}
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "steps")]
		/// <summary>
		/// 子节点
		/// </summary>
		public List<Node> Steps { get; set; } = new List<Node>();
		[BsonIgnoreIfNull]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		/// <summary>
		/// 输入参数
		/// </summary>
		public List<Parameter> InParams { get; set; }
		#region 参数处理
		[IgnoreDataMember]
		[BsonIgnore]
		[JsonIgnore]
		/// <summary>
		/// 字典
		/// </summary>
		public Dictionary<string, ValueInfo> DictionaryParam { get; set; }

		/// <summary>
		/// 节点图像
		/// </summary>
		public string Img { get; set; }
		#endregion
		[IgnoreDataMember]
		[BsonIgnore]
		[JsonIgnore]
		public BaseProperty BaseProperty { get; set; }
		[BsonIgnoreIfNull]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "switch")]
		public List<Node[]> Switch { get; set; }
		[IgnoreDataMember]
		[BsonIgnore]
		[JsonIgnore]
		public Flow Flow { get; set; }
	}
	public class Parameter
	{
		[JsonProperty(PropertyName = "key")]
		public string Key { get; set; }
		[JsonProperty(PropertyName = "value")]
		public ValueInfo Value { get; set; }

		public string DisplayVlaue
		{
			get
			{
				if (Value == null) return default;
				if (Types.Contains(typeof(ElementPath)) && Value.ActionType == ValueActionType.RequestValue)
				{
					if (Value.Value == null || string.IsNullOrEmpty(Value.Value + "")) return default;
					var path = JsonHelper.FromJson<ElementPath>(Value.Value.ToString());
					return path.Path;
				}
				return Value.Value + "";
			}
		}
		public string DisplayExp
		{
			get
			{
				if (Value == null)
					return $"{CmdDisplayName}({""}).";
				string val;
				if (Value.ActionType == ValueActionType.RequestList)
				{
					val = DisplayVlaue ?? Value.Value + "";
					return $"{CmdDisplayName}(%{val}%).";
				}
				val = DisplayVlaue ?? Value.Value + "";
				return $"{CmdDisplayName}({val}).";
			}
		}
		[JsonProperty(PropertyName = "title")]
		public string DisplayName { get; set; }
		public string _cmdDisplayName = "";
		public string CmdDisplayName
		{
			get
			{
				if (string.IsNullOrEmpty(_cmdDisplayName))
					return DisplayName;
				return _cmdDisplayName;
			}
			set
			{
				_cmdDisplayName = value;
			}
		}
		/// <summary>
		/// 唯一识别
		/// </summary>
		public long Id { get; set; }
		public string Title { get; set; }
		public List<ValueInfo> Values { get; set; }
		public List<Parameter> Parameters { get; set; }
		public bool IsInput { get; set; }

		public List<Type> Types { get; set; }
		public bool IsFinish { get; set; }
	}
	public class ValueInfo
	{
		public ValueInfo(string displayName, object value, List<Type> types, ValueActionType acationType = ValueActionType.Value, CommponetRequest action = null)
		{
			DisplayName = displayName;
			Value = value;
			Types = types;
			ActionType = acationType;
			Action = action;
		}
		public ValueInfo(string displayName, object value) : this(displayName, value, default)
		{

		}
		public ValueInfo(object value) : this("", value)
		{

		}
		public ValueInfo()
		{

		}
		public string DisplayName { get; set; }
		public object Value { get; set; }
		public List<Type> Types { get; set; }
		public ValueActionType ActionType { get; set; } = ValueActionType.Value;
		public CommponetRequest Action { get; set; }
	}
	public enum ValueActionType
	{
		RequestValue,
		RequestList,
		Input,
		Value
	}
	public class ParameterInfo
	{
		public string DefalutCommand { get; set; }
		public Parameter Parameter { get; set; }
	}
	public class NodeState
	{
		public Node Node { get; set; }
		public bool IsSucess { get; set; }
		public string Msg { get; set; }
	}

	public class BaseProperty
	{
		private Dictionary<string, ValueInfo> dic;
		public BaseProperty(Dictionary<string, ValueInfo> paramObjs)
		{
			dic = paramObjs;
		}
		/// <summary>
		/// 前置延时
		/// </summary>
		public int PreTimeDelay
		{
			get
			{
				if (dic == null)
					return default;
				dic.TryGetValue(nameof(PreTimeDelay).ToLower(), out ValueInfo obj);
				if (obj == null)
					return default;
				int.TryParse(obj.Value.ToString(), out int result);
				return result;
			}
		}
		/// <summary>
		/// 后置延时
		/// </summary>
		public int RearTimeDelay
		{
			get
			{
				if (dic == null)
					return default;
				dic.TryGetValue(nameof(RearTimeDelay).ToLower(), out ValueInfo obj);
				if (obj == null)
					return default;
				int.TryParse(obj.Value.ToString(), out int result);
				return result;
			}
		}
		/// <summary>
		/// timeout
		/// </summary>
		public int Timeout
		{
			get
			{
				if (dic == null)
					return default;
				dic.TryGetValue(nameof(Timeout).ToLower(), out ValueInfo obj);
				if (obj == null)
					return default;
				int.TryParse(obj.Value.ToString(), out int result);
				return result;
			}
		}
		/// <summary>
		/// 异常处理
		/// </summary>
		public int Exceptionhandling
		{
			get
			{
				if (dic == null)
					return 1;
				dic.TryGetValue(nameof(Exceptionhandling).ToLower(), out ValueInfo obj);
				if (obj == null)
					return 1;
				int.TryParse(obj.Value.ToString(), out int result);
				return result;
			}
		}
		/// <summary>
		/// 自定义异常信息
		/// </summary>
		public string CustomExceptionInfo
		{
			get
			{
				if (dic == null)
					return default;
				dic.TryGetValue(nameof(CustomExceptionInfo).ToLower(), out ValueInfo obj);

				return obj == null ? default : obj.Value.ToString();
			}
		}
		/// <summary>
		/// 重试次数
		/// </summary>
		public int RetryCount
		{
			get
			{
				if (dic == null)
					return default;
				dic.TryGetValue(nameof(RetryCount).ToLower(), out ValueInfo obj);
				if (obj == null)
					return default;
				int.TryParse(obj.Value.ToString(), out int result);
				return result;
			}
		}
		/// <summary>
		/// 重试间隔
		/// </summary>
		public string RetryInterval
		{
			get
			{
				if (dic == null)
					return default;
				dic.TryGetValue(nameof(RetryInterval).ToLower(), out ValueInfo obj);
				return obj == null ? default : obj.Value.ToString();
			}
		}
	}


}
