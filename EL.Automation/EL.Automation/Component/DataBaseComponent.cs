using EL;
using Newtonsoft.Json.Linq;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Automation
{

    public class TableData
    {
        public string ColumnName { get; set; }
        public object ColumnValue { get; set; }
        public int Index { get; set; }
    }
    public class DataBaseComponent : Entity
    {
        public DataSet DataSet { get; set; } = new DataSet();
        public List<TableData> CurrentData = new List<TableData>();

    }
    public static class DataBaseComponentSystem
    {
        private static Type Compile(params string[] columns)
        {
            ICodeCompiler comp = new Microsoft.CSharp.CSharpCodeProvider().CreateCompiler();
            CompilerParameters parms = new CompilerParameters();
            StringBuilder code = new StringBuilder();
            code.Append("using System; \n");
            code.Append("namespace Automation.Parser { \n");

            code.Append("      public class TableObject    {\n");
            foreach (var item in columns)
            {
                code.Append($"public object {item} {{ get; set; }}\n");
            }

            code.Append("} \n");
            code.Append("} \n");
            CompilerResults cr = comp.CompileAssemblyFromSource(parms, code.ToString());
            Assembly a = cr.CompiledAssembly;
            return a.GetType("Automation.Parser.TableObject");
        }

        public static List<object> ConvertObject(List<TableData> tableDatas)
        {
            List<object> objs = new();
            var type = Compile(tableDatas.Select(x => x.ColumnName).Distinct().ToArray());
            var groupObjs = tableDatas.GroupBy(x => x.Index);
            foreach (var groupObj in groupObjs)
            {
                object obj = Activator.CreateInstance(type);
                if (obj == default) continue;
                foreach (var ele in groupObj)
                    type.GetProperty(ele.ColumnName)?.SetValue(obj, ele.ColumnValue);
                objs.Add(obj);
            }
            return objs;
        }
        public static string ConvertJson(List<TableData> tableDatas)
        {
            var dataTable = ConvertTable(tableDatas);
            var json = JsonHelper.ToJson(dataTable);
            return json;
        }

        public static string ConvertXml(List<TableData> tableDatas)
        {
            var dataTable = ConvertTable(tableDatas);
            StringWriter writer = new StringWriter();
            dataTable.WriteXml(writer, mode: XmlWriteMode.DiffGram);
            var xml = writer.ToString();
            DataTable temp1 = new();
            temp1.ReadXml(xml);
            return xml;
        }
        public static DataTable ConvertTableByXml(string xml)
        {

            DataTable temp = new();
            temp.ReadXml(xml);
            return temp;
        }
        public static DataTable ConvertTable(List<TableData> tableDatas)
        {
            DataTable temp = new(IdGenerater.Instance.GenerateId() + "")
            {
                Namespace = "temp"
            };
            var columns = tableDatas.Select(x => x.ColumnName).Distinct().ToArray();
            foreach (var column in columns)
                temp.Columns.Add(column);
            var groupObjs = tableDatas.GroupBy(x => x.Index);
            foreach (var groupObj in groupObjs)
            {
                DataRow dr = temp.NewRow();
                foreach (var ele in groupObj)
                    dr[ele.ColumnName] = ele.ColumnValue;
                temp.Rows.Add(dr);
            }
            return temp;
        }
        public static DataTable ConvertTable(string json)
        {
            var obj = JsonHelper.FromJson<DataTable>(json);
            return obj;
        }
        public static string ConvertJson(DataTable data)
        {
            var json = JsonHelper.ToJson(data);
            return json;
        }

        public static void UpdateDataSet(this DataBaseComponent self, DataTable dt)
        {
            self.Remove(dt);
            self.DataSet.Tables.Add(dt);
        }
        public static void Remove(this DataBaseComponent self, DataTable dt)
        {
            if (self.DataSet.Tables.Contains(dt.TableName))
                self.DataSet.Tables.Remove(dt.TableName);
        }


    }


}