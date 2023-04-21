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
using WpfInspect.Core;
using WpfInspect.ViewModels;

namespace WpfInspect
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public readonly MainViewModel _vm;
        public MainWindow()
        {
            DispatcherHelper.Init();
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            _vm = new MainViewModel();
            _vm.Hide = () => { WindowState = WindowState.Minimized; };
            _vm.Show = () => { WindowState = WindowState.Normal; Topmost = true; Topmost = false; };
            DataContext = _vm;
            _vm.window = this;
            _vm.HighLightButton = HighLightButton;
        }

        private void MainWindow_Loaded(object sender, System.EventArgs e)
        {
            if (!_vm.IsInitialized)
            {
                _vm.Initialize();
                Loaded -= MainWindow_Loaded;
            }
        }

        private void TreeViewSelectedHandler(object sender, RoutedEventArgs e)
        {
            var item = sender as TreeViewItem;
            if (item != null)
            {
                item.BringIntoView();
                e.Handled = true;
            }
        }

        private void OnWindowInitialized(object sender, EventArgs e)
        {
            RefreshMaximizeRestoreButton();
        }

        private void OnMinimizeButtonClick(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private void OnMaximizeRestoreButtonClick(object sender, RoutedEventArgs e)
        {
            if (App.Current.MainWindow.WindowState == WindowState.Maximized)
            {
                App.Current.MainWindow.WindowState = WindowState.Normal;
            }
            else if (App.Current.MainWindow.WindowState == WindowState.Normal)
            {
                App.Current.MainWindow.WindowState = WindowState.Maximized;
            }
        }

        private void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void RefreshMaximizeRestoreButton()
        {
            if (WindowState == WindowState.Maximized)
            {
                MaximizeButton.Visibility = Visibility.Collapsed;
                RestoreButton.Visibility = Visibility.Visible;
            }
            else
            {
                MaximizeButton.Visibility = Visibility.Visible;
                RestoreButton.Visibility = Visibility.Collapsed;
            }
        }

        private void WindowStateChangedHandler(object sender, EventArgs e)
        {
            RefreshMaximizeRestoreButton();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        string SortName = "排序：名称";
        string SortUpOrDown = "↓";
        private void SortName_Click(object sender, RoutedEventArgs e)
        {
            SortName = "排序：" + ((System.Windows.Controls.HeaderedItemsControl)sender).Header.ToString();
            NameMenuItem.Header = $"{SortName} {SortUpOrDown}";
        }

        private void SortUp_Click(object sender, RoutedEventArgs e)
        {
            SortUpOrDown = ((System.Windows.Controls.HeaderedItemsControl)sender).Header.ToString();
            NameMenuItem.Header = $"{SortName} {SortUpOrDown}";
        }
    }
}
