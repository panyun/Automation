using Robot.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Robot.ViewModel
{
    public class StartUpViewModel : PropertyChangeBase
    {
        public StartUpViewModel()
        {
            new Thread(Run) { IsBackground = true }.Start();
        }
        private string _image;
        public string Image
        {
            set
            {
                _image = value;
                NC();
            }
            get { return _image; }
        }


        public int Sleep { get; set; } = 50;
        private void Run()
        {
            for (var i = 0; i < 100 + 1; i++)//100
            {
                var uri = $"pack://application:,,,/EL.Robot.WpfMain;component/Resources/Images/Loading/{i:00000}.png";
                Image = uri;
                Thread.Sleep(Sleep);
            }

            End?.Invoke();
        }

        public Action End { get; set; }
    }
}
