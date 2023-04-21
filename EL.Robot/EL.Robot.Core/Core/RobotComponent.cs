using Automation.Inspect;
using EL.Robot.Component;
using EL.Robot.Core.SqliteEntity;
using EL.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL.Robot.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class RobotComponent : Entity
    {
        #region Flow Action
        public Action StartExecRobotAction;
        public Action StopExecRobotAction;
        public Action FlowUpdateAction;
        /// <summary>
        /// 暂停动作
        /// </summary>
        public Action PausedAction;
        /// <summary>
        /// 运行状态 1 运行状态 0 空闲状态
        /// </summary>
        public int State { get; set; }
        public bool IsSelfMachine { get; set; } = true;
        public string ClientNo { get; set; } = "RPAII";
        /// <summary>
        /// 正常运行
        /// </summary>
        public Action NoneAction;
        public Node CurrentNode { get; set; }
        public string RpaJson { get; set; }
        public Async.ELTask<bool> ELTaskPaused { get; set; }
        public ExecState ExecState
        {
            get;
            set;
        } = ExecState.None;
        #endregion

        #region Catch RequestHandler
        public Action StartCatchAction;
        public Action StopCatchAction;
        #endregion

    }

}

//[
//   {
//        flow_id:232342349394329432,
//        flow_name:"流程名称",
//        is_main:true,
//        params:"{'p1':124,'p2;:'123','p3':'[abc,124]'}",
//        output:"{'out1':123}",
//        parent_id:0,//上级流程
//        flow_nodes:[
//            {
//                id:23234234939432555,
//                name:"节点1",//节点名称
//                group:"group1",//所属分组，层级已英文逗号分隔
//                data_type:""，//节点类型
//                annotation:"",//节点注释
//                ignore:0,//是否忽略
//                debug:0,//是否调试
//                lock:0,//是否锁定
//                base64,"",//截图
//            }
//        ],//节点列表
//        flow_variables:
//[
//            { name: "age",value: 12,is_fx: 0,type: "Number",context: "group1"},
//            { name: "friends",value: abc.getList(),is_fx: 1,type: "Array",context: "group1"}
//        ],//流程变量池
//    },
//   {
//flow_id: 232342349394329433,
//        flow_name: "流程名称",
//        is_main: true,
//        params:"{'p1':124,'p2;:'123','p3':'[abc,124]'}",
//        output: "{'out1':123}",
//        parent_id: 232342349394329432,//上级流程
//        flow_nodes:
//    [
//            {
//    id: 23234234939432344,
//                name: "节点2",//节点名称
//                group: "group1",//所属分组，层级已英文逗号分隔
//                data_type: ""，//节点类型
//                annotation: "",//节点注释
//                ignore: 0,//是否忽略
//                debug: 0,//是否调试
//                lock:0,//是否锁定
//                base64,"",//截图
//            }
//        ],//节点列表
//    },

//]