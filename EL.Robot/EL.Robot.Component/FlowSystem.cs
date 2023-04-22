using EL.Robot.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL.Robot.Component
{
	public static class FlowSystem
	{
		public static object GetFlowParamterValue(this Flow self, string key)
		{
			if (self == null) throw new ArgumentNullException(nameof(self));
			key = key.ToLower().Trim();
			if (self.ParamsManager != null)
			{
				self.ParamsManager.TryGetValue(key, out var value);
				if (value == default)
				{
					var isValue = self.ParamsManager.TryGetValue(key.TrimStart('@'), out value);
					if (isValue) return value;
				}
				if (value != null && value is Variable variable)
					return variable.Value;
				if (value != null)
					return value;
			}
			return default;
		}
		public static string SetFlowParamBy(this Flow self, string key, object value)
		{
			if (string.IsNullOrWhiteSpace(key)) return default;
			key = key.ToLower().Trim().TrimStart('@');
			if (string.IsNullOrWhiteSpace(key)) return default;
			string msg = $"         [变量赋值 {key}={JsonHelper.ToJson(value)}]";
			self.ParamsManager ??= new Dictionary<string, object>();
			if (self.ParamsManager.ContainsKey(key))
				self.ParamsManager[key] = value;
			else
				self.ParamsManager.Add(key, value);
			return msg;
		}

	}
}

