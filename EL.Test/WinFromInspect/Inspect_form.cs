using Automation;
using Automation.Inspect;
using Automation.Parser;
using EL;
using EL.Async;
using EL.Basic.Component.Clipboard;
using EL.Capturing;
using EL.Overlay;
using EL.Sqlite;

namespace WinFromInspect
{
    public partial class Inspect_form : Form
    {
        public static ElementPath elementPath = default;

        public Inspect_form()
        {

            InitializeComponent();
            //var sqlComponent = Boot.AddComponent<ElSqliteComponent>();
            //var entityList = sqlComponent.GetEntitys<ElementPath>($"SELECT * FROM ElementPath ORDER BY Name");

        }
        public bool GetMsg(IResponse res)
        {
            if (res.Error > 0)
            {
                MessageBox.Show(res.Message);
                return false;
            }
            return true;
        }
        private async void btn_ElementCatch_Click(object sender, EventArgs e)
        {
            CatchUIRequest catchElementRequest = new CatchUIRequest()
            {
                Msg = "打开界面探测器"
            };
            var res = (CatchUIResponse)await InspectRequestManager.StartAsync(catchElementRequest);
            var json = JsonHelper.ToJson(res.ElementPath);
            elementPath = JsonHelper.FromJson<ElementPath>(json);
            var y = res.ElementPath;
            if (res.ElementPath == default || res.Error > 0)
            {
                MessageBox.Show(res.Message);
                return;
            }
            this.Invoke(() =>
            {
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
                txt_ClassName.Text = y.PathNode.CurrentElementWin != null ? y.PathNode.CurrentElementWin.ClassName : "";
                txt_ControlRole.Text = y.ControlType;
                if (y.PathNode.CurrentElementWin != null)
                {
                    textBox2.Text = y.PathNode.CurrentElementWin.HelpText;
                    textBox4.Text = y.PathNode.CurrentElementWin.Described;
                    textBox3.Text = y.PathNode.CurrentElementWin.Labeled;
                }
                textBox1.Text = y.Text;
                if (y.PathNode.CurrentElementPlaywright != null)
                {
                    textBox6.Text = y.PathNode.CurrentElementPlaywright.WindowUrl;
                    textBox7.Text = y.PathNode.CurrentElementPlaywright.IdXpath;
                }

                SetForeground();
            });

        }


        //左键单击
        private async void button1_Click(object sender, EventArgs e)
        {
            var request = new MouseActionRequest()
            {
                ElementPath = elementPath,
                ActionType = ActionType.Mouse,
                ClickType = ClickType.LeftClick
            };
            var res = await InspectRequestManager.StartAsync(request);
            GetMsg(res);
            SetForeground();
        }
        private void SetForeground()
        {
            this.Invoke(new Action(() =>
            {
                this.Handle.SetForeground();
                this.Activate();
            }));

        }
        private async void button2_ClickAsync(object sender, EventArgs e)
        {
            var inspect = Boot.GetComponent<InspectComponent>();
            var request = new MouseActionRequest()
            {
                ElementPath = elementPath,
                ActionType = ActionType.Mouse,
                ClickType = ClickType.LeftDoubleClick
            };
            var res = await InspectRequestManager.StartAsync(request);
            GetMsg(res);
            SetForeground();
        }

        private async void button3_ClickAsync(object sender, EventArgs e)
        {
            var json = JsonHelper.ToJson(elementPath);
            var path = JsonHelper.FromJson<ElementPath>(json);

            ElementActionRequest elementActionRequest = new ElementActionRequest()
            {
                ElementPath = path
            };

            var json1 = $"{(int)RequestType.ElementActionRequest}{JsonHelper.ToJson(elementActionRequest)}";
            var res1 = (ElementActionResponse)await InspectRequestManager.StartAsync(elementActionRequest);

            json = JsonHelper.ToJson(res1.ElementPath);
            path = JsonHelper.FromJson<ElementPath>(json);

            var inspect = Boot.GetComponent<InspectComponent>();
            var request = new MouseActionRequest()
            {
                ElementPath = path,
                ActionType = ActionType.Mouse,
                ClickType = ClickType.RightClick
            };
            var res = await InspectRequestManager.StartAsync(request);
            GetMsg(res);
            SetForeground();
        }

        private async void button4_ClickAsync(object sender, EventArgs e)
        {
            var inspect = Boot.GetComponent<InspectComponent>();
            var request = new MouseActionRequest()
            {
                ElementPath = elementPath,
                ActionType = ActionType.ElementEvent
            };
            var res = await InspectRequestManager.StartAsync(request);
            GetMsg(res);
            SetForeground();
        }

        private async void button5_ClickAsync(object sender, EventArgs e)
        {
            var inspect = Boot.GetComponent<InspectComponent>();
            var request = new InputActionRequest()
            {
                ElementPath = elementPath,
                InputType = InputType.ElementInput,
                InputTxt = txt_Content.Text.Trim(),
                IsClear = true
            };
            var res = await InspectRequestManager.StartAsync(request);
            GetMsg(res);
            SetForeground();
        }

        private async void button6_ClickAsync(object sender, EventArgs e)
        {
            var request = new InputActionRequest()
            {
                ElementPath = elementPath,
                InputType = InputType.Keyboard,
                InputTxt = txt_Content.Text.Trim(),
                IsClear = true
            };
            var res = await InspectRequestManager.StartAsync(request);
            GetMsg(res);
            SetForeground();
        }

        private async void button8_ClickAsync(object sender, EventArgs e)
        {
            var inspect = Boot.GetComponent<InspectComponent>();
            var request = new ElementActionRequest()
            {
                ElementPath = elementPath,
                TimeOut = 10000,
                LightProperty = new LightProperty()
                {
                    Count = 3,
                    Time = 500
                }
            };
            var res = await InspectRequestManager.StartAsync(request);
            GetMsg(res);
            SetForeground();
        }
        private async void button9_ClickAsync(object sender, EventArgs e)
        {
            var inspect = Boot.GetComponent<InspectComponent>();
            var nodes = elementPath.ElementEditNodes;
            //nodes.Skip(10).First().GetProperty("Name").Value = "邱玲";
            var request = new ElementVerificationActionRequest()
            {
                ElementPath = elementPath,
                TimeOut = default,
                ElementEditNodes = nodes,
                LightProperty = new LightProperty()
                {
                    Time = 500,
                    Count = 3
                }
            };
            var res = await InspectRequestManager.StartAsync(request);
            GetMsg(res);
            SetForeground();
        }

        private async void button10_ClickAsync(object sender, EventArgs e)
        {
            var inspect = Boot.GetComponent<InspectComponent>();
            var request = new InputActionRequest()
            {
                ElementPath = elementPath,
                InputType = InputType.Paste,
                InputTxt = txt_Content.Text.Trim(),
                IsClear = true
            };
            var res = await InspectRequestManager.StartAsync(request);
            GetMsg(res);
            SetForeground();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            var inspect = Boot.GetComponent<InspectComponent>();
            //var javaInspect = inspect.GetComponent<JavaFormInspectComponent>();
            //javaInspect.EnumJvms().Coroutine();
        }

        private async void button12_ClickAsync(object sender, EventArgs e)
        {
            string param = "10005{\"RpcId\":1}";
            var res = await InspectRequestManager.StartAsync(param);
            GetMsg(res);
            SetForeground();
        }
        private ElementPath SimilarElement { get; set; }
        private async void button13_ClickAsync(object sender, EventArgs e)
        {
            GenerateCosineSimilarActionRequest request = new GenerateCosineSimilarActionRequest()
            {
                CosineValue = numericUpDown1.Value == 0 ? 0.5F : (double)numericUpDown1.Value,
                ElementPath = elementPath,
                LightProperty = new LightProperty()
                {
                    Count = 3,
                    Time = 1000
                }
            };
            var res = (GenerateCosineSimilarActionResponse)await InspectRequestManager.StartAsync(request);
            GetMsg(res);
            var inspect = Boot.AddComponent<InspectComponent>();
            MessageBox.Show($"查找到相似元素{res.Count}条！");
            var json = JsonHelper.ToJson(res.ElementPath);
            var path = JsonHelper.FromJson<ElementPath>(json);
            SimilarElement = path;
            SetForeground();
        }

        private async void button14_Click(object sender, EventArgs e)
        {
            new DataTableForm().ShowDialog();

        }

        private async void button7_Click(object sender, EventArgs e)
        {

            CosineSimilarElementActionRequest request1 = new CosineSimilarElementActionRequest()
            {
                ElementPath = SimilarElement,
                IsElementPath = true,
            };
            var resExec = (CosineSimilarElementActionResponse)await InspectRequestManager.StartAsync(request1);
            if (resExec.Error == 0)
            {
                foreach (var item in resExec.ElementPaths)
                {
                    var request = new MouseActionRequest()
                    {
                        ElementPath = item,
                        ActionType = ActionType.Mouse,
                        ClickType = ClickType.LeftClick
                    };
                    var res = await InspectRequestManager.StartAsync(request);
                }

            }
            SetForeground();
        }

        private async void button15_Click(object sender, EventArgs e)
        {
            CatchUIRequest catchElementRequest = new CatchUIRequest()
            {
                Msg = "捕获相似元素"
            };
            var res = (CatchUIResponse)await InspectRequestManager.StartAsync(catchElementRequest);

            GenerateSimilarElementActionRequest generateSimilarElementActionRequest = new GenerateSimilarElementActionRequest()
            {
                ElementPath = elementPath,
                LastElementPath = res.ElementPath,
            };
            var res1 = (GenerateSimilarElementActionResponse)await InspectRequestManager.StartAsync(generateSimilarElementActionRequest);
            var json = JsonHelper.ToJson(res1.ElementPath);
            var path = JsonHelper.FromJson<ElementPath>(json);
            SimilarElement = path;
            SetForeground();
        }

        private async void button16_Click(object sender, EventArgs e)
        {
            var json = JsonHelper.ToJson(SimilarElement);
            var path = JsonHelper.FromJson<ElementPath>(json);
            SimilarElementActionRequest request1 = new()
            {
                ElementPath = path,
                IsElementPath = true,
            };
            var resExec = (SimilarElementActionResponse)await InspectRequestManager.StartAsync(request1);
            foreach (var item in resExec.ElementPaths)
            {
                var request = new MouseActionRequest()
                {
                    ElementPath = item,
                    ActionType = ActionType.Mouse,
                    ClickType = ClickType.LeftClick
                };
                var res = await InspectRequestManager.StartAsync(request);
            }
            SetForeground();
        }

        private void btn_Playwright_Click(object sender, EventArgs e)
        {
            new PlaywrightForm().Show();
        }

        private async void button17_Click(object sender, EventArgs e)
        {
            var url = textBox5.Text;
            OpenBrowserActionRequest request = new OpenBrowserActionRequest()
            {
                BrowserType = BrowserType.Msedge,
                Url = url
            };
            var res = await InspectRequestManager.StartAsync(request);
            SetForeground();
        }

        private async void button18_Click(object sender, EventArgs e)
        {
            var url = textBox5.Text;
            OpenBrowserActionRequest request = new OpenBrowserActionRequest()
            {
                BrowserType = BrowserType.Chromium,
                Url = url
            };
            var res = await InspectRequestManager.StartAsync(request);
            SetForeground();
        }

        private void button19_Click(object sender, EventArgs e)
        {
            CaptureImageTool captureImageTool = new CaptureImageTool();

            if (captureImageTool.ShowDialog() == DialogResult.OK)
            {

            }

        }
    }
}