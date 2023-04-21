using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL.CollectWebPages.Excel
{
    internal class NPOITypeUtils
    {
        public static NPOIType GetType(string fileName)
        {
            var ext = Path.GetExtension(fileName);
            return GetTypeByExt(ext);
        }

        public static NPOIType GetTypeByContentType(string contentType)
        {
            var ext = MimeTypeMap.GetExtension(contentType);
            return GetTypeByExt(ext);
        }

        private static NPOIType GetTypeByExt(string ext)
        {
            if (ext == "." + NPOIType.xlsx.ToString())
            {
                return NPOIType.xlsx;
            }
            else if (ext == "." + NPOIType.xls.ToString())
            {
                return NPOIType.xls;
            }
            throw new NotSupportedException("不支持的Excel文件类型");
        }
    }
    public enum NPOIType
    {
        [DefaultValue("application/vnd.ms-excel")]
        xls,
        [DefaultValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
        xlsx
    }
}
