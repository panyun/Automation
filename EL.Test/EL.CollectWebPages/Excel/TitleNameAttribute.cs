using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL.CollectWebPages.Excel
{
    public class TitleNameAttribute : Attribute
    {
        public string Value { get; set; }
        public bool Change { get; set; }
        public int CellIndex { get; set; }
        public TitleNameAttribute(string value, int cellIndex, bool Change = false)
        {
            Value = value;
            CellIndex = cellIndex;
            this.Change = Change;
        }
    }
}
