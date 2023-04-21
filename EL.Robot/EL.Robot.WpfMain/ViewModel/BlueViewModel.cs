using ControlzEx.Standard;
using EL.Robot.WpfMain.Common;
using EL.Robot.WpfMain.Model;
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
using System.Xml.Linq;

namespace Robot.ViewModel
{
    public class BlueViewModel : PropertyChangeBase
    {
        public BlueViewModel()
        {
            //  Image = new BitmapImage(new Uri(localinfo.Instance.headImgUrl));
            if (localinfo.Instance.nickname.Length > 3)
            {
                Name = localinfo.Instance.nickname.Substring(0, 3);
            }
            else
            {
                Name = localinfo.Instance.nickname;
            }
            IsOpen = false;
        }
        public string Name { get; set; }
        private BitmapImage _image;
        public BitmapImage Image
        {
            set
            {
                _image = value;
                NC();
            }
            get { return _image; }
        }
        private bool _isOpen;
        public bool IsOpen
        {
            set
            {
                _isOpen = value;
                NC();
            }
            get { return _isOpen; }
        }
    }
    //public class VersionInfo
    //{
    //    public string Version { get; set; }
    //    public string Time { get; set; }
    //    public List<string> Updates { get; set; }

    //}
}
