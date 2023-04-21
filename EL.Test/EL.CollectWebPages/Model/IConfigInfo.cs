using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL.CollectWebPages.Model
{
    public interface IConfigInfo
    {
        List<UrlTable> GetAllUrlTable();
        List<UrlTable> GetAllUrlTable(URLState uRLState);
        UrlTable GetFirstTask();
        int Insert(UrlTable urlTable);
        int Insert(List<UrlTable> urlTables);
        int Update(UrlTable urlTable);
        int Update(List<UrlTable> urlTables);
        bool Exsit(string ID);
        bool Exsit(UrlTable urlTable);
    }
}
