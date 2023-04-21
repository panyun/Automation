using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.AxHost;

namespace EL.CollectWebPages.Model
{
    public class SingleTask : IConfigInfo
    {
        public string ID { get; set; }
        public Dictionary<string, UrlTable> Data { get; set; } = new Dictionary<string, UrlTable>();
        public DateTime AddTime { get; set; } = DateTime.Now;
        public bool IsEmpty()
        {
            return Data.Count == 0;
        }
        public bool IsFull()
        {
            return Data.Count > 10000;
        }
        public bool IsSuccess()
        {
            return Data.Values.Any(t => t.State != URLState.默认 || t.State != URLState.处理中);
        }
        public bool Exsit(string ID)
        {
            return Data.ContainsKey(ID);
        }

        public bool Exsit(UrlTable urlTable)
        {
            if (urlTable == null || urlTable.ID == null)
            {
                return false;
            }
            return Data.ContainsKey(urlTable.ID);
        }

        public List<UrlTable> GetAllUrlTable()
        {
            return Data.Values.ToList();
        }

        public List<UrlTable> GetAllUrlTable(URLState uRLState)
        {
            return Data.Values.Where(t => t.State == uRLState).ToList();
        }

        public UrlTable GetFirstTask()
        {
            var process = Data.Values.Where(t => t.State == URLState.处理中).FirstOrDefault();
            if (process != null)
            {
                return process;
            }
            process = Data.Values.Where(t => t.State == URLState.默认).FirstOrDefault();
            if (process != null)
            {
                return process;
            }
            return null;
        }

        public int Insert(List<UrlTable> urlTables)
        {
            var index = 0;
            if (urlTables?.Any() == true)
            {
                foreach (var item in urlTables)
                {
                    if (!Data.ContainsKey(item.ID))
                    {
                        Data.Add(item.ID, item);
                        index++;
                    }
                }
            }
            return index;
        }
        public int Insert(UrlTable urlTable)
        {
            if (urlTable != null)
            {
                return 0;
            }
            return Insert(new List<UrlTable>() { urlTable });
        }

        public int Update(List<UrlTable> urlTables)
        {
            var index = 0;
            if (urlTables?.Any() == true)
            {
                foreach (var item in urlTables)
                {
                    if (Data.ContainsKey(item.ID))
                    {
                        if (item.Title != null)
                        {
                            Data[item.ID].Title = item.Title;
                        }
                        if (item.State != URLState.None)
                        {
                            Data[item.ID].State = item.State;
                        }
                        if (item.EndTime != null)
                        {
                            Data[item.ID].EndTime = item.EndTime;
                        }
                        index++;
                    }
                }
            }
            return index;
        }
        public int Update(UrlTable urlTable)
        {
            return Update(new List<UrlTable>() { urlTable });
        }
    }
}
