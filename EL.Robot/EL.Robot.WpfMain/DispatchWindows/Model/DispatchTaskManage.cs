using EL.Robot.Core.SqliteEntity;
using EL.Robot.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EL.Robot.WpfMain.Common;
using System.Windows.Threading;
using EL.Robot.Component;
using EL.Robot.Component.PIP;
using System.Threading;

namespace EL.Robot.WpfMain.DispatchWindows.Model
{
    public static class DispatchTaskManage
    {
        public static void AddJob()
        {
            var Dispatch = Boot.GetComponent<RobotComponent>().GetComponent<DispatchComponent>();
            var planDatas = RobotDataManagerService.GetAllEnablePlanData();
            if (planDatas != null)
            {
                foreach (var item in planDatas)
                {
                    var cron = item.Expression.ToObj<PlanDataModel>().GetCronExpression();
                    Dispatch.AddJob($"Plan-{item.Id}", cron, () =>
                    {
                        AddQueue(item.FlowId);
                    });
                }
            }
        }
        public static void AddJob(PlanData planData)
        {
            var Dispatch = Boot.GetComponent<RobotComponent>().GetComponent<DispatchComponent>();
            var cron = planData.Expression.ToObj<PlanDataModel>().GetCronExpression();
            Dispatch.AddJob($"Plan-{planData.Id}", cron, () =>
            {
                AddQueue(planData.FlowId);
            });
        }
        public static void AddJob(long planId)
        {
            var planData = RobotDataManagerService.GetPlanDataId(planId);
            if (planData?.Id == planId)
            {
                var Dispatch = Boot.GetComponent<RobotComponent>().GetComponent<DispatchComponent>();
                var cron = planData.Expression.ToObj<PlanDataModel>().GetCronExpression();
                Dispatch.AddJob($"Plan-{planData.Id}", cron, () =>
                {
                    AddQueue(planData.FlowId);
                });
            }
        }
        public static void RemoveJob(PlanData planData)
        {
            var Dispatch = Boot.GetComponent<RobotComponent>().GetComponent<DispatchComponent>();
            Dispatch.RemoveJob($"Plan-{planData.Id}");
        }
        public static void RemoveJob(long Id)
        {
            var Dispatch = Boot.GetComponent<RobotComponent>().GetComponent<DispatchComponent>();
            Dispatch.RemoveJob($"Plan-{Id}");
        }
        public static void AddQueue(long FlowId)
        {
            var flow = RobotDataManagerService.GetFlowById(FlowId);
            var summary = RobotDataManagerService.GetFlowSummaryById(FlowId);
            if (summary.CheckPip())
            {
                Task.Run(async () =>
                {
                    var flow = new FlowScript() { Id = FlowId };
                    var pip = Boot.GetComponent<RobotComponent>().GetComponent<PIPServerComponent>();
                    var result = await pip.StartAsync();
                    if (result.Item1)
                    {
                        if (!pip.Ready)
                        {
                            SpinWait.SpinUntil(() => pip.Ready, 60 * 1000);
                        }
                        if (pip.Ready)
                        {
                            pip.Send(flow);
                        }
                        else
                        {
                            System.Windows.MessageBox.Show($"服务连接超时!");
                        }
                    }
                    else
                    {
                        System.Windows.MessageBox.Show(result.Item2);
                    }
                });
            }
            else
            {
                Boot.GetComponent<RobotComponent>().AddLocalFlowQueue(flow);
            }
        }
    }
}
