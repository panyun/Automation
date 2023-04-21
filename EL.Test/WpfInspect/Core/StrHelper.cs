using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfInspect.Core
{
    public static class StrHelper
    {
        public static string ContrlStr(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;
            str = str.Replace(Environment.NewLine, " ").Replace('\r', ' ').Replace('\n', ' ');
            if (str.Length > 18)
                str = str.Substring(0, 18) + "..";
            return str;
        }
    }
}
