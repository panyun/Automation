using Automation.Inspect;
using Automation.Parser;
using EL.Async;
using EL.Robot.Component.PIP.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace EL.Robot.Component
{
    public class CatchElementComponent : Entity
    {
        public async ELTask<ComponentResponse> Main(CommponetRequest request)
        {
            CatchUIRequest catchElementRequest = new();
            var respose = (CatchUIResponse)await UtilsComponent.Exec(catchElementRequest);
            var result = new ComponentResponse
            {
                Error = respose.Error,
                Message = respose.Message,
                StackTrace = respose.StackTrace,
                Data = respose.ElementPath
            };
            return result;
        }
    }
}
