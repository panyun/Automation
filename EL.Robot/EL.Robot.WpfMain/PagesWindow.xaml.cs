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
    /// PagesWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PagesWindow : Window
    {
        public PagesWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            /*new StartUp().Show(); */
            WindowManager.Show<StartUp>();
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            /*new BlueWindow().Show();*/
            WindowManager.Show<BlueWindow>();
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            /*new VioletWindow().Show();*/
            WindowManager.Show<VioletWindow>();
        }

        private void Button3_Click(object sender, RoutedEventArgs e)
        {
            /*new SmallRobot().Show();*/
            WindowManager.Show<SmallRobot>();
        }

        private void Button4_Click(object sender, RoutedEventArgs e)
        {
            /*new DispatchWindow().Show();*/
            WindowManager.Show<DispatchWindow>();
        }

        private void Button5_Click(object sender, RoutedEventArgs e)
        {
            /*new ManageWindow().Show();*/
            WindowManager.Show<ManageWindow>();

        }

        private void Button6_Click(object sender, RoutedEventArgs e)
        {
            /*new FloatWindow().Show();*/
            WindowManager.Show<FloatWindow>();
        }

        private void Button7_Click(object sender, RoutedEventArgs e)
        {
            /* new BigRobot().Show();*/
            WindowManager.Show<BigRobot>();
        }

        private void Button8_Click(object sender, RoutedEventArgs e)
        {
            /*new WxLoginWindow().Show();*/
            WindowManager.Show<WxLoginWindow>();
        }
    }
}
