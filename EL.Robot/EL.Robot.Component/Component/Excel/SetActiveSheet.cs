
using EL.Async;
using NPOI.SS.UserModel;
using System.Diagnostics;

namespace EL.Robot.Component
{
    /// <summary>
    /// 设置活动sheet
    /// </summary>
    public class SetActiveSheet : BaseComponent
    {
        public override async ELTask<INodeContent> Main(INodeContent self)
        {
            await base.Main(self);
            var openExcel = (OpenExcelInfo)self.CurrentNode.GetParamterValue("openExcel");
            var type = self.CurrentNode.GetParamterString("type");
            ISheet sheet = default;
            if (type.ToLower() == "name")
            {
                var name = self.CurrentNode.GetParamterString("name");
                sheet = openExcel.Workbook.GetSheet(name);
                if (sheet == null)
                    throw new ELNodeHandlerException("未找到工作表");
                sheet.SetActive(true);
            }
            if (type.ToLower() == "index")
            {
                var indexStr = self.CurrentNode.GetParamterString("index");
                int.TryParse(indexStr, out var index);
                openExcel.Workbook.SetActiveSheet(index);
                sheet = openExcel.Workbook.GetSheetAt(index);
            }
            self.CurrentFlow.SetFlowParamBy("activesheet", sheet);
            self.Out = sheet;
            self.Value = true;
            return self;
        }
    }
}
