using AduSkin;
using AduSkin.Controls.Attach;
using AduSkin.Controls.Metro;
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
using ViewModel;

namespace MiniRobotForm.Views
{
    /// <summary>
    /// DesignMainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DesignMainWindow : UserControl
    {
        public DesignMainWindow()
        {
            InitializeComponent();
            scrollFlow.ScrollToBottom();
          
        }
        private void AduTreeView_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (sender is TreeView && !e.Handled)
            {
                e.Handled = true;
                var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
                eventArg.RoutedEvent = UIElement.MouseWheelEvent;
                eventArg.Source = sender;
                var parent = ((Control)sender).Parent as UIElement;
                parent.RaiseEvent(eventArg);
            }
        }

        private void AduTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tab = sender as AduTabControl;
            var item = tab.SelectedItem as AduTabItem;
            item.Content = new ContentControl
            {
                Content = new ComponentWindow()
            };
            //tab.AnimateScrollIntoView(tab.SelectedItem);
        }
    }
}
