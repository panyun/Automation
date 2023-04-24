using EL.Async;
using EL.Capturing;
using EL.Http;
using EL.Overlay;
using EL.Robot.Component;
using EL.Robot.Core.SqliteEntity;
using EL.Sqlite;
using EL.Video;

namespace EL.Robot.Core
{
	/// <summary>
	/// 界面探测器入口
	/// </summary>
	public class RobotComponentAwake : AwakeSystem<RobotComponent>
	{
		public override void Awake(RobotComponent self)
		{
			var flowComponent = self.AddComponent<FlowComponent>();
			flowComponent.AddComponent<NodeComponent>();
			self.AddComponent<HttpComponent>();
			self.AddComponent<CaptureComponent>();
			self.AddComponent<ConcurrentQueueComponent>();
			self.AddComponent<DispatchComponent>();
			self.AddComponent<HotkeyComponent>();
			var sqliteComponent = self.AddComponent<SqliteComponent, string>("robot.db");

			if (!ConfigItemsHelper.IsExist)
				self.InitRobotData();
			var videoRecorderSettings = new VideoRecorderSettings
			{
				VideoFormat = VideoFormat.xvid,
				VideoQuality = 6,
				ffmpegPath = Path.Combine(ConfigItemsHelper.Log_BackupDirectory, "ffmpeg.exe")
			};
			var cap = self.GetComponent<CaptureComponent>();
			Func<VideoRecorderComponent, Task<CaptureImage>> _recorder = async (self) =>
			{
				return cap.MainScreen();
			};
			var video = self.AddComponent<VideoRecorderComponent, VideoRecorderSettings, Func<VideoRecorderComponent, Task<CaptureImage>>>(videoRecorderSettings, _recorder);
			self.AddComponent<AudioRecorderComponent>();
			//var tests = RobotDataManagerService.GetFlowHistorys(0, 10);
			//var count = RobotDataManagerService.GetFlowHistorysLength();
			//var data = RobotDataManagerService.GetFlowById(243);
			//var datas = RobotDataManagerService.GetFlowSummarys();
		}
	}
	/// <summary>
	/// 
	/// </summary>
	public static class RobotComponentSystem
	{
		public static async ELTask Main(this RobotComponent self, Flow flow)
		{
			var flowComponent = self.GetComponent<FlowComponent>();
			self.Start(flow);
			await flowComponent.Main();
			self.Finish();
		}
		private static void Start(this RobotComponent self, Flow flow)
		{
			var flowComponent = self.GetComponent<FlowComponent>();
			self.State = 1;
			Flow flow1 = flow;
			flowComponent.MainFlow = flow1;
			flowComponent.FlowHistory = FlowHistory.CreateFlowHistory();
			if (ConfigItemsHelper.Log_IsScreenRecording)
				self.StartVideo();
			flowComponent.LogMsgs.Clear();
			self.StartExecRobotAction?.Invoke();
			self.NoneAction?.Invoke();
			self.ExecState = ExecState.None; //更新流程执行状态为正常
		}
		private static void Finish(this RobotComponent self)
		{
			self.WriteRunLog();
			self.StopExecRobotAction?.Invoke();
			Thread.Sleep(2000);
			self.State = 0;
			if (ConfigItemsHelper.Log_IsScreenRecording)
				self.StopVideo();
		}
		private static void WriteRunLog(this RobotComponent self)
		{
			var flowComponent = self.GetComponent<FlowComponent>();
			var execFlowInfo = flowComponent.FlowHistory;
			void WriteLogFile()
			{
				try
				{
					//写入日志
					if (!Directory.Exists(execFlowInfo.FileDir))
						Directory.CreateDirectory(execFlowInfo.FileDir);
					var logs = string.Join("\r\n", flowComponent.LogMsgs.Select(x => x.ShowMsg));
					using (var file = File.Open(execFlowInfo.LogFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
					{
						file.Close();
						file.Dispose();
					}
					File.AppendAllText(execFlowInfo.LogFilePath, logs);
				}
				catch (Exception ex)
				{
					Log.Error(ex);
				}
			}
			WriteLogFile();
			void WriteLogDB()
			{
				try
				{
					var insertExecLog = $@"INSERT INTO {nameof(FlowHistory)}
            ({nameof(FlowHistory.Id)}, {nameof(FlowHistory.FlowId)}, {nameof(FlowHistory.Name)}, {nameof(FlowHistory.StartTime)}, {nameof(FlowHistory.TakeTime)}, {nameof(FlowHistory.State)}, {nameof(FlowHistory.LogFilePath)}, {nameof(FlowHistory.VideoFilePath)}, {nameof(FlowHistory.ExNodeName)}, {nameof(FlowHistory.ExMsg)})
            VALUES({execFlowInfo.Id}, {execFlowInfo.FlowId}, '{execFlowInfo.Name}',
            {execFlowInfo.StartTime}, {execFlowInfo.TakeTime}, {execFlowInfo.State},
            '{execFlowInfo.LogFilePath}', '{execFlowInfo.VideoFilePath}', '{execFlowInfo.ExNodeName}', '{execFlowInfo.ExMsg}');";
					var sqliteComponent = self.GetComponent<SqliteComponent>();
					sqliteComponent.ExecuteNonQueryAsync(insertExecLog);
				}
				catch (Exception ex)
				{
					Log.Error(ex);
				}
			}
			WriteLogDB();
		}

		private static void LocalConfig(this RobotComponent self)
		{
			self.IsSelfMachine = false;
			self.ClientNo = default;
		}
		/// <summary>
		/// 本地执行机器人脚本
		/// </summary>
		/// <param name="self"></param>
		/// <param name="flow"></param>
		/// <returns></returns>
		public static async ELTask LocalMain(this RobotComponent self, Flow flow, bool isDebug)
		{
			flow.IsDebug = isDebug;
			self.LocalConfig();
			await self.Main(flow);
		}
		public static void InitRobotData(this RobotComponent self)
		{
			var sqliteComponent = self.GetComponent<SqliteComponent>();
			sqliteComponent.CreateDataFile("robot.db");
			#region create Config table 
			var configTableCmd = $@"CREATE TABLE  if not exists {nameof(ConfigItems)} (
                                    {nameof(ConfigItems.Id)} INTEGER DEFAULT(1) PRIMARY KEY AUTOINCREMENT,
	                                ""{nameof(ConfigItems.Key)}"" TEXT,
	                                {nameof(ConfigItems.Value)} TEXT ); ";
			//运行日志
			var execLogTableCmd = $@"CREATE TABLE  if not exists {nameof(FlowHistory)} (
                                    {nameof(FlowHistory.Id)} INTEGER  PRIMARY KEY ,
                                    {nameof(FlowHistory.FlowId)} INTEGER,
	                                {nameof(FlowHistory.Name)} TEXT,
	                                {nameof(FlowHistory.StartTime)} INTEGER,
                                    {nameof(FlowHistory.TakeTime)}    INTEGER,
                                    ""{nameof(FlowHistory.State)}""       INTEGER,
                                    {nameof(FlowHistory.LogFilePath)} TEXT,
                                    {nameof(FlowHistory.VideoFilePath)} TEXT,
                                    {nameof(FlowHistory.ExNodeName)} TEXT,
                                    {nameof(FlowHistory.ExMsg)} TEXT
                                    ); ";

			var flowDataTableCmd = $@"CREATE TABLE  if not exists {nameof(FlowData)} (
                                    {nameof(FlowData.Id)} INTEGER  PRIMARY KEY ,
	                                {nameof(FlowData.Content)} TEXT,
	                                {nameof(FlowData.Features)} TEXT,
                                    {nameof(FlowData.DesignMsg)} TEXT
                                    ); ";

			var planDataTableCmd = $@"CREATE TABLE  if not exists {nameof(PlanData)} (
                                    {nameof(PlanData.Id)} INTEGER  PRIMARY KEY , 
                                    {nameof(PlanData.FlowId)} INTEGER,
	                                {nameof(PlanData.FlowName)} TEXT,
	                                {nameof(PlanData.Expression)} TEXT, 
	                                {nameof(PlanData.Enable)}   INTEGER,
	                                {nameof(PlanData.UpdateTime)}  INTEGER
                                    ); ";

			sqliteComponent.CreateTable(configTableCmd); // 创建配置表
			sqliteComponent.CreateTable(execLogTableCmd); //创建执行日志表
			sqliteComponent.CreateTable(flowDataTableCmd); //创建流程数据表
			sqliteComponent.CreateTable(planDataTableCmd); //创建任务调度计划表
			var dir = $"{System.IO.Directory.GetCurrentDirectory()}\\FlowLog";
			var initConfigData = $@"INSERT INTO ConfigItems (""{nameof(ConfigItems.Key)}"", {nameof(ConfigItems.Value)}) VALUES(""{nameof(ConfigItemsHelper.Log_IsAutomaticBackup)}"", ""True"");";
			initConfigData += $@"INSERT INTO ConfigItems (""{nameof(ConfigItems.Key)}"", {nameof(ConfigItems.Value)}) VALUES(""{nameof(ConfigItemsHelper.Log_BackupDays)}"", ""7"");";
			initConfigData += $@"INSERT INTO ConfigItems (""{nameof(ConfigItems.Key)}"", {nameof(ConfigItems.Value)}) VALUES(""{nameof(ConfigItemsHelper.Log_BackupDirectory)}"", ""{dir}"");";
			initConfigData += $@"INSERT INTO ConfigItems (""{nameof(ConfigItems.Key)}"", {nameof(ConfigItems.Value)}) VALUES(""{nameof(ConfigItemsHelper.Log_IsScreenRecording)}"", ""False"");";
			sqliteComponent.ExecuteNonQueryAsync(initConfigData);
			#endregion
		}
		public static void StartVideo(this RobotComponent self, string fileName = default)
		{
			var video = self.GetComponent<VideoRecorderComponent>();
			var ffmpeg = Path.Combine(ConfigItemsHelper.Log_BackupDirectory, "ffmpeg.exe");
			try
			{
				if (!File.Exists(ffmpeg))
					File.Copy(Path.Combine(Environment.CurrentDirectory, "ffmpeg.exe"), ffmpeg, true);
			}
			catch (Exception ex)
			{

			}
			try
			{
				SystemInfo.RefreshAll();
			}
			catch (Exception)
			{
			}
			try
			{
				video._settings.ffmpegPath = ffmpeg;
				video._settings.TargetVideoPath = fileName == default ? self.GetComponent<FlowComponent>().FlowHistory.VideoFilePath : fileName;
				video.Start();
			}
			catch (Exception ex)
			{
				Log.Error(ex);
			}
		}
		public static void StopVideo(this RobotComponent self)
		{
			var video = self.GetComponent<VideoRecorderComponent>();
			video.Stop();
		}
		#region FlowQueue
		public static void AddLocalFlowQueue(this RobotComponent self, Flow flow, bool isDebug = false)
		{
			self.GetComponent<ConcurrentQueueComponent>().Post(new QueueObject()
			{
				Action = async () =>
				{
					await self.LocalMain(flow, isDebug);
				},
				Data = flow,
				Key = IdGenerater.Instance.GenerateId(),
			});
		}

		public static void Update(this RobotComponent self)
		{
			if (self.State == 0)
			{
				self.GetComponent<ConcurrentQueueComponent>().Update();
			}
		}
		public static List<QueueObject> FlowQueueInfos(this RobotComponent self)
		{
			return self.GetComponent<ConcurrentQueueComponent>().Infos();
		}
		public static bool RemoveFlowQueue(this RobotComponent self, long keyId)
		{
			return self.GetComponent<ConcurrentQueueComponent>().Remove(keyId);
		}
		#endregion
		public static void StopFlow(this RobotComponent self)
		{
			var robot = Boot.GetComponent<RobotComponent>();
			var flowComponent = self.GetComponent<FlowComponent>();
			flowComponent.WriteNodeLog(robot.CurrentNode, $"程序手动停止！");
			Task.Run(() =>
			{
				robot.ExecState = ExecState.IsStop;
				if (robot.ELTaskPaused != null && !robot.ELTaskPaused.IsCompleted)
					robot.ELTaskPaused.SetResult(true);
			});
		}
	}
}
