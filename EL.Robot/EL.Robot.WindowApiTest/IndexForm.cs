﻿using EL.Robot.Component;
using EL.Robot.Core;
using NPOI.Util;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static EL.Robot.Core.ComponentSystem;

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
                //logsForm.Activate();
                logsForm.LogsAction?.Invoke(x);
            };
            designComponent.RefreshNodeCmdEndAction = () =>
            {
                if (start > 0 && end > 0)
                {
                    rtxt_msg.SelectionStart = start;
                    rtxt_msg.Select(start, end);
                    start = -1;
                    end = -1;
                }

                //HandleFolding(0, rtxt_msg.Text.Length);
            };
            designComponent.RefreshNodeCmdAction = (x, y) =>
            {
                this.Activate();
                if (x.LinkNode != null)
                {
                    CreateFoldButtion(x, y);
                }
                this.Invoke(() =>
                {
                    Color color = Color.Black;
                    if (x.ComponentName == nameof(CommentComponent))
                        color = Color.Green;
                    AppendTextColorfulCmd(y, true, x);
                });
            };
            designComponent.ClearNodeCmdAction = () =>
            {
                rtxt_msg.Controls.Clear();
                this.Invoke(() =>
                {
                    rtxt_msg.Text = "";
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
            rtxt_msg.ReadOnly = true;
            pl_components.Visible = false;
            pl_components.Leave += (x, y) =>
            {
                pl_components.Visible = false;
            };
            var list = designComponent.LoadRobots();
            RefreshRobots(default, false);
            InitRichTextBoxContextMenu(rtxt_msg);
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
                int left = 5;
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
                        int left = 2;
                        int index = 0;
                        int top = 5;
                        foreach (var item in componentConfigs.Data)
                        {
                            if (index % 7 == 0)
                            {
                                left = 2;
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
                //var btn_catchElement = new Button()
                //{
                //    Text = "捕获",
                //    Width = 85,
                //    Height = 22,
                //    Top = 5,
                //    TextAlign = ContentAlignment.MiddleCenter,
                //    Left = left,
                //};
                //btn_catchElement.Click += async (x, y) =>
                //{
                //    CommponetRequest requestBase = new()
                //    {
                //        ComponentName = nameof(CatchElementComponent)
                //    };
                //    var json = BsonHelper.ToJson(requestBase);
                //    var obj = BsonHelper.FromJson<CommponetRequest>(json);
                //    var result = await RequestManager.StartAsync(json);
                //};
                //pl_Category.Controls.Add(btn_catchElement);
            };

        }
        public Node currentNode = default;
        public Node insertNode = default;
        private void TrunRowsId(int iCodeRowsID)
        {
            rtxt_msg.SelectionStart = rtxt_msg.GetFirstCharIndexFromLine(iCodeRowsID);
            rtxt_msg.SelectionLength = 0;
            rtxt_msg.Focus();
            rtxt_msg.ScrollToCaret();
        }
        public void SetParamDic(List<Parameter> parameters)
        {
            if (parameters is null) return;
            for (int i = 0; i < parameters.Count; i++)
            {
                ParamDic.TryGetValue(parameters[i].Key, out var temp);
                parameters[i] = temp;
            }
            foreach (var item in parameters)
                SetParamDic(item.Parameters);
            return;
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
            rtBox.HideSelection = false;
        }
        static int start;
        static int end;
        public void AppendTextColorfulCmd(string addtext, bool IsaddNewLine, Node node)
        {
            //var index = designComponent.CurrentDesignFlow.Steps.IndexOf(node);
            var index = addtext.IndexOf(node.Name) + node.Name.Length;
            if (IsaddNewLine)
                addtext += Environment.NewLine;
            var temp1 = rtxt_msg.Text.Length;
            rtxt_msg.HideSelection = true;
            rtxt_msg.SelectionStart = rtxt_msg.Text.Length;
            rtxt_msg.SelectionFont = new Font("黑体", 11, FontStyle.Underline);
            rtxt_msg.SelectionColor = Color.Blue;
            rtxt_msg.SelectionIndent = 25;
            rtxt_msg.SelectionLength = 0;
            rtxt_msg.AppendText(addtext.Substring(0, index));
            rtxt_msg.SelectionStart = rtxt_msg.TextLength;
            rtxt_msg.SelectionFont = new Font("黑体", 10);
            rtxt_msg.SelectionColor = Color.Black;
            rtxt_msg.SelectionIndent = 25;
            rtxt_msg.SelectionLength = 0;
            if (node.ComponentName == nameof(CommentComponent))
                rtxt_msg.SelectionColor = Color.Green;
            rtxt_msg.AppendText(addtext.Substring(index, addtext.Length - index));
            rtxt_msg.SelectionColor = rtxt_msg.ForeColor;
       
            var temp2 = rtxt_msg.Text.Length;
            if (node.IsNew)
            {
                start = temp1;
                end = temp2 - temp1;
                node.IsNew = false;
            }
        }
        private Node GetSelectNode()
        {
            try
            {
                string indexStr;
                rtxt_msg.SelectionStart = rtxt_msg.GetFirstCharIndexOfCurrentLine();
                var str = rtxt_msg.Text.Substring(rtxt_msg.SelectionStart, 10);
                string pattern = @"^\d+";
                Regex regex = new(pattern);
                var match = regex.Match(str);
                if (!match.Success)
                {
                    WriteLog("当前选中的行信息有误！");
                    return default;
                }
                indexStr = match.Value;
                var id = rtxt_msg.Text.Substring(rtxt_msg.SelectionStart, indexStr.Length);
                if (indexStr != id)
                {
                    WriteLog("不是有效的行号信息！");
                    return default;
                }
                int.TryParse(indexStr, out int index);
                var node = designComponent.CurrentDesignFlow.Steps[index - 1];
                var dis = index + node.DisplayExp.Replace("\r", "");
                var tempDis = rtxt_msg.Text.Substring(rtxt_msg.SelectionStart, dis.Length);
                if (dis != tempDis)
                {
                    WriteLog("不是有效的行号信息！");
                    return default;
                }
                rtxt_msg.Select(rtxt_msg.SelectionStart, dis.Length);
                return node;
            }
            catch (Exception)
            {
                WriteLog("对不起，未识别到操作符");
            }
            return default;
        }
        private bool IsEdit = false;
        private Node EditNode = default;
        //private Node DragNode = default;
        private void InitRichTextBoxContextMenu(RichTextBox textBox)
        {
            rtxt_msg.AllowDrop = true;
            rtxt_msg.DetectUrls = true;
            rtxt_msg.HideSelection= true;
            rtxt_msg.EnableAutoDragDrop = true;
            //rtxt_msg.MouseMove += (x, y) =>
            //{
            //	var charv = rtxt_msg.GetCharIndexFromPosition(rtxt_msg.PointToClient(MousePosition));
            //	rtxt_msg.SelectionStart = charv;
            //	rtxt_msg.SelectionStart = rtxt_msg.GetFirstCharIndexOfCurrentLine();
            //};
            //rtxt_msg.MouseUp += (x, y) =>
            //{
            //	WriteLog(JsonHelper.ToJson(MousePosition));
            //};
            //rtxt_msg.DragDrop += (x, y) =>
            //{
            //    string pattern = @"^\d+";
            //    var index = rtxt_msg.GetCharIndexFromPosition(rtxt_msg.PointToClient(MousePosition));
            //    var line = rtxt_msg.GetLineFromCharIndex(index);
            //    rtxt_msg.SelectionStart = rtxt_msg.GetFirstCharIndexFromLine(line);
            //    var str = rtxt_msg.Text.Substring(rtxt_msg.SelectionStart, 10);
            //    Regex regex = new(pattern);
            //    var match = regex.Match(str);
            //    if (!match.Success)
            //    {
            //        WriteLog("当前选中的行信息有误！");
            //        return;
            //    }
            //    var indexStr = match.Value;
            //    var id = rtxt_msg.Text.Substring(rtxt_msg.SelectionStart, indexStr.Length);
            //    if (indexStr != id)
            //    {
            //        WriteLog("不是有效的行号信息！");
            //        return;
            //    }
            //    int.TryParse(indexStr, out int temp2);
            //    var node = designComponent.CurrentDesignFlow.Steps[temp2 - 1];
            //    var dis = id + node.DisplayExp;
            //    var tempDis = rtxt_msg.Text.Substring(rtxt_msg.SelectionStart, dis.Length);
            //    if (dis != tempDis)
            //    {
            //        WriteLog("不是有效的行号信息！");
            //        return;
            //    }
            //    if (DragNode != null)
            //    {
            //        designComponent.CurrentDesignFlow.Steps.Remove(DragNode);
            //        DragNode.IsNew = true;
            //        var currentIndex = designComponent.CurrentDesignFlow.Steps.IndexOf(node)+1;
            //        designComponent.CurrentDesignFlow.Steps.Insert(currentIndex, DragNode);
            //        designComponent.RefreshAllStepCMD();
            //        DragNode = default;
            //    }
            //};
            //rtxt_msg.DragEnter += (x, y) =>
            //{
            //    DragNode = GetSelectNode();

            //    if (y.Data.GetDataPresent(DataFormats.Text))
            //    {

            //        y.Effect = DragDropEffects.Copy;
            //    }
            //    else
            //    {
            //        y.Effect = DragDropEffects.None;
            //    }
            //};

            //创建右键菜单并将子菜单加入到右键菜单中
            var contextMenu = new ContextMenuStrip();
            //创建剪切子菜单
            ToolStripMenuItem copy = new();
            copy.Text = "复制";
            copy.Click += (x, y) =>
            {
                currentNode = GetSelectNode();
                if (currentNode != null)
                    WriteLog($"已将[{currentNode.DisplayExp}]命令复制成功！");
            };
            ToolStripMenuItem paste = new();
            paste.Text = "粘贴";
            paste.Click += (x, y) =>
            {
                if (currentNode == null)
                {
                    WriteLog("复制的节点不存在！");
                    return;
                }
                var node = GetSelectNode();
                if (node == null) return;
                var index = designComponent.CurrentDesignFlow.Steps.IndexOf(node) + 1;
                var copyObj = JsonHelper.FromJson<Node>(JsonHelper.ToJson(currentNode));
                copyObj.Id = IdGenerater.Instance.GenerateId();
                designComponent.CreateNode(copyObj, index);
                index = rtxt_msg.GetFirstCharIndexOfCurrentLine();
                WriteLog($"已将[{copyObj.DisplayExp}]命令追加到流程了");
                //TrunRowsId(index);
            };
            ToolStripMenuItem update = new ToolStripMenuItem();
            update.Text = "编辑";
            update.Click += (x, y) =>
            {
                EditNode = GetSelectNode();
                if (EditNode == null) return;
                var component = designComponent.GetComponentInfo(EditNode.ComponentName);
                component.GetConfig();
                ClearCurrentComponent();
                CurrentConfig = component.Config;
                var parameters = JsonHelper.FromJson<List<Parameter>>(JsonHelper.ToJson(EditNode.Parameters));
                foreach (var item in parameters)
                    ParamDic.Add(item.Key, item);
                SetParamDic(CurrentConfig.Parameters);
                CreateExp(CurrentConfig);
                GenerateCmd();
                IsEdit = true;
            };
            ToolStripMenuItem delete = new ToolStripMenuItem();
            delete.Text = "删除";
            delete.Click += (x, y) =>
            {
                var node = GetSelectNode();
                if (node == null) return;

                var row = designComponent.CurrentDesignFlow.Steps.IndexOf(node) - 1;
                if (row < designComponent.CurrentDesignFlow.Steps.Count)
                    end = designComponent.CurrentDesignFlow.Steps[row].DisplayExp.Length;
                start = start - end - 1;
                designComponent.CurrentDesignFlow.Steps.Remove(node);
                designComponent.RefreshAllStepCMD();
                WriteLog($"已将[{node.DisplayExp}]命令从当前流程移除");
            };
            ToolStripMenuItem insert = new ToolStripMenuItem();
            insert.Text = "当前插入";
            insert.Click += (x, y) =>
            {
                if (insert.Text == "当前插入")
                {
                    insertNode = GetSelectNode();
                    if (insertNode == null) return;
                    insert.Text = "取消当前";
                    WriteLog($"在[{insertNode.DisplayExp}]命令后插入新节点");
                }
                else
                {
                    insert.Text = "当前插入";
                    insertNode = null;
                }
            };
            contextMenu.Items.Add(copy);
            contextMenu.Items.Add(paste);
            contextMenu.Items.Add(update);
            contextMenu.Items.Add(delete);
            contextMenu.Items.Add(insert);
            textBox.ContextMenuStrip = contextMenu;
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
                var list = designComponent.Features.OrderByDescending(x => x.ViewSort).ToList();
                foreach (var item in list)
                {
                    var robot = new RobotListView(item.Id, item.Name, item.HeadImg);
                    robot.Dock = DockStyle.Top;
                    flp_robotList.Controls.Add(robot);
                }
                return;
                //加载流程
            }
            var list1 = designComponent.Features.OrderByDescending(x => x.ViewSort).ToList();
            foreach (var item in list1)
            {
                var robot = new RobotListView(item.Id, item.Name, item.HeadImg);
                robot.Dock = DockStyle.Top;
                flp_robotList.Controls.Add(robot);
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
                $"【{CurrentConfig.CmdDisplayName}的指令】", Color.MediumBlue, new Font("黑体", 10), false);
            var color = Color.BlueViolet;
            AppendTextColorful(txt_exp, " ->", color, new Font("黑体", 10), false);
            if (ParamDic.Values.Any())
                foreach (var item in ParamDic.Values)
                    AppendTextColorful(txt_exp, item.DisplayExp, color, new Font("黑体", 10), false);
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
            panel.Width = 70;
            panel.Height = 22;
            panel.Top = 10;
            ComboBox comboBox = new()
            {
                DisplayMember = nameof(Parameter.DisplayName),
                Left = 0,
                Width = 70
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
                    panel.Width = 140;
                    var cb = new ComboBox
                    {
                        Name = param.Key,
                        Left = 70,
                        Width = 70
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
            link.Width = 70;
            panel.Controls.Add(link);
            panel.Width = 70;
            if (param.IsInput)
            {
                panel.Width += 70;
                var cb = new ComboBox
                {
                    Left = 70,
                    Width = 70
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
                        valueInfo.Value = response.Data;
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
                WriteLog($"你创建了一个[{name}]流程");
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
            if (IsEdit && EditNode != null)
            {
                for (int i = 0; i < EditNode.Parameters.Count; i++)
                {
                    ParamDic.TryGetValue(EditNode.Parameters[i].Key, out var para);
                    EditNode.Parameters[i] = para;
                }
                EditNode.IsNew = true;
                designComponent.RefreshAllStepCMD();
                IsEdit = false;
                EditNode = default;
                WriteLog($"[{txt}]命令已编辑应用到流程");
                ClearCurrentComponent();
                return;
            }
            try
            {

                //var index = rtxt_msg.SelectionStart;
                if (insertNode != null)
                {
                    var index = designComponent.CurrentDesignFlow.Steps.IndexOf(insertNode) + 1;
                    if (index > 1)
                    {
                        var node = new Node()
                        {
                            ComponentName = CurrentConfig.ComponentName,
                            Id = IdGenerater.Instance.GenerateId(),
                            Name = CurrentConfig.CmdDisplayName,
                            Parameters = ParamDic.Values.ToList()
                        };
                        designComponent.CreateNode(node, index);
                        insertNode = node;
                        WriteLog($"已将[{txt}]命令追加到流程了");
                        ClearCurrentComponent();
                        return;
                    }
                }
                designComponent.CreateNode(new Node()
                {
                    ComponentName = CurrentConfig.ComponentName,
                    Id = IdGenerater.Instance.GenerateId(),
                    Name = CurrentConfig.CmdDisplayName,
                    Parameters = ParamDic.Values.ToList()
                });
                WriteLog($"已将[{txt}]命令追加到流程了");
                ClearCurrentComponent();
                return;
            }
            catch (Exception ex)
            {
                designComponent.WriteFlowLog(ex.Message);
            }

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
            await designComponent.SaveRobot(false);
            this.Close();
            Application.Exit();
        }
        private void CreateFoldButtion(Node node, string display)
        {
            if (node.LinkNode == null)
                return;
            var point = rtxt_msg.GetPositionFromCharIndex(rtxt_msg.SelectionStart);
            //point = rtxt_msg.PointToScreen(point);
            Label btn = new Label();
            btn.Height = 15;
            btn.Width = 20;
            btn.BackColor = Color.Transparent;
            btn.Font = rtxt_msg.Font;
            if (node.IsFold)
            {
                btn.Text = "...";
                btn.Location = new Point(point.X-25, point.Y+2);
            }
            else
            {
                btn.Text = ">" + display;
                btn.Location = new Point(point.X- 25, point.Y+2);
            }
            this.rtxt_msg.Controls.Add(btn);
        }
        // 在 RichTextBox 的 TextChanged 事件中调用此方法
        private void HandleFolding(int start, int end)
        {
            // 用“[+]”和“[-]”标记来表示折叠和展开状态
            string foldStart = "[+]"; string foldEnd = "[-]";
            while (start < end)
            {
                int startIndex = rtxt_msg.Find(foldStart, start, end, RichTextBoxFinds.None);
                int endIndex = rtxt_msg.Find(foldEnd, start, end, RichTextBoxFinds.None);
                if (startIndex != -1 && endIndex != -1 && endIndex > startIndex)
                {
                    // 折叠起始标记和结束标记之间的文本
                    int foldStartIndex = startIndex + foldStart.Length;
                    int foldEndIndex = endIndex - foldEnd.Length;
                    int foldLength = foldEndIndex - foldStartIndex;
                    string foldedText = rtxt_msg.Text.Substring(foldStartIndex, foldLength);
                    // 创建一个可折叠的区域
                    int startLength = startIndex - start;
                    int endLength = end - endIndex;
                    rtxt_msg.Select(start + startLength, endIndex - start - endLength);
                    rtxt_msg.SelectedText = foldStart + foldedText + foldEnd;
                }
                else
                {
                    // 没有更多的折叠标记
                    break;
                }
                // 更新下一次搜索的起始位置
                start = endIndex + foldEnd.Length;
            }
        } // 处理折叠的事件处理程序
        private void Unfold(object sender, EventArgs e)
        {
            // 如果选中的文本是“[+]”或“[-]”，则展开或折叠文本
            if (start < end && (rtxt_msg.Text.Substring(start, 3) == "[+]" || rtxt_msg.Text.Substring(start, 3) == "[-]"))
            {
                if (rtxt_msg.Text.Substring(start, 3) == "[+]")
                {
                    // 展开文本
                    int foldEndIndex = rtxt_msg.Find("[-]", start, end, RichTextBoxFinds.None);
                    if (foldEndIndex != -1)
                    {
                        int foldStartIndex = start + 3;
                        int foldLength = foldEndIndex - foldStartIndex;
                        rtxt_msg.Select(foldStartIndex, foldLength);
                        rtxt_msg.SelectedText = "";
                    }
                }
                else if (rtxt_msg.Text.Substring(start, 3) == "[-]")
                { // 折叠文本
                    int foldStartIndex = rtxt_msg.Find("[+]", start, end, RichTextBoxFinds.None);
                    if (foldStartIndex != -1)
                    {
                        int foldEndIndex = start + rtxt_msg.SelectionLength;
                        int foldLength = foldEndIndex - foldStartIndex - 3;
                        string foldedText = rtxt_msg.Text.Substring(foldStartIndex + 3, foldLength);
                        // 创建一个可折叠的区域
                        rtxt_msg.Select(foldStartIndex, foldEndIndex - foldStartIndex);
                        rtxt_msg.SelectedText = "[+]" + foldedText + "[-]";
                    }
                }
            }
        }
    }
}