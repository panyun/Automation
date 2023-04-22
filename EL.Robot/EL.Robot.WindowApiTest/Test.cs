using EL.Robot.Component;
using EL.Robot.Core;
using Flow = EL.Robot.Component.Flow;

namespace EL.Robot.WindowApiTest
{
	public partial class Test : Form
	{
		public Test()
		{
			InitializeComponent();
			DispatcherHelper.BaseForm = this;
		}
		public void ShowBox(ComponentResponse componentResponse)
		{
			if (componentResponse.Error == 0)
			{
				MessageBox.Show("执行成功");
				return;
			}
			MessageBox.Show(componentResponse.Message);

		}
		private async void btn_catch_Click(object sender, EventArgs e)
		{
			CommponetRequest requestBase = new()
			{
				ComponentName = nameof(CatchElementComponent)
			};
			var json = BsonHelper.ToJson(requestBase);
			var obj = BsonHelper.FromJson<CommponetRequest>(json);
			var result = await RequestManager.StartAsync(json);
			ShowBox(result);
		}

		private async void button1_Click(object sender, EventArgs e)
		{
			CommponetRequest commponetRequest = new()
			{
				ComponentName = "ExecFlow",
				Data = new Flow()
				{
					Id = IdGenerater.Instance.GenerateId(),
					Steps = new List<Node>
						{
						   new Node()
						   {
								ComponentName = nameof(StartComponent)
						   }
						}
				}
			};
			var json = BsonHelper.ToJson(commponetRequest);
			var result = await RequestManager.StartAsync(json);
			ShowBox(result);
		}

		private async void button2_Click(object sender, EventArgs e)
		{
			var flow = new Flow()
			{
				Id = IdGenerater.Instance.GenerateId(),
				Steps = new List<Node>
						{
						   new Node()
						   {
								ComponentName = nameof(StartComponent)
						   }
						}
			};
			//add
			CommponetRequest commponetRequest = new()
			{
				ComponentName = nameof(DesignComponentSystem.CreateRobot),
				Data = flow
			};
			var json = BsonHelper.ToJson(commponetRequest);
			var result = await RequestManager.StartAsync(json);
			commponetRequest = new()
			{
				FlowId = flow.Id,
				Data = new Node()
				{
					ComponentName = nameof(StartComponent)
				}
			};
			json = BsonHelper.ToJson(commponetRequest);
			result = await RequestManager.StartAsync(json);
			ShowBox(result);
		}

		private async void button3_Click(object sender, EventArgs e)
		{
			var commponetRequest = new CommponetRequest()
			{
				ComponentName = nameof(DesignComponentSystem.PreviewNodes),
				Data = new List<Node>()
				{
					new Node()
					{
						ComponentName = nameof(StartComponent)
					}
				}
			};
			var json = BsonHelper.ToJson(commponetRequest);
			var result = await RequestManager.StartAsync(json);
			ShowBox(result);
		}
	}
}
