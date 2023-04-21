using ET;

namespace RPCBus.Server
{
    public class ClientAddressHelper
    {
        public static MultiMap<int, StartSceneConfig> Client = new MultiMap<int, StartSceneConfig>();

        public static void Init()
        {
            foreach (StartSceneConfig startSceneConfig in StartSceneConfigCategory.Instance.GetAll().Values)
            {
                Client.Add(startSceneConfig.Zone, startSceneConfig);
            }

        }

        public static StartSceneConfig GetGate(int zone)
        {
            try
            {
                var gate = Client[zone].Where(x => x.SceneType.ToUpper() == "GATE").FirstOrDefault();
                return gate;
            }
            catch (Exception ex)
            {

                throw;
            }
         
        }
        /// <summary>
        /// 获得指定玩家的 Stage Server 的配置数据
        /// </summary>
        /// <param name="zone"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        public static StartSceneConfig Get(int zone, long playerId, int type)
        {
            if (type == 1)
                return GetRobotClient(zone, playerId);
            if (type == 2)
                return GetDesigner(zone);
            return GetControllers(zone);
        }
        public static StartSceneConfig GetRobotClient(int zone, long playerId)
        {
            var robotClient = Client[zone].Where(x => x.Name.ToLower().StartsWith(SceneType.RobotClient.ToString().ToLower())).ToList();
            int playerIdx = (int)(playerId % robotClient.Count());
            return robotClient[playerIdx];
        }
        public static StartSceneConfig GetDesigner(int zone)
        {
            return Client[zone].Where(x => x.Name.ToLower().StartsWith(SceneType.Designer.ToString().ToLower())).FirstOrDefault();
        }
        public static StartSceneConfig GetControllers(int zone)
        {
            return Client[zone].Where(x => x.Name.ToLower().StartsWith(SceneType.Controller.ToString().ToLower())).FirstOrDefault();

        }
        /// <summary>
        /// 获得指定 Zone 的 Stage 列表
        /// </summary>
        /// <param name="zone"></param>
        /// <returns></returns>
        public static List<StartSceneConfig> WithZone(int zone)
        {
            return Client[zone];
        }

        public static int IndexOf(int zone, long id)
        {
            var games = ClientAddressHelper.WithZone(zone);
            return games.IndexOf(games.FirstOrDefault((value) => value.Id == id));
        }

        public static int Count(int zone)
        {
            return Client[zone].Count;
        }
    }
}
