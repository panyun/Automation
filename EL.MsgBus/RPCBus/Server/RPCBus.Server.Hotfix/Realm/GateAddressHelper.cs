using System;
using System.Collections.Generic;
using ET;

namespace RPCBus.Server.Realm
{
    public static class GateAddressHelper
    {
        /// <summary>
        /// 随机分配一个网关
        /// </summary>
        /// <param name="zone"></param>
        /// <returns></returns>
        public static StartSceneConfig Random(int zone)
        {
            List<StartSceneConfig> gates = StartSceneConfigCategory.Instance.Gates[zone];
            int index = RandomHelper.RandomNumber(0, gates.Count);
            return gates[index];
        }
    }
}
