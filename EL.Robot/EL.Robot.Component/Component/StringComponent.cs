using EL.Async;
using Newtonsoft.Json;
using NPOI.OpenXml4Net.OPC;
using NPOI.OpenXmlFormats.Dml;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Markup;

namespace EL.Robot.Component
{
	/// <summary>
	/// 字符串处理
	/// </summary>
	public class StringComponent : BaseComponent
	{
		public StringComponent()
		{
			//Config.Category = Category.基础组件;
		}
		public override Config GetConfig()
		{
			if (Config.IsInit) return Config;
			Config.Parameters = new List<Parameter>()
			{
				new Parameter()
				{
					Key = "insert",
					DisplayName = "字符插入",
					Value = "insert",
					Title = "字符插入",
					Type = new List<Type>(){ typeof(string) },
					IsInput = true,
					Values = new List<ValueInfo> {
					   new ValueInfo()
					   {
							DisplayName = "目标字符串",
					   }
					},
					Parameters = new List<Parameter>()
					{
						new Parameter()
						{
							Key = "match",
							DisplayName = "匹配插入",
							Value = "match",
							Type = new List<Type>(){ typeof(string) },
								IsInput = true,
							Title = "匹配插入",
							Values = new List<ValueInfo> {
					   new ValueInfo()
					   {
							DisplayName = "目标字符串",
					   }
					},
							Parameters = new List<Parameter>()
							{
								new Parameter()
						{
							Key = "Loction",
							DisplayName = "插入位置",
								Type = new List<Type>(){ typeof(Point) },
							Title = "插入位置",
								IsInput = true,
							Values = new List<ValueInfo> {
					   new ValueInfo()
					   {
							DisplayName = "{x,y}",

					   }
					},
						}
							}
						},

					}
				},
				new Parameter()
				{
					Key = "replace",
					DisplayName = "字符替换",
					Value = "replace",
					Title = "字符串替换",
						IsInput = true,
					Type = new List<Type>(){ typeof(string) },
					Values =new List<ValueInfo>()
					{
						new ValueInfo()
						{
							 DisplayName= "目标字符串",
						}
					},
					Parameters = new List<Parameter>()
					{
						new Parameter()
						{
							Key = "Loction",
							DisplayName = "位置",
							IsInput = true,
							Value = "{x,y}",
							Type = new List<Type>(){ typeof(Point) },
							Title = "将字串替换到的位置",
							Values = new List < ValueInfo > () { new ValueInfo() { DisplayName = "{x,y}" } },
						}
					}
				}
	};
			Config.ButtonDisplayName = "字符串函数";
			return base.GetConfig();
		}
		public string DefalutCommand(Parameter parameters)
		{
			if (parameters.Parameters == null || parameters.Parameters.Count == 0)
				return parameters.DisplayName;
			string current = $@"{parameters.DisplayName}";
			if (parameters.IsInput)
				current += "[请输入变量]";
			return $@"[{parameters.DisplayName}]-[{DefalutCommand(parameters.Parameters.First())}]";
		}


		public override async ELTask<INodeContent> Main(INodeContent self)
		{
			await base.Main(self);
			var function = self.CurrentNode.GetParamterString("function");
			switch (function)
			{

				case "insert":
					Insert(self, function);
					break;
				case "replace":
					Replace(self, function);
					break;
				case "split":
					var targettext = self.CurrentNode.GetParamterString("targettext");
					var splitsymbol = self.CurrentNode.GetParamterString("splitsymbol");
					self.Out = targettext.Split(splitsymbol.ToArray());
					break;
				case "substr":
					Substr(self, function);
					break;
				case "del":
					Del(self, function);
					break;
				case "conversion":
					Conversion(self, function);
					break;
				case "random":
					var randomlength = self.CurrentNode.GetParamterInt("randomlength");
					var characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
					var Charsarr = new char[randomlength];
					var random = new Random();
					for (int i = 0; i < Charsarr.Length; i++)
					{
						Charsarr[i] = characters[random.Next(characters.Length)];
					}
					self.Out = new String(Charsarr);
					break;
				case "contains":
					targettext = self.CurrentNode.GetParamterString("targettext");
					var containtext = self.CurrentNode.GetParamterString("containtext");
					self.Out = targettext.Contains(containtext);
					break;
				case "join":
					List<Object> targetObj = new List<object>();
					var targetTextList = self.CurrentNode.GetParamterString("targetTextList");
					var joinSymbol = self.CurrentNode.GetParamterString("joinSymbol");
					if (targetTextList is string)
						targetObj = JsonConvert.DeserializeObject<List<object>>(targetTextList + "");
					char[] trimChar = { '\"' };
					List<Object> targetList = new List<Object>();
					foreach (string item in targetObj)
					{
						try
						{
							var val = self.CurrentNode.GetParamterString(item);
							targetList.Add(val);
						}
						catch (Exception)
						{
							targetList.Add(item.Trim(trimChar));
						}
					}
					self.Out = string.Join(joinSymbol.Trim(trimChar), targetList);
					break;
				default:
					break;
			}
			self.Value = true;
			return self;

		}
		Dictionary<string, string> numbers = new Dictionary<string, string>()
		{
			{ "0","零"},
			{ "1","一"},
			{ "2","二"},
			{ "3","三"},
				{ "4","四"},
				{ "5","五"},
				{ "6","六"},
				{ "7","七"},
				{ "8","八"},
				{ "9","九"},

		};
		Dictionary<string, string> numbersChar = new Dictionary<string, string>()
		{
			{ "零","零"},
			{ "一","壹"},
			{ "二","贰"},
			{ "三","叁"},
			{ "四","肆"},
			{ "五","伍"},
			{ "六","陆"},
			{ "七","柒"},
			{ "八","捌"},
			{ "九","玖"},

		};
		string[] DX_SZ = { "零", "壹", "贰", "叁", "肆", "伍", "陆", "柒", "捌", "玖", "拾" };//大写数字

		Dictionary<string, string> numbersUpper = new Dictionary<string, string>()
		{
			{ "0","零"},
			{ "1","壹"},
			{ "2","贰"},
			{ "3","叁"},
			{ "4","肆"},
			{ "5","伍"},
			{ "6","陆"},
			{ "7","柒"},
			{ "8","捌"},
			{ "9","玖"},

		};
		public void Conversion(INodeContent self, string function)
		{
			var targettext = self.CurrentNode.GetParamterString("targettext");
			var conversiontype = self.CurrentNode.GetParamterInt("conversiontype");
			if (conversiontype == 1)
			{
				self.Out = targettext.ToArray();
				return;
			}
			if (conversiontype == 2)
			{
				var obj = JsonConvert.DeserializeObject<Dictionary<string, object>>(targettext);
				self.Out = JsonConvert.SerializeObject(obj);
				return;
			}
			if (conversiontype == 3)
			{
				self.Out = DateTime.Parse(targettext);
				return;
			}
			if (conversiontype == 4)
			{

				foreach (var item in numbers)
					targettext = targettext.Replace(item.Key, item.Value);
				self.Out = targettext;
				return;
			}
			if (conversiontype == 5)
			{
				foreach (var item in numbers)
					targettext = targettext.Replace(item.Value, item.Key);
				self.Out = targettext;
				return;
			}
			if (conversiontype == 6)
			{
				var matches = Regex.Matches(targettext, "[0-9]+");
				var values = matches.Cast<Match>().Select(m => m.Value).ToList();
				StringBuilder valStr = new StringBuilder();
				foreach (var x in values)
				{
					double.TryParse(x, out double temp);
					string s = temp.ToString("#L#E#D#C#K#E#D#C#J#E#D#C#I#E#D#C#H#E#D#C#G#E#D#C#F#E#D#C#.0B0A");
					var val = Regex.Replace(s, @"((?<=-|^)[^1-9]*)|((?'z'0)[0A-E]*((?=[1-9])|(?'-z'(?=[F-L\\.]|$))))|((?'b'[F-L])(?'z'0)[0A-L]*((?=[1-9])|(?'-z'(?=[\\.]|$))))", "${b}${z}");
					valStr.Append(Regex.Replace(val, ".", m => "负元空零壹贰叁肆伍陆柒捌玖空空空空空空空分角拾佰仟萬億兆京垓秭穰"[m.Value[0] - '-'].ToString()));
				}
				self.Out = valStr.ToString();
				return;
			}
			if (conversiontype == 7)
			{

				//foreach (var item in numbersUpper)
				//    targettext = targettext.Replace(item.Value, item.Key);
				//var m = "负元空零壹贰叁肆伍陆柒捌玖空空空空空空空分角拾佰仟萬億兆京垓秭穰";
				//foreach (var c in m.ToArray())
				//{
				//    targettext = targettext.Replace(c.ToString(), "");
				//}
				self.Out = ChineseConvertToNumber(targettext);
				return;
			}
			if (conversiontype == 8)
			{
				foreach (var item in numbersChar)
					targettext = targettext.Replace(item.Value, item.Key);
				self.Out = targettext;
				return;
			}
			if (conversiontype == 9)
			{
				foreach (var item in numbersChar)
					targettext = targettext.Replace(item.Key, item.Value);
				self.Out = targettext;
				return;
			}
		}
		public void Substr(INodeContent self, string function)
		{
			var targettext = self.CurrentNode.GetParamterString("targettext");
			var substrtype = self.CurrentNode.GetParamterInt("substrtype");
			int startposition = 0;
			int sublength = 0;
			if (substrtype == 1)
			{
				startposition = self.CurrentNode.GetParamterInt("startposition") - 1;
				sublength = self.CurrentNode.GetParamterInt("sublength");
			}
			else if (substrtype == 2)
			{
				var starttext = self.CurrentNode.GetParamterString("starttext");
				var endtext = self.CurrentNode.GetParamterString("endtext");
				startposition = targettext.IndexOf(starttext);
				sublength = Math.Abs(targettext.IndexOf(endtext) - startposition) + 1;

			}

			if (startposition < 0 || startposition + sublength > targettext.Length)
				throw new ELNodeHandlerException("索引异常！");
			self.Out = targettext.Substring(startposition, sublength);
		}
		public void Insert(INodeContent self, string function)
		{
			var targettext = self.CurrentNode.GetParamterString("targettext");
			var inserttext = self.CurrentNode.GetParamterString("inserttext");
			var inserttype = self.CurrentNode.GetParamterInt("insertype");
			switch (inserttype)
			{
				case 1:  //插入
					var inserposition = self.CurrentNode.GetParamterString("inserposition");
					var insertindex = self.CurrentNode.GetParamterInt("insertindex");
					insertindex = inserposition == "front" ? insertindex - 1 : insertindex;
					insertindex = insertindex < 0 ? 0 : insertindex;
					self.Out = targettext.Insert(insertindex, inserttext);
					break;
				case 2:
					inserposition = self.CurrentNode.GetParamterString("inserposition");
					var matchtext = self.CurrentNode.GetParamterString("matchtext");
					var isfirstmatch = self.CurrentNode.GetParamterBool("isfirstmatch");
					if (isfirstmatch)
						insertindex = targettext.IndexOf(matchtext);
					else
						insertindex = targettext.LastIndexOf(matchtext);
					if (insertindex == -1)
						throw new ELNodeHandlerException("没有找到匹配项！");
					insertindex++;
					insertindex = inserposition == "front" ? insertindex - 1 : insertindex;
					insertindex = insertindex < 0 ? 0 : insertindex;
					self.Out = targettext.Insert(insertindex, inserttext);
					break;
				default:
					break;
			}
		}
		public void Del(INodeContent self, string function)
		{
			var targettext = self.CurrentNode.GetParamterString("targettext");
			var deltype = self.CurrentNode.GetParamterString("deltype");
			var isfirstmatch = self.CurrentNode.GetParamterBool("isfirstmatch");
			string toReplacedText = String.Empty;
			if (deltype == "custom")
			{
				var deltext = self.CurrentNode.GetParamterString("deltext");
				if (isfirstmatch)
				{
					var index = targettext.IndexOf(deltext);
					if (index == -1)
						throw new ELNodeHandlerException("没有找到匹配项！");
					targettext = targettext.Remove(index, deltext.Length);
					self.Out = targettext.Insert(index, toReplacedText);
				}
				self.Out = targettext.Replace(deltext, toReplacedText);
				return;
			}
			if (deltype == "specifiedText")
			{
				var starttext = self.CurrentNode.GetParamterString("starttext");
				var endtext = self.CurrentNode.GetParamterString("endtext");
				var startposition = targettext.IndexOf(starttext);
				var endposition = targettext.IndexOf(endtext);
				//toReplacedText = self.CurrentNode.GetParamterString("toreplacedtext");
				if (startposition < 0 || endposition < 0 || endposition < startposition || startposition + Math.Abs(endposition - startposition) >= targettext.Length)
					throw new ELNodeHandlerException("索引异常！");
				targettext = targettext.Remove(startposition, Math.Abs(endposition - startposition));
				self.Out = targettext.Insert(startposition, toReplacedText);
				return;
			}
			if (deltype == "specifiedPosition")
			{
				var startposition = self.CurrentNode.GetParamterInt("startposition");
				var endposition = self.CurrentNode.GetParamterInt("endposition");
				//toReplacedText = self.CurrentNode.GetParamterString("toreplacedtext");
				if (startposition < 0 || endposition < 0 || endposition < startposition || startposition + Math.Abs(endposition - startposition) >= targettext.Length)
					throw new ELNodeHandlerException("索引异常！");
				targettext = targettext.Remove(startposition, Math.Abs(endposition - startposition));
				self.Out = targettext.Insert(startposition, toReplacedText);
				return;
			}
			var pattern = "[0-9]+";
			if (deltype == "num")
				pattern = "[0-9]+";
			if (deltype == "date")
				pattern = "([0-9]{4}|[0-9]{2})[./-]([0]?[1-9]|[0-9][0,1,2])[./-]([0-9]{1,2})";
			if (deltype == "IdNum")
				pattern = "(\\d{15}$|\\d{18}$|\\d{17}(\\d|X|x))";
			if (deltype == "mobileNum")
				pattern = "(1[3|4|5|6|7|8|9]\\d{9})|(0\\d{2,3}-\\d{7,8})|(400-\\d{3}-\\d{4})|(400\\d{7})";
			if (deltype == "email")
				pattern = "\\w+([-+.]\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
			//toReplacedText = self.CurrentNode.GetParamterString("toreplacedtext");
			RegexOptions regexOptions = default(RegexOptions);
			regexOptions = RegexOptions.IgnoreCase;
			var matches = Regex.Matches(targettext, pattern, regexOptions);
			var values = matches.Cast<Match>().Select(m => m.Value).ToList();
			if (values == null || values.Count == 0) throw new ELNodeHandlerException("没有找到匹配项！");
			if (isfirstmatch)
			{
				var val = values.FirstOrDefault();
				self.Out = targettext.Replace(val, toReplacedText);
				return;
			}
			self.Out = Regex.Replace(targettext, pattern, toReplacedText, regexOptions);

		}
		public void Replace(INodeContent self, string function)
		{
			var targettext = self.CurrentNode.GetParamterString("targettext");
			var replacetype = self.CurrentNode.GetParamterString("replacetype");
			var isfirstmatch = self.CurrentNode.GetParamterBool("isfirstmatch");
			string toReplacedText = String.Empty;
			if (replacetype == "custom")
			{
				var beReplacedText = self.CurrentNode.GetParamterString("beReplacedText");
				toReplacedText = self.CurrentNode.GetParamterString("toreplacedtext");
				if (isfirstmatch)
				{
					var index = targettext.IndexOf(beReplacedText);
					if (index == -1)
						throw new ELNodeHandlerException("没有找到匹配项！");
					targettext = targettext.Remove(index, beReplacedText.Length);
					self.Out = targettext.Insert(index, toReplacedText);
					return;
				}
				self.Out = targettext.Replace(beReplacedText, toReplacedText);
				return;
			}
			if (replacetype == "specifiedText")
			{
				var starttext = self.CurrentNode.GetParamterString("starttext");
				var endtext = self.CurrentNode.GetParamterString("endtext");
				var startposition = targettext.IndexOf(starttext);
				var endposition = targettext.IndexOf(endtext);
				toReplacedText = self.CurrentNode.GetParamterString("toreplacedtext");
				if (startposition < 0 || endposition < 0 || endposition < startposition || startposition + Math.Abs(endposition - startposition) >= targettext.Length)
					throw new ELNodeHandlerException("索引异常！");
				targettext = targettext.Remove(startposition, Math.Abs(endposition - startposition));
				self.Out = targettext.Insert(startposition, toReplacedText);
				return;
			}
			if (replacetype == "specifiedPosition")
			{
				var startposition = self.CurrentNode.GetParamterInt("startposition");
				var endposition = self.CurrentNode.GetParamterInt("endposition");
				toReplacedText = self.CurrentNode.GetParamterString("toreplacedtext");
				if (startposition < 0 || endposition < 0 || endposition < startposition || startposition + Math.Abs(endposition - startposition) >= targettext.Length)
					throw new ELNodeHandlerException("索引异常！");
				targettext = targettext.Remove(startposition, Math.Abs(endposition - startposition));
				self.Out = targettext.Insert(startposition, toReplacedText);
				return;
			}
			var pattern = "[0-9]+";
			if (replacetype == "num")
				pattern = "[0-9]+";
			if (replacetype == "date")
				pattern = "([0-9]{4}|[0-9]{2})[./-]([0]?[1-9]|[0-9][0,1,2])[./-]([0-9]{1,2})";
			if (replacetype == "IdNum")
				pattern = "(\\d{15}$|\\d{18}$|\\d{17}(\\d|X|x))";
			if (replacetype == "mobileNum")
				pattern = "(1[3|4|5|6|7|8|9]\\d{9})|(0\\d{2,3}-\\d{7,8})|(400-\\d{3}-\\d{4})|(400\\d{7})";
			if (replacetype == "email")
				pattern = "\\w+([-+.]\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
			toReplacedText = self.CurrentNode.GetParamterString("toreplacedtext");
			RegexOptions regexOptions = default(RegexOptions);
			regexOptions = RegexOptions.IgnoreCase;
			var matches = Regex.Matches(targettext, pattern, regexOptions);
			var values = matches.Cast<Match>().Select(m => m.Value).ToList();
			if (values == null || values.Count == 0) throw new ELNodeHandlerException("没有找到匹配项！");
			if (isfirstmatch)
			{
				var val = values.FirstOrDefault();
				self.Out = targettext.Replace(val, toReplacedText);
				return;
			}
			self.Out = Regex.Replace(targettext, pattern, toReplacedText, regexOptions);

		}
		/// <summary>
		/// 中文金额转换为数字金额(不包含负金额)
		/// </summary>
		/// <param name="chineseAmount">中文金额</param>
		/// <returns>数字金额</returns>
		public static string ChineseConvertToNumber(string chineseAmount)
		{
			if (string.IsNullOrEmpty(chineseAmount))
			{
				return string.Empty;
			}

			chineseAmount = chineseAmount.Replace("零", "").Replace("元", "").Replace("整", "");//移除计算干扰文字
			var wordCharArray = chineseAmount.ToCharArray();

			double numberAmount = 0;//最终要返回的数字金额

			//金额位标志量
			bool wan = false;//表示有万位
			bool yi = false;//表示有亿位
			bool fen = false;//表示有分位
			bool jiao = false;//表示有角位
			bool shi = false;//表示有十位
			bool bai = false;//表示有百位
			bool qian = false;//表示有千位

			for (int i = (wordCharArray.Length - 1); i >= 0; i--)//从低位到高位计算
			{
				double currentPlaceAmount = 0;//当前位金额值

				//判断当前位对应金额标志量
				if (wordCharArray[i] == '分')
				{
					fen = true;
					continue;
				}
				else if (wordCharArray[i] == '角')
				{
					jiao = true;
					fen = false;
					continue;
				}
				else if (wordCharArray[i] == '拾')
				{
					fen = false;
					jiao = false;
					shi = true;
					continue;
				}
				else if (wordCharArray[i] == '佰')
				{
					bai = true;
					fen = false;
					jiao = false;
					shi = false;
					continue;
				}
				else if (wordCharArray[i] == '仟')
				{
					qian = true;
					fen = false;
					jiao = false;
					shi = false;
					bai = false;
					continue;
				}
				else if (wordCharArray[i] == '万')
				{
					wan = true;
					fen = false;
					jiao = false;
					shi = false;
					bai = false;
					qian = false;
					continue;
				}
				else if (wordCharArray[i] == '亿')
				{
					yi = true;
					wan = false;
					fen = false;
					jiao = false;
					shi = false;
					bai = false;
					qian = false;
					continue;
				}

				//根据标志量转换金额为实际金额
				if (fen) currentPlaceAmount = ConvertNameToSmall(wordCharArray[i]) * 0.01;
				else if (jiao)
				{
					currentPlaceAmount = ConvertNameToSmall(wordCharArray[i]) * 0.1;
					jiao = false;
				}
				else if (shi) currentPlaceAmount = ConvertNameToSmall(wordCharArray[i]) * 10;
				else if (bai) currentPlaceAmount = ConvertNameToSmall(wordCharArray[i]) * 100;
				else if (qian) currentPlaceAmount = ConvertNameToSmall(wordCharArray[i]) * 1000;
				else
				{
					currentPlaceAmount = ConvertNameToSmall(wordCharArray[i]);
				}

				//每万位处理
				if (yi)
				{
					currentPlaceAmount = currentPlaceAmount * 100000000;
				}
				else if (wan)
				{
					currentPlaceAmount = currentPlaceAmount * 10000;
				}

				numberAmount += currentPlaceAmount;
			}

			return numberAmount.ToString();
		}


		/// <summary>
		///  转换中文数字为阿拉伯数字
		/// </summary>
		/// <param name="chinese">中文数字</param>
		/// <returns></returns>
		private static int ConvertNameToSmall(char chinese)
		{
			int number = 0;
			switch (chinese.ToString())
			{
				case "零": number = 0; break;
				case "壹": number = 1; break;
				case "贰": number = 2; break;
				case "叁": number = 3; break;
				case "肆": number = 4; break;
				case "伍": number = 5; break;
				case "陆": number = 6; break;
				case "柒": number = 7; break;
				case "捌": number = 8; break;
				case "玖": number = 9; break;
				default: throw new Exception("中文金额数字不正确：" + chinese);
			}
			return number;
		}

		/// <summary>
		/// 阿拉伯数字转中文
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public static string NumericToZH(string index)
		{
			var indexInt = int.Parse(index);
			if (indexInt > 10)
			{
				return string.Empty;
			}
			string[] num = { "零", "一", "二", "三", "四", "五", "六", "七", "八", "九", "十" };

			return num[indexInt];
		}
	}
}
