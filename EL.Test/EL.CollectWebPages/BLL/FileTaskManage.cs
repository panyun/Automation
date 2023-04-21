using EL.CollectWebPages.Excel;
using EL.CollectWebPages.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EL.CollectWebPages.BLL
{
    /// <summary>
    /// 文件任务管理
    /// </summary>
    public class FileTaskManage
    {
        public string fileName;
        public int start;
        public int end;
        public FileTaskManage(string fileName, int start, int end)
        {
            this.fileName = fileName;
            this.start = start;
            this.end = end;
        }
        public List<ExcelInfo> GetAllTask()
        {
            return NPOIHelper.ReadData<ExcelInfo>(fileName, "Sheet1", 1);
        }
        public List<ExcelInfo> GetTask()
        {
            var list = GetAllTask();
            var result = list.Where(t => t.IsNotNullOrEmpty() && t.RowNumber > start && t.RowNumber < end && string.IsNullOrEmpty(t.状态)).ToList();
            return result;
        }
        public void SaveTask(List<ExcelInfo> excelInfos)
        {
            NPOIHelper.WriteData(fileName, "Sheet1", excelInfos, 1);
        }
    }
    public class ExcelInfo : IExcelInfo
    {
        private static Dictionary<string, PropertyInfo> PropertyInfos = new Dictionary<string, PropertyInfo>(StringComparer.InvariantCultureIgnoreCase);

        static ExcelInfo()
        {
            foreach (var item in typeof(ExcelInfo).GetProperties())
            {
                PropertyInfos.Add(item.Name, item);
            }
        }

        [TitleName("一级分类", 0)]
        public string 一级分类 { get; set; }
        [TitleName("二级分类", 1)]
        public string 二级分类 { get; set; }
        [TitleName("网站名称", 2)]
        public string 网站名称 { get; set; }
        [TitleName("网址", 3)]
        public string 网址 { get; set; }
        [TitleName("产品", 4)]
        public string 产品 { get; set; }
        [TitleName("责任人", 5)]
        public string 责任人 { get; set; }
        [TitleName("计划完成时间", 6, true)]
        public string 计划完成时间 { get; set; }
        [TitleName("完成实际时间", 7, true)]
        public string 完成实际时间 { get; set; }
        [TitleName("状态", 8, true)]
        public string 状态 { get; set; }
        [TitleName("备注", 9)]
        public string 备注 { get; set; }
        public int RowNumber { get; set; }
        public bool IsChange { get; set; }
        public int Count { get; set; }
        public bool IsNotNullOrEmpty()
        {
            return !string.IsNullOrEmpty(一级分类) && !string.IsNullOrEmpty(网址);
        }
        public string GetUrl()
        {
            return $"http://{网址}/";
        }

        public void SetValue(string PropName, string value)
        {
            if (PropertyInfos.TryGetValue(PropName, out var info))
            {
                info.SetValue(this, value);
                IsChange = true;
            }
        }
    }
}
