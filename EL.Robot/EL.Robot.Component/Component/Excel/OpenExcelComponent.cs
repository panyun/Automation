using EL.Async;
using NPOI.SS.UserModel;
using System.Diagnostics;

namespace EL.Robot.Component
{
    public class OpenComponent : BaseComponent
    {
        public override async ELTask<INodeContent> Main(INodeContent self)
        {
            await base.Main(self);
            var excelPath = self.CurrentNode.GetParamterString("excelPath");
            OpenExcelInfo openExcelInfo = new()
            {
                Workbook = WorkbookFactory.Create(excelPath),
                Process = Process.Start(excelPath),
                File = excelPath
            };
            
            self.Out = openExcelInfo;
            self.Value = true;
            return self;
        }
    }
    /// <summary>
    /// excel操作对象
    /// </summary>
    public class OpenExcelInfo
    {
        public string File { get; set; }
        /// <summary>
        /// 工作表
        /// </summary>
        public IWorkbook Workbook { get; set; }
        /// <summary>
        /// 处理程序
        /// </summary>
        public Process Process { get; set; }
    }
}
