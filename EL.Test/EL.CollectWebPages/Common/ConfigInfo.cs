using EL.CollectWebPages.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EL.CollectWebPages.Common
{
    /// <summary>
    /// 配置信息(用来读取相应配置文件)
    /// </summary>
    public class ConfigInfo<T> where T : class, new()
    {
        /// <summary>
        /// 文件名
        /// </summary>
        public string FilePath { get; set; }
        /// <summary>
        /// 当前配置
        /// </summary>
        public T CurrentConfig { get; set; }
        private string Key { get; set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        public ConfigInfo(string FileName, bool isPath = false)
        {
            if (!isPath)
            {
                this.FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FileName);
            }
            else
            {
                this.FilePath = FileName;
            }
            Key = UrlTable.GetMd5Value(this.FilePath);
            CurrentConfig = Read();
        }
        /// <summary>
        /// 读取配置信息
        /// </summary>
        /// <returns></returns>
        public T Read()
        {
            T t = new T();
            if (File.Exists(FilePath))
            {
                Mutex mutex = new Mutex(false, string.Concat("Global/", Key));
                try
                {
                    mutex.WaitOne();
                    var Str = File.ReadAllText(FilePath);
                    if (!string.IsNullOrEmpty(Str))
                    {
                        try
                        {
                            t = JsonConvert.DeserializeObject<T>(Str);
                            CurrentConfig = t;
                            return t;
                        }
                        catch (Exception)
                        { }
                    }
                }
                finally
                {
                    mutex.ReleaseMutex();
                }
            }
            else
            {
                Save(t);
            }
            return t;
        }
        /// <summary>
        /// 写配置
        /// </summary>
        /// <param name="t"></param>
        public void Save(T t)
        {
            Mutex mutex = new Mutex(false, string.Concat("Global/", Key));
            try
            {
                mutex.WaitOne();
                if (!File.Exists(FilePath))
                {
                    try
                    {
                        Directory.CreateDirectory(new FileInfo(FilePath).DirectoryName);
                        CurrentConfig = t;
                    }
                    catch (Exception)
                    { }
                }
                File.WriteAllText(FilePath, t.ToJson());
            }
            catch (Exception)
            {
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }
        /// <summary>
        /// 写配置
        /// </summary>
        /// <param name="t"></param>
        public void Save()
        {
            Mutex mutex = new Mutex(false, string.Concat("Global/", Key));
            try
            {
                mutex.WaitOne();
                if (!File.Exists(FilePath))
                {
                    try
                    {
                        Directory.CreateDirectory(new FileInfo(FilePath).DirectoryName);
                    }
                    catch (Exception)
                    { }
                }
                File.WriteAllText(FilePath, CurrentConfig.ToJson());
            }
            catch (Exception)
            {
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }
    }
}
