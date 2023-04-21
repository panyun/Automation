
using System;
using System.Collections.Generic;
using System.Reflection;
using ET;

namespace RPCBus.Server.Client
{
    /// <summary>
    /// 玩家数据管理组件
    /// </summary>
    public class PlayerComponent : Entity
    {
        public Dictionary<long, Player> Cache = new Dictionary<long, Player>();

        public List<Type> ObjectTypes = new List<Type>();

        private Dictionary<Type, ICreateSystem> createSystems = new Dictionary<Type, ICreateSystem>();


        public ICreateSystem GetCreateSystem(Type type)
        {
            this.createSystems.TryGetValue(type, out ICreateSystem value);
            return value;
        }

        public void Awake()
        {
            this.ObjectTypes.Clear();
            this.createSystems.Clear();

            foreach (Type type in Game.EventSystem.GetTypes(typeof(PlayerObjectAttribute)))
            {
                this.ObjectTypes.Add(type);
            }

            foreach (Type type in Game.EventSystem.GetTypes(typeof(PlayerSystemAttribute)))
            {
                object obj = Activator.CreateInstance(type);
                if (obj is ICreateSystem createSystem)
                {
                    this.createSystems.Add(createSystem.Type(), createSystem);
                }
            }
        }
    }
}
