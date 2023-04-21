using Automation;
using Automation.Com;
using Automation.Inspect;
using Automation.Parser;
using EL;
using EL.Async;
using EL.Basic.Component.Clipboard;
using EL.Http;
using EL.Overlay;
using EL.Sqlite;

namespace WinFromInspect
{
    public partial class InspectWinXinMsg_form : Form
    {
        public static ElementPath elementPath = default;
        public InspectWinXinMsg_form()
        {
            InitializeComponent();
            var sqlComponent = Boot.AddComponent<SqliteComponent>();
            Boot.AddComponent<HttpComponent>();
            try
            {
                sqlComponent.CreateDataFile("WinXinMsg.db");
                sqlComponent.CreateTable("");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Log.Error(ex);
                Environment.Exit(0);
            }

            WeiXinMsgCatchServer.ActionAddMsg = (x) =>
            {
                this.Invoke(() =>
                {
                    this.textBox1.Text = x.Nickname + ": " + x.Msg + "\r\n" + this.textBox1.Text;
                });
            };
        }
        public bool GetMsg()
        {
            IResponse res = default;
            var inspect = Boot.AddComponent<InspectComponent>();
            var json = inspect.GetComponent<ClipboardComponent>().GetFromClipboard();
            if (string.IsNullOrEmpty(json))
            {
                MessageBox.Show("获取返回信息为空字符串！");
                return false;
            }
            try
            {
                res = JsonHelper.FromJson<Response>(json);
                if (res == null)
                    MessageBox.Show("获取返回信息为空对象！");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }

            if (res.Error > 0)
            {
                MessageBox.Show(res.Message);
                return false;
            }
            return true;
        }
        private void btn_ElementCatch_Click(object sender, EventArgs e)
        {
            string param = "10000{\"$type\":\"Automation.Inspect.CatchElementRequest, EL.Automation\",\"RpcId\":1}";
            InspectRequestManager.Start(param);
            //GetMsg();

        }
        private void button7_Click(object sender, EventArgs e)
        {
            var inspect = Boot.AddComponent<InspectComponent>();
            var parser = Boot.GetComponent<ParserComponent>();
            if (!GetMsg())
                return;
            var json = inspect.GetComponent<ClipboardComponent>().GetFromClipboard();
            CatchUIResponse res = JsonHelper.FromJson<CatchUIResponse>(json);
            var y = res.ElementPath;
            elementPath = y;
            if (y == null)
                return;
            pictureBox1.BackgroundImage = WinPathComponentSystem.Base64ToImage(y.Img, new Size(pictureBox1.Width, pictureBox1.Height));
            pictureBox1.SizeMode = PictureBoxSizeMode.Normal;
            txt_Title.Text = y.MainWindowTitle;
            txt_NativeWindowTitle.Text = y.NativeWindowTitle;
            txt_Name.Text = y.Name;
            txt_Path.Text = y.Path;
            txt_ProcessName.Text = y.ProcessName;
            txt_Value.Text = y.Value;
            txt_wh.Text = $"{y.Width}/{y.Height}";
            txt_XY.Text = $"{y.X}/{y.Y}";
            txt_ProgramType.Text = y.ElementType + "";
            txt_ClassName.Text = y.PathNode.CurrentElementWin.ClassName;
            txt_ControlRole.Text = y.PathNode.CurrentElementWin.Role;
        }


        //左键单击
        private async void button1_Click(object sender, EventArgs e)
        {
            var inspect = Boot.GetComponent<InspectComponent>();
            var request = new MouseActionRequest()
            {
                ElementPath = elementPath,
                ActionType = ActionType.Mouse,
                ClickType = ClickType.LeftClick
            };
            await InspectRequestManager.Start($"{(int)RequestType.MouseActionRequest}{JsonHelper.ToJson(request)}");
            GetMsg();
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            var inspect = Boot.GetComponent<InspectComponent>();
            var request = new MouseActionRequest()
            {
                ElementPath = elementPath,
                ActionType = ActionType.Mouse,
                ClickType = ClickType.LeftDoubleClick
            };
            await InspectRequestManager.Start($"{(int)RequestType.MouseActionRequest}{JsonHelper.ToJson(request)}");
            GetMsg();
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            var inspect = Boot.GetComponent<InspectComponent>();
            var request = new MouseActionRequest()
            {
                ElementPath = elementPath,
                ActionType = ActionType.Mouse,
                ClickType = ClickType.RightClick
            };
            await InspectRequestManager.Start($"{(int)RequestType.MouseActionRequest}{JsonHelper.ToJson(request)}");
            GetMsg();
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            var inspect = Boot.GetComponent<InspectComponent>();
            var request = new MouseActionRequest()
            {
                ElementPath = elementPath,
                ActionType = ActionType.ElementEvent
            };
            await InspectRequestManager.Start($"{(int)RequestType.MouseActionRequest}{JsonHelper.ToJson(request)}");
            GetMsg();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var inspect = Boot.GetComponent<InspectComponent>();
            var request = new InputActionRequest()
            {
                ElementPath = elementPath,
                InputType = InputType.ElementInput,
                InputTxt = txt_Content.Text.Trim(),
                IsClear = true
            };
            InspectRequestManager.Start($"{(int)RequestType.InputActionRequest}{JsonHelper.ToJson(request)}");
            GetMsg();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            var inspect = Boot.GetComponent<InspectComponent>();
            var request = new InputActionRequest()
            {
                ElementPath = elementPath,
                InputType = InputType.Keyboard,
                InputTxt = txt_Content.Text.Trim(),
                IsClear = true
            };
            InspectRequestManager.Start($"{(int)RequestType.InputActionRequest}{JsonHelper.ToJson(request)}");
            GetMsg();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            var inspect = Boot.GetComponent<InspectComponent>();
            var request = new ElementActionRequest()
            {
                ElementPath = elementPath,
                TimeOut = 10000
            };
            InspectRequestManager.Start($"{(int)RequestType.ElementActionRequest}{JsonHelper.ToJson(request)}");
            GetMsg();
        }

        private void button9_Click(object sender, EventArgs e)
        {

            if (WeiXinMsgCatchServer.NewMsgRepeatedTimerId == default)
            {
                try
                {
                    WeiXinMsgCatchServer.Main(elementPath);
                    button9.Text = "停止监听！";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    Log.Error(ex);
                }

            }
            else
            {
                Boot.GetComponent<TimerComponent>().Remove(WeiXinMsgCatchServer.NewMsgRepeatedTimerId);
                WeiXinMsgCatchServer.NewMsgRepeatedTimerId = default;
                button9.Text = "开始监听！";
            }
        }

    }
}