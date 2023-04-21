using EL.Async;
using Microsoft.Win32;
using Newtonsoft.Json;
using NLog.LayoutRenderers.Wrappers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Management;
using System.Reflection.Emit;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Text;

namespace EL
{
    /// <summary>
    /// windows api 名称
    /// </summary>
    public enum WindowsAPIType
    {
        Win32_ComputerSystemProduct,
        Win32_Processor,// —— CPU 处理器
        Win32_PhysicalMemory,// —— 物理内存条
        Win32_Keyboard,// —— 键盘
        Win32_PointingDevice,// —— 点输入设备，包括鼠标。
        Win32_FloppyDrive,// —— 软盘驱动器
        Win32_DiskDrive,// —— 硬盘驱动器
        Win32_CDROMDrive,// —— 光盘驱动器
        Win32_BaseBoard,// —— 主板
        Win32_BIOS,// —— BIOS 芯片
        Win32_ParallelPort,// —— 并口
        Win32_SerialPort,// —— 串口
        Win32_SerialPortConfiguration,// —— 串口配置
        Win32_SoundDevice,// —— 多媒体设置，一般指声卡。
        Win32_SystemSlot,// —— 主板插槽 (ISA & PCI & AGP)
        Win32_USBController,// —— USB 控制器
        Win32_NetworkAdapter,// —— 网络适配器
        Win32_NetworkAdapterConfiguration,// —— 网络适配器设置
        Win32_Printer,// —— 打印机
        Win32_PrinterConfiguration,// —— 打印机设置
        Win32_PrintJob,// —— 打印机任务
        Win32_TCPIPPrinterPort,// —— 打印机端口
        Win32_POTSModem,// —— MODEM
        Win32_POTSModemToSerialPort,// —— MODEM 端口
        Win32_DesktopMonitor,// —— 显示器
        Win32_DisplayConfiguration,// —— 显卡
        Win32_DisplayControllerConfiguration,// —— 显卡设置
        Win32_VideoController,// —— 显卡细节。
        Win32_VideoSettings,// —— 显卡支持的显示模式
        Win32_TimeZone,// —— 时区
        Win32_SystemDriver,// —— 驱动程序
        Win32_DiskPartition,// —— 磁盘分区
        Win32_LogicalDisk,// —— 逻辑磁盘
        Win32_LogicalDiskToPartition,// —— 逻辑磁盘所在分区及始末位置。
        Win32_LogicalMemoryConfiguration,// —— 逻辑内存配置
        Win32_PageFile,// —— 系统页文件信息
        Win32_PageFileSetting,// —— 页文件设置
        Win32_BootConfiguration,// —— 系统启动配置
        Win32_ComputerSystem,// —— 计算机信息简要
        Win32_OperatingSystem,// —— 操作系统信息
        Win32_StartupCommand,// —— 系统自动启动程序
        Win32_Service,// —— 系统安装的服务
        Win32_Group,// —— 系统管理组
        Win32_GroupUser,// —— 系统组帐号
        Win32_UserAccount,// —— 用户帐号
        Win32_Process,// —— 系统进程
        Win32_Thread,// —— 系统线程
        Win32_Share,// —— 共享
        Win32_NetworkClient,// —— 已安装的网络客户端
        Win32_NetworkProtocol,//—— 已安装的网络协议
        Win32_PnPEntity,// —— all device

    }
    public class EnvironmentInfo
    {
        public static EnvironmentInfo Instance { get; internal set; }
        [JsonProperty("电脑信息")]
        public dynamic ComputerInfo { get; set; }
        [JsonProperty("处理器信息")]
        public dynamic ProcessorInfo { get; set; }
        [JsonProperty("驱动信息")]
        public List<dynamic> Drives { get; set; }
        [JsonProperty("浏览器信息")]
        public dynamic BrowserInfo
        {
            get; set;
        } = new List<dynamic>();
       
        [JsonProperty("默认系统环境遍历")]
        public string[] EnvironmentPaths { get; set; }
        [JsonProperty("java路径")]
        public string[] JreFolders { get; set; }
        [JsonProperty("机器码")]
        public string MachineCode { get; set; }
        public List<dynamic> ComputerSystemProduct { get; set; }
        public List<dynamic> Processor { get; set; }
        public List<dynamic> OperatingSystem { get; set; }
        public List<dynamic> DiskDrive { get; set; }
        public List<dynamic> DesktopMonitor { get; set; }
        public List<dynamic> PhysicalMemory { get; set; }
        public List<dynamic> BaseBoard { get; set; }

        public string ComputerName { get; set; }
        public string SystemName { get; set; }
        public string ComputerModel { get; set; }
        public string ComputerTotalPhysicalMemory { get; set; }
        public string UserName { get; set; }
        public string ProcessorName { get; set; }
        public string ProcessorId { get; set; }
        public string DiskSerialNumber { get; set; }
        public string ThreadCount { get; set; }
        public string NumberOfEnabledCore { get; set; }
        public bool Is64BitOperatingSystem { get; set; }
    }
    public static class EnvironmentInfoSystem
    {

        public static async ELTask<EnvironmentInfo> Get(this EnvironmentInfo self)
        {
            await ELTask.CompletedTask;
            EnvironmentInfo.Instance = self;

            self.Is64BitOperatingSystem = Environment.Is64BitOperatingSystem;
            self.Drives = new List<dynamic>();
            foreach (var item in DriveInfo.GetDrives())
            {
                self.Drives.Add(new
                {
                    item.Name,
                    item.DriveType,
                    AvailableFreeSpace = (item.AvailableFreeSpace / 1024 / 1024 / 1024) + " G",
                    TotalSize = (item.TotalSize / 1024 / 1024 / 1024) + " G",
                    TotalFreeSpace = (item.TotalFreeSpace / 1024 / 1024 / 1024) + " G",
                    item.RootDirectory
                }
                );
            }
            var querys = Enum.GetNames(typeof(WindowsAPIType));
            foreach (var query in querys)
            {
                List<dynamic> vals;
                switch (Enum.Parse(typeof(WindowsAPIType), query))
                {
                    case WindowsAPIType.Win32_PhysicalMemory:
                        vals = GetInfo(query);
                        self.PhysicalMemory = vals;
                        break;
                    case WindowsAPIType.Win32_Processor:
                        vals = GetInfo(query);
                        self.Processor = vals;
                        break;
                    case WindowsAPIType.Win32_DiskDrive:
                        vals = GetInfo(query);
                        self.DiskDrive = vals;
                        break;
                    case WindowsAPIType.Win32_ComputerSystemProduct:
                        vals = GetInfo(query);
                        self.ComputerSystemProduct = vals;
                        break;
                    case WindowsAPIType.Win32_DesktopMonitor:
                        vals = GetInfo(query);
                        self.DesktopMonitor = vals;
                        break;
                    case WindowsAPIType.Win32_VideoController:
                        break;
                    case WindowsAPIType.Win32_OperatingSystem:
                        vals = GetInfo(query);
                        self.OperatingSystem = vals;
                        break;
                    case WindowsAPIType.Win32_BaseBoard:
                        //vals = GetInfo(query);
                        //self.BaseBoard = vals;
                        break;
                    case WindowsAPIType.Win32_BIOS:
                        break;
                    case WindowsAPIType.Win32_NetworkAdapter:
                        break;
                    default:
                        break;
                }
            }
            self.ComputerName = GetVlue("root\\CIMV2",
                    "SELECT * FROM Win32_ComputerSystem", "Name") + "";
            self.SystemName = GetVlue("root\\CIMV2",
              "SELECT * FROM Win32_OperatingSystem", "Name") + "";
            self.ComputerModel = GetVlue("root\\CIMV2",
                    "SELECT * FROM Win32_ComputerSystem", "Model") + "";
            try
            {
                self.ComputerTotalPhysicalMemory = (long.Parse(GetVlue("root\\CIMV2",
            "SELECT * FROM Win32_ComputerSystem", "TotalPhysicalMemory") + "") / 1024 / 1024 / 1024) + "";
            }
            catch (Exception)
            {
            }
            self.UserName = GetVlue("root\\CIMV2",
                "SELECT * FROM Win32_ComputerSystem", "UserName") + "";
            self.ProcessorName = GetVlue("root\\CIMV2",
                    "SELECT * FROM Win32_Processor", "Name") + "";
            self.ProcessorId = GetVlue("root\\CIMV2",
              "SELECT * FROM Win32_Processor", "ProcessorId") + "";
            self.ThreadCount = GetVlue("root\\CIMV2",
            "SELECT * FROM Win32_Processor", "ThreadCount") + "";
            self.NumberOfEnabledCore = GetVlue("root\\CIMV2",
       "SELECT * FROM Win32_Processor", "NumberOfEnabledCore") + "";
            self.DiskSerialNumber = GetVlue("root\\CIMV2",
      "SELECT * FROM Win32_DiskDrive", "SerialNumber") + "";
            self.ProcessorInfo = new
            {
                self.ProcessorName,
                self.ProcessorId,
                self.ThreadCount,
                self.NumberOfEnabledCore
            };
            self.ComputerInfo = new
            {
                self.ComputerName,
                self.ComputerModel,
                self.ComputerTotalPhysicalMemory,
                self.UserName,
                self.SystemName,
                self.Is64BitOperatingSystem,
                self.DiskSerialNumber
            };
            self.EnvironmentPaths = Environment.GetEnvironmentVariable("Path").Split(';');
            self.JreFolders = JavaBridge.Installer.InstallHelper.GetJreFolders();
            var msEdge = self.GetFileVersion(@"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe");
            var msEdge1 = self.GetFileVersion(@"C:\Program Files\Microsoft\Edge\Application\msedge.exe");
            var chrome = self.GetFileVersion(@"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe");
            var chrome1 = self.GetFileVersion(@"C:\Program Files\Google\Chrome\Application\chrome.exe");
            msEdge = msEdge ?? msEdge1;
            chrome = chrome ?? chrome1;
            self.BrowserInfo = new
            {
                IsExistEdge = msEdge != null,
                IsExistChrome = chrome != null,
                Edge = msEdge,
                Chrome = chrome,
            };
            self.MachineCode = GetMachineCode();
            return self;
        }

        public static dynamic GetFileVersion(this EnvironmentInfo self, string file)
        {
            dynamic info = default;
            try
            {
                // Get the path to the Edge executable
                string edgePath = file;
                // Use the FileVersionInfo class to get the version information
                FileVersionInfo edgeVersionInfo = FileVersionInfo.GetVersionInfo(edgePath);
                string fileName = Path.GetFileName(file);
                // Get the version number as a string
                info = new
                {
                    FileName = fileName,
                    edgeVersionInfo.ProductVersion
                };
            }
            catch (Exception)
            {
            }
            return info;
        }
        public static string GetMachineCode()
        {
            string pwd = "";
            var srt = EnvironmentInfo.Instance.ProcessorId + EnvironmentInfo.Instance.DiskSerialNumber;
            MD5 md5 = MD5.Create(); //实例化一个md5对像
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(srt));
            // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
            for (int i = 0; i < s.Length; i++)
            {
                // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符 
                pwd = pwd + s[i].ToString("X");
            }
            return pwd;
        }

        private static List<dynamic> GetInfo(string query)
        {
            try
            {
                var moc = new ManagementClass(query).GetInstances();
                List<dynamic> s = new List<dynamic>();
                foreach (ManagementObject mo in moc)
                {
                    foreach (var item in mo.Properties)
                        s.Add(new { item.Name, item.Value });
                }
                return s;
            }
            catch (Exception)
            {

            }
            return default;
        }
        private static object GetVlue(string scope, string queryString, string key)
        {
            try
            {
                ManagementObjectSearcher searcher = new(scope, queryString);
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    if (queryObj[key] != default && !string.IsNullOrEmpty(queryObj[key] + ""))
                        return queryObj[key];
                }
            }
            catch (ManagementException e)
            {

            }
            return default;
        }
    }
    public class AppInfo
    {
        public static AppInfo Instance { get; internal set; }
        public List<dynamic> LnkFiles { get; set; } = new List<dynamic>();
        public List<dynamic> App { get; set; }
    }
    public static class AppInfoSystem
    {
        public static async ELTask<AppInfo> Get(this AppInfo self)
        {
            await ELTask.CompletedTask;
            List<string> linkFiles = new List<string>();
            string startMenuPath = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu);
            var files = Directory.GetFiles(Path.Combine(startMenuPath, "Programs"), "*", SearchOption.AllDirectories);
            if (files != default)
                linkFiles.AddRange(files);
            files = Directory.GetFiles(startMenuPath);
            if (files != default)
                linkFiles.AddRange(files);
            files = Directory.GetFiles(Path.Combine(startMenuPath, "Programs"));
            if (files != default)
                linkFiles.AddRange(files);
            linkFiles = linkFiles.Distinct().ToList();
            foreach (var item in linkFiles)
            {
                FileInfo fileInfo = new FileInfo(item);
                FileAttributes attributes = fileInfo.Attributes;
                self.LnkFiles.Add(new { AppName = fileInfo.Name, fileInfo.FullName });
            }
            return self;
        }

        #region MyRegion
        static List<dynamic> GetInstalledSoftwareList()
        {
            string displayName;
            string installLocation;
            string installSource;
            List<dynamic> gInstalledSoftware = new();
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", false))
            {
                foreach (String keyName in key.GetSubKeyNames())
                {
                    RegistryKey subkey = key.OpenSubKey(keyName);
                    displayName = subkey.GetValue("DisplayName") as string;
                    installLocation = subkey.GetValue("InstallLocation") as string;
                    installSource = subkey.GetValue("InstallSource") as string;
                    var uninstallString = subkey.GetValue("UninstallString");
                    if (string.IsNullOrEmpty(displayName))
                        continue;

                    gInstalledSoftware.Add(new
                    {
                        DisplayName = displayName,
                        installDir = installLocation ?? installSource,
                        UninstallDir = uninstallString
                    });

                }
            }

            //using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", false))
            using (var localMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
            {
                var key = localMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", false);
                foreach (String keyName in key.GetSubKeyNames())
                {
                    RegistryKey subkey = key.OpenSubKey(keyName);
                    displayName = subkey.GetValue("DisplayName") as string;
                    installLocation = subkey.GetValue("InstallLocation") as string;
                    installSource = subkey.GetValue("InstallSource") as string;
                    var uninstallString = subkey.GetValue("UninstallString");
                    if (string.IsNullOrEmpty(displayName))
                        continue;

                    gInstalledSoftware.Add(new
                    {
                        DisplayName = displayName,
                        installDir = installLocation ?? installSource,
                        UninstallDir = uninstallString
                    });
                }
            }

            using (var localMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
            {
                var key = localMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", false);
                foreach (String keyName in key.GetSubKeyNames())
                {
                    RegistryKey subkey = key.OpenSubKey(keyName);
                    displayName = subkey.GetValue("DisplayName") as string;
                    installLocation = subkey.GetValue("InstallLocation") as string;
                    installSource = subkey.GetValue("InstallSource") as string;
                    var uninstallString = subkey.GetValue("UninstallString");
                    if (string.IsNullOrEmpty(displayName))
                        continue;

                    gInstalledSoftware.Add(new
                    {
                        DisplayName = displayName,
                        installDir = installLocation ?? installSource,
                        UninstallDir = uninstallString
                    });
                }
            }

            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall", false))
            {
                foreach (String keyName in key.GetSubKeyNames())
                {
                    RegistryKey subkey = key.OpenSubKey(keyName);
                    displayName = subkey.GetValue("DisplayName") as string;
                    installLocation = subkey.GetValue("InstallLocation") as string;
                    installSource = subkey.GetValue("InstallSource") as string;
                    var uninstallString = subkey.GetValue("UninstallString");
                    if (string.IsNullOrEmpty(displayName))
                        continue;

                    gInstalledSoftware.Add(new
                    {
                        DisplayName = displayName,
                        installDir = installLocation ?? installSource,
                        UninstallDir = uninstallString
                    });
                }
            }
            return gInstalledSoftware;
        }
        #endregion

    }

}
