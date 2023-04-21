using EL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automation.Parser
{
    public static class ResponseExtensions
    {
        public static IResponse GetFail(this IResponse response, string message, int error = 701)
        {
            response.Error = error;
            response.Message = message;
            return response;
        }
        public static IResponse GetFail(this IResponse response, Exception ex, int error = 702)
        {
            var message = LogHelper.GetStackTrace(ex.Message);
            response.Error = error;
            response.Message = ex.Message;
            response.StackTrace = message;
            return response;
        }

    }
}
