using EL.Async;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace EL.Robot.Component
{
    /// <summary>
    /// json组件
    /// </summary>
    public class JsonComponent : BaseComponent
    {
		public JsonComponent()
		{
			//Config.Category = Category.基础组件;
		}
		public override Config GetConfig()
		{
			if (Config.IsInit) return Config;
			Config.ButtonDisplayName = "Json";
			return base.GetConfig();
		}
		public override async ELTask<INodeContent> Main(INodeContent self)
        {
            await base.Main(self);
            var targetvalue = self.CurrentNode.GetParamterString("targetvalue");
            var type = self.CurrentNode.GetParamterInt("type");
            switch (type)
            {
                case 1: // json字符串转xml
                    var obj = JsonConvert.DeserializeObject<Dictionary<string, object>>(targetvalue);
                    XElement el = new("root",
                    obj.Select(kv => new XElement(kv.Key, kv.Value)));
                    self.Out = el.ToString();
                    break;
                case 2: //xml转json字符串
                    XmlDocument document = new XmlDocument();
                    document.LoadXml(targetvalue);
                    self.Out = JsonConvert.SerializeXmlNode(document);
                    break;
                case 5:
                case 3: //json对象转字符串
                    var jsonObj = self.CurrentNode.GetParamterValue("targetvalue");
                    if (jsonObj != null)
                        self.Out = JsonConvert.SerializeObject(jsonObj);
                    break;
                case 4: //json对象转字符串
                    obj = JsonConvert.DeserializeObject<Dictionary<string, object>>(targetvalue);
                    if (obj != null)
                        self.Out = obj;
                    break;
            }

            self.Value = true;
            return self;
        }
    }
}
