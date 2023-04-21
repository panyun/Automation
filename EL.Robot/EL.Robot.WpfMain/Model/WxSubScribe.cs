using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL.Robot.WpfMain.Model
{
    /// <summary>
    /// 获取当前token是否已关注
    /// </summary>
    public class WxSubScribe : AWxRequest<WxSubScribe.Data>
    {
        public class Data
        {
            /// <summary>
            /// 机器人ID
            /// </summary>
            public int robotId { get; set; }
            /// <summary>
            /// 正式token     jwt
            /// </summary>
            public string token { get; set; }
            /// <summary>
            /// 用户ID
            /// </summary>
            public int userId { get; set; }
            /// <summary>
            /// 昵称
            /// </summary>
            public string nickname { get; set; }
            /// <summary>
            /// 头像
            /// </summary>
            public string headImgUrl { get; set; }
        }
    }

}
