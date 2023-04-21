using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL.Robot.WpfMain.ModelData
{
    public class TreeNodeViewModel
    {
        public string? Name { get; set; }

        public List<TreeNodeViewModel> ChildrenModel { get; set; }
    }
}
