using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EL.CollectWebPages.Model
{
    public class UrlTable
    {
        public string ID { get; set; }
        public string URL { get; set; }
        public string? Title { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public URLState State { get; set; }
        public DateTime AddTime { get; set; }
        public DateTime? EndTime { get; set; }
        //public DateTime addTime
        //{
        //    get
        //    {
        //        return GetDatetime(AddTime);
        //    }
        //    set
        //    {
        //        AddTime = (int)GenerateTimestamp(value);
        //    }
        //}
        //public DateTime endTime
        //{
        //    get
        //    {
        //        return GetDatetime(EndTime);
        //    }
        //    set
        //    {
        //        EndTime = (int)GenerateTimestamp(value);
        //    }
        //}
        public void SetUrl(string url)
        {
            this.URL = url;
            this.ID = GetMd5Value(url + url.Length);
        }
        private long GenerateTimestamp(DateTime dateTime)
        {
            return new DateTimeOffset(dateTime.ToUniversalTime()).ToUnixTimeSeconds();
        }

        private DateTime GetDatetime(long timeStamp)
        {
            var d = DateTimeOffset.FromUnixTimeSeconds(timeStamp);
            return d.LocalDateTime;
        }
        public static string GetMd5Value(string input)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            using var md5 = new MD5CryptoServiceProvider();
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hashBytes)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
        public static string GetCreateSql()
        {
            return @"CREATE TABLE IF NOT EXISTS UrlTable (
	ID TEXT(32) PRIMARY KEY  NOT NULL,
	URL TEXT,
	Title TEXT,
	State TEXT,
	AddTime INTEGER,
	EndTime INTEGER
);";
        }
    }
}
