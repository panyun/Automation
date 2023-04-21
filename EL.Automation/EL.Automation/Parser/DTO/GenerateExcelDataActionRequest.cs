using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automation.Parser
{
    public class GenerateExcelDataActionRequest : RequestBase
    {
        public string ExcelPath { get; set; }
    }
    public class GenerateExcelDataActionResponse : ResponseBase
    {
        public string DataTableJson { get; set; }
        [JsonIgnore]
        [BsonIgnore]
        public DataTable DataTable { get; set; }
    }
}
