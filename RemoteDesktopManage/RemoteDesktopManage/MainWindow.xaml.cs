using RemoteDesktopManage.Model;
using RemoteDesktopManage.ViewModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RemoteDesktopManage
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel mainViewModel;
        public MainWindow()
        {
            InitializeComponent();
            UseTheScrollViewerScrolling(grid);
            mainViewModel = new MainViewModel();
            this.DataContext = mainViewModel;
            this.Loaded += MainWindow_Loaded;

        }
        public static void UseTheScrollViewerScrolling(FrameworkElement fElement)
        {
            fElement.PreviewMouseWheel += (sender, e) =>
            {
                var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
                eventArg.RoutedEvent = UIElement.MouseWheelEvent;
                eventArg.Source = sender;
                fElement.RaiseEvent(eventArg);
            };
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            mainViewModel?.Load();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Manage_Click(object sender, RoutedEventArgs e)
        {
          
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //RdpHost.Child = mainViewModel.ItemsSource.First().Control.RDPClient;
        }

        private void ItemsControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
