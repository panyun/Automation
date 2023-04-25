using EL.Robot.Component;
using EL.Robot.Core;
using EL.Robot;
using System.Security.Cryptography.X509Certificates;
using static EL.Robot.Core.ComponentSystem;
using System.Runtime.InteropServices;
using System.Text;
using Utils;
using Automation.Inspect;
using static Google.Protobuf.Reflection.GeneratedCodeInfo.Types;
using SixLabors.Fonts.Tables.AdvancedTypographic;

namespace EL.Robot.WindowApiTest
{
    public partial class IndexForm : Form
    {
        [DllImport("user32.dll", EntryPoint = "SendMessageA")]
        private static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, StringBuilder lParam);

        [DllImport("user32 ")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);
        public Config CurrentConfig;
        public Dictionary<string, Parameter> ParamDic = new Dictionary<string, Parameter>();
        public DesignComponent designComponent = Boot.GetComponent<RobotComponent>().GetComponent<DesignComponent>();
        public static IndexForm Ins;
        public static LogsForm logsForm = new LogsForm();
        public IndexForm()
        {
            Ins = this;
            InitializeComponent();

            designComponent.RefreshLogMsgAction = (x) =>
            {
                logsForm.LogsAction?.Invoke(x);
            };
            designComponent.RefreshNodeAction = (x, y) =>
            {
                this.Activate();
                this.Invoke(() =>
                {
                    AppendTextColorful(rtxt_msg, y, Color.Black, new Font("黑体", 11), true);
                });

            };
            lbl_name.Text = "";
            logsForm.StartPosition = FormStartPosition.Manual;
            logsForm.Width = this.Width;
            logsForm.Show();
            logsForm.Hide();
            DispatcherHelper.BaseForm = this;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = CenterScreen(this.Width, this.Height);
            txt_exp.Font = new Font("黑体", 10);
            txt_exp.ForeColor = Color.Black;
            pl_components.Visible = false;
            pl_components.Leave += (x, y) =>
            {
                pl_components.Visible = false;
            };
            var list = designComponent.LoadRobots();
            RefreshRobots(default, false);
            this.Load += async (x, y) =>
            {
                CommponetRequest commponetRequest = new CommponetRequest()
                {
                    ComponentName = nameof(ComponentSystem),
                    Data = new ComponentSystemReuqest
                    {

                        Action = nameof(ComponentSystem.GetCategorys)
                    }
                };
                var categorys = await RequestManager.StartAsync(commponetRequest);
                int left = 30;
                foreach (var category in categorys.Data)
                {
                    var btn = new Button()
                    {
                        Text = Helper.GetValue<string>(category, "CategoryName"),
                        Width = 80,
                        Height = 22,
                        Top = 5,
                        //Font = new Font("隶书", 8),
                        TextAlign = ContentAlignment.MiddleCenter,
                        Tag = Helper.GetValue<int>(category, "CategoryId"),
                        Left = left,
                    };
                    btn.Click += async (x, y) =>
                    {
                        pl_components.Controls.Clear();
                        var tag = (x as Button).Tag;
                        CommponetRequest commponetRequest = new CommponetRequest()
                        {
                            ComponentName = nameof(ComponentSystem),
                            Data = new ComponentSystemReuqest
                            {
                                Action = nameof(ComponentSystem.GetComponentsById),
                                Paramters = new Dictionary<string, object>()
                                {
                                    {"Id", tag }
                                }
                            }
                        };
                        var componentConfigs = await RequestManager.StartAsync(commponetRequest);
                        if (componentConfigs.Data is null) return;
                        pl_components.Visible = true;
                        pl_components.Focus();
                        int left = 10;
                        int index = 0;
                        int top = 5;
                        foreach (var item in componentConfigs.Data)
                        {
                            if (index % 7 == 0)
                            {
                                left = 10;
                                top = top + 25 * (int)Math.Floor(index / 7f);
                            }

                            var config = item as Config;
                            var btn = new Button()
                            {
                                Text = config.ButtonDisplayName,
                                Width = 80,
                                Height = 22,
                                //Font = new Font("隶书", 8),
                                Top = top,
                                TextAlign = ContentAlignment.MiddleCenter,
                                Tag = config,
                                Left = left,
                            };

                            pl_components.Controls.Add(btn);
                            left += 80;
                            btn.Click += async (x, y) =>
                            {
                                var tag = (x as Button).Tag as Config;
                                CommponetRequest commponetRequest = new CommponetRequest()
                                {
                                    ComponentName = nameof(ComponentSystem),
                                    Data = new ComponentSystemReuqest
                                    {
                                        Action = nameof(ComponentSystem.GetExpression),
                                        Paramters = new Dictionary<string, object>()
                                        {
                                            {nameof(tag.ComponentName), tag.ComponentName }
                                        }
                                    }
                                };
                                ClearCurrentComponent();
                                CurrentConfig = tag;
                                CreateParamDic(tag.Parameters);
                                CreateExp(config);
                                GenerateCmd();
                            };
                            index++;
                        }
                    };
                    pl_Category.Controls.Add(btn);
                    left += 60 + 30;
                }
                var btn_catchElement = new Button()
                {
                    Text = "捕获",
                    Width = 85,
                    Height = 22,
                    Top = 5,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Left = left,
                };
                btn_catchElement.Click += async (x, y) =>
                {
                    CommponetRequest requestBase = new()
                    {
                        ComponentName = nameof(CatchElementComponent)
                    };
                    var json = BsonHelper.ToJson(requestBase);
                    var obj = BsonHelper.FromJson<CommponetRequest>(json);
                    var result = await RequestManager.StartAsync(json);
                };
                pl_Category.Controls.Add(btn_catchElement);
            };

        }
        public Point CenterScreen(int width, int height)
        {
            //设置窗体位置居中
            return new Point((SystemInformation.PrimaryMonitorSize.Width - width) / 2,
                 (SystemInformation.PrimaryMonitorSize.Height - height) / 2);
        }

        public void RefreshRobots(long flowId, bool isVisible = true)
        {
            panel5.Visible = !isVisible;
            if (!designComponent.Features.Any()) return;
            flp_robotList.Controls.Clear();
            var list = designComponent.Features.OrderByDescending(x => x.ViewSort).ToList();
            foreach (var item in list)
            {
                var robot = new RobotListView(item.Id, item.Name, item.HeadImg);
                robot.Dock = DockStyle.Top;
                flp_robotList.Controls.Add(robot);
            }
            if (isVisible && flowId != default)
            {
                rtxt_msg.Text = "";
                var flow = designComponent.StartDesign(flowId);
                this.Location = CenterScreen(this.Width, this.Height + logsForm.Height);
                logsForm.Location = new Point(this.Left, this.Bottom);
                logsForm.Show();
                if (designComponent.CurrentDesignFlow != null)
                    lbl_name.Text = designComponent.CurrentDesignFlow.Name;
               
                var msgs = designComponent.GetDesignMsg();
                logsForm.LogsClearAction();
                foreach (var item in msgs)
                    logsForm.LogsAction?.Invoke(item);
                return;
                //加载流程

            }

            logsForm.Hide();
            this.Location = CenterScreen(this.Width, this.Height);
            return;
        }
        public void CreateParamDic(List<Parameter> parameters)
        {
            if (parameters is null) return;
            foreach (var item in parameters)
                ParamDic.Add(item.Key, item);
            foreach (var item in parameters)
                CreateParamDic(item.Parameters);
            return;
        }
        public void CreateExp(Config config)
        {
            //var lbl = new Label()
            //{
            //	Text = config.CmdDisplayName + "指令：",
            //	Height = 20,
            //	BackColor = Color.FromArgb(0, Color.Red),
            //	ForeColor = Color.Red,
            //	Left = 5,
            //};
            //pl_cmd.Controls.Add(lbl);
            pl_cmd.Controls.Add(CreateCmd(config.Parameters, pl_cmd.Location.X));
            pl_cmd.AutoScroll = true;
        }
        public void GenerateCmd()
        {
            txt_exp.Text = "";
            AppendTextColorful(txt_exp,
                $"【{CurrentConfig.CmdDisplayName}的指令】", Color.MediumBlue, new Font("黑体", 11), false);
            var color = Color.BlueViolet;
            AppendTextColorful(txt_exp, " ->", color, new Font("黑体", 11), false);
            if (ParamDic.Values.Any())
                foreach (var item in ParamDic.Values)
                    AppendTextColorful(txt_exp, item.DisplayExp, color, new Font("黑体", 11), false);
        }
        public void AppendTextColorful(RichTextBox rtBox, string addtext, Color color, Font font, bool IsaddNewLine)
        {
            if (IsaddNewLine)
            {
                addtext += Environment.NewLine;
            }
            rtBox.SelectionStart = rtBox.TextLength;
            rtBox.SelectionLength = 0;
            rtBox.SelectionFont = font;
            rtBox.SelectionColor = color;
            rtBox.AppendText(addtext);
            rtBox.SelectionColor = rtBox.ForeColor;
        }
        public Control CreateCmd(List<Parameter> parameters, int left)
        {
            if (parameters == null || parameters.Count == 0)
                return null;

            if (parameters.Count == 1)
            {
                var pl = CreateControl(parameters.First(), left);
                pl_cmd.Controls.Add(pl);
                return pl;
            }
            return CreateControl(parameters, left);
        }
        public Panel CreateControl(List<Parameter> parameters, int left)
        {
            Panel panel = new Panel();
            panel.Left = left;
            panel.Width = 80;
            panel.Height = 22;
            panel.Top = 10;
            ComboBox comboBox = new()
            {
                DisplayMember = nameof(Parameter.DisplayName),
                Left = 0,
                Width = 80
            };
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            panel.Controls.Add(comboBox);
            comboBox.Width = panel.Width;

            foreach (var item in parameters)
            {
                comboBox.Items.Add(item);

            }
            comboBox.SelectedValueChanged += (x, y) =>
            {
                var param = comboBox.SelectedItem as Parameter;
                foreach (var item in comboBox.Items)
                    (item as Parameter).IsFinish = false;
                if (param.IsInput)
                {
                    panel.Width = 160;
                    var cb = new ComboBox
                    {
                        Name = param.Key,
                        Left = 80,
                        Width = 80
                    };
                    cb.DropDownStyle = ComboBoxStyle.DropDownList;
                    cb.DisplayMember = nameof(ValueInfo.DisplayName);
                    foreach (var item in param.Values)
                    {
                        cb.Items.Add(item);
                    }
                    panel.Controls.Add(cb);
                    cb.SelectedValueChanged += async (x, y) =>
                    {
                        var temp = cb.PointToScreen(cb.Location);
                        var p = new Point(temp.X - cb.Width, temp.Y + cb.Height);
                        var valueInfo = cb.SelectedItem as ValueInfo;
                        if (valueInfo.ActionType == ValueActionType.RequestList)
                        {
                            var response = await RequestManager.StartAsync(valueInfo.Action);
                            var win = new PopuForm(valueInfo, p, response.Data);
                            var result = win.ShowDialog();
                            if (result == DialogResult.OK)
                                param.Value = win.Value;
                        }
                        if (valueInfo.ActionType == ValueActionType.RequestValue)
                        {
                            var response = await RequestManager.StartAsync(valueInfo.Action);
                            valueInfo.Value = response.Data;
                            param.Value = valueInfo;
                        }
                        if (valueInfo.ActionType == ValueActionType.Input)
                        {
                            var win = new PopuForm(valueInfo, p);
                            var result = win.ShowDialog();
                            if (result == DialogResult.OK)
                                param.Value = win.Value;
                        }
                        if (valueInfo.ActionType != ValueActionType.Value)
                        {
                            param.Value = valueInfo;
                        }
                    };
                    ParamDic[param.Key] = param;
                    GenerateCmd();
                }
                List<Control> cons = new();
                var keys1 = parameters.Select(x => x.Key);
                var keys2 = parameters.SelectMany(x => x.Parameters).Select(x => x.Key);
                var keys = keys1.Concat(keys2);
                foreach (Control item in pl_cmd.Controls)
                {

                    if (keys.Contains(item.Name))
                        cons.Add(item);
                }
                cons.ForEach(x => pl_cmd.Controls.Remove(x));
                pl_cmd.Controls.Add(CreateCmd(param.Parameters, panel.Right));

            };
            comboBox.SelectedIndex = 0;
            return panel;
        }

        public Panel CreateControl(Parameter param, int left)
        {
            Panel panel = new();
            panel.Name = param.Key;
            panel.Left = left;
            panel.Top = 10;
            panel.Height = 22;
            Label link = new LinkLabel();
            var notExist = param.Parameters == null || param.Parameters.Count == 0;
            if (notExist)
                link = new Label();
            link.TextAlign = ContentAlignment.MiddleRight;
            link.Tag = param;
            link.Text = param.DisplayName + ":";
            link.Width = 80;
            panel.Controls.Add(link);
            panel.Width = 80;
            if (param.IsInput)
            {
                panel.Width += 90;
                var cb = new ComboBox
                {
                    Left = 80,
                    Width = 80
                };
                cb.DropDownStyle = ComboBoxStyle.DropDownList;
                cb.DisplayMember = nameof(ValueInfo.DisplayName);
                foreach (var item in param.Values)
                {
                    cb.Items.Add(item);
                }
                cb.SelectedValueChanged += async (x, y) =>
                {
                    var temp = cb.PointToScreen(cb.Location);
                    var p = new Point(temp.X - cb.Width, temp.Y + cb.Height);
                    var valueInfo = cb.SelectedItem as ValueInfo;
                    if (valueInfo.ActionType == ValueActionType.RequestList)
                    {
                        var response = await RequestManager.StartAsync(valueInfo.Action);
                        if (response.Data is List<string> listStr)
                        {
                            var popu = new PopuForm(valueInfo, p, listStr);
                            var result = popu.ShowDialog();
                            if (result == DialogResult.OK)
                            {
                                param.Value = popu.Value;
                            }

                        }
                    }
                    if (valueInfo.ActionType == ValueActionType.RequestValue)
                    {
                        var response = await RequestManager.StartAsync(valueInfo.Action);
                        valueInfo.Value= response.Data;
                        param.Value = valueInfo;
                    }
                    if (valueInfo.ActionType == ValueActionType.Input)
                    {
                        var popu = new PopuForm(valueInfo, p);
                        var result = popu.ShowDialog();
                        if (result == DialogResult.OK)
                            param.Value = popu.Value;
                    }
                    if (valueInfo.ActionType == ValueActionType.Value)
                    {
                        param.Value = valueInfo;
                    }
                    param.IsFinish = true;
                    ParamDic[param.Key] = param;
                    this.Invoke(() => { GenerateCmd(); });
                };
                //cb.Click += (x, y) =>
                //{
                //	pl_key.Controls.Clear();
                //	foreach (var val in param.Values)
                //	{
                //		var lin = new LinkLabel()
                //		{
                //			Tag = param,
                //			Text = param.DisplayName,
                //			Width = 80
                //		};
                //		pl_key.Controls.Add(lin);
                //		lin.Click += (x, y) =>
                //		{
                //			cb.Tag = val;
                //		};
                //	}
                //};
                panel.Controls.Add(cb);
            }
            if (!notExist)
            {
                link.Click += (x, y) =>
                {
                    pl_cmd.Controls.Add(CreateCmd(param.Parameters, panel.Right));
                };
            }
            return panel;
        }




        private void txt_exp_TextChanged(object sender, EventArgs e)
        {
            //var p = GetCursorPos();
            //pl_key.Visible = true;
            //var y = (int)((p.X * 20) + txt_exp.Top - (p.X * 1.5f));
            //if (y > txt_exp.Top + txt_exp.Height)
            //{
            //	y = txt_exp.Top + txt_exp.Height;
            //}
            ////查询字典
            //pl_key.Location = new Point(pl_key.Location.X, y);
        }
        //获取位置的函数
        private Point GetCursorPos()
        {
            int EM_LINEINDEX = 0xBB;
            int EM_LINEFROMCHAR = 0xC9;
            int EM_GETSEL = 0xB0;
            int EM_GETLINE = 0xC4;
            int i = 0, j = 0, k = 0;
            int lParam = 0, wParam = 0;
            i = (int)SendMessage(txt_exp.Handle, EM_GETSEL, wParam, lParam);
            j = i / 65536;
            int lineNo = (int)SendMessage(txt_exp.Handle, EM_LINEFROMCHAR, j, lParam) + 1;
            k = (int)SendMessage(txt_exp.Handle, EM_LINEINDEX, -1, lParam);
            int colNo = j - k + 1;
            Point ret = new Point(lineNo, colNo);
            return ret;
        }

        private async void lbl_exec_Click(object sender, EventArgs e)
        {
            var txt = txt_exp.Text;
            if (txt.Length == 0)
            {
                WriteLog("你没有生成指令，我不能为你服务！");
                return;
            }
            var node = GetCurrentNode();
            var logs = await designComponent.PreviewNodes(new List<Node>() { node });
            WriteLog(logs, "恭喜哟，你预览的节点成功执行！");

        }

        public Node GetCurrentNode()
        {
            return new Node()
            {
                ComponentName = CurrentConfig.ComponentName,
                Id = IdGenerater.Instance.GenerateId(),
                Parameters = ParamDic.Values.ToList(),
            };
        }

        private void btn_add_Click(object sender, EventArgs e)
        {
            var result = new AddRobotForm().ShowDialog();
            if (result == DialogResult.OK)
            {
                var name = designComponent.CurrentDesignFlow.Name;
                lbl_name.Text = name;
                RefreshRobots(designComponent.CurrentDesignFlow.Id);
                WriteLog($"你已经创建了一个[{name}]");
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            var txt = txt_exp.Text;
            if (txt.Length == 0)
            {
                WriteLog("你没有生成指令，我不能为你追加到我的指令集！");
                return;
            }
            try
            {
                designComponent.CreateNode(new Node()
                {
                    ComponentName = CurrentConfig.ComponentName,
                    Id = IdGenerater.Instance.GenerateId(),
                    Name = CurrentConfig.ButtonDisplayName,
                    Parameters = ParamDic.Values.ToList()
                });
            }
            catch (Exception ex)
            {
                designComponent.WriteFlowLog(ex.Message);
            }
            WriteLog($"已将[{txt}]命令追加到流程了");
            ClearCurrentComponent();
        }

        private async void btn_run_Click(object sender, EventArgs e)
        {
            var logs = await designComponent.RunRobot();
            WriteLog(logs, "恭喜哟，你成功运行了执行！");
        }
        private void WriteLog(string msg)
        {
            designComponent.WriteLog(msg);
        }
        private void WriteLog(List<ExecLog> logs, string msg)
        {
            if (logs is null) return;
            var errorLogs = logs.Where(x => x.IsException).ToList();

            if (!errorLogs.Any())
            {
                designComponent.WriteFlowLog(msg);
                return;
            }
            designComponent.WriteFlowLog("你的流程执行失败，以下是失败消息！");
            foreach (var item in errorLogs)
            {
                designComponent.WriteFlowLog(item.Msg);
                return;
            }
        }
        public void ClearCurrentComponent()
        {
            CurrentConfig = null;
            ParamDic.Clear();
            txt_exp.Text = "";
            pl_cmd.Controls.Clear();
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            await designComponent.SaveRobot();
            this.Close();
            Application.Exit();
        }
    }
}