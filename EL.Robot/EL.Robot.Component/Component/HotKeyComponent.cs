using EL.Async;
using EL.Input;
using System.Text.RegularExpressions;
using System.Linq;
using EL.WindowsAPI;

namespace EL.Robot.Component.Component
{
    //HotKeyComponent
    public class HotKeyComponent : BaseComponent
    {
        Dictionary<string, string> keyDic = new Dictionary<string, string>()
        {
            {"CONTROLLEFT",VirtualKeyShort.LCONTROL.ToString() },
            {"CONTROLRIGHT",VirtualKeyShort.RCONTROL.ToString() },
            {"SHIFTLEFT",VirtualKeyShort.LSHIFT.ToString() },
            {"SHIFTRIGHT",VirtualKeyShort.RSHIFT.ToString() },
            {"WINLEFT",VirtualKeyShort.LWIN.ToString() },
            {"WINRIGHT",VirtualKeyShort.RWIN.ToString() },
             {"ALTLEFT",VirtualKeyShort.ALT.ToString() },
            {"ALTRIGHT",VirtualKeyShort.ALT.ToString() },

        };
      
        public override async ELTask<INodeContent> Main(INodeContent self)
        {
            await base.Main(self);
            self.Value = true;
            var hotKeys = self.CurrentNode.GetParamterString("combinationkey");
            var matchs = Regex.Matches(hotKeys, @"({\w+})|({\W})");
            if (matchs == null || matchs.Count == 0)
                throw new ELNodeHandlerException($"参数为null或参数格式不正确");
            var keys = matchs.Cast<Match>().Select(x => x.Value.Trim('{', '}')).ToList();
            var virtualKeys = GetVirtualKey(keys);
            Keyboard.TypeSimultaneously(virtualKeys);
            return self;
        }

        public ushort[] GetVirtualKey(List<string> keys)
        {
            if (keys == null || keys.Count == 0) throw new ELNodeHandlerException($"参数为null或参数格式不正确");
            List<ushort> virtualKeys = new List<ushort>();
            foreach (var key in keys)
            {
                var keyUpper = key.ToUpper();
                if (keyDic.TryGetValue(keyUpper, out string keyTemp))
                    keyUpper = keyTemp;
                if (Enum.TryParse(keyUpper, out VirtualKeyShort virtualKey))
                {
                    virtualKeys.Add((ushort)virtualKey);
                    continue;
                }
                keyUpper = "KEY_" + keyUpper;
                if (Enum.TryParse(keyUpper, out virtualKey))
                {
                    virtualKeys.Add((ushort)virtualKey);
                    continue;
                }
                char charKey = key.ToCharArray()[0];
                var code = User32.VkKeyScan(charKey);
                virtualKeys.Add((ushort)code);
            }
            return virtualKeys.ToArray();
        }
    }
}
