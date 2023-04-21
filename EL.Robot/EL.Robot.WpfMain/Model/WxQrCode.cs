using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL.Robot.WpfMain.Model
{
    /// <summary>
    /// 获取微信二维码
    /// </summary>

    public class WxQrCode: AWxRequest<WxQrCode.Data>
    {
        public class Data
        {
            /// <summary>
            /// 
            /// </summary>
            public string url { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string token { get; set; }
        }
    }
}
