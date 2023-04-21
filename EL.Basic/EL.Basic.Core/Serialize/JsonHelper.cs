using MongoDB.Bson.IO;
using Newtonsoft.Json;
using System;

namespace EL
{
    public static class JsonHelper
    {

        public static string ToJson(object message)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(message);
        }
        public static object FromJson(Type type, string json)
        {
            var jSetting = new JsonSerializerSettings();
            jSetting.NullValueHandling = NullValueHandling.Ignore;
            return Newtonsoft.Json.JsonConvert.DeserializeObject(json, type, jSetting);
        }
        public static T FromJson<T>(string json)
        {
            var jSetting = new JsonSerializerSettings();
            jSetting.NullValueHandling = NullValueHandling.Ignore;
            if (json == null)
                return default(T);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json, jSetting);
        }
    }
    public static class BsonHelper
    {

        private static readonly JsonWriterSettings logDefineSettings = new JsonWriterSettings() { OutputMode = JsonOutputMode.RelaxedExtendedJson };
        public static string ToJson(object message)
        {
            return MongoDB.Bson.BsonExtensionMethods.ToJson(message, logDefineSettings);
        }

        public static object FromJson(Type type, string json)
        {
            return MongoDB.Bson.Serialization.BsonSerializer.Deserialize(json, type);
        }

        public static T FromJson<T>(string json)
        {
            return MongoDB.Bson.Serialization.BsonSerializer.Deserialize<T>(json);
        }
    }
}