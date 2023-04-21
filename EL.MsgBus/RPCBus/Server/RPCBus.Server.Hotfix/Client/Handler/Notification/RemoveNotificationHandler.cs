using ET;

namespace RPCBus.Server.Client
{
    [ActorMessageHandler]
    public class RemoveNotificationHandler : AMActorNotificationHandler<Scene, Protos.RemoveNotification>
    {
        protected override async ETTask Run(Scene scene, Protos.RemoveNotification request)
        {

            async ETTask HandleAsync(Player player)
            {
                await ETTask.CompletedTask;

                player.GetComponent<NotificationComponent>().Remove(request);
            }

            await PlayerHelper.HandleAsync(scene, request.Receiver, HandleAsync);
        }
    }
}
