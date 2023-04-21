using EL.Robot.Core.SqliteEntity;
using EL.Robot.WpfMain.Command;
using EL.Robot.WpfMain.Common;
using EL.Robot.WpfMain.DispatchWindows.Model;
using EL.Robot.WpfMain.Utils;
using EL.Robot.WpfMain.ViewModel;
using Robot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace EL.Robot.WpfMain.DispatchWindows.ViewModel
{
    /// <summary>
    /// 计划列表
    /// </summary>
    public class PlanViewModel : MvvmNotifyPropertyChanged
    {
        private PlanData planData = null;
        private Action callback = null;
        public PlanViewModel(PlanKind PlanKind = PlanKind.Create, long flowId = 0, long planId = 0, Action callback = null)
        {
            this.callback = callback;
            this.PlanKind = PlanKind;
            PlanTitle = this.PlanKind == PlanKind.Create ? "新建计划" : "编辑计划";
            //天
            for (int i = 1; i < 32; i++)
            {
                DaysInfo.Add(i.ToString().PadLeft(2, '0'));
            }
            //小时
            for (int i = 0; i < 24; i++)
            {
                HoursInfo.Add(i.ToString().PadLeft(2, '0'));
            }
            //分钟
            for (int i = 0; i < 60; i++)
            {
                MinuteInfo.Add(i.ToString().PadLeft(2, '0'));
            }
            //秒
            for (int i = 0; i < 60; i++)
            {
                SecondInfo.Add(i.ToString().PadLeft(2, '0'));
            }
            WeekInfos = new WeekInfos(this);
            this.Plans = RobotDataManagerService.GetFlowSummarys();

            if (this.Plans == null)
            {
                this.Plans = new List<FlowSummary>();
            }
            if (this.PlanKind == PlanKind.Create)
            {
                PlanChange = true;
                FlowSummary FlowSummary = null;
                if (flowId > 0)
                {
                    FlowSummary = this.Plans.Where(t => t.Id == flowId).FirstOrDefault();
                }
                else
                {
                    FlowSummary = this.Plans.FirstOrDefault();
                }
                if (FlowSummary == null)
                {
                    FlowSummary = new FlowSummary();
                }
                this.SelectPlanName = FlowSummary;
                this.SelectKind = SelectKind.Day;

                //默认值
                SelectDaysInfo = DaysInfo.First();
                SelectHoursInfo = HoursInfo.First();
                SelectMinuteInfo = MinuteInfo.First();
                SelectSecondInfo = SecondInfo.First();
            }
            else //如果是修改
            {
                PlanChange = false;
                planData = RobotDataManagerService.GetPlanDataId(planId);
                if (planData != null)
                {
                    var list = this.Plans.Where(t => t.Id == planData.FlowId).FirstOrDefault();
                    if (list != null)
                    {
                        this.SelectPlanName = list;
                    }

                    SetPlanDataModel(planData.Expression.ToObj<PlanDataModel>());
                }
                else
                {
                    MessageBox.Show("获取不到计划ID，此计划无效，请重新打开！");
                }
            }
        }
        public List<FlowSummary> Plans { get; set; }
        private FlowSummary _SelectPlanName;
        public FlowSummary SelectPlanName
        {
            get => _SelectPlanName;
            set => SetProperty(ref _SelectPlanName, value);
        }
        public PlanKind PlanKind { get; set; }
        public string PlanTitle { get; set; }
        private SelectKind _SelectKind;
        public SelectKind SelectKind
        {
            get => _SelectKind;
            set
            {
                SetProperty(ref _SelectKind, value);
                if (value == SelectKind.Week)
                {
                    WeekInfo = true;
                }
                else
                {
                    WeekInfo = false;
                }
                if (value == SelectKind.Month)
                {
                    MonthInfo = true;
                }
                else
                {
                    MonthInfo = false;
                }
                ChangeJobInfo();
            }
        }
        private bool _PlanChange;
        public bool PlanChange
        {
            get => _PlanChange;
            set => SetProperty(ref _PlanChange, value);
        }
        private bool _WeekInfo;
        public bool WeekInfo
        {
            get => _WeekInfo;
            set => SetProperty(ref _WeekInfo, value);
        }
        private bool _MonthInfo;
        public bool MonthInfo
        {
            get => _MonthInfo;
            set => SetProperty(ref _MonthInfo, value);
        }
        private WeekInfos _WeekInfos;
        public WeekInfos WeekInfos
        {
            get => _WeekInfos;
            set => SetProperty(ref _WeekInfos, value);
        }
        public List<string> DaysInfo { get; set; } = new List<string>();
        public List<string> HoursInfo { get; set; } = new List<string>();
        public List<string> MinuteInfo { get; set; } = new List<string>();
        public List<string> SecondInfo { get; set; } = new List<string>();
        private string _SelectDaysInfo;
        public string SelectDaysInfo
        {
            get => _SelectDaysInfo;
            set
            {
                SetProperty(ref _SelectDaysInfo, value);
                ChangeJobInfo();
            }
        }
        private string _SelectHoursInfo;
        public string SelectHoursInfo
        {
            get => _SelectHoursInfo;
            set
            {
                SetProperty(ref _SelectHoursInfo, value);
                ChangeJobInfo();
            }
        }
        private string _SelectMinuteInfo;
        public string SelectMinuteInfo
        {
            get => _SelectMinuteInfo;
            set
            {
                SetProperty(ref _SelectMinuteInfo, value);
                ChangeJobInfo();
            }
        }
        private string _SelectSecondInfo;
        public string SelectSecondInfo
        {
            get => _SelectSecondInfo;
            set
            {
                SetProperty(ref _SelectSecondInfo, value);
                ChangeJobInfo();
            }
        }
        private string _PlanTip;
        public string PlanTip
        {
            get => _PlanTip;
            set => SetProperty(ref _PlanTip, value);
        }
        public void ChangeJobInfo()
        {
            switch (SelectKind)
            {
                case SelectKind.Day:
                    PlanTip = $"此计划会在每天的 {SelectHoursInfo} : {SelectMinuteInfo} : {SelectSecondInfo} 重复";
                    break;
                case SelectKind.Week:
                    PlanTip = $"此计划会在{string.Join("、", WeekInfos.GetSelectWeekInfo())}，{SelectHoursInfo} : {SelectMinuteInfo} : {SelectSecondInfo} 重复";
                    break;
                case SelectKind.Month:
                    PlanTip = $"此计划会在每月 {SelectDaysInfo} 日 {SelectHoursInfo} : {SelectMinuteInfo} : {SelectSecondInfo} 重复";
                    break;
            }
        }
        private MvvmCommand _CancelCommand;
        public ICommand CancelCommand => _CancelCommand ?? new MvvmCommand(Cancel);
        public void Cancel()
        {
            WindowManager.Close<Plan>();
        }
        private MvvmCommand _SaveCommand;
        public ICommand SaveCommand => _SaveCommand ?? new MvvmCommand(Save);
        public void Save()
        {
            if (Plans.Count == 0 || SelectPlanName == null || SelectPlanName.Name == null || SelectPlanName.Id == 0)
            {
                MessageBox.Show("没有选择脚本，无法创建!");
                return;
            }
            if (SelectKind == SelectKind.Week)
            {
                if (!WeekInfos.GetWeeks().Any(t => t))
                {
                    MessageBox.Show("没有选择具体的周，无法创建!");
                    return;
                }
            }

            var plan = GetPlanData(planData);
            if (this.PlanKind == PlanKind.Create)
            {
                var plainId = RobotDataManagerService.AddPlanData(plan);
                DispatchTaskManage.AddJob(plainId);
                DispatchViewModel.Instance?.Update();
                this.callback?.Invoke();
            }
            else
            {
                RobotDataManagerService.UpdatePlanData(plan);
                this.callback?.Invoke();
            }
            WindowManager.Close<Plan>();
        }
        public PlanData GetPlanData(PlanData planData = null)
        {
            if (planData == null)
            {
                planData = new PlanData();
                planData.Enable = 1;
                planData.FlowId = SelectPlanName.Id;
                planData.FlowName = SelectPlanName.Name;
            }
            planData.Update();
            planData.Expression = GetPlanDataModel().ToJson();
            return planData;
        }
        public PlanDataModel GetPlanDataModel()
        {
            return new PlanDataModel()
            {
                SelectKind = SelectKind,
                WeekInfos = WeekInfos,
                SelectDaysInfo = SelectDaysInfo,
                SelectHoursInfo = SelectHoursInfo,
                SelectMinuteInfo = SelectMinuteInfo,
                SelectSecondInfo = SelectSecondInfo
            };
        }
        public void SetPlanDataModel(PlanDataModel planDataModel)
        {
            this.SelectKind = planDataModel.SelectKind;
            this.WeekInfos.SetValues(planDataModel.WeekInfos);
            this.SelectDaysInfo = planDataModel.SelectDaysInfo;
            this.SelectHoursInfo = planDataModel.SelectHoursInfo;
            this.SelectMinuteInfo = planDataModel.SelectMinuteInfo;
            this.SelectSecondInfo = planDataModel.SelectSecondInfo;
        }
    }
    /// <summary>
    /// 每周的信息
    /// </summary>
    public class WeekInfos : MvvmNotifyPropertyChanged
    {
        private PlanViewModel planViewModel;
        public WeekInfos(PlanViewModel planViewModel)
        {
            this.planViewModel = planViewModel;
        }
        private bool _Week1;
        public bool Week1
        {
            get => _Week1;
            set
            {
                SetProperty(ref _Week1, value);
                Change();
            }
        }
        private bool _Week2;
        public bool Week2
        {
            get => _Week2;
            set
            {
                SetProperty(ref _Week2, value);
                Change();
            }
        }
        private bool _Week3;
        public bool Week3
        {
            get => _Week3;
            set
            {
                SetProperty(ref _Week3, value);
                Change();
            }
        }
        private bool _Week4;
        public bool Week4
        {
            get => _Week4;
            set
            {
                SetProperty(ref _Week4, value);
                Change();
            }
        }
        private bool _Week5;
        public bool Week5
        {
            get => _Week5;
            set
            {
                SetProperty(ref _Week5, value);
                Change();
            }
        }
        private bool _Week6;
        public bool Week6
        {
            get => _Week6;
            set
            {
                SetProperty(ref _Week6, value);
                Change();
            }
        }
        private bool _Week7;
        public bool Week7
        {
            get => _Week7;
            set
            {
                SetProperty(ref _Week7, value);
                Change();
            }
        }
        private void Change()
        {
            planViewModel?.ChangeJobInfo();
        }
        public List<int> GetInts()
        {
            var listinfo = new List<int>();
            var list = GetWeeks();
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i])
                {
                    var week = i + 1;
                    switch (week)
                    {
                        case 1:
                            listinfo.Add(1);
                            break;
                        case 2:
                            listinfo.Add(2);
                            break;
                        case 3:
                            listinfo.Add(3);
                            break;
                        case 4:
                            listinfo.Add(4);
                            break;
                        case 5:
                            listinfo.Add(5);
                            break;
                        case 6:
                            listinfo.Add(6);
                            break;
                        case 7:
                            listinfo.Add(7);
                            break;
                    }
                }
            }
            return listinfo;
        }
        public List<bool> GetWeeks()
        {
            return new List<bool>() { Week1, Week2, Week3, Week4, Week5, Week6, Week7 };
        }
        public void SetValues(WeekInfos WeekInfos)
        {
            Week1 = WeekInfos.Week1;
            Week2 = WeekInfos.Week2;
            Week3 = WeekInfos.Week3;
            Week4 = WeekInfos.Week4;
            Week5 = WeekInfos.Week5;
            Week6 = WeekInfos.Week6;
            Week7 = WeekInfos.Week7;
        }
        public List<string> GetSelectWeekInfo()
        {
            var listinfo = new List<string>();
            var list = GetWeeks();
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i])
                {
                    var week = i + 1;
                    switch (week)
                    {
                        case 1:
                            listinfo.Add("周一");
                            break;
                        case 2:
                            listinfo.Add("周二");
                            break;
                        case 3:
                            listinfo.Add("周三");
                            break;
                        case 4:
                            listinfo.Add("周四");
                            break;
                        case 5:
                            listinfo.Add("周五");
                            break;
                        case 6:
                            listinfo.Add("周六");
                            break;
                        case 7:
                            listinfo.Add("周日");
                            break;
                    }
                }
            }
            return listinfo;
        }
    }
    public enum PlanKind
    {
        Create,
        Edit
    }
    public enum SelectKind
    {
        [Description("每天")]
        Day = 0,
        [Description("每周")]
        Week,
        [Description("每月")]
        Month
    }
    public class SelectKindConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            SelectKind s = (SelectKind)value;
            return s == (SelectKind)int.Parse(parameter.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool isChecked = (bool)value;
            if (!isChecked)
            {
                return null;
            }
            return (SelectKind)int.Parse(parameter.ToString());
        }
    }
}
