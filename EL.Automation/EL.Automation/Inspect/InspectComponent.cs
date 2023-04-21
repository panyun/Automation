using EL;
using System.Windows.Forms;

namespace Automation.Inspect
{
    /// <summary>
    /// 探测器组件
    /// </summary>
    public class InspectComponent : Entity
    {
        public ElementIns CurrentElement { get; set; }
        public Action<Element, ElementPath> Action { get; set; }
        public ElementPath ElementPath { get; set; }
        public Keys CatchKeyStart { get; set; } = Keys.F3;
        public Keys CatchKeyEnd { get; set; } = Keys.Q;
        public long NewRepeatedTimerId { get; set; }
    }
    
}
