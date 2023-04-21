using Automation.Inspect;
using EL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automation.Parser
{
   
    public class InputActionRequest : RequestBase
    {
        public bool IsClear { get; set; } = true;
        public string InputTxt { get; set; }
        public InputType InputType { get; set; } = InputType.Keyboard;
    }
    public enum InputType
    {
        Keyboard,
        ElementInput,
        Paste
    }
     
  
    public class InputActionResponse : IResponse
    {
        public int Error { get; set; }
        public string Message { get; set; }
        public int RpcId { get; set; }
        public string StackTrace { get; set; }

    }
}
