using EL.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL.Net.Network
{
    // 刚accept的session只持续5秒，必须通过验证，否则断开
    public class SessionAcceptTimeoutComponent : Entity
    {
        public long Timer;
    }
}
