using EL.Async;
using EL.Robot.Component;
using EL.Robot.Component.DTO;
using EL.Robot.Core.SqliteEntity;
using NPOI.HSSF.Record;
using NPOI.SS.Formula.Functions;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Bcpg.Sig;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Text;

namespace EL.Robot.Core
{
    public static class DesignComponentSystem
    {
        public static Flow CreateRobot(this DesignComponent self, Flow inFlow)
        {
            inFlow.Id = IdGenerater.Instance.GenerateId();
            Node startNode = new()
            {
                Id = IdGenerater.Instance.GenerateId(),
                ComponentName = nameof(StartComponent),
                Name = "开始流程"
            };
            Node endNode = new()
            {
                Id = IdGenerater.Instance.GenerateId(),
                ComponentName = nameof(EndComponent),
                Name = "结束流程"
            };
            inFlow.Steps.Add(JsonHelper.FromJson<Node>(JsonHelper.ToJson(startNode)));
            inFlow.Steps.Add(JsonHelper.FromJson<Node>(JsonHelper.ToJson(endNode)));
            inFlow.CreateDate = TimeHelper.ServerNow();
            inFlow.ViewSort = TimeHelper.ServerNow();
            self.DesignFlowDic.Add(inFlow.Id, inFlow);
            self.CurrentDesignFlow = inFlow;
            self.Features.Add(new Feature()
            {
                Id = inFlow.Id,
                Name = inFlow.Name,
                ViewSort = inFlow.ViewSort,
                CreateDate = inFlow.CreateDate,
                HeadImg = inFlow.HeadImg
            });
            return inFlow;
        }
        public static Flow CreateRobot(this DesignComponent self, Feature feature)
        {
            Flow inFlow = new()
            {
                Id = IdGenerater.Instance.GenerateId(),
                DesignSteps = new List<Node>(),
                Name = feature.Name,
                Note = feature.Note,
            };
            Node startNode = new()
            {
                Id = IdGenerater.Instance.GenerateId(),
                ComponentName = nameof(StartComponent),
                Name = "开始流程"
            };
            Node endNode = new()
            {
                Id = IdGenerater.Instance.GenerateId(),
                ComponentName = nameof(EndComponent),
                Name = "结束流程"
            };
           
            inFlow.Steps.Add(JsonHelper.FromJson<Node>(JsonHelper.ToJson(startNode)));
            inFlow.Steps.Add(JsonHelper.FromJson<Node>(JsonHelper.ToJson(endNode)));
            inFlow.CreateDate = TimeHelper.ServerNow();
            inFlow.ViewSort = TimeHelper.ServerNow();
            self.DesignFlowDic.Add(inFlow.Id, inFlow);
            self.CurrentDesignFlow = inFlow;
            feature.Id = inFlow.Id;
            self.Features.Add(feature);
            feature.CreateDate = inFlow.CreateDate;
            feature.ViewSort = inFlow.ViewSort;
            return inFlow;
        }
        public static ComponentResponse CreateRobot(this DesignComponent self, CommponetRequest requst)
        {
            var tempFlow = requst.Data as Flow;
            var flow = self.CreateRobot(tempFlow);
            return new ComponentResponse() { Data = flow };
        }
        public static async ELTask SaveRobot(this DesignComponent self, bool isAll = true)
        {
            foreach (var flowKey in self.DesignFlowDic)
            {
                var flow = flowKey.Value as Flow;
                string flowData = JsonHelper.ToJson(flow);
                var features = new Feature()
                {
                    Id = flow.Id,
                    Name = flow.Name,
                    HeadImg = flow.HeadImg,
                    CreateDate = flow.CreateDate,
                    ViewSort = flow.ViewSort,
                    Note = flow.Note
                };
                string featuresData = JsonHelper.ToJson(features);
                var logs = self.LogMsgs.Where(x => x.Id == flow.Id).ToList();
                string log = JsonHelper.ToJson(logs);
                await RobotDataManagerService.SaveFlow(flow.Id, flowData, featuresData, log);
            }

            //var flow = self.CurrentDesignFlow;
            //string flowData = JsonHelper.ToJson(self.CurrentDesignFlow);
            //var features = new Feature()
            //{
            //    Id = flow.Id,
            //    Name = flow.Name,
            //    HeadImg = flow.HeadImg,
            //    CreateDate = flow.CreateDate,
            //    ViewSort = flow.ViewSort,
            //    Note = flow.Note
            //};
            //string featuresData = JsonHelper.ToJson(features);
            //var logs = self.LogMsgs.Where(x => x.Id == flow.Id).ToList();
            //string log = JsonHelper.ToJson(logs);
            //return await RobotDataManagerService.SaveFlow(flow.Id, flowData, featuresData, log);
        }
        public static List<Feature> LoadRobots(this DesignComponent self)
        {
            self.FlowDatas = RobotDataManagerService.LoadFlows();
            List<Feature> features = new();
            if (self.FlowDatas is null) return features;
            self.FlowDatas.ForEach(x =>
            {
                var feature = JsonHelper.FromJson<Feature>(x.Features);
                features.Add(feature);
            });
            self.Features = features;
            return features;
        }
        public static Flow StartDesign(this DesignComponent self, long Id)
        {
            self.DesignFlowDic.TryGetValue(Id, out Flow flow);
            if (flow == null)
            {
                var flowData = self.FlowDatas.FirstOrDefault(x => x.Id == Id);
                flow = JsonHelper.FromJson<Flow>(flowData.Content);
                self.DesignFlowDic.Add(flow.Id, flow);
                var logs = JsonHelper.FromJson<List<DesignMsg>>(flowData.DesignMsg);
                if (logs is not null)
                    self.LogMsgs.AddRange(logs);
            }
            self.CurrentDesignFlow = flow;
            self.LoadVariable(self.CurrentDesignFlow.Steps);
            self.CurrentDesignFlow.ViewSort = TimeHelper.ServerNow();
            var fea = self.Features.FirstOrDefault(x => x.Id == Id);
            fea.ViewSort = TimeHelper.ServerNow();
            return flow;
        }
        private static void Get(this DesignComponent self, Node node)
        {
            var steps = self.CurrentDesignFlow.DesignSteps.Where(x => x.DesignParent == node);
            foreach (var item in steps)
            {
                item.DesignParent = null;
                node.Children.Add(item);
                self.Get(item);
            }

        }

        private static void GetDesignSteps(this DesignComponent self, List<Node> steps, Node node)
        {
            foreach (var item in steps)
            {
                if (node != null)
                    item.DesignParent = node;
                self.CurrentDesignFlow.DesignSteps.Add(item);
                self.GetDesignSteps(item.Children, item);
                item.Children.Clear();
            }
        }
        public static void RefreshAllStepCMD(this DesignComponent self)
        {
            self.ClearNodeCmdAction?.Invoke();
            string[] strs = new string[self.CurrentDesignFlow.DesignSteps.Count];
            foreach (var step in self.CurrentDesignFlow.DesignSteps)
            {
                var index = self.CurrentDesignFlow.DesignSteps.IndexOf(step);
                strs[index] = (index + 1) + step.DisplayExp;
            }
            self.RefreshNodeCmdAction?.Invoke(strs);
            self.RefreshNodeCmdEndAction?.Invoke();
        }
        public static BaseComponent GetComponentInfo(this DesignComponent self, string componentName)
        {
            return self.NodeComponentRoot.FindComponent(componentName);
        }
        public static ComponentResponse StartDesign(this DesignComponent self, CommponetRequest requst)
        {
            var tempFlow = (long)requst.Data;
            var flow = self.StartDesign(tempFlow);
            return new ComponentResponse() { Data = flow };
        }
        public static List<DesignMsg> GetDesignMsg(this DesignComponent self)
        {
            return self.LogMsgs.Where(x => x.Id == self.CurrentDesignFlow.Id).ToList();
        }
        public static void LoadVariable(this DesignComponent self, List<Node> nodes)
        {
            foreach (var node in nodes)
            {
                if (node.Parameters is null) return;
                var parameter = node.Parameters.FirstOrDefault(x => x.Key == nameof(Node.OutParameterName));
                if (parameter != null && parameter.Value != null && !self.CurrentDesignFlow.ParamsManager.ContainsKey(parameter.Value.Value + ""))
                {
                    self.CurrentDesignFlow.SetFlowParam(parameter.Value.Value + "", null);
                    if (node.ComponentName == nameof(SetVariableComponent))
                    {
                        var parameterValue = node.Parameters.FirstOrDefault(x => x.Key == nameof(SetVariableComponent.VariableValue));
                        self.CurrentDesignFlow.SetFlowParam(parameter.Value.Value + "", parameterValue.Value);
                    }
                }
                if (parameter is not null && parameter.Key == nameof(Node.OutParameterName))
                    node.OutParameterName = (string)parameter.Value.Value;
                if (node.Children.Any())
                    self.LoadVariable(node.Children);
            }
        }
        public static List<string> SelectVariable(this DesignComponent self, List<Type> types)
        {
            var keys = new List<string>();
            if (self.CurrentDesignFlow == null) return new List<string>();
            if (self.CurrentDesignFlow.ParamsManager != null)
                foreach (var item in self.CurrentDesignFlow.ParamsManager)
                {
                    if (item.Value == null || item.Value.Type == default) continue;
                    if (types.Contains(item.Value.Type))
                        keys.Add(item.Key);
                }
            return keys;
        }
        public static ComponentResponse SelectVariable(this DesignComponent self, SelectVariableRequest requst)
        {
            var data = self.SelectVariable(requst.Types);
            return new ComponentResponse() { Data = data };
        }
        public static void WriteDesignLog(this DesignComponent self, string msg)
        {
            self.WriteDesignLog(msg, false);
        }
        public static void WriteDesignLog(this DesignComponent self, string msg, bool isException = false)
        {
            long id = default;
            if (self.CurrentDesignFlow != null) id = self.CurrentDesignFlow.Id;
            var flow = Boot.GetComponent<RobotComponent>().GetComponent<FlowComponent>().MainFlow;
            var entity = new DesignMsg(id, msg, isException);
            //self.LogMsgs.Add(entity);
            self.RefreshLogMsgAction?.Invoke(entity);
        }
        public static List<Node> CreateNode(this DesignComponent self, Node node)
        {
            if (self.CurrentDesignFlow == null)
            {
                self.WriteDesignLog("对不起，请先打开机器人哟！");
                return default;
            }
            return self.CreateNode(node, self.CurrentDesignFlow.DesignSteps.Count);
        }
        public static List<Node> CreateNode(this DesignComponent self, Node node, int index)
        {
            if (self.CurrentDesignFlow == null)
            {
                self.WriteDesignLog("对不起，请先打开机器人哟！");
                return default;
            }

            node.IsNew = true;
            var parent = self.FindParent(index);
            node.DesignParent = parent;
            self.CurrentDesignFlow.DesignSteps.Insert(index, node);
            List<Node> nodes = new List<Node>();
            nodes.Add(node);
            if (node.ComponentName == nameof(IFStartComponent))
            {
                var component = self.GetComponentInfo(nameof(BlockEndComponent));
                component.GetConfig();
                var temp = new Node()
                {
                    Id = IdGenerater.Instance.GenerateId(),
                    Name = component.Config.CmdDisplayName,
                    ComponentName = component.Config.ComponentName,
                    DesignParent = parent
                };
                nodes.Add(temp);
                node.LinkNode = temp;
                self.CurrentDesignFlow.DesignSteps.Insert(index + 1, temp);
            }
            // self.RefreshAllStepCMD();
            var parameter = node.Parameters.FirstOrDefault(x => x.Key == nameof(Node.OutParameterName));
            if (parameter == null || parameter.Value == null)
                return nodes;
            self.CurrentDesignFlow.SetFlowParam(parameter.Value.Value + "", null);
            if (node.ComponentName.ToLower() == nameof(SetVariableComponent).ToLower())
            {
                var parameterValue = node.Parameters.FirstOrDefault(x => x.Key == nameof(SetVariableComponent.VariableValue));
                self.CurrentDesignFlow.SetFlowParam(parameter.Value.Value + "", parameterValue.Value);
            }
            return nodes;
        }
        public static Node FindParent(this DesignComponent self, int index)
        {
            Node node = default;
            for (int i = index - 1; i > 0; i--)
            {
                if (i >= self.CurrentDesignFlow.DesignSteps.Count) return default;
                node = self.CurrentDesignFlow.DesignSteps[i];
                if (node.LinkNode != null)
                {
                    var link = self.CurrentDesignFlow.DesignSteps.FirstOrDefault(x => x.Id == node.LinkNode.Id);
                    if (link == default) continue;
                    var endIndex = self.CurrentDesignFlow.DesignSteps.IndexOf(link);
                    if (index <= endIndex) return node;
                }
            }
            return default;
        }
        //模拟执行组件
        public static async ELTask<List<ExecLog>> PreviewNodes(this DesignComponent self, List<Node> nodes)
        {
            await ELTask.CompletedTask;
            CommponetRequest commponetRequest = new CommponetRequest()
            {
                Data = new Flow()
                {
                    Id = IdGenerater.Instance.GenerateId(),
                    Steps = nodes
                }
            };
            await RequestManager.ExecFlow(commponetRequest);
            var flowComponent = Boot.GetComponent<RobotComponent>().GetComponent<FlowComponent>();
            return flowComponent.LogMsgs;
        }
        //模拟执行组件
        public static async ELTask<ComponentResponse> PreviewNodes(this DesignComponent self, CommponetRequest request)
        {
            await ELTask.CompletedTask;
            CommponetRequest commponetRequest = new CommponetRequest()
            {
                Data = new Flow()
                {
                    Id = IdGenerater.Instance.GenerateId(),
                    Steps = request.Data
                }
            };
            return await RequestManager.ExecFlow(commponetRequest);
        }
        public static async Task<List<ExecLog>> RunRobot(this DesignComponent self)
        {
            if (self.CurrentDesignFlow == null)
            {
                self.WriteDesignLog("请先打开机器人才能运行哟！");
                return default;
            }
            try
            {
                var robot = Boot.GetComponent<RobotComponent>();
                await robot.LocalMain(self.CurrentDesignFlow, false);
                var logs = robot.GetComponent<FlowComponent>().LogMsgs;
                return logs;
            }
            catch (Exception ex)
            {
                self.WriteDesignLog(ex.Message);
            }
            return default;

        }


    }
}
