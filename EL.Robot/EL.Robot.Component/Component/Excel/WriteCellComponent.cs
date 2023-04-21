
using EL.Async;
using Microsoft.JScript;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System.Diagnostics;

namespace EL.Robot.Component
{
    /// <summary>
    /// 设置活动sheet
    /// </summary>
    public class WriteCellComponent : BaseComponent
    {
        public override async ELTask<INodeContent> Main(INodeContent self)
        {
            await base.Main(self);
            var openExcel = (OpenExcelInfo)self.CurrentNode.GetParamterValue("openExcel");
            var sheet = self.GetActiveSheet();
            var val = self.CurrentNode.GetParamterValue("value") + "";
            ICell cell;
            if (self.CurrentNode.GetParamterBool("isActive"))
            {
                cell = sheet.GetRow(sheet.ActiveCell.Row).GetCell(sheet.ActiveCell.Column);
            }
            else
            {
                var col = self.CurrentNode.GetParamterInt("col");
                var row = self.CurrentNode.GetParamterInt("row");
                cell = sheet.GetRow(row).GetCell(col);
            }
            IRichTextString richText = openExcel.Workbook.GetCreationHelper().CreateRichTextString(val);
            cell.SetCellValue(richText);
            self.Out = self.CurrentFlow.SetFlowParamBy("writecell", cell);
            return self;
        }
    }
    public static class ExcelSystem
    {
        public static ISheet GetActiveSheet(this INodeContent self)
        {
            var sheet = self.CurrentNode.GetParamterValue("activesheet");
            if (sheet != null) return (ISheet)sheet;
            var openExcel = (OpenExcelInfo)self.CurrentNode.GetParamterValue("openExcel");
            sheet = openExcel.Workbook.GetSheetAt(0);
            if (sheet is not null) return (ISheet)sheet;
            throw new ELNodeHandlerException("未找到活动的工作表");
        }
    }

}
