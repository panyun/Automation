using EL.CollectWebPages.Common;
using EL.CollectWebPages.Model;
using NPOI.SS.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL.CollectWebPages.BLL
{

    public class UrlConfigManage : IConfigInfo
    {
        public static string RootPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TaskList");
        private List<ConfigInfo<SingleTask>> configInfos = new List<ConfigInfo<SingleTask>>();
        public UrlConfigManage()
        {
            if (!Directory.Exists(RootPath))
            {
                Directory.CreateDirectory(RootPath);
            }
            Load();
        }
        public void Clear()
        {
            configInfos.Clear();
            Directory.Delete(RootPath, true);
            Directory.CreateDirectory(RootPath);
            Load();
        }
        private void Load()
        {
            var file = Directory.GetFiles(RootPath, "*.json");
            foreach (var item in file)
            {
                var url = new ConfigInfo<SingleTask>(item, true);
                if (url.CurrentConfig.IsEmpty())
                {
                    try
                    {
                        File.Delete(item);
                    }
                    catch (Exception ex)
                    {
                    }
                }
                else
                {
                    configInfos.Add(url);
                }
            }
            configInfos = configInfos.OrderByDescending(t => t.CurrentConfig.AddTime).ToList();
        }

        public bool Exsit(string ID)
        {
            foreach (var item in configInfos)
            {
                if (item.CurrentConfig.Exsit(ID))
                {
                    return true;
                }
            }
            return false;
        }

        public bool Exsit(UrlTable urlTable)
        {
            return Exsit(urlTable.ID);
        }

        public List<UrlTable> GetAllUrlTable()
        {
            return configInfos.SelectMany(t => t.CurrentConfig.GetAllUrlTable()).ToList();
        }

        public List<UrlTable> GetAllUrlTable(URLState uRLState)
        {
            return configInfos.SelectMany(t => t.CurrentConfig.GetAllUrlTable(uRLState)).ToList();
        }

        public UrlTable GetFirstTask()
        {
            foreach (var item in configInfos)
            {
                var first = item.CurrentConfig.GetFirstTask();
                if (first != null)
                {
                    return first;
                }
            }
            return null;
        }
        public ConfigInfo<SingleTask> GetNotFullConfig()
        {
            var config = configInfos.Where(t => !t.CurrentConfig.IsFull()).FirstOrDefault();
            if (config == null)
            {
                var newID = Guid.NewGuid().ToString("N");
                var newConfig = new ConfigInfo<SingleTask>(Path.Combine(RootPath, $"{newID}.json"), true);
                newConfig.CurrentConfig.ID = newID;
                newConfig.Save();
                configInfos.Add(newConfig);
                config = newConfig;
            }
            return config;
        }
        public int Insert(UrlTable urlTable)
        {
            return Insert(new List<UrlTable>() { urlTable });
        }

        public int Insert(List<UrlTable> urlTables)
        {
            var index = 0;
            var NeedInsertList = urlTables.Where(t => !Exsit(t.ID)).ToList();
            if (NeedInsertList?.Count > 0)
            {
                var config = GetNotFullConfig();
                var result = config.CurrentConfig.Insert(NeedInsertList);
                if (result > 0)
                {
                    config.Save();
                }
                index += result;
                return result;
            }
            return index;
        }
        public int Update(UrlTable urlTable)
        {
            return Update(new List<UrlTable>() { urlTable });
        }

        public int Update(List<UrlTable> urlTables)
        {
            var index = 0;
            foreach (var item in configInfos)
            {
                var result = item.CurrentConfig.Update(urlTables);
                if (result > 0)
                {
                    item.Save();
                }
                index += result;
            }
            return index;
        }
        public int InsertOrUpdate(UrlTable urlTable)
        {
            return InsertOrUpdate(new List<UrlTable>() { urlTable });
        }
        public int InsertOrUpdate(List<UrlTable> urlTables)
        {
            var NeedInsertList = urlTables.Where(t => !Exsit(t.ID)).ToList();
            var NeedUpdateList = urlTables.Where(t => Exsit(t.ID)).ToList();
            if (NeedInsertList?.Any() == true)
            {
                foreach (var item in NeedInsertList)
                {
                    item.State = URLState.默认;
                }
                return Insert(NeedInsertList);
            }
            if (NeedUpdateList?.Any() == true)
            {
                return Update(NeedUpdateList);
            }
            return 0;
        }
    }
}
