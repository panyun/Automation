using Automation.Parser;
using EL;
using EL.Async;
using EL.Basic.Component.Clipboard;
using EL.Sqlite;
using System.Windows.Forms;

namespace Automation.Inspect
{
    public partial class InspectChat : Form
    {
        public static ElementPath elementPath = default;
        public static StartMsgActionRequest Param;
        public static StartMsgActionResponse IdentifyMsgActionResponse;
        public static CatchWXIdentifyMsgAction Current;
        public InspectChat(CatchWXIdentifyMsgAction temp)
        {
            InitializeComponent();
            temp.ActionAddMsg += (x) =>
            {
                this.Invoke(() =>
                {
                    this.textBox1.Text = x.Nickname + ": " + x.Message + "\r\n" + this.textBox1.Text;
                });
            };
            this.FormClosed += (x, y) =>
            {
                temp.Dispose();
            };
        }

        private async void button9_Click(object sender, EventArgs e)
        {

        }
    }
}
