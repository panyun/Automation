using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL.Robot.WpfMain.ModelData
{
    public class VariableModel
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string Name => Childs == null || Childs.Count == 0 ? $"{Key} = {Value}" : Key;
        public List<VariableModel> Childs { get; set; } = new List<VariableModel>();

    }
}
