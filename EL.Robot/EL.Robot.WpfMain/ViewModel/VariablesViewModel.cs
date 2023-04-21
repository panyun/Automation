using EL.Robot.Component;
using EL.Robot.Core;
using EL.Robot.WpfMain.ModelData;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;



namespace EL.Robot.WpfMain.ViewModel
{

    public class VariablesViewModel
    {
        /// <summary>
        /// 返回所有流程变量信息
        /// </summary>
        /// <returns></returns>
        public static List<VariableModel> GetFlowVariableTree()
        {
            List<VariableModel> variableModels = new();
            var flowComponent = Boot.GetComponent<RobotComponent>().GetComponent<FlowComponent>();
            VariableModel temp = new();
            var flowName = default(string);
            if (flowComponent.MainFlow != null)
            {
                flowName = flowComponent.MainFlow.Name;
                temp.Key = $"主流程[{flowName}]";
                variableModels.Add(temp);
                temp.Childs = GetVariableTree(flowComponent.MainFlow);
            }
            if (flowComponent.MainFlow == null || flowComponent.MainFlow.ChildrenFlows == null || flowComponent.MainFlow.ChildrenFlows.Count == 0) return variableModels;
            foreach (var flow in flowComponent.MainFlow.ChildrenFlows)
            {
                temp = new();
                temp.Key = $"子流程[{flow.Name}]";
                variableModels.Add(temp);
                temp.Childs = GetVariableTree(flow);
            }
            return variableModels;
        }
        /// <summary>
        /// 获取流程变量树
        /// </summary>
        /// <param name="flow"></param>
        /// <returns></returns>
        private static List<VariableModel> GetVariableTree(Flow flow)
        {
            List<VariableModel> variableModels = new();
            var flowComponent = Boot.GetComponent<RobotComponent>().GetComponent<FlowComponent>();
            if (flow == null || flow.ParamsManager == null || flow.ParamsManager.Count == 0) return default;
            var pars = flow.ParamsManager;
            if (pars == default)
                return default;
            foreach (var item in pars)
            {
                VariableModel temp = new();
                temp.Key = item.Key;
                try
                {
                    var val = JsonHelper.ToJson(item.Value);
                    var obj = JsonConvert.DeserializeObject(val + "");
                    temp.Value = val;
                    temp.Childs = GetVariable(obj);
                    variableModels.Add(temp);
                }
                catch (Exception ex)
                {
                    temp.Value = item.Value + "";
                    variableModels.Add(temp);
                }
            }
            return variableModels;
        }
        public static List<VariableModel> GetChildrenParamTree()
        {
            List<VariableModel> variableModels = new();
            var flowComponent = Boot.GetComponent<RobotComponent>().GetComponent<FlowComponent>();
            if (flowComponent == null ||
                flowComponent.MainFlow == null) return default;
            if (flowComponent.MainFlow.ChildrenFlows == null || flowComponent.MainFlow.ChildrenFlows.Count == 0) return default;
            foreach (var flow in flowComponent.MainFlow.ChildrenFlows)
            {
                VariableModel temp = new();
                temp.Key = $"子流程[{flow.Name}]";
                variableModels.Add(temp);
                if (flow.InParams != null && flow.InParams.Count > 0)
                {
                    foreach (var para in flow.InParams)
                    {
                        var paraValue = default(object);
                        try
                        {
                            paraValue = flow.GetFlowParamterValue(para.Key);
                        }
                        catch (Exception)
                        {
                        }
                        VariableModel somTemp = new();
                        somTemp.Key = $"输入参数 {para.Key}";
                        var val = JsonHelper.ToJson(paraValue);
                        var obj = JsonConvert.DeserializeObject(val + "");
                        somTemp.Value = val;
                        somTemp.Childs = GetVariable(obj);
                        temp.Childs.Add(somTemp);
                    }
                }
                if (flow.OutParams != null && flow.OutParams.Count > 0)
                {
                    foreach (var para in flow.OutParams)
                    {
                        var paraValue = default(object);
                        try
                        {
                            paraValue = flow.GetFlowParamterValue(para.Key);
                        }
                        catch (Exception)
                        {
                        }
                        VariableModel somTemp = new();
                        somTemp.Key = $"输出参数 {para.Key}";
                        var val = JsonHelper.ToJson(paraValue);
                        var obj = JsonConvert.DeserializeObject(val + "");
                        somTemp.Value = val;
                        somTemp.Childs = GetVariable(obj);
                        temp.Childs.Add(somTemp);
                    }
                }

            }
            return variableModels;
        }
        public static List<VariableModel> GetVariable(dynamic obj)
        {
            List<VariableModel> vals = new();

            if (obj is JObject jobj)
            {
                foreach (var item in jobj)
                {
                    var variab = new VariableModel();
                    variab.Key = item.Key;
                    if (item.Value == null || item.Value.Count() == 0)
                    {
                        variab.Value = item.Value + "";
                    }
                    else if (item.Value is JObject)
                    {
                        variab.Childs = GetVariable(item.Value);
                    }
                    else if (item.Value is JArray)
                    {
                        variab.Childs = GetVariable(item.Value);
                        variab.Value = "[...]";
                    }
                    vals.Add(variab);
                }
                return vals;
            }

            if (obj is JArray jArray)
            {
                int i = 0;
                foreach (var item in jArray)
                {
                    var variab = new VariableModel();
                    variab.Childs = GetVariable(item);
                    variab.Key = $"item{i}";
                    variab.Value = item.ToString();
                    vals.Add(variab);
                    i++;
                }
            }
            return vals;
        }
    }

}
