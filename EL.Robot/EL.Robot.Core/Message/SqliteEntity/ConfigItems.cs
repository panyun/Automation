using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL.Robot.Core.SqliteEntity
{
    public class ConfigItems
    {
        public long Id { get; set; }
        public string Key { get; set; }
        public object Value { get; set; }
    }
    
}
