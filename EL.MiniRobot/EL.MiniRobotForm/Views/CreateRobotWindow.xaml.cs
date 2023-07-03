using AduSkin.Controls.Metro;
using EL;
using EL.Robot.Component;
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
using ViewModel;
using static EL.Robot.Core.Request.MsgAgentComonpmentSystem;
using Flow = EL.Robot.Component.Flow;

namespace MiniRobotForm.Views
{
    /// <summary>
    /// CreateRobotWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CreateRobotWindow : MetroWindow
    {
        public CreateRobotWindow()
        {
            InitializeComponent();
            this.EscClose = true;
        }

        private void MetroButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txt_name.Text))
            {
                lbl_error.Text = "机器人名称不能为空！";
                return;
            }
            var temp = new Feature()
            {
                Name = txt_name.Text.Trim(),
                Note = txt_note.Text.Trim()
            };
            var design = Boot.GetComponent<RobotComponent>().GetComponent<DesignComponent>();
            design.CreateRobot(temp);
            this.DialogResult = true;
        }
        private void btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
