using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteDesktopManage.Common
{
    public static class JsonHelper
    {
        public static string ToJson(this object data)
        {
            return JsonConvert.SerializeObject(data);
        }
        public static T ToObj<T>(this string data)
        {
            return JsonConvert.DeserializeObject<T>(data);
        }
    }
}
