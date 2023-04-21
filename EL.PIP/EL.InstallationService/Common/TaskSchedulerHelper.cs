using Microsoft.Win32.TaskScheduler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL.InstallationService
{
    /// <summary>
    /// 任务计划程序库
    /// </summary>
    public static class TaskSchedulerHelper
    {
        public static void AddTaskScheduler(string exePath, string args = "", string description = "")
        {
            TaskService.Instance.AddTask(GetTaskSchedulerPath(), new LogonTrigger() { UserId = $@"{Environment.MachineName}\{Environment.UserName}" }, new ExecAction(exePath, args), userId: Environment.UserName, logonType: TaskLogonType.InteractiveToken, description: description);
        }
        public static void RemoveTaskScheduler()
        {
            var currentTask = TaskService.Instance.GetTask(GetTaskSchedulerPath());
            if (currentTask != null)
            {
                TaskService.Instance.GetFolder(Path.GetDirectoryName(currentTask.Path)).DeleteTask(currentTask.Name, false);
            }
        }
        public static string GetTaskSchedulerPath()
        {
            return $"PIPServer-{Environment.UserName}";
        }
    }
}
