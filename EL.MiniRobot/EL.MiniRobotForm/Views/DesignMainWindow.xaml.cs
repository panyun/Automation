using AduSkin;
using AduSkin.Controls.Attach;
using AduSkin.Controls.Metro;
using EL.BaseUI.Themes;
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
            tv_Component.PreviewMouseDown += Tv_Component_PreviewMouseDown;
            tv_Component.MouseMove += Tv_Component_MouseMove;
            tv_Robot.AllowDrop= true;
            tv_Robot.Drop += Tv_Robot_Drop;
        }

        private void Tv_Robot_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(typeof(NodeComponentModel)) is NodeComponentModel nodeComponentModel)
            {

            }
        }

        private void Tv_Component_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var c = sender as FrameworkElement;
                DragDrop.DoDragDrop(c, moveTreeItem, DragDropEffects.Copy);
            }
        }

        private Point _lastMouseDown;
        private NodeComponentModel moveTreeItem;
        private void Tv_Component_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                var ele = e.OriginalSource as FrameworkElement;
                if (ele != null)
                {
                     _lastMouseDown = e.GetPosition(tv_Component);
                     moveTreeItem = (NodeComponentModel)ele.DataContext;
                }
            }
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
