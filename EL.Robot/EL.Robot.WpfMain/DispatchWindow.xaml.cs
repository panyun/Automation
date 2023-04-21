using EL.Robot.WpfMain.Controls;
using Robot.Controls.Dispatch;
using Robot.Test;
using Robot.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Robot
{
    /// <summary>
    /// DispatchWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DispatchWindow : Window
    {
        public DispatchWindow()
        {
            InitializeComponent();
            ScrollViewerScrolling.UseTheScrollViewerScrolling(grid);
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            WindowManager.Close<DispatchWindow>();
        }

        private void Button_Click_MaxWindow(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
            else if (this.WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Maximized;
            }
        }

        private void Button_Click_MinWindow(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
    }
}
/*namespace EL.Robot.WpfMain.ViewModel
{
    public class DispatchViewModel : PropertyChangeBase
    {
        private ContentControl contentControl;

        public DispatchViewModelNew()
        {
            ItemMenus = new List<ItemMenu>() {
                new ItemMenu{
                    Header="我的脚本" ,
                    Type=0,
                    Icon=new BitmapImage(new Uri("pack://application:,,,/Robot;component/Resources/Images/script.png")),
                    SubItems = new TestData().Data1
                },
                new ItemMenu{
                    Header="运行中的"  ,
                    Icon=new BitmapImage(new Uri("pack://application:,,,/Robot;component/Resources/Images/running.png")),
                    Type=1,
                    SubItems = new TestData().Data2
                },
                new ItemMenu{
                    Header="排队中的"  ,
                    Icon=new BitmapImage(new Uri("pack://application:,,,/Robot;component/Resources/Images/running.png")),
                    Type=2,
                      SubItems = new TestData().Data3
                },
                new ItemMenu{ Header="历史运行的"  ,
                    Icon=new BitmapImage(new Uri("pack://application:,,,/Robot;component/Resources/Images/running.png")),
                    Type=3
                },
            };

            Historys = new TestData().Data4;
            SelectItemChangedCommand = new RelayCommand() { ExecCmd = ExecSelectedCmd };
        }

        private void ExecSelectedCmd(object o)
        {
            if (o is Scrpit script)
            {
                if (script.Type == 0)
                    ContentControl = new MyScript() { DataContext = script };
            }
            if (o is ItemMenu menu)
            {
                if (menu.Type == 1)
                    ContentControl = new Running() { DataContext = menu.SubItems };
                if (menu.Type == 2)
                    ContentControl = new LineUp() { DataContext = menu.SubItems };
                if (menu.Type == 3)
                    ContentControl = new History() { DataContext = Historys };
            }
        }

        public List<HistoryItem> Historys { get; set; }

        public List<ItemMenu> ItemMenus { get; set; }


        public ICommand SelectItemChangedCommand { get; set; }

        public ContentControl ContentControl
        {
            get => contentControl; set
            {
                contentControl = value;
                NC();
            }
        }
    }

    public class RelayCommand : ICommand
    {
        public Action<object> ExecCmd;
        public event EventHandler CanExecuteChanged;
        public bool CanExecute(object parameter)
        {
            return true;
        }
        public void Execute(object parameter)
        {
            ExecCmd?.Invoke(parameter);
        }
    }

    public class ItemMenu : PropertyChangeBase
    {
        public int Type { get; set; }
        public ImageSource Icon { get; set; }
        public string Header { get; set; }
        public List<Scrpit> SubItems { get; set; } = new List<Scrpit>();
    }

    public class HistoryItem : PropertyChangeBase
    {
        public string ScriptName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string State { get; set; }
    }
}*/
