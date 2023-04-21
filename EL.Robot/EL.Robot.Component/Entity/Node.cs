using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace EL.Robot.Component
{
	public class Node
	{
		[JsonProperty(PropertyName = "id")]
		public long Id { get; set; }
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
			get
			{
				return parameters;
			}
			set
			{
				parameters = value;
				DictionaryParam = new Dictionary<string, object>();
				if (parameters == null)
					return;
				foreach (var x in parameters)
				{
					if (x == null) continue;
					if (x.Key == null) continue;
					var key = x.Key.Trim().ToLower();
					if (DictionaryParam.ContainsKey(key.ToLower()))
						DictionaryParam[key] = x.Value;
					else
						DictionaryParam.Add(key, x.Value);
				}

			}


		}

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "steps")]
		/// <summary>
		/// 子节点
		/// </summary>
		public List<Node> Steps { get; set; }
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
		public Dictionary<string, object> DictionaryParam { get; set; }
		[IgnoreDataMember]
		[BsonIgnore]
		[JsonIgnore]
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
		public object Value { get; set; }
		public object DefaultValue { get; set; }
		[JsonProperty(PropertyName = "title")]
		public string DisplayName { get; set; }
		public string Title { get; set; }
		public List<string> Values { get; set; }
	}
	public class NodeState
	{
		public Node Node { get; set; }
		public bool IsSucess { get; set; }
		public string Msg { get; set; }
	}

	public class BaseProperty
	{
		private Dictionary<string, object> dic;
		public BaseProperty(Dictionary<string, object> paramObjs)
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
				dic.TryGetValue(nameof(PreTimeDelay).ToLower(), out object obj);
				if (obj == null)
					return default;
				int.TryParse(obj.ToString(), out int result);
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
				dic.TryGetValue(nameof(RearTimeDelay).ToLower(), out object obj);
				if (obj == null)
					return default;
				int.TryParse(obj.ToString(), out int result);
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
				dic.TryGetValue(nameof(Timeout).ToLower(), out object obj);
				if (obj == null)
					return default;
				int.TryParse(obj.ToString(), out int result);
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
				dic.TryGetValue(nameof(Exceptionhandling).ToLower(), out object obj);
				if (obj == null)
					return 1;
				int.TryParse(obj.ToString(), out int result);
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
				dic.TryGetValue(nameof(CustomExceptionInfo).ToLower(), out object obj);

				return obj == null ? default : obj.ToString();
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
				dic.TryGetValue(nameof(RetryCount).ToLower(), out object obj);
				if (obj == null)
					return default;
				int.TryParse(obj.ToString(), out int result);
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
				dic.TryGetValue(nameof(RetryInterval).ToLower(), out object obj);
				return obj == null ? default : obj.ToString();
			}
		}
	}
}
