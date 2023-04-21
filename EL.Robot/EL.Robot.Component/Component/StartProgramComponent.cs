using EL.Async;
using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace EL.Robot.Component
{

    public class StartOrCmdProgramComponent : BaseComponent
    {

        public ELTask<bool> WaitEvaluation = ELTask<bool>.Create();
        public override async ELTask<INodeContent> Main(INodeContent self)
        {
            await base.Main(self);
            if (self.CurrentNode.GetParamterValue("SelectStartType") + "" == "运行Windows命令")
            {
                var cmd = self.CurrentNode.GetParamterValue("cmd") + "";
                if (!string.IsNullOrEmpty(cmd))
                {
                    RunCmd(cmd);
                    self.Value = true;
                    return self;
                }
                throw new ELNodeHandlerException("未找到命令行参数和执行路径！");
            }

            var filePath = self.CurrentNode.GetParamterValue("filePath") + "";
            if (!string.IsNullOrEmpty(filePath))
            {
                var isPath = CheckPath(filePath);
                if (!isPath) throw new ELNodeHandlerException("文件路径不正确！");
                var id = Process.Start(filePath).Id;
                self.Out = id;
                self.Value = true;
            }
            return self;
        }
        /// <summary>
        /// 文件路径验证
        /// </summary>
        /// <remarks>
        /// 创建人：zhujt<br/>
        /// 创建日期：2012-08-24 21:57:07
        /// </remarks>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool CheckPath(string path)
        {
            string pattern = @"^[a-zA-Z]:(((\\(?! )[^/:*?<>\""|\\]+)+\\?)|(\\)?)\s*$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(path);
        }
        private static string CmdPath = @"C:\Windows\System32\cmd.exe";
        /// <summary>
        /// 执行cmd命令 返回cmd窗口显示的信息
        /// 多命令请使用批处理命令连接符：
        /// <![CDATA[
        /// &:同时执行两个命令
        /// |:将上一个命令的输出,作为下一个命令的输入
        /// &&：当&&前的命令成功时,才执行&&后的命令
        /// ||：当||前的命令失败时,才执行||后的命令]]>
        /// </summary>
        ///<param name="cmd">执行的命令</param>
        public static void RunCmd(string cmd)
        {

            if (cmd.ToLower() =="cmd")
            {
                Process p = new Process();
                p.StartInfo.FileName = CmdPath;
                //p.StartInfo.UseShellExecute = false; //是否使用操作系统shell启动
                //p.StartInfo.RedirectStandardInput = true; //接受来自调用程序的输入信息
                //p.StartInfo.RedirectStandardOutput = true; //由调用程序获取输出信息
                //p.StartInfo.RedirectStandardError = true; //重定向标准错误输出
                //p.StartInfo.CreateNoWindow = true; //不显示程序窗口
                p.Start();
                return;

            }

            cmd = cmd.Trim().TrimEnd('&') + "&exit";//说明：不管命令是否成功均执行exit命令，否则当调用ReadToEnd()方法时，会处于假死状态
            using (Process p = new Process())
            {
                p.StartInfo.FileName = CmdPath;
                p.StartInfo.UseShellExecute = false; //是否使用操作系统shell启动
                p.StartInfo.RedirectStandardInput = true; //接受来自调用程序的输入信息
                p.StartInfo.RedirectStandardOutput = true; //由调用程序获取输出信息
                p.StartInfo.RedirectStandardError = true; //重定向标准错误输出
                p.StartInfo.CreateNoWindow = true; //不显示程序窗口
                p.Start();//启动程序
                //向cmd窗口写入命令
                p.StandardInput.WriteLine(cmd);
                p.StandardInput.AutoFlush = true;
                //获取cmd窗口的输出信息
                //string output = p.StandardOutput.ReadToEnd();
                //p.WaitForExit();//等待程序执行完退出进程
                p.Close();
                //return output;
            }

        }
    }
}

