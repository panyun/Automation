using Automation.Inspect;
using EL;
using EL.Basic.Component.Clipboard;
using EL.Capturing;
using EL.Overlay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automation.Parser
{

    /// <summary>
    /// 探测器组件
    /// </summary>
    public class ParserComponent : Entity
    {
        public Action<bool, IResponse> CallBack;
    }
    /// <summary>
    /// 界面探测器入口
    /// </summary>
    public class ParserComponentAwake : AwakeSystem<ParserComponent>
    {
        public override void Awake(ParserComponent self)
        {
            ////桌面软件探测
            //var inspect = self.AddComponent<WinFormInspectComponent>();
            ////路径 组件
            //inspect.AddComponent<WinPathComponent>();
            ////Ie 探测器
            //self.AddComponent<IEInspectComponent>();
            ////剪切板组件
            //self.AddComponent<ClipboardComponent>();
            ////截图组件
            //self.AddComponent<CutComponent>();
            ////截图组件
            //self.AddComponent<CaptureComponent>();
            ////高亮显示
            //self.AddComponent<FormOverLayComponent>();
            ////java界面控件
            //var javaInspect = self.AddComponent<JavaFormInspectComponent>();
            ////路径 组件
            //javaInspect.AddComponent<JavaPathComponent>();
            var dataBase = self.AddComponent<DataBaseComponent>(); //临时数据库
        }
    }

}
