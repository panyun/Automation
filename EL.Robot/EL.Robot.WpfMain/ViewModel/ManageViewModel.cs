using Robot.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robot.ViewModel
{
    public class ManageViewModel : PropertyChangeBase
    {
        private string userHeader;
        public string UserHeader
        {
            get => userHeader; set
            {
                userHeader = value;
                NC();
            }
        }

 

        private bool wxLogined;
        public bool WxLogined
        {
            get => wxLogined; set
            {
                wxLogined = value;
                NC();
            }
        }
    }
}
