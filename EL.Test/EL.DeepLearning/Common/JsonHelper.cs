using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL.DeepLearning.Common
{
    public static class JsonHelper
    {
        public static string ToJson(this object data)
        {
            var settings = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore };
            return JsonConvert.SerializeObject(data, settings);
        }
        public static T ToObj<T>(this string data)
        {
            return JsonConvert.DeserializeObject<T>(data);
        }
    }
}
