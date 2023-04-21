using EL.Robot.Core;
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

namespace EL.Robot.WpfMain
{
    /// <summary>
    /// ToolWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ToolWindow : Window
    {
        public ToolWindow()
        {
            InitializeComponent();
            //test1.Source =   new BitmapImage(new Uri("pack://application:,,,/Resources/暂停.png"));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Boot.GetComponent<RobotComponent>().Main(null);
        }

    }
}
