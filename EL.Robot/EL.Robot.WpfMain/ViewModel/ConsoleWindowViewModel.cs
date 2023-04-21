using Robot.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL.Robot.WpfMain.ViewModel
{
    public class ConsoleWindowViewModel: PropertyChangeBase
    {
        private string _logdata;
        public string LogData 
        {
            get => _logdata;
            set => _logdata = value;
        } 


    }
}
