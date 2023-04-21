using ET;

namespace Example.Server.Client
{
    //[ActorMessageHandler]
    //public class FTObjectAddHandler : AMActorNotificationHandler<Scene, Protos.FTObjectAdd>
    //{
    //    protected override async ETTask Run(Scene scene, Protos.FTObjectAdd request)
    //    {
    //        async ETTask HandleAsync(Player player)
    //        {
    //            // TODO: 执行添加道具的逻辑
    //            //Item item = new Item(request.Value.ConfigId);
    //            //item.ItemId = request.Value.ItemId;

    //            // 空间不足，发邮件
    //            //if (itemComponent.Items.Count + 1 > itemComponent.Capacity)
    //            //{
    //            //    player.GetComponent<MailComponent>().SendSystemMail("")
    //            //    return;
    //            //}

    //            // 直接添加到背包
    //            //player.GetComponent<ItemComponent>().Items.Add(item);

    //            await ETTask.CompletedTask;

    //            Log.Debug($"[STAGE]: {player.Id} 添加道具: {request.Value}  ");
    //        }

    //        await PlayerHelper.HandleAsync(scene, request.Receiver, request, HandleAsync);
    //    }
    //}
}
