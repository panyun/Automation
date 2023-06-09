﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace EL.PIP
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private static System.Threading.Mutex mutex;
        protected override void OnStartup(StartupEventArgs e)
        {
            mutex = new System.Threading.Mutex(true, nameof(EL.PIP));
            if (mutex.WaitOne(0, false))
            {
                base.OnStartup(e);
                Log.Clear();
            }
            else
            {
                this.Shutdown();
            }
        }
    }
}
