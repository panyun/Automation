using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace WpfInspect.Core
{
    public static class DispatcherHelper
    {
        public static void Init()
        {
            dispatcher = Dispatcher.CurrentDispatcher;
        }

        static Dispatcher dispatcher;
        public static void UpdateUi(Action action)
        {
            dispatcher?.BeginInvoke(action);
        }
    }
}
