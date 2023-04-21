using Automation.Inspect;
using EL.Robot.Component;
using EL.Robot.Core.SqliteEntity;
using EL.Sqlite;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL.Robot.Core
{
    /// <summary>
    ///  定时器组件
    /// </summary>
    public class DispatchComponent : Entity
    {
        private readonly ConcurrentDictionary<string, TaskProcess> Queue = new();
        public void AddJob(string name, string cron, Action action)
        {
            var process = new TaskProcess(cron, name, action);
            Queue.TryAdd(name, process);
            process.Start();
        }
        public bool RemoveJob(string name)
        {
            if (Queue.TryRemove(name, out var taskProcess))
            {
                return taskProcess.Close();
            }
            return true;
        }

        /// <summary>
        /// 任务处理
        /// </summary>
        public class TaskProcess
        {
            /// <summary>
            /// 任务调度器(单例)
            /// </summary>
            static IScheduler scheduler = null;
            /// <summary>
            /// 对象锁
            /// </summary>
            static object LockObj = new object();
            /// <summary>
            ///  内置唯一Key
            /// </summary>
            string JobKey = Guid.NewGuid().ToString("N");
            /// <summary>
            /// 内置唯一Key
            /// </summary>
            string TriggerKey = Guid.NewGuid().ToString("N");
            /// <summary>
            /// cron表达式
            /// </summary>
            public string cron { get; set; }
            /// <summary>
            /// 对象类型
            /// </summary>
            public string TypeName { get; set; }
            /// <summary>
            /// 触发的回调
            /// </summary>
            public Action Action { get; set; }

            /// <summary>
            /// 获取调度器
            /// </summary>
            /// <returns></returns>
            IScheduler GetScheduler()
            {
                if (scheduler == null)
                {
                    lock (LockObj)
                    {
                        if (scheduler == null)
                        {
                            StdSchedulerFactory factory = new StdSchedulerFactory();
                            scheduler = factory.GetScheduler().GetAwaiter().GetResult();
                            scheduler.Start();
                        }
                    }
                }
                return scheduler;
            }
            public TaskProcess(string Cron, string TypeName, Action action)
            {
                this.Action = action;
                this.cron = Cron;
                this.TypeName = TypeName;
            }
            public void Start()
            {
                AddJob(cron, TypeName).GetAwaiter().GetResult();
            }
            /// <summary>
            /// 增加任务
            /// </summary>
            /// <returns></returns>
            public async Task AddJob(string cron, string typeName)
            {
                var IScheduler = GetScheduler();
                IJobDetail jobDetail = JobBuilder.Create<TaskJob>()
                .WithIdentity(JobKey)
                .Build();
                jobDetail.JobDataMap.Put("Type", typeName);
                jobDetail.JobDataMap.Put("Action", Action);
                ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity(TriggerKey)
                .WithCronSchedule(cron)
                .ForJob(jobDetail)
                .Build();

                await IScheduler.ScheduleJob(jobDetail, trigger);
            }
            /// <summary>
            /// 关闭任务
            /// </summary>
            public bool Close()
            {
                var IScheduler = GetScheduler();
                TriggerKey triggerKey = new TriggerKey(TriggerKey);
                IScheduler.PauseTrigger(triggerKey);
                var unschedule = IScheduler.UnscheduleJob(triggerKey).GetAwaiter().GetResult();
                var result = IScheduler.DeleteJob(new JobKey(JobKey)).GetAwaiter().GetResult();
                if (unschedule || result)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        /// <summary>
        /// 任务
        /// </summary>
        public class TaskJob : IJob
        {
            public Task Execute(IJobExecutionContext context)
            {
                string type = string.Empty;
                Action Action = null;
                JobDataMap data = context.JobDetail.JobDataMap;
                if (data.ContainsKey("Type"))
                {
                    type = data["Type"].ToString();
                }
                if (data.ContainsKey("Action"))
                {
                    Action = data["Action"] as Action;
                }
                return Task.Factory.StartNew(() =>
                {
                    Action?.Invoke();
                });
            }
        }
    }

}