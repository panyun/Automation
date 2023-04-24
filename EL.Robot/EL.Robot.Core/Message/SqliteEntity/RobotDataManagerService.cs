using EL.Async;
using EL.Robot.Component;
using EL.Sqlite;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.SS.Formula.Functions;

namespace EL.Robot.Core.SqliteEntity
{
	public class FlowSummary
	{
		[JsonProperty(PropertyName = "flowId")]
		public long Id { get; set; }
		[JsonProperty(PropertyName = "flowName")]
		public string Name { get; set; }
		/// <summary>
		/// 设计类型 pc
		/// </summary>
		[JsonProperty(PropertyName = "designerType")]
		public string DesignerType { get; set; }
		/// <summary>
		/// 流程类型 前台 后台
		/// </summary>
		[JsonProperty(PropertyName = "flowType")]
		public string FlowType { get; set; }
		public string FlowTypeStr { get { if (FlowType == "front") return "前台流程"; else return "后台流程"; } }
		[JsonProperty(PropertyName = "ufrole")]
		public string Ufrole { get; set; }
		//[JsonProperty(PropertyName = "output")]
		//public string output { get; set; }
		//[JsonProperty(PropertyName = "params")]
		//public string param { get; set; }
		[JsonProperty(PropertyName = "quotations")]
		public string[] Quotations { get; set; }
		[JsonProperty(PropertyName = "isPip")]
		public int IsPip { get; set; }
		public bool CheckPip()
		{
			if (IsPip == 1)
			{
				return true;
			}
			return false;
		}
	}
	public class FlowQueue
	{
		public long Id { get; set; }
		public string Name { get; set; }
		public string Dscribe { get; set; }
		public string FlowTypeStr { get; set; }
		public long Key { get; set; }
	}
	public class FlowRuning
	{
		public long Id { get; set; }
		public string Name { get; set; }
		public string FlowTypeStr { get; set; }
		public long Second { get; set; }
		public DateTime StartDate { get; set; }
		public string Dscribe { get; set; }
	}
	public static class RobotDataManagerService
	{
		static readonly SqliteComponent sqliteComponent = Boot.GetComponent<RobotComponent>().GetComponent<SqliteComponent>();
		public static async ELTask<int> SaveFlow(long id, string flowData, string features,string logs)
		{
			try
			{
				string cmd = $@"REPLACE INTO FlowData
						(Id, Content, Features,DesignMsg)
						VALUES({id}, '{flowData}', '{features}','{logs}');
						";
				return await sqliteComponent.ExecuteNonQueryAsync(cmd);
			}
			catch (Exception ex)
			{
				throw;
			}

		}
		public static List<FlowData> LoadFlows()
		{
			string cmd = $@"SELECT Id, Content, Features,DesignMsg
FROM FlowData;";
			return sqliteComponent.Query<FlowData>(cmd);
		}

		/// <summary>
		///  我的脚本概要列表
		/// </summary>
		public static List<FlowSummary> GetFlowSummarys()
		{
			string cmd = $"select {nameof(FlowData.Id)},{nameof(FlowData.Features)} from {nameof(FlowData)}";
			var flowDatas = sqliteComponent.Query<FlowData>(cmd);
			if (flowDatas == null || flowDatas.Count == 0) return default;
			List<FlowSummary> flowSummarys = new List<FlowSummary>();
			foreach (var flowData in flowDatas)
			{
				if (string.IsNullOrEmpty(flowData.Features)) continue;
				var flowSummary = JsonConvert.DeserializeObject<FlowSummary>(flowData.Features);
				flowSummarys.Add(flowSummary);
			}
			return flowSummarys;
		}
		public static FlowSummary GetFlowSummaryById(long id)
		{
			string cmd = $"select {nameof(FlowData.Id)},{nameof(FlowData.Features)} from {nameof(FlowData)} where {nameof(FlowData.Id)}={id}";
			var flowData = sqliteComponent.Find<FlowData>(cmd);
			if (flowData == null || string.IsNullOrEmpty(flowData.Features)) return default;
			return JsonConvert.DeserializeObject<FlowSummary>(flowData.Features);
		}
		/// <summary>
		/// 根据id获取流程内容
		/// </summary>
		/// <returns></returns>
		public static Flow GetFlowById(long id)
		{
			string cmd = $"select {nameof(FlowData.Id)},{nameof(FlowData.Content)}  from {nameof(FlowData)} where {nameof(FlowData.Id)}={id}";
			var flowData = sqliteComponent.Find<FlowData>(cmd);
			if (flowData == null || flowData.Content == null) return default;
			var flowContent = (JObject)JsonConvert.DeserializeObject(flowData.Content);
			var execJson = flowContent["execJson"];
			if (execJson == null) return default;
			var json = execJson.ToString();
			if (string.IsNullOrWhiteSpace(json)) return default;
			return JsonHelper.FromJson<Flow>(json);
		}
		/// <summary>
		/// 根据id获取流程内容
		/// </summary>
		/// <returns></returns>
		public static string GetFlowDataById(long id)
		{
			string cmd = $"select {nameof(FlowData.Id)},{nameof(FlowData.Content)}  from {nameof(FlowData)} where {nameof(FlowData.Id)}={id}";
			var flowData = sqliteComponent.Find<FlowData>(cmd);
			if (flowData == null || flowData.Content == null) return default;
			var flowContent = (JObject)JsonConvert.DeserializeObject(flowData.Content);
			var execJson = flowContent["execJson"];
			if (execJson == null) return default;
			var json = execJson.ToString();
			if (string.IsNullOrWhiteSpace(json)) return default;
			return json;
		}

		/// <summary>
		/// 获取历史运行数据
		/// </summary>
		public static List<FlowHistory> GetFlowHistorys(int currentIndex, int pageSize)
		{
			if (currentIndex < 0 || pageSize < 1) return default;
			var historys = $"select * from {nameof(FlowHistory)} order by {nameof(FlowHistory.StartTime)} Desc limit {pageSize} offset {currentIndex * pageSize}";
			return sqliteComponent.Query<FlowHistory>(historys);
		}
		/// <summary>
		/// 历史运行记录的总条数
		/// </summary>
		/// <returns></returns>
		public static int GetFlowHistorysLength()
		{
			var historys = $"select count(*) from {nameof(FlowHistory)}";
			return sqliteComponent.Scalar<int>(historys);
		}
		/// <summary>
		/// 当前运行中的程序
		/// </summary>
		/// <returns></returns>
		public static FlowRuning GetFlowRuning()
		{
			var robot = Boot.GetComponent<RobotComponent>();
			if (robot.State == 0) return default;
			var flowComponent = robot.GetComponent<FlowComponent>();
			var time = flowComponent.FlowHistory.StartTime;
			var second = (TimeHelper.ServerNow() - time) / 1000;
			var date = TimeHelper.ToDateTime(time);
			var sum = GetFlowSummaryById(flowComponent.MainFlow.Id);
			return new FlowRuning()
			{
				FlowTypeStr = sum?.FlowTypeStr ?? "前台流程",
				Id = flowComponent.MainFlow.Id,
				Name = flowComponent.MainFlow.Name,
				Second = second,
				StartDate = date,
				Dscribe = $"{date} 开始，已运行{second}"
			};
		}
		//终止流程
		public static bool StopFlow()
		{
			var robot = Boot.GetComponent<RobotComponent>();
			robot.StopFlow();
			return true;
		}
		/// <summary>
		/// 获取排队流程信息
		/// </summary>
		/// <returns></returns>
		public static List<FlowQueue> GetFlowQueues()
		{
			var robot = Boot.GetComponent<RobotComponent>();
			var queueDatas = robot.FlowQueueInfos();
			if (queueDatas == null || queueDatas.Count() == 0) return default;
			int index = 0;
			List<FlowQueue> list = new();
			foreach (var item in queueDatas)
			{
				var flow = item.Data as Flow;
				var sum = GetFlowSummaryById(flow.Id);
				index++;
				list.Add(new FlowQueue
				{
					Name = sum.Name,
					Id = flow.Id,
					Key = item.Key,
					Dscribe = $"前面已有{index}脚本",
					FlowTypeStr = sum.FlowTypeStr
				});
			}
			return list;
		}
		/// <summary>
		/// 移除流程队列
		/// </summary>
		/// <param name="flowId"></param>
		/// <returns></returns>
		public static bool RemoveFlowQueue(long key)
		{
			var robot = Boot.GetComponent<RobotComponent>();
			return robot.RemoveFlowQueue(key);
		}
		#region 计划
		/// <summary>
		///  获取当前计划
		/// </summary>
		public static List<PlanData> GetPlanDataFlowId(long FlowId = -1)
		{
			string cmd = $"select * from {nameof(PlanData)}";
			if (FlowId > 0)
			{
				cmd = $"select * from {nameof(PlanData)} where {nameof(PlanData.FlowId)}={FlowId}";
			}
			var flowDatas = sqliteComponent.Query<PlanData>(cmd);
			if (flowDatas == null || flowDatas.Count == 0) return default;
			return flowDatas;
		}
		/// <summary>
		///  获取当前计划
		/// </summary>
		public static List<PlanData> GetAllEnablePlanData(int enable = 1)
		{
			string cmd = $"select * from {nameof(PlanData)} where {nameof(PlanData.Enable)}={enable}";
			var flowDatas = sqliteComponent.Query<PlanData>(cmd);
			if (flowDatas == null || flowDatas.Count == 0) return default;
			return flowDatas;
		}
		/// <summary>
		///  获取当前计划
		/// </summary>
		public static PlanData GetPlanDataId(long Id)
		{
			var cmd = $"select * from {nameof(PlanData)} where {nameof(PlanData.Id)}={Id}";
			return sqliteComponent.Find<PlanData>(cmd);
		}
		public static int DeletePlanData(long Id)
		{
			string cmd = $"DELETE FROM {nameof(PlanData)} WHERE {nameof(PlanData.Id)}={Id};";
			return sqliteComponent.ExecuteNonQueryAsync(cmd).GetResult();
		}
		public static long AddPlanData(PlanData planData)
		{
			string cmd = @$"INSERT INTO {nameof(PlanData)}
({nameof(PlanData.FlowId)}, {nameof(PlanData.FlowName)}, {nameof(PlanData.Expression)}, {nameof(PlanData.Enable)}, {nameof(PlanData.UpdateTime)})
VALUES({planData.FlowId}, '{planData.FlowName}', '{planData.Expression}', {planData.Enable},{planData.UpdateTime});
select MAX(rowid) from {nameof(PlanData)};";
			return sqliteComponent.Scalar<long>(cmd);
		}
		public static int UpdatePlanData(PlanData planData)
		{
			string cmd = @$"UPDATE {nameof(PlanData)}
SET {nameof(PlanData.FlowId)}={planData.FlowId}, {nameof(PlanData.FlowName)}='{planData.FlowName}', {nameof(PlanData.Expression)}='{planData.Expression}', {nameof(PlanData.Enable)}= {planData.Enable}, {nameof(PlanData.UpdateTime)}={planData.UpdateTime} WHERE {nameof(PlanData.Id)}={planData.Id};";
			return sqliteComponent.ExecuteNonQueryAsync(cmd).GetResult();
		}
		public static int EnablePlanData(long planId, int Enable)
		{
			string cmd = @$"UPDATE {nameof(PlanData)} SET {nameof(PlanData.Enable)} = {Enable}, {nameof(PlanData.UpdateTime)}={PlanData.GetTime()} WHERE {nameof(PlanData.Id)}={planId};";
			return sqliteComponent.ExecuteNonQueryAsync(cmd).GetResult();
		}
		#endregion
	}
}
