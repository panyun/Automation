

using EL.Async;
using NPOI.SS.UserModel;
using System.Diagnostics;

namespace EL.Robot.Component
{
    /// <summary>
    /// 设置活动sheet
    /// </summary>
    public class CreateSheetComponent : BaseComponent
    {
        public override async ELTask<INodeContent> Main(INodeContent self)
        {
            await base.Main(self);
            var openExcel = (OpenExcelInfo)self.CurrentNode.GetParamterValue("openExcel");
            var name = self.CurrentNode.GetParamterString("name");
            var sheet = openExcel.Workbook.CreateSheet(name);
            bool isFirst = self.CurrentNode.GetParamterBool("isfirst");
            if (isFirst)
                openExcel.Workbook.SetSheetOrder(sheet.SheetName, 0);
            self.CurrentFlow.SetFlowParamBy("activesheet", sheet);
            self.Out = sheet;
            self.Value = true;
            return self;
        }
    }
}
