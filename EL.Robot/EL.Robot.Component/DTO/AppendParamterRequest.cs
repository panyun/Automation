using Automation.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL.Robot.Component.DTO
{
    public class AppendParamterRequest
    {
        public Parameter Parameter { get; set; }
        public string ComponentName { get; set; }
    }
    public class SelectParamterRequest
    {
        public string ComponentName { get; set; }
    }
    public class SelectParamterRespose
    {
        public List<Parameter> Parameters { get; set; }
    }

}
