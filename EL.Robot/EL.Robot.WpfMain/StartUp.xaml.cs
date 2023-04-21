using Automation;
using Automation.Inspect;
using EL;
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

namespace Robot
{
    /// <summary>
    /// StartUp.xaml 的交互逻辑
    /// </summary>
    public partial class StartUp : Window
    {
        public StartUp()
        {
            InitializeComponent();

        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void PlayEnd()
        {
            this.Dispatcher.Invoke(() =>
            {
                //new DispatchWindow().Show();
                //new ProjectWindow().Show();
                //new FloatWindow().Show();
                /*new BlueWindow().Show();*/
                //WindowManager.Show<BlueWindow>();
                //WindowManager.Show<BigRobot>();
                WindowManager.Show<WxLoginWindow>();
                //new VioletWindow().Show();
                //new SmallRobot().Show();


                //MessageBox.Show("动画播放完成");

                this.Hide();
            });
        }
    }
}
