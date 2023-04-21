using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL.Robot.WpfMain.ViewModel
{
    public class ProjectViewModel : ObservableObject
    {
        public string Name { get; set; }
        public bool IsSelected
        {
            get
            {
                return GetProperty<bool>();
            }
            set
            {
                SetProperty<bool>(value);
            }
        }
    }
}
