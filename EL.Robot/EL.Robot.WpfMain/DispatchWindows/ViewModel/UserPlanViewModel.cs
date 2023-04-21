using CefSharp;
using EL.Robot.Core;
using EL.Robot.Core.SqliteEntity;
using EL.Robot.WpfMain.Command;
using EL.Robot.WpfMain.Common;
using EL.Robot.WpfMain.DispatchWindows;
using EL.Robot.WpfMain.DispatchWindows.Model;
using EL.Robot.WpfMain.DispatchWindows.ViewModel;
using EL.Robot.WpfMain.Utils;
using Robot;
using Robot.Test;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Windows.Controls;
using System.Windows.Input;
using Plan = EL.Robot.WpfMain.DispatchWindows.Plan;

namespace EL.Robot.WpfMain.ViewModel
{
    public class UserPlanViewModel : MvvmNotifyPropertyChanged
    {

        public UserPlanViewModel(long flowId)
        {
            Lists = new ObservableCollection<PlanDataInfo>();
            FlowId = flowId;
            Update();
        }
        public void Update()
        {
            var planDatas = RobotDataManagerService.GetPlanDataFlowId(FlowId);
            JobName = planDatas.First().FlowName;
            Lists.Clear();
            foreach (var item in planDatas.Select(t => new PlanDataInfo(t, this)).ToList())
            {
                Lists.Add(item);
            }
            JobCount = Lists.Count();
        }
        public ObservableCollection<PlanDataInfo> Lists { get; set; }
        public string JobName { get; set; }
        public long FlowId { get; set; }
        private int _JobCount;
        public int JobCount
        {
            get => _JobCount;
            set => SetProperty(ref _JobCount, value);
        }
        private MvvmCommand _CreateCommand;
        public ICommand CreateCommand => _CreateCommand ?? new MvvmCommand(Create);
        public void Create()
        {
            WindowManager.Show<Plan>(new PlanViewModel(flowId: FlowId, callback: () =>
            {
                Update();
            }));
        }
        public void Delete(PlanDataInfo planDataInfo)
        {
            RobotDataManagerService.DeletePlanData(planDataInfo.PlanId);
            DispatchTaskManage.RemoveJob(planDataInfo.PlanId);
            Lists.Remove(planDataInfo);
            JobCount = Lists.Count();
            DispatchViewModel.Instance?.Update();
        }
    }
    public class PlanDataInfo : MvvmNotifyPropertyChanged
    {
        private PlanData planData;
        private UserPlanViewModel userPlanViewModel;
        public PlanDataInfo(PlanData planData, UserPlanViewModel userPlanViewModel)
        {
            this.userPlanViewModel = userPlanViewModel;
            this.planData = planData;
            this.MenuShow = false;
            PlanId = this.planData.Id;
            Update();
        }
        public long PlanId { get; private set; }
        private string _EnableInfo;
        public string EnableInfo
        {
            get => _EnableInfo;
            set => SetProperty(ref _EnableInfo, value);
        }
        private string _EnableMenuInfo;
        public string EnableMenuInfo
        {
            get => _EnableMenuInfo;
            set => SetProperty(ref _EnableMenuInfo, value);
        }
        private bool _MenuShow;
        public bool MenuShow
        {
            get => _MenuShow;
            set => SetProperty(ref _MenuShow, value);
        }
        private MvvmCommand _MenuClickCommand;
        public ICommand MenuClickCommand => _MenuClickCommand ?? new MvvmCommand(MenuClick);
        public void MenuClick()
        {
            MenuShow = true;
        }
        private MvvmCommand _EnableCommand;
        public ICommand EnableCommand => _EnableCommand ?? new MvvmCommand(IsEnable);
        public void IsEnable()
        {
            var data = planData.Enable == 1 ? 0 : 1;
            var result = RobotDataManagerService.EnablePlanData(planData.Id, data);
            if (result > 0)
            {
                if (data == 1)
                {
                    DispatchTaskManage.AddJob(this.planData);
                }
                else
                {
                    DispatchTaskManage.RemoveJob(this.planData);
                }
                planData.Enable = (short)data;
                Update();
            }
        }
        private MvvmCommand _EditCommand;
        public ICommand EditCommand => _EditCommand ?? new MvvmCommand(Edit);
        public void Edit()
        {
            WindowManager.Show<Plan>(new PlanViewModel(PlanKind.Edit, planId: planData.Id, callback: () =>
            {
                var plan = RobotDataManagerService.GetPlanDataId(planData.Id);
                DispatchTaskManage.RemoveJob(plan);
                DispatchTaskManage.AddJob(plan);
                this.userPlanViewModel.Update();
            }));
        }
        private MvvmCommand _DeleteCommand;
        public ICommand DeleteCommand => _DeleteCommand ?? new MvvmCommand(Delete);
        public void Delete()
        {
            var rtn = System.Windows.Forms.MessageBox.Show($"您确定要删除{planData.FlowName}计划吗？", "删除", System.Windows.Forms.MessageBoxButtons.OKCancel, System.Windows.Forms.MessageBoxIcon.Information, System.Windows.Forms.MessageBoxDefaultButton.Button1, System.Windows.Forms.MessageBoxOptions.DefaultDesktopOnly);
            if (rtn == System.Windows.Forms.DialogResult.OK)
            {
                this.userPlanViewModel.Delete(this);
            }
        }
        private string _PlanInfo;
        public string PlanInfo
        {
            get => _PlanInfo;
            set => SetProperty(ref _PlanInfo, value);
        }
        private string _PlanDetailInfo;
        public string PlanDetailInfo
        {
            get => _PlanDetailInfo;
            set => SetProperty(ref _PlanDetailInfo, value);
        }
        private string _TakeTime;
        public string TakeTime
        {
            get => _TakeTime;
            set => SetProperty(ref _TakeTime, value);
        }

        public DateTime GetRunDateTime(PlanDataModel planData)
        {
            //返回cron表达式
            var cron = planData.GetCronExpression();
            return CronHelper.GetNextTime(cron).First();
        }

        public string GetRunDateTimeInfo(DateTime dateTime)
        {
            var time = (dateTime - DateTime.Now);
            if ((int)time.TotalDays > 0)
            {
                return $"{(int)time.TotalDays}天";
            }
            if ((int)time.TotalMinutes > 0)
            {
                return $"{time.Hours}小时{time.Minutes}分钟";
            }
            else
            {
                return $"{time.Seconds} 秒";
            }
        }
        private void Update()
        {
            var plan = planData.Expression.ToObj<PlanDataModel>();
            PlanInfo = $"{plan.SelectHoursInfo}:{plan.SelectMinuteInfo}:{plan.SelectSecondInfo} {plan.SelectKind.ToDescription()}";
            var time = GetRunDateTime(plan);
            PlanDetailInfo = $"预计下次{time.ToString("yyyy-MM-dd HH:mm:ss")}执行，还有";
            TakeTime = GetRunDateTimeInfo(time);
            if (planData.Enable == 1)
            {
                EnableInfo = "启用";
                EnableMenuInfo = "停用";
            }
            else
            {
                EnableInfo = "停用";
                EnableMenuInfo = "启用";
            }
        }
    }
}
