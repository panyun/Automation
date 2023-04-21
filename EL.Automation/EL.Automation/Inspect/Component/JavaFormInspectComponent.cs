using EL;
using EL.Async;
using EL.Overlay;
using WindowsAccessBridgeInterop;

namespace Automation.Inspect
{

    public class JavaFormInspectComponent : Entity
    {
        public AccessBridge AccessBridge { get; set; }
        public List<AccessibleJvm> Jvms { get; set; } = new List<AccessibleJvm>();
        public ELTask<List<AccessibleJvm>> ELTaskJvm { get; set; }
        public OverlayRectangleForm From { get; set; } = new OverlayRectangleForm();
    
    }
}
