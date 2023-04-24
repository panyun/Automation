using EL.Async;
using Newtonsoft.Json;
using System.Xml.Linq;

namespace EL.Robot.Component
{

	/// <summary>
	/// json组件
	/// </summary>
	public class ArrayComponent : BaseComponent
	{
		public ArrayComponent()
		{
			//Config.Category = Category.基础组件;
		}
		public override Config GetConfig()
		{
			if (Config.IsInit) return Config;
			Config.ButtonDisplayName = "数组";
			return base.GetConfig();
		}
		public override async ELTask<INodeContent> Main(INodeContent self)
		{

			await base.Main(self);
			var function = self.CurrentNode.GetParamterString("function");
			switch (function)
			{
				case "del":
					Del(self);
					break;
				case "insert":
					Insert(self);
					break;
				case "modify":
					Modify(self);
					break;
				// 去重
				case "duplicateRemoval":
					DuplicateRemoval(self);
					break;
				// 截取
				case "split":
					Split(self);
					break;
				case "length":
					Length(self);
					break;
				case "conversion":
					Conversion(self);
					break;
				case "concat":
					Concat(self);
					break;
				default:
					break;
			}
			self.Value = true;

			return self;
			/* Type type = typeof(ArrayComponent);
             object obj = Activator.CreateInstance(type);
             string function_name = char.ToUpper(function[0]) + function.Substring(1);
             MethodInfo methodInfo = type.GetMethod(function_name);
             methodInfo.Invoke(obj, new object[] { self });
             self.Value = true;
             return self;*/
		}

		public void Del(INodeContent self)
		{
			var targetValueList = self.CurrentNode.GetParamterValue("targetValue");
			List<Object> targetObj = new List<object>();
			if (targetValueList is string)
				targetObj = JsonConvert.DeserializeObject<List<object>>(targetValueList + "");
			else if (!(targetValueList is List<object>))
				throw new ELNodeHandlerException($"当前对象的数据类型不正确！");
			else
				targetObj = targetValueList as List<object>;
			var delType = self.CurrentNode.GetParamterInt("delType");
			var function = self.CurrentNode.GetParamterString("function");
			if (function == "del" && delType == 1)
			{
				var startPosition = self.CurrentNode.GetParamterInt("startPosition");
				var endPosition = self.CurrentNode.GetParamterInt("endPosition");
				if (startPosition > endPosition)
					throw new ELNodeHandlerException("开始索引不能大于结束索引");
				targetObj.RemoveRange(startPosition, endPosition);
			}
			else if (function == "del" && delType == 2)
			{
				var isFirstMatch = self.CurrentNode.GetParamterBool("isFirstMatch");
				var delText = self.CurrentNode.GetParamterValue("delText");
				if (isFirstMatch)
					targetObj.Remove(delText);
				else
					targetObj.RemoveAll(x => x + "" == delText + "");
			}
			else if (function == "del" && delType == 3)
				targetObj.Clear();
			self.Out = targetObj;
			self.Value = true;
		}


		public void Concat(INodeContent self)
		{
			var targetValueList = self.CurrentNode.GetParamterValueExrp("targetValueList");
			List<object> targetObj = new List<object>();
			if (targetValueList is string)
				targetObj = JsonConvert.DeserializeObject<List<object>>(targetValueList + "");
			else if (!(targetValueList is List<object>))
				throw new ELNodeHandlerException($"当前对象的数据类型不正确！");
			else
				targetObj = targetValueList as List<object>;
			List<object> new_list = new List<object>();
			foreach (var item in targetObj)
			{
				var result = JsonConvert.DeserializeObject<List<object>>(item + "");
				new_list.AddRange(result);
			}
			self.Out = new_list;
			self.Value = true;
		}

		public void Conversion(INodeContent self)
		{
			var targetvalue = self.CurrentNode.GetParamterValue("targetvalue");
			List<object> targetObj = new List<object>();
			if (targetvalue is string)
				targetObj = JsonConvert.DeserializeObject<List<object>>(targetvalue + "");
			else if (!(targetvalue is List<object>))
				throw new ELNodeHandlerException($"当前对象的数据类型不正确！");
			else
				targetObj = targetvalue as List<object>;
			var conversionType = self.CurrentNode.GetParamterInt("conversionType");
			if (conversionType == 1)
			{
				XDocument doc = new XDocument();
				doc.Add(new XElement("root", targetObj.Select(x => new XElement("item", x))));
				self.Out = doc;
				self.Value = true;
			}
			else if (conversionType == 2)
			{
				var json = JsonConvert.SerializeObject(targetObj);
				self.Out = json;
				self.Value = true;
			}
			else if (conversionType == 3)
			{


			}


		}
		public void Length(INodeContent self)
		{
			var targetvalue = self.CurrentNode.GetParamterValue("targetvalue");
			List<object> targetObj = new List<object>();
			if (targetvalue is string)
				targetObj = JsonConvert.DeserializeObject<List<object>>(targetvalue + "");
			else if (!(targetvalue is List<object>))
				throw new ELNodeHandlerException($"当前对象的数据类型不正确！");
			else
				targetObj = targetvalue as List<object>;
			self.Out = targetObj.Count;
			self.Value = true;
		}

		public void Split(INodeContent self)
		{

			List<object> targetObj = new List<object>();
			var targetvalue = self.CurrentNode.GetParamterValue("targetvalue");
			if (targetvalue is string)
				targetObj = JsonConvert.DeserializeObject<List<object>>(targetvalue + "");
			else if (!(targetvalue is List<object>))
				throw new ELNodeHandlerException($"当前对象的数据类型不正确！");
			else
				targetObj = targetvalue as List<object>;
			var function = self.CurrentNode.GetParamterString("function");
			var splitType = self.CurrentNode.GetParamterInt("splitType");
			if (function == "split" && splitType == 1)
			{
				var splitPosition = self.CurrentNode.GetParamterInt("splitPosition");
				if (splitPosition > targetObj.Count)
					throw new ELNodeHandlerException("截取的下标超出数组的总长度");
				self.Out = targetObj.Skip(splitPosition).ToArray();
				self.Value = true;
			}
			else
			{
				var isMatchFirst = self.CurrentNode.GetParamterValue("isFirstMatch");
				if (isMatchFirst?.ToString() == "true")
				{
					for (int i = 0; i < targetObj.Count; i++)
					{
					}
				}
			}
		}

		public void DuplicateRemoval(INodeContent self)
		{
			var targetvalue = self.CurrentNode.GetParamterValue("targetvalue");
			List<object> targetObj = new List<object>();
			if (targetvalue is string)
				targetObj = JsonConvert.DeserializeObject<List<object>>(targetvalue + "");
			else if (!(targetvalue is List<object>))
				throw new ELNodeHandlerException($"当前对象的数据类型不正确！");
			else
				targetObj = targetvalue as List<object>;
			targetObj = targetObj.Distinct().ToList();
			self.Out = targetObj;
			self.Value = true;
		}


		public void Insert(INodeContent self)
		{
			var targetvalue = self.CurrentNode.GetParamterValue("targetvalue");
			var function = self.CurrentNode.GetParamterString("function");
			var insertvalue = self.CurrentNode.GetParamterValue("insertvalue");
			var insertype = self.CurrentNode.GetParamterInt("insertype");
			List<object> targetObj = default;
			if (targetvalue is string)
				targetObj = JsonConvert.DeserializeObject<List<object>>(targetvalue + "");
			else if (targetvalue is not List<object>)
				throw new ELNodeHandlerException($"当前对象的数据类型不正确！");
			else
				targetObj = targetvalue as List<object>;
			if (function == "insert" && insertype == 1)
			{
				var insertindex = self.CurrentNode.GetParamterInt("insertindex");
				insertindex--;
				if (insertindex < 0 || insertindex > targetObj.Count)
					throw new ELNodeHandlerException($"插入索引不正确!");
				targetObj.Insert(insertindex, insertvalue);
				self.Out = targetObj;
			}
			else if (function == "insert" && insertype == 2)
			{
				var inserPosition = self.CurrentNode.GetParamterString("inserPosition");
				var isFirstMatch = self.CurrentNode.GetParamterBool("isFirstMatch");
				var matchText = self.CurrentNode.GetParamterValue("matchText");
				List<Object> target = new();
				for (int i = 0; i < targetObj.Count; i++)
				{
					if (targetObj[i] + "" != matchText + "")
					{
						target.Add(targetObj[i]);
						continue;
					}
					if (targetObj[i] + "" == matchText + "")
					{
						if (inserPosition == "front")
						{
							target.Add(insertvalue);
							target.Add(targetObj[i]);

						}
						if (inserPosition == "back")
						{
							target.Add(targetObj[i]);
							target.Add(insertvalue);
						}
						if (isFirstMatch)
						{
							var arrar = targetObj.Skip(i + 1);
							if (arrar != null && arrar.Count() > 0)
								target.AddRange(arrar);
							break;
						}
					}
				}
				self.Out = target;
				self.Value = true;
			}
			else
			{
				targetObj.Add(insertvalue);
				self.Out = targetObj;
			}
			self.Value = true;
		}



		public void Modify(INodeContent self)
		{
			var targetvalue = self.CurrentNode.GetParamterValue("targetvalue");
			var function = self.CurrentNode.GetParamterString("function");
			var newvalue = self.CurrentNode.GetParamterValue("newValue");
			var modifytype = self.CurrentNode.GetParamterInt("modifyType");
			List<object> targetObj = new List<object>();
			if (function == "modify" && modifytype == 1)
			{
				var modifyindex = self.CurrentNode.GetParamterInt("position");
				if (targetvalue is string)
					targetObj = JsonConvert.DeserializeObject<List<object>>(targetvalue + "");
				else if (!(targetvalue is List<object>))
					throw new ELNodeHandlerException($"当前对象的数据类型不正确！");
				else
					targetObj = targetvalue as List<object>;
				if (modifyindex < 0 || modifyindex > targetObj?.Count)
					throw new ELNodeHandlerException("修改值的索引不正确！");
				targetObj[modifyindex] = newvalue;
			}
			else
			{
				if (function == "modify" && modifytype == 2)
				{
					var isFirstMatch = self.CurrentNode.GetParamterBool("isFirstMatch");
					var modifyValue = self.CurrentNode.GetParamterValue("modifyValue");
					if (targetvalue is string)
						targetObj = JsonConvert.DeserializeObject<List<object>>(targetvalue + "");
					else if (!(targetvalue is List<object>))
						throw new ELNodeHandlerException($"当前对象的数据类型不正确！");
					else
						targetObj = targetvalue as List<object>;
					for (int i = 0; i < targetObj?.Count; i++)
					{
						if (targetObj[i] == modifyValue)
							targetObj[i] = newvalue;
						if (isFirstMatch)
							break;
						continue;
					}
				}
			}
			self.Out = targetObj;
			self.Value = true;
		}
	}
}
