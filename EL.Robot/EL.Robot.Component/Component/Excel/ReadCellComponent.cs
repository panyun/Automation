using EL.Async;
using System.Data;

namespace EL.Robot.Component
{
    /// <summary>
    /// 设置活动sheet
    /// </summary>
    public class ReadCellComponent : BaseComponent
    {
        public override async ELTask<INodeContent> Main(INodeContent self)
        {
            await base.Main(self);
            var openExcel = (OpenExcelInfo)self.CurrentNode.GetParamterValue("openExcel");
            var sheet = self.GetActiveSheet();
            var cellType = (ReadCellType)self.CurrentNode.GetParamterInt("type");
            if (cellType == ReadCellType.单个单元格值)
            {
                var col = self.CurrentNode.GetParamterInt("col");
                var row = self.CurrentNode.GetParamterInt("row");
                var cell = sheet.GetRow(row).GetCell(col);
                if (cell is null) throw new ELNodeHandlerException("未找到单元格");
                self.Out = cell.StringCellValue;
                self.Value = true;
                return self;
            }
            DataTable dataTable = new();
            if (cellType == ReadCellType.一组单元格的值)
            {
                var startcol = self.CurrentNode.GetParamterInt("startcol");
                var startrow = self.CurrentNode.GetParamterInt("startrow");
                var endcol = self.CurrentNode.GetParamterInt("endcol");
                var endrow = self.CurrentNode.GetParamterInt("endrow");
                for (int i = startcol; i <= endcol; i++)
                    dataTable.Columns.Add(new DataColumn());
                for (int r = startrow; r <= endrow; r++)
                {
                    DataRow row = dataTable.NewRow();
                    for (int c = startcol; c <= endcol; c++)
                        row[c] = sheet.GetRow(r).GetCell(c).StringCellValue;
                    dataTable.Rows.Add(row);
                }
            }
            if (cellType == ReadCellType.工作表中所有可用的值)
            {
                var rowIndex = sheet.LastRowNum;

                for (int r = 1; r < rowIndex; r++)
                {
                    var row = dataTable.NewRow();
                    int index = 1;
                    foreach (var item in sheet.GetRow(r))
                        row[index] = item.StringCellValue;
                    dataTable.Rows.Add(row);
                }
            }
            self.Value = true;
            self.Out = dataTable;
            return self;
        }
    }
    public enum ReadCellType
    {
        单个单元格值,
        一组单元格的值,
        工作表中所有可用的值
    }

}