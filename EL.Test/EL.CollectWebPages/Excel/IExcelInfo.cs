using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL.CollectWebPages.Excel
{
    public interface IExcelInfo
    {
        int RowNumber { get; set; }
        bool IsChange { get; set; }
        bool IsNotNullOrEmpty();
        void SetValue(string PropName, string value);
    }
}
