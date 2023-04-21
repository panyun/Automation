using Automation.Parser;
using EL;
using EL.Async;
using EL.Basic.Component.Clipboard;
using EL.Sqlite;
using System.Windows.Forms;

namespace Automation.Inspect
{
    public partial class InspectChatStart : Form
    {
        public static ElementPath elementPath = default;
        public InspectChatStart()
        {
            InitializeComponent();
            this.Load += InspectChat_Load;
        }



        private void InspectChat_Load(object? sender, EventArgs e)
        {

        }

        private async void button12_Click(object sender, EventArgs e)
        {
            CatchUIRequest catchElementChatRequest = new CatchUIRequest()
            {
                CatchType = CatchType.WxChat,
                Msg = "捕获聊天窗口开始"
            };
            var res = (CatchUIResponse)await InspectRequestManager.StartAsync(catchElementChatRequest);
            elementPath = res.ElementPath;
        }

        private async void button9_Click(object sender, EventArgs e)
        {
            var requestSql = new StartMsgActionRequest()
            {
                ElementPath = elementPath,
                OutType = OutType.Sqlite,
                Params = new Dictionary<string, string>()
                    {
                        { "ConnectionString", @"Data Source=D:\Work Space\c-automation\EL.Bin\WinFromInspect\x64\Debug\net47\SQLite\WinXinMsg.db;Mode=ReadWriteCreate" }
                    }
            };
            var requestHttp = new StartMsgActionRequest()
            {
                ElementPath = elementPath,
                OutType = OutType.HttpApi,
                Params = new Dictionary<string, string>()
                    {
                        { "url", @"http://wunlzt.cdwh.gov.cn/apis/events/0595a674ca32c1ef24cd66a3607b66f1" }
                    }
            };
            var res = await InspectRequestManager.StartAsync(requestSql);
            return;


            //var requestHttp = new IdentifyMsgActionRequest()
            //{
            //    ElementPath = elementPath,
            //    IdentifyMsgAction = new IdentifyMsgAction()
            //    {
            //        OutType = OutType.HttpApi,
            //        Params = new Dictionary<string, string>()
            //        {
            //            { "type", @"post" },
            //            { "url",  @"http://wunlzt.cdwh.gov.cn/apis/events/0595a674ca32c1ef24cd66a3607b66f1" },
            //            { "msg_type","json"},
            //            {"header","" }
            //        }
            //    }
            //};

        }
    }
}
