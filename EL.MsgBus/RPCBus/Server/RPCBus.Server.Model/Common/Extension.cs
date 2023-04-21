using ET;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPCBus.Server
{
    public static class Extension
    {
        private static List<string> nullStrs = new List<string>() { "&nbsp;", "null", "Null", "NULL" };
        public static bool IsNull<T>(this T value)
        {
            if (value == null) return true;
            if (value is string)
            {
                if (nullStrs.Contains(value.ToString())) return true;
                return string.IsNullOrEmpty(value.ToString()) || string.IsNullOrWhiteSpace(value.ToString());
            }
            if (value is Guid)
            {
                return (Guid.Parse(value.ToString())) == Guid.Empty;
            }
            if (value is IDictionary) return (value as IDictionary).Count == 0;
            if (value.IsObjectGenericIEnumerable())
            {
                if (value is Array)
                    return (value as Array).Length == 0;
                if (value.GetType().GetMethod("ToArray") != null)
                {
                    var array = value.GetType().GetMethod("ToArray").Invoke(value, null);
                    return (array as Array).Length == 0;
                }
                if (value.GetType().GetProperty("Count") != null)
                {
                    var val = (int)value.GetType().GetProperty("Count").GetValue(value, null);
                    return val == 0;
                }

            }
            return false;
        }
        public static bool HasValue<T>(this T value)
        {
            if (value is string)
            {
                if (nullStrs.Contains(value.ToString())) return false;
                return !string.IsNullOrEmpty(value.ToString()) && !string.IsNullOrWhiteSpace(value.ToString());
            }
            if (value is Guid)
                return (Guid.Parse(value.ToString())) == Guid.Empty;
            if (value is int)
                return int.TryParse(value.ToString(), out int temp) ? temp != default : false;
            if (value is long)
                return long.TryParse(value.ToString(), out long temp) ? temp != default : false;
            if (value is IDictionary) return (value as IDictionary).Count > 0;
            if (value.IsObjectGenericIEnumerable())
            {
                if (value is Array)
                    return (value as Array).Length == 0;
                if (value.GetType().GetMethod("ToArray") != null)
                {
                    var array = value.GetType().GetMethod("ToArray").Invoke(value, null);
                    return (array as Array).Length > 0;
                }
                if (value.GetType().GetProperty("Count") != null)
                {
                    var val = (int)value.GetType().GetProperty("Count").GetValue(value, null);
                    return val > 0;
                }
            }
            return value != null;
        }
        public static bool IsObjectGenericIEnumerable<T>(this T o)
        {
            return o is IEnumerable && (o.GetType().IsGenericType || o is Array);
        }
        /// <summary>
        /// 委托实现List处理
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="items">每一个项目</param>
        /// <param name="action">待执行的委托</param>
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (T item in items)
            {
                action(item);
            }
        }
        public static void SetResponseReply(this IActorResponse response, string msg, Action reply, int ErrorCode = ET.ErrorCode.ERR_BLL)
        {
            response.Error = ErrorCode;
            response.Message = msg;
            reply();
        }
    }
    
}
