using EL.Robot.Core;
using EL.Robot.Core.SqliteEntity;
using EL.Robot.WpfMain.Command;
using EL.Robot.WpfMain.DispatchWindows;
using EL.Robot.WpfMain.DispatchWindows.ViewModel;
using EL.Robot.WpfMain.Utils;
using Robot.Controls.Dispatch;
using Robot.Test;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Instrumentation;
using System.Security.Policy;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Input;

namespace EL.Robot.WpfMain.ViewModel
{
    public class DispatchViewModel : MvvmNotifyPropertyChanged
    {
        public static DispatchViewModel Instance;
        public DispatchViewModel()
        {
            Instance = this;
            ItemMenus = new ObservableCollection<MenuItemsInfo>();
            Update();
        }
        public ObservableCollection<MenuItemsInfo> ItemMenus { get; set; }

        private ContentControl _ContentControl;
        public ContentControl ContentControl
        {
            get => _ContentControl;
            set => SetProperty(ref _ContentControl, value);
        }
        private MvvmCommand<object> _SelectItemChangedCommand;
        public ICommand SelectItemChangedCommand => _SelectItemChangedCommand ?? new MvvmCommand<object>(SelectItemChanged);
        private int i = 0;
        public void SelectItemChanged(object sender)
        {
            if (sender is MenuItemsInfo menu)
            {
                CancelSelect();
                menu.Select = true;
                //如果主菜单没有其他子菜单是能点开的
                //if (menu.SubItems == null || menu.SubItems.Any() == false)
                {
                    this.ContentControl = GetContentControl(menu.Screen);
                    if (menu.DataContext != null)
                    {
                        this.ContentControl.DataContext = menu.DataContext;
                    }
                }
            }
            else if (sender is MenuItem menuItem)
            {
                CancelSelect();
                menuItem.Select = true;
                this.ContentControl = GetContentControl(menuItem.Screen);
                if (menuItem.DataContext != null)
                {
                    this.ContentControl.DataContext = menuItem.DataContext;
                }
            }
        }
        private void CancelSelect()
        {
            foreach (var item in ItemMenus)
            {
                item.Select = false;
                if (item.NotEmpty)
                {
                    foreach (var subItem in item.SubItems)
                    {
                        subItem.Select = false;
                    }
                }
            }
        }
        private ContentControl GetContentControl(Type objectType)
        {
            return (ContentControl)Activator.CreateInstance(objectType);
        }
        public void Update()
        {

            var myScriptCount = RobotDataManagerService.GetFlowSummarys()?.Count ?? 0;
            var flowHistorysLength = RobotDataManagerService.GetFlowHistorysLength();
            var flowQueues = RobotDataManagerService.GetFlowQueues();
            var flowQueuesLength = flowQueues?.Count ?? 0;
            var runing = RobotDataManagerService.GetFlowRuning();
            var runingLength = runing == null ? 0 : 1;
            string myScriptHeader = "我的脚本";
            string runingHeader = "运行中的";
            string queueHeader = "排队中的";
            string flowHistoryHeader = "历史运行中的";
            //获取所有计划
            var planDatas = RobotDataManagerService.GetPlanDataFlowId();
            //var item1 = new MenuItemsInfo(myScriptHeader, "/Resources/Images/yxz.png", typeof(MyScript));
            //var item2 = new MenuItemsInfo($"我的计划", "/Resources/Images/yxz.png", typeof(NewPlan), null, GetMenuItems(planDatas));
            //var item3 = new MenuItemsInfo(runingHeader, "/Resources/Images/yxz.png", typeof(Running));
            //var item4 = new MenuItemsInfo(queueHeader, "/Resources/Images/pd.png", typeof(LineUp));
            //var item5 = new MenuItemsInfo(flowHistoryHeader, "/Resources/Images/ls.png", typeof(History));
            var item1 = new MenuItemsInfo(myScriptCount, myScriptHeader, "/Resources/viewIcons/调度管理_我的脚本.png", typeof(MyScript));
            var item2 = new MenuItemsInfo(-1, "我的计划", "/Resources/viewIcons/调度管理_我的计划.png", typeof(NewPlan), null, GetMenuItems(planDatas));
            var item3 = new MenuItemsInfo(runingLength, runingHeader, "/Resources/viewIcons/调度管理_运行中的.png", typeof(Running));
            var item4 = new MenuItemsInfo(flowQueuesLength, queueHeader, "/Resources/viewIcons/调度管理_排队中的.png", typeof(LineUp));
            var item5 = new MenuItemsInfo(flowHistorysLength, flowHistoryHeader, "/Resources/viewIcons/调度管理_历史运行的.png", typeof(History));
            DispatcherHelper.ExecDispatcher(() =>
            {
                ItemMenus.Clear();
                ItemMenus.Add(item1);
                ItemMenus.Add(item2);
                ItemMenus.Add(item3);
                ItemMenus.Add(item4);
                ItemMenus.Add(item5);
            });

        }
        private List<MenuItem> GetMenuItems(List<PlanData> planDatas)
        {
            var items = new List<MenuItem>();
            if (planDatas != null)
            {
                foreach (var item in planDatas.GroupBy(t => t.FlowId))
                {
                    var flowId = item.Key;
                    var scriptName = item.First().FlowName;
                    var count = item != null ? item.Count() : 0;
                    items.Add(new MenuItem(count, scriptName, typeof(UserPlan), new UserPlanViewModel(flowId)));
                }
            }
            return items;
        }
    }
    public class MenuItemsInfo : MvvmNotifyPropertyChanged
    {
        public MenuItemsInfo(int count, string header, string icon, Type Screen, object DataContext = null, List<MenuItem> subItems = null)
        {
            Header = header;
            this.Count = count;
            if (count > 0)
            {
                ShowCount = true;
            }
            if (subItems != null)
            {
                SubItems = new ObservableCollection<MenuItem>(subItems);
            }
            else
            {
                SubItems = new ObservableCollection<MenuItem>();
            }
            Icon = icon;
            this.Screen = Screen;
            this.DataContext = DataContext;
        }
        public string Header { get; set; }
        public string Icon { get; set; }
        public ObservableCollection<MenuItem> SubItems { get; set; }
        public object DataContext { get; set; }
        public Type Screen { get; set; }
        public bool NotEmpty { get { return SubItems != null && SubItems.Count > 0; } }
        public int Count { get; set; }
        private bool _Select;
        public bool Select
        {
            get => _Select;
            set => SetProperty(ref _Select, value);
        }
        public bool ShowCount { get; set; }
    }
    public class MenuItem : MvvmNotifyPropertyChanged
    {
        public MenuItem(int count, string Header, Type Screen = null, object DataContext = null)
        {
            this.Count = count;
            //if (count > 0)
            //{
            //    ShowCount = true;
            //}
            this.Header = Header;
            this.Screen = Screen;
            this.DataContext = DataContext;
        }
        public int Count { get; set; }
        public bool ShowCount { get; set; }
        public string Header { get; set; }
        public Type Screen { get; set; }
        public object DataContext { get; set; }
        public ObservableCollection<MenuItem> Children { get; set; }
        private bool _Select;
        public bool Select
        {
            get => _Select;
            set => SetProperty(ref _Select, value);
        }
    }
}
