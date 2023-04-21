using RemoteDesktopManage.Common;
using RemoteDesktopManage.Model;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Windows.Input;

namespace RemoteDesktopManage.ViewModel
{

    public class MainViewModel : MvvmNotifyPropertyChanged
    {
        public MainViewModel()
        {
            var config = ConfigInfo.GetInfos();
            ItemsSource = new ObservableCollection<RDPInfo>(config.Select(t => new RDPInfo(t)));
        }
        public ObservableCollection<RDPInfo> ItemsSource { get; set; }
        public void Load()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (var item in ItemsSource)
                {
                    if (!item.Start)
                    {
                        item.Conn();
                    }
                }
            });
        }
        public void Close(RDPInfo rDPInfo)
        {
            ItemsSource.Remove(rDPInfo);
        }
        public void Add(RDPInfo rDPInfo)
        {
            ItemsSource.Add(rDPInfo);
        }
        private MvvmCommand _CreateCommand;
        public ICommand CreateCommand => _CreateCommand ?? new MvvmCommand(Create);
        public void Create()
        {

        }
        private MvvmCommand<object> _ViewCommand;
        public ICommand ViewCommand => _ViewCommand ?? new MvvmCommand<object>(View);
        public void View(object i)
        {

        }
        public bool Any => ItemsSource.Any();
        public bool NotAny => !ItemsSource.Any();
    }
    public class RDPInfo : MvvmNotifyPropertyChanged
    {
        public static object Lock = new object();
        public RDPInfo(RDPConfig rDPConfig)
        {
            this.Header = rDPConfig.Title;
            this.Server = rDPConfig.Server;
            this.UserName = rDPConfig.UserName;
            this.RDPKind = RDPKind.NoConn;
            RDPView = new RDPView(rDPConfig);
            RDPView.ConnChange = (RDPKind) =>
            {
                this.RDPKind = RDPKind;
                Log($"Name : {this.Header} RDPKind : {RDPKind.ToDescription()}");
            };
            EnableInfo = "启动";
        }
        public RDPView RDPView { get; set; }
        public bool Start { get { return RDPView.Start; } }
        public string Header { get; set; }
        public string Server { get; set; }
        public string UserName { get; set; }
        public void Conn()
        {
            RDPView.Conn();
        }
        private RDPKind _RDPKind;
        public RDPKind RDPKind
        {
            get => _RDPKind;
            set
            {
                SetProperty(ref _RDPKind, value);
                RDPState = value.ToDescription();
            }
        }
        private string _EnableInfo;
        public string EnableInfo
        {
            get => _EnableInfo;
            set => SetProperty(ref _EnableInfo, value);
        }
        private string _RDPState;
        public string RDPState
        {
            get => _RDPState;
            set => SetProperty(ref _RDPState, value);
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
        private MvvmCommand _ViewCommand;
        public ICommand ViewCommand => _ViewCommand ?? new MvvmCommand(View);
        public void View()
        {
            RDPView.Show();
        }
        private MvvmCommand _EditCommand;
        public ICommand EditCommand => _EditCommand ?? new MvvmCommand(Edit);
        public void Edit()
        {

        }
        private MvvmCommand _DeleteCommand;
        public ICommand DeleteCommand => _DeleteCommand ?? new MvvmCommand(Delete);
        public void Delete()
        {

        }
        static void Log(string info)
        {
            lock (Lock)
            {
                File.AppendAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt"), DateTime.Now + "   " + info + Environment.NewLine);
            }
        }
    }
}
