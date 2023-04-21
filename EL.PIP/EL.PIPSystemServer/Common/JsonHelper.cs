using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL.PIPSystemServer.Common
{
    public static class JsonHelper
    {
        public static string ToJson(this object message)
        {
            return JsonConvert.SerializeObject(message, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        }

        public static T ToObj<T>(this string message)
        {
            return JsonConvert.DeserializeObject<T>(message);
        }
    }
}
