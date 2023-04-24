using Automation;
using Automation.Parser;
using EL.Async;
using EL.Robot.Core;
using System.Reflection;

namespace EL.Robot.Component
{
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
					res = await InspectRequestManager.StartAsync(request);
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
