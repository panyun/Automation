﻿using EL;
using NPOI.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automation.Inspect
{

    public class CatchWindowRequest : IRequest
    {
        public int RpcId { get; set; }
        /// <summary>
        /// 界面探测器打开时显示信息
        /// </summary>
        public string Msg { get; set; }
    }
    public class CatchWindowResponse : IResponse
    {
        public int Error { get; set; }
        public string Message { get; set; }
        public int RpcId { get; set; }
        public ElementIns CurrentElement { get; set; }
        public ElementPath ElementPath { get; set; }
        public string StackTrace { get; set; }
    }

}
