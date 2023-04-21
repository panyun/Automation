
using EL.Async;
using NPOI.SS.UserModel;
using System.Diagnostics;

namespace EL.Robot.Component
{
    public class CloseExcelCompoennt : BaseComponent
    {
        public override async ELTask<INodeContent> Main(INodeContent self)
        {
            await base.Main(self);
            var openExcel = (OpenExcelInfo)self.CurrentNode.GetParamterValue("openExcel");
            openExcel.Process.CloseMainWindow();
            openExcel.Process.Close();
            openExcel.Process.Dispose();
            var isSave = self.CurrentNode.GetParamterBool("issave");
            if (isSave)
            {
                FileStream fileStream;
                using (fileStream = new FileStream(openExcel.File, FileMode.Create, FileAccess.Write))
                {
                    openExcel.Workbook.Write(fileStream, default);
                }
            }
            openExcel.Workbook.Dispose();
            self.Value = true;
            return self;
        }
    }
}