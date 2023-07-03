using AduSkin.Utility.AduMethod;
using AutoMapper;
using EL;
using EL.Async;
using EL.Robot.Core;
using MiniRobotForm.Mode;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MiniRobotForm
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            RequestManager.CreateBoot();
            RequestManager.Init();
          
            this.Exit += async (x, y) =>
            {
                DesignComponent designComponent = Boot.GetComponent<RobotComponent>().GetComponent<DesignComponent>();
                await designComponent.SaveRobot();
            };
        }

    }
}
