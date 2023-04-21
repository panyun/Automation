using EL.Robot.Component;
using EL.Robot.Core.SqliteEntity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL.Robot.Core
{
    public class FlowComponent : Entity
    {
        public Flow MainFlow { get; set; }
        public Flow CurrentFlow { get; set; }
        public Dictionary<long, Node> Steps { get; set; } = new Dictionary<long, Node>();
        public Action StartFlowAction { get; set; }
        public Action EndFlowAction { get; set; }
        public List<ExecLog> LogMsgs { get; set; } = new List<ExecLog>();
        public Action<ExecLog> RefreshLogMsgAction { get; set; }
        public Action<string> RefreshVariablesAction { get; set; }
        public FlowHistory FlowHistory { get; set; }
    }
    
    //public class VariableModel
    //{
    //    public string Key { get; set; }
    //    public string Value { get; set; }
    //    public List<VariableModel> Childs { get; set; }

    //}
    //public static class VariablesTest
    //{
    //    public static List<VariableModel> Get()
    //    {
    //        var str = "{\"Id\":-2010054040376770560,\"ProcessName\":\"explorer\",\"MainWindowTitle\":\"\",\"BoundingRectangle\":\"75, 3, 75, 67\",\"Width\":75,\"Height\":67,\"X\":75,\"Y\":3,\"Img\":\"\",\"Name\":\"IEPageTest.html\",\"ControlType\":\"ListItem\",\"Path\":\"pane[0]/list[0]/listitem[9]\",\"Index\":9,\"PathNode\":{\"Id\":\"6064904626984189952\",\"LevelIndex\":2,\"Length\":68,\"Index\":9,\"CurrentElementWin\":{\"Name\":\"IEPageTest.html\",\"ControlType\":50007,\"ClassName\":\"\",\"Id\":\"6064904626984189952\"},\"Parent\":{\"Id\":\"6064905314178957312\",\"LevelIndex\":1,\"Length\":1,\"Index\":0,\"CurrentElementWin\":{\"ControlType\":50008,\"ClassName\":\"SysListView32\",\"Id\":\"6064905314178957312\"},\"Parent\":{\"Id\":\"6064906344971108352\",\"LevelIndex\":0,\"Length\":0,\"Index\":0,\"CurrentElementWin\":{\"Name\":\"桌面 1\",\"ControlType\":50033,\"ClassName\":\"#32769\",\"Id\":\"6064906344971108352\"},\"CompareValues\":{\"name\":\"8F63A950CD755653187D316C11724E4B\",\"compareid\":\"C4EB7243E14122F47B7A22C9E289D8C3\",\"comparetempid\":\"56822FAF2196F1A87F822487C79E3FF\"}},\"CompareValues\":{\"name\":\"D41D8CD98F0B24E980998ECF8427E\",\"compareid\":\"6396E22982C85951F129287D378634\",\"comparetempid\":\"BBD1B3F3D0F6D5B864FD8049BC2155D5\"}},\"CompareValues\":{\"name\":\"7666ED5F8A97531062D917A43496EE2F\",\"compareid\":\"FAE14479AB2241C3BE289EC7811DDCD6\",\"comparetempid\":\"125F3652258C968898777986AB20E2D0\",\"comparechildrenid\":\"1DFF50B2301C5AC32951A09FA04D83\"}},\"ProgramType\":1,\"ElementNodes\":[{\"Id\":\"6064906344971108352\",\"ElementPropertys\":[{\"Key\":\"IsChecked\",\"Expression\":\"true\",\"Value\":true,\"IsActive\":true,\"DefalutRuntimeId\":\"12B9E79E4C2B9BB9189253F98FEC608C\"},{\"Key\":\"Name\",\"Expression\":\"桌面 1\",\"Value\":\"桌面 1\",\"IsActive\":true,\"DefalutRuntimeId\":\"6E67E424577C611743269C4EB9E6B630\"},{\"Key\":\"Index\",\"Expression\":\"0\",\"Value\":0,\"IsActive\":true,\"DefalutRuntimeId\":\"F6ABEA42A9F712F2D7204CFF62CF8\"},{\"Key\":\"ControlType\",\"Expression\":\"50033\",\"Value\":50033,\"IsActive\":true,\"DefalutRuntimeId\":\"B1D8677A96CEF535EE775420FB6A71B5\"}]},{\"Id\":\"6064905314178957312\",\"ElementPropertys\":[{\"Key\":\"IsChecked\",\"Expression\":\"true\",\"Value\":true,\"IsActive\":true,\"DefalutRuntimeId\":\"12B9E79E4C2B9BB9189253F98FEC608C\"},{\"Key\":\"Name\",\"Expression\":null,\"Value\":null,\"IsActive\":true,\"DefalutRuntimeId\":\"F89D404AAF181D212DD134C3BE3F3897\"},{\"Key\":\"Index\",\"Expression\":\"0\",\"Value\":0,\"IsActive\":true,\"DefalutRuntimeId\":\"F6ABEA42A9F712F2D7204CFF62CF8\"},{\"Key\":\"ControlType\",\"Expression\":\"50008\",\"Value\":50008,\"IsActive\":true,\"DefalutRuntimeId\":\"6199EFF28F94203C479A97D4FFA9B1\"}]},{\"Id\":\"6064904626984189952\",\"ElementPropertys\":[{\"Key\":\"IsChecked\",\"Expression\":\"true\",\"Value\":true,\"IsActive\":true,\"DefalutRuntimeId\":\"12B9E79E4C2B9BB9189253F98FEC608C\"},{\"Key\":\"Name\",\"Expression\":\"IEPageTest.html\",\"Value\":\"IEPageTest.html\",\"IsActive\":true,\"DefalutRuntimeId\":\"1995239F10A3914C55DF8C4F2C9511\"},{\"Key\":\"Index\",\"Expression\":\"9\",\"Value\":9,\"IsActive\":true,\"DefalutRuntimeId\":\"B63ED7BABFCDC38503334B37730CB\"},{\"Key\":\"ControlType\",\"Expression\":\"50007\",\"Value\":50007,\"IsActive\":true,\"DefalutRuntimeId\":\"7076D1223B4ECF29DF44DAD98CB4FF1\"}]}]}";
    //        var obj = (JObject)JsonConvert.DeserializeObject(str);
    //        var list = GetVariable(obj);
    //        return default;
    //    }
    //    public static List<VariableModel> GetVariable(dynamic obj)
    //    {
    //        List<VariableModel> vals = new List<VariableModel>();

    //        if (obj is JObject jobj)
    //        {
    //            foreach (var item in jobj)
    //            {
    //                var variab = new VariableModel();
    //                variab.Key = item.Key;
    //                if (item.Value == null || item.Value.Count() == 0)
    //                {
    //                    variab.Value = item.Value + "";
    //                    vals.Add(variab);
    //                    continue;
    //                }
    //                if (item.Value is JObject)
    //                {
    //                    variab.Childs = GetVariable((JObject)item.Value);
    //                    vals.Add(variab);
    //                    continue;
    //                }
    //            }
    //            return vals;
    //        }
    //        if (obj is JArray jArray)
    //        {
    //            int index = 0;
    //            foreach (var item in jArray)
    //            {
    //                var variab = new VariableModel();
    //                variab.Key = $"[{index}]";
    //                variab.Childs = GetVariable(item);
    //                vals.Add(variab);
    //            }
    //        }

    //        return vals;
    //   }
    // }
}
