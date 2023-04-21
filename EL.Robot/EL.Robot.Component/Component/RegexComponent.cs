using EL.Async;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EL.Robot.Component
{
    public class RegexComponent : BaseComponent
    {

        public override async ELTask<INodeContent> Main(INodeContent self)
        {
            await base.Main(self);
            var targetvalue = self.CurrentNode.GetParamterString("targetvalue");
            var regularexpression = self.CurrentNode.GetParamterString("regularexpression");
            var isfirstmatch = self.CurrentNode.GetParamterBool("isfirstmatch");
            var ignorecase = self.CurrentNode.GetParamterBool("ignorecase");
            var function = self.CurrentNode.GetParamterInt("function");
            RegexOptions regexOptions = default(RegexOptions);
            if (ignorecase) regexOptions = RegexOptions.IgnoreCase;
            switch (function)
            {
                case 1:
                    self.Out = Regex.IsMatch(targetvalue, regularexpression, regexOptions);
                    break; 
                case 2:
                    var matches = Regex.Matches(targetvalue, regularexpression, regexOptions);
                    if (matches != null && matches.Count > 0)
                    {
                        self.Out = matches.Cast<Match>().Select(m => m.Value).ToList();
                        if (isfirstmatch)
                            self.Out = new string[] { matches.Cast<Match>().Select(m => m.Value).FirstOrDefault() };
                    }
                    break;
                case 3:
                    var replace = self.CurrentNode.GetParamterString("replace");
                    self.Out = Regex.Replace(targetvalue, regularexpression, replace, regexOptions);
                    break;
                default:
                    break;
            }
            self.Value = true;
            return self;
        }
    }
}
