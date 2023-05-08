using Automation;
using Automation.Inspect;
using Automation.Parser;
using EL.Async;
using EL.Robot.Core;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

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
		private static string Md5(string json)
		{
			if (json == null) json = "";
			string cl = json;
			string pwd = "";
			MD5 md5 = MD5.Create(); //实例化一个md5对像
			byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
			// 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
			for (int i = 0; i < s.Length; i++)
			{
				// 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符 
				pwd = pwd + s[i].ToString("X");
			}
			return pwd;
		}
		/// <summary>
		/// 短网址应用 ，例如QQ微博的url.cn   http://url.cn/2hytQx
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public static string GetId(string str)
		{
			//可以自定义生成MD5加密字符传前的混合KEY
			//要使用生成URL的字符
			string[] chars = new string[]{
		"a","b","c","d","e","f","g","h" ,"i","j","k","l","m","n","o","p","q","r","s","t","u","v","w","x","y","z",
		"0","1","2","3","4","5","6","7","8","9",
		"A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T" ,"U","V","W","X","Y","Z"
		};
			//对传入网址进行MD5加密
			MD5 md5 = MD5.Create(); //实例化一个md5对像
			string hex = Md5(str);
			int hexint = 0x3FFFFFFF & Convert.ToInt32("0x" + hex.Substring(0 * 8, 8), 16);
			string outChars = string.Empty;
			for (int j = 0; j < 4; j++)
			{
				//把得到的值与0x0000003D进行位与运算，取得字符数组chars索引
				int index = 0x0000003D & hexint;
				//把取得的字符相加
				outChars += chars[index];
				//每次循环按位右移5位
				hexint = hexint >> 5;
			}
			//把字符串存入对应索引的输出数组
			return outChars;

		}

		public static string GetDisplayVlaue(ValueInfo variable)
		{
			if (variable == default || variable.Value == default) return "";
			var obj = variable.Value;
			if (variable.Value != default && variable.Value.GetType() != variable.Type)
				obj = JsonHelper.FromJson(variable.Type, variable.Value + "");
			if (obj is ElementPath ep)
				obj = $"{ep.ProcessName}.{ep.ControlType}.{ep.Name}";
			return obj + "";
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
