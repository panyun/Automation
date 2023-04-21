using ET;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace RPCBus.Server.Client
{
    [ObjectSystem]
    class PlayerAwakeSystem : AwakeSystem<Player>
    {
        public override void Awake(Player self)
        {
           
            // 每分钟刷新一次缓存，卸载超时的缓存数据
            self.CheckRepeatedTimer = TimerComponent.Instance.NewRepeatedTimer(60 * 1000, async () =>
            {
                using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.PlayerActor, self.Id))
                {
                    await self.Check();
                }
            });

            // 每10秒保存一次脏数据
            self.SaveRepeatedTimer = TimerComponent.Instance.NewRepeatedTimer(10 * 1000, async () =>
            {
                using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.PlayerActor, self.Id))
                {
                    await self.Save();
                }
            });

        }
    }

    [ObjectSystem]
    class PlayerDestroySystem : DestroySystem<Player>
    {
        public override void Destroy(Player self)
        {
            TimerComponent.Instance.Remove(ref self.CheckRepeatedTimer);
            TimerComponent.Instance.Remove(ref self.SaveRepeatedTimer);
            Log.Info("[STAGE]： Destroy()");
        }
    }

    public static class PlayerSystem
    {
        /// <summary>
        /// 内部方法，外部不要调用
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static async ETTask Check(this Player self)
        {
            // 10分钟内使用过不移除
            if (self.LastTimestamp + 10 * 60 * 1000 > TimeHelper.ServerNow()) return;
            // 客户端在线,自动续约
            if (self.DomainScene().GetComponent<PlayerActorComponent>().Exists(self.Id))
            {
                self.LastTimestamp = TimeHelper.ServerNow();
                return;
            }
            // 从缓存中移除
            self.GetParent<PlayerComponent>().Remove(self.Id);
            await self.Save();
            self.Dispose();
            Log.Debug($"[STAGE]: 移除 {self.Id} 的缓存");
        }

        public static async ETTask Save(this Player self)
        {
            if (self.DirtyQueue.Count == 0)
            {
                return;
            }

            List<Entity> dirtyList = new List<Entity>();
            foreach (var v in self.DirtyQueue)
            {
                dirtyList.Add(v);
            }
            self.DirtyQueue.Clear();
            await DBComponent.Instance.Save(self.Id, dirtyList);
            Log.Info($"[STAGE]: 保存 {self.Id} 的脏数据");
        }

        public static void Send(this Player self, IActorMessage message)
        {
            var playerActor = self.DomainScene().GetComponent<PlayerActorComponent>().Get(self.ClientId);
            if (playerActor != null)
            {
                playerActor.Send(message);
            }
        }

        public static async ETTask<IActorResponse> Call(this Player self, IActorRequest request, bool needException = true)
        {
            var playerActor = self.DomainScene().GetComponent<PlayerActorComponent>().Get(self.ClientId);
            if (playerActor != null)
            {
                return await playerActor.Call(request, needException);
            }
            return null;
        }

        public static async ETTask<TResponseType> Call<TResponseType>(this Player self, IActorRequest request, bool needException = true)
            where TResponseType : IActorResponse
        {
            var response = await self.Call(request, needException);
            return response == null ? default : (TResponseType)response;
        }

        public static void Disconnect(this Player self, int code, string reason)
        {
            var playerActor = self.DomainScene().GetComponent<PlayerActorComponent>().Get(self.ClientId);
            if (playerActor != null)
            {
                playerActor.Disconnect(code, reason);
            }
        }

        public static void Disconnect(this Player self, Exception exception)
        {
            var playerActor = self.DomainScene().GetComponent<PlayerActorComponent>().Get(self.ClientId);
            if (playerActor != null)
            {
                playerActor.Disconnect(exception);
            }
        }

        public static void Disconnect(this Player self, ETException exception)
        {
            var playerActor = self.DomainScene().GetComponent<PlayerActorComponent>().Get(self.ClientId);
            if (playerActor != null)
            {
                playerActor.Disconnect(exception);
            }
        }

    }
}
