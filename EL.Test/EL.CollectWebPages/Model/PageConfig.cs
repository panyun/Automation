using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL.CollectWebPages.Model
{
    public class PageConfig
    {
        public string RootPath { get; set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "images");
        public string RootUrl { get; set; } = "https://baike.baidu.com/vbaike";
        public string ExePath { get; set; }
        public int UrlCount { get; set; }  
        public string ExcelPath { get; set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "清单.xlsx");
        public int startIndex { get; set; }
        public int endIndex { get; set; }
    }
}
