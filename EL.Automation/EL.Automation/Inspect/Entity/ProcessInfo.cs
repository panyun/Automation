using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automation.Inspect
{
    public class ProcessInfo
    {
        private Process process;
        public ProcessInfo(Process self) => process = self;
        [JsonIgnore]
        public int ProcessId => process?.Id ?? default;
        public string MainWindowTitle => process?.MainWindowTitle ?? default;
        public string ProcessName => process?.ProcessName ?? default;
        [JsonIgnore]
        public IntPtr MainWindowHandle => process?.MainWindowHandle ?? default;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string FileName => process?.MainModule?.FileName ?? default;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ModuleName => process?.MainModule?.ModuleName ?? default;
    }
}
