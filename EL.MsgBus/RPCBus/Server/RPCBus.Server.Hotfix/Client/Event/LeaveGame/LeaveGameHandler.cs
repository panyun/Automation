using ET;
using RPCBus.Server.EventType;

namespace RPCBus.Server.Stage
{
    /// <summary>
    /// 
    /// </summary>
    public partial class LeaveGameHandler : ET.AEvent<LeaveGame>
    {
        protected override async ET.ETTask Run(LeaveGame self)
        {
            Log.Debug($"[STAGE]: 客服端 {self.Actor.AccountId}:{self.Actor.ClientId} 下线");
            await ET.ETTask.CompletedTask;
            //释放玩家进入的信号通知
            self.Actor.EnterGameCancellationToken.Cancel();
            //Logger.Leave(self.Player);
            self.Actor.Dispose();
        }
    }
}
