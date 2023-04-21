using ET;
using System;
using System.Diagnostics;
using System.IO;

namespace RPCBus.Server.Client
{
    public static class PlayerHelper
    {
        public static async ETTask HandleAsync(PlayerActor actor, Func<Player, ETTask> action)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.PlayerActor, actor.ClientId))
            {
                try
                {
                    await action(actor.DomainScene().GetComponent<PlayerComponent>().Get(actor.ClientId));
                }
                catch (ETException exception)
                {
                    actor.Disconnect(exception);
                }
                catch (Exception exception)
                {
                    actor.Disconnect(exception);
                }
            }
        }

        public static async ETTask HandleAsync(Scene scene, long playerId, Func<Player, ETTask> action)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.PlayerActor, playerId))
            {
                await action(await scene.GetComponent<PlayerComponent>().Query(playerId));
            }
        }

        public static async ETTask HandleAsync(Scene scene, long playerId, IActorNotification notification, Func<Player, ETTask> action)
        {
            async ETTask handleActorNotification(Player player)
            {
                if (player.GetComponent<NotificationComponent>().Exists(notification))
                {
                    return;
                }
                await action(player);
                player.GetComponent<NotificationComponent>().Add(notification);
            }

            await HandleAsync(scene, playerId, handleActorNotification);
        }
    }
}
