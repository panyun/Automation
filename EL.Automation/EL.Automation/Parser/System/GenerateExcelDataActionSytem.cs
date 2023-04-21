using Automation.Inspect;
using EL;
using Interop.UIAutomationClient;
using NPOI.SS.UserModel;
using System.Data;
using System.Text.RegularExpressions;

namespace Automation.Parser
{
    public static class GenerateExcelDataActionSytem
    {
        // ControlType Table
        //

        public static DataTable Main(this GenerateExcelDataActionRequest self)
        {
            var data = GetExcelData(self.ExcelPath);
            var dataExcele = DataBaseComponentSystem.ConvertTable(data);
            return dataExcele;
        }
        public static List<TableData> GetExcelData(string excelPath)
        {
            List<TableData> tableDatas = new List<TableData>();
            IWorkbook workbook = WorkbookFactory.Create(excelPath);
            foreach (var sheet in workbook)
            {
                var lastRowNum = sheet.LastRowNum;
                for (int row = 0; row <= lastRowNum; row++)
                {
                    var rowData = sheet.GetRow(row);
                    if (rowData == default || rowData.Cells == default)
                        continue;
                    foreach (var cell in rowData.Cells)
                    {
                        var add = cell.Address + "";
                        var col= Regex.Replace(add, @"[^A-Z]+", "");
                        TableData tableData = new TableData()
                        {
                            ColumnName = col,
                            ColumnValue = cell + "",
                            Index = cell.Address.Row + 1,
                        };
                        tableDatas.Add(tableData);
                    }
                }
                break;
            }
            return tableDatas;
        }

    }
}
