using Automation.Inspect;
using EL.Async;
using EL.Robot.Component;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto.Tls;

namespace EL.Robot.Component
{

	public static class NodeSystem
	{
		public static Parameter GetParameter(this Node self, string key)
		{
			return self.Parameters.FirstOrDefault(x => x.Key == key);
		}

		public static ValueInfo GetParameterValueInfo(this Node self, string key)
		{
			var parameter = self.Parameters.FirstOrDefault(x => x.Key == key);
			ValueInfo info = parameter.Value;
			if (parameter.Value != null)
			{
				if (parameter.Value.ActionType == ValueActionType.RequestList)
					info = self.Flow.ParamsManager[key];
			}
			return info;
		}
		public static T GetParamterValue<T>(this Node self, string key)
		{
			var info = self.GetParameterValueInfo(key);
			if (info.ActionType == ValueActionType.RequestList)
				info = self.Flow.ParamsManager[key];
			var type = typeof(T);
			try
			{
				if (type.BaseType == typeof(Enum))
				{
					var str = info.Value + "";
					return  (T)Enum.Parse(typeof(T), str);
				}
				return (T)info.Value;
			}
			catch (Exception)
			{
				return JsonHelper.FromJson<T>(info.Value + "");
			}

		}
		public static string GetParamterString(this Node self, string key)
		{
			var obj = self.GetParameterValueInfo(key).Value;
			return obj + "";
		}
		public static DateTime GetParamterDateTime(this Node self, string key)
		{
			var obj = self.GetParameterValueInfo(key).Value;
			if (obj is DateTime date) return date;
			var targettime = self.GetParamterString(key);
			if (!DateTime.TryParse(targettime, out date))
			{
				if (!long.TryParse(targettime, out long time))
					throw new ELNodeHandlerException("时间参数无效！");
				date = TimeHelper.ToDateTime(time);
			}
			return date;
		}

		public static bool GetParamterBool(this Node self, string key)
		{
			var obj = self.GetParameterValueInfo(key).Value;
			if (obj is bool b) return b;
			bool.TryParse(obj + "", out bool result);
			return result;
		}
		public static int GetParamterInt(this Node self, string key)
		{
			var obj = self.GetParameterValueInfo(key).Value;
			if (obj is int i) return i;
			int.TryParse(obj + "", out int result);
			return result;
		}
		public static long GetParamterLong(this Node self, string key)
		{
			var obj = self.GetParameterValueInfo(key).Value;
			if (obj is long l) return l;
			long.TryParse(obj + "", out long result);
			return result;
		}
		public static double GetParamterDouble(this Node self, string key)
		{
			var obj = self.GetParameterValueInfo(key).Value;
			if (obj is double d) return d;
			double.TryParse(obj + "", out double result);
			return result;
		}
		public static double GetParamterFloat(this Node self, string key)
		{
			var obj = self.GetParameterValueInfo(key).Value;
			if (obj is float f) return f;
			double.TryParse(obj + "", out double result);
			return result;
		}
		public static object GetParamterValueExrp(this Node self, string key)
		{
			key = key.ToLower();
			if (self == null) throw new ArgumentNullException(nameof(self));
			if (self.DictionaryParam != null)
			{
				self.DictionaryParam.TryGetValue(key, out var value);
				if (value != default)
				{
					if ((value + "").IndexOf("@") != -1 && self.Flow != null)
					{
						var val = value.ToString();
						var str = val.Split(' ');
						str.Where(x => x.StartsWith("@")).ToList().ForEach((x) =>
						{
							string str = default;
							var varValue = self.Flow.GetFlowParamterValue(x);
							if (varValue == null) str = "";
							else if (!(varValue is string)) str = JsonConvert.SerializeObject(varValue);
							else str = varValue + "";
							val = val.Replace(x, str);
							val = val.Replace(" ", "");
						});
						return val;
					}
					return value;
				}
				if (key.StartsWith("@") && self.Flow != null)
					return self.Flow.GetFlowParamterValue(key);
			}
			if (self.Flow != null)
				return self.Flow.GetFlowParamterValue(key);
			throw new ELNodeHandlerException($"获取属性[{key}]为空！");
		}

		public static BaseProperty GetBaseProperty(this Node self)
		{
			try
			{
				if (self.BaseProperty != null) return self.BaseProperty;
				self.BaseProperty = new BaseProperty(self.DictionaryParam);
				return self.BaseProperty;
			}
			catch (Exception ex)
			{
				return default;
			}

		}
	}
}
