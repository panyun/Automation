using EL.Robot.WpfMain.Common;
using EL.Robot.WpfMain.Config;
using Robot.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Robot.ViewModel
{
    public class VersionViewModel : PropertyChangeBase
    {
        public VersionViewModel()
        {
            VersonInfo = new ObservableCollection<VersionInfo>(VersionInfo.GetVersionInfos());
        }
        public ObservableCollection<VersionInfo> VersonInfo { get; set; }
    }
}
