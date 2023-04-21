using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfInspect.Core
{
    public static class StringHelper
    {
        public static string NormalizeString(this string value, int length = 18)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }
            value = value.Replace(Environment.NewLine, " ").Replace('\r', ' ').Replace('\n', ' ').Replace(".", "");
            if (value.Length > length)
                value = value.Substring(0, length) + "...";
            return value;
        }
    }
}
