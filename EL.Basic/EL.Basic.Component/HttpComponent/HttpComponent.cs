using EL.Async;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace EL.Http
{
    public class HttpComponent : Entity
    {

    }
    public static class HttpComponentSystem
    {
        public static async ELTask<T> Post<T>(this HttpComponent self, string url, string json)
        {
            var result = await self.Post(url, json);
            return JsonHelper.FromJson<T>(result);
        }
        /// <summary>
        /// 通过http上传图片及传参数
        /// </summary>
        /// <param name="imgPath">图片地址(绝对路径：D:\demo\img\123.jpg)</param>
        public static async ELTask<T> PostImg<T>(this HttpComponent self, string url, Bitmap bmp,string hwnd)
        {
            try
            {
                EncoderParameters ep = new EncoderParameters(1);
                ep.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 50);//设置压缩的比例1-100
                ImageCodecInfo[] arrayICI = ImageCodecInfo.GetImageEncoders();
                ImageCodecInfo jpegICIinfo = arrayICI.FirstOrDefault(t => t.FormatID == ImageFormat.Png.Guid);
                byte[] array = default;
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    bmp.Save(memoryStream, jpegICIinfo, ep);
                    array = new byte[memoryStream.Length];
                    memoryStream.Position = 0L;
                    memoryStream.Read(array, 0, (int)memoryStream.Length);
                    memoryStream.Close();
                }
                using (var client = new HttpClient())
                using (var content = new MultipartFormDataContent())
                {
                    client.BaseAddress = new Uri(url.Substring(0, url.LastIndexOf("/")));
                    var fileContent1 = new ByteArrayContent(array);
                    fileContent1.Headers.ContentDisposition = new ContentDispositionHeaderValue("file")
                    {
                        FileName = "img",
                        Name = "img",
                    };
                    content.Add(fileContent1, "img");
                    //var windowName = new StringContent(hwnd);
                    //content.Add(windowName, "hwnd");
                     var result = await client.PostAsync(url.Substring(url.LastIndexOf("/") + 1), content);
                    var rtn = await result.Content.ReadAsStringAsync();
                    return JsonHelper.FromJson<T>(rtn);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return default;
        }


        public static async ELTask<string> Post(this HttpComponent self, string url, string json)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json;charset=utf8"; //;charset=UTF-8
            httpWebRequest.Method = "POST";
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }
            string result = default;
            try
            {
                var httpResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }
                return result;
            }
            catch (Exception ex)
            {
                Log.Trace($"消息写入失败！error:{result};\r\n Wxmsg:{json}");
                Log.Error(ex);
            }
            return result;
        }
    }
}
