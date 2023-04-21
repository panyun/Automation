using EL.Robot.Core;
using EL.Robot.Core.Websocket;
using Protos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace EL.Robot
{
    public class RoboatServerComponent
    {
        public static RoboatServerComponent Instance = new RoboatServerComponent();
        /// <summary>
        /// 客户端在线对象
        /// </summary>
        public static List<G2C_OnLine> OnLineClients = new List<G2C_OnLine>();
  
        public static long ClientId { get; set; }
        public static string ClientNo { get; set; }
        /// <summary>
        /// 中控对象
        /// </summary>
        public static G2C_OnLine CentralControl
        {
            get
            {
                return OnLineClients.FirstOrDefault(x => x.ClientType == "中控平台");
            }
        }
        /// <summary>
        /// 当前通信通道
        /// </summary>
        public static WChannel CurrentTChannel { get; set; }
        /// <summary>
        /// 登录信息
        /// </summary>
        public static R2C_Login CurrentUser { get; set; }

        /// <summary>
        /// 异步信号
        /// </summary>
        public static CancellationTokenSource cts = new CancellationTokenSource();
        public static long LastRecvTime { get; set; }

        public static void Add(G2C_OnLine g2C_OnLineClient)
        {
            if (!OnLineClients.Exists(x => x.ClientId == g2C_OnLineClient.ClientId))
                OnLineClients.Add(g2C_OnLineClient);
        }
        public static void Remove(G2C_OffLine g2C_OffLine)
        {
            if (OnLineClients.Exists(x => x.ClientId == g2C_OffLine.ClientId))
                OnLineClients.RemoveAll(x => x.ClientId == g2C_OffLine.ClientId);
        }
    }
}
