

using EL;
using EL.Async;
using EL.Robot.Component;
/// <summary>
/// 设置活动sheet
/// </summary>
public class MaxRowColComponent : BaseComponent
{
    public override async ELTask<INodeContent> Main(INodeContent self)
    {
        await base.Main(self);
        var openExcel = (OpenExcelInfo)self.CurrentNode.GetParamterValue("openExcel");
        var index = openExcel.Workbook.ActiveSheetIndex;
        var sheet = openExcel.Workbook.GetSheetAt(index);
        int maxCol = default;
        for (int i = 0; i < sheet.LastRowNum; i++)
        {
            var colIndex = sheet.GetRow(i).LastCellNum;
            if (colIndex > maxCol) maxCol = colIndex;
        }
        self.Out = new { MaxRow = sheet.LastRowNum, MaxCol = maxCol };
        self.Value = true;
        return self;
    }
}