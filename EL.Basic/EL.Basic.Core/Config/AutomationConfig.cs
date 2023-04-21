using EL.WindowsAPI;
using Newtonsoft.Json;

namespace EL
{
    public static class ConfigSystem
    {
        public static string ConfigPath
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config");
            }
        }
        public static void GetAutomationConfig()
        {
            var path = Path.Combine(ConfigPath, $"{nameof(AutomationConfig)}.json");
            Log.Trace($"配置文件路径：{path}");
            AutomationConfig.Instance = new();
            string configJson;
            if (File.Exists(path))
            {
                try
                {
                    var json = File.ReadAllText(path);
                    if (!string.IsNullOrEmpty(json))
                    {
                        AutomationConfig.Instance = JsonConvert.DeserializeObject<AutomationConfig>(json);
                        configJson = JsonConvert.SerializeObject(AutomationConfig.Instance);
                        Log.Trace($"配置文件路径：{path} 当前系统配置：" + configJson);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
            }
            if (!Directory.Exists(ConfigPath))
                Directory.CreateDirectory(ConfigPath);
            configJson = JsonConvert.SerializeObject(AutomationConfig.Instance, Formatting.Indented);
            Log.Trace("当前系统默认配置：" + configJson);
            var stream = File.Open(path, FileMode.Create, FileAccess.ReadWrite);
            stream.Close();
            File.WriteAllText(path, configJson);
            return;
        }
        public static void LazyLoad()
        {
            _ = Task.Run(() =>
            {
                Log.Trace($"————Inin Config Start！——--");
                GetAutomationConfig();
                GenerateEnvironmentConfig();
                GenerateAppConfig();
                Log.Trace($"————Inin Config End！——--");
            });
        }
        public static void GenerateEnvironmentConfig()
        {
            try
            {
                EnvironmentInfo.Instance = new();
                EnvironmentInfo.Instance.Get().Coroutine();
                var json = JsonConvert.SerializeObject(EnvironmentInfo.Instance, Formatting.Indented);
                var path = Path.Combine(ConfigPath, $"{nameof(EnvironmentInfo)}.json");
                var stream = File.Open(path, FileMode.Create, FileAccess.ReadWrite);
                stream.Close();
                File.WriteAllText(path, json);
            }
            catch (Exception ex)
            {
                Log.Error("序列化系统环境信息出现异常：" + ex.Message);
            }
        }
        public static void GenerateAppConfig()
        {
            try
            {
                AppInfo.Instance = new();
                AppInfo.Instance.Get().Coroutine();
                var json = JsonConvert.SerializeObject(AppInfo.Instance, Formatting.Indented);
                var path = Path.Combine(ConfigPath, $"{nameof(AppInfo)}.json");
                var stream = File.Open(path, FileMode.Create, FileAccess.ReadWrite);
                stream.Close();
                File.WriteAllText(path, json);
            }
            catch (Exception ex)
            {
                Log.Error("序列化APP信息出现异常：" + ex.Message);
            }
        }
    }
    public partial class AutomationConfig
    {
        public static AutomationConfig Instance { get; set; }
        [JsonProperty($"功能快捷键({nameof(VkKeyScanModifiers.ALT) + "," + nameof(VkKeyScanModifiers.SHIFT) + "," + nameof(VkKeyScanModifiers.CONTROL)})")]
        public string FunctionKey { get; set; } = "Alt";
        [JsonProperty("是否启用键盘监测快捷键(会影响性能")]
        public bool IsKeyboardHook { get; set; } = false;
        [JsonProperty("是否启用鼠标监听+鼠标点击(会影响性能)")]
        public bool IsMouseHook { get; set; } = false;
        [JsonProperty("完成捕获快捷键")]
        public string ComplateHotKey { get; set; } = "Q";
        [JsonProperty("退出快捷键盘")]
        public string ExitHotKey { get; set; } = "Escape";
        [JsonProperty("截图捕获快捷键（在vcocr捕获模式下生效）")]
        public string ScreenshotHotKey { get; set; } = "E";
        [JsonProperty("切换捕获技术")]
        public string ModeHotKey { get; set; } = "W";
        [JsonProperty("ocr识别地址")]
        public string OcrImgUri { get; set; } = "http://192.168.0.107:10200/ocr_img";
        [JsonProperty("是否启用日志")]
        public bool IsLog { get; set; } = true;

    }
}
