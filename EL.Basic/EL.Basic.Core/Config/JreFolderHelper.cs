using EL.WindowsAPI;
using Microsoft.Win32;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace EL.JavaBridge.Installer
{
    public static class JreFolderHelper
    {
        // Token: 0x06000039 RID: 57 RVA: 0x00002D38 File Offset: 0x00000F38
        public static string[] GetJreFoldersByProcessName(string processName)
        {
            List<string> folders = new List<string>();
            foreach (Process item in Process.GetProcessesByName(processName))
            {
                try
                {
                    string binFolder = JreFolderHelper.GetBinDirPathById(item.Id);
                    if (Directory.Exists(binFolder + "\\server") || Path.GetFileName(Path.GetDirectoryName(binFolder)).StartsWith("jre", StringComparison.OrdinalIgnoreCase) || Path.GetFileName(Path.GetDirectoryName(binFolder)).StartsWith("java", StringComparison.OrdinalIgnoreCase))
                    {
                        string jreFolder = Path.GetDirectoryName(binFolder);
                        folders.Add(jreFolder);
                    }
                    else
                    {
                        string jdkFolder = Path.GetDirectoryName(binFolder);
                        jdkFolder = jdkFolder.Substring(0, jdkFolder.LastIndexOf("\\"));
                        string jreFolder2 = Directory.EnumerateDirectories(jdkFolder).FirstOrDefault((string f) => Path.GetFileName(f).StartsWith("jre"));
                        Log.Info("jrefolder: " + jreFolder2);
                        if (!string.IsNullOrEmpty(jreFolder2))
                        {
                            folders.Add(jreFolder2);
                        }
                    }
                }
                catch (Exception e)
                {
                    //DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(67, 2);
                    //defaultInterpolatedStringHandler.AppendLiteral("GetJreFoldersByProcessName Error. Error Info ProcessName ：");
                    //defaultInterpolatedStringHandler.AppendFormatted(processName);
                    //defaultInterpolatedStringHandler.AppendLiteral(" Error：\n ");
                    //defaultInterpolatedStringHandler.AppendFormatted<Exception>(e);
                    //Logging.Info(defaultInterpolatedStringHandler.ToStringAndClear());
                    Log.Error(e);
                }
            }
            if (folders.Count == 0)
            {
                Log.Warn("fail to locate JreFolder: " + processName);
            }
            return folders.ToArray();
        }

        // Token: 0x0600003A RID: 58 RVA: 0x00002EB0 File Offset: 0x000010B0
        public static string[] GetJreFoldersFromRegistry()
        {
            string[] result = default;
            try
            {
                List<string> list = new List<string>();
                string[] cur32Folders = JreFolderHelper.RetrieveRegistryHive(RegistryHive.CurrentUser, JreFolderHelper._regsoft32Path);
                JreFolderHelper.UnionJreFolder(list, cur32Folders);
                string[] curFolders = JreFolderHelper.RetrieveRegistryHive(RegistryHive.CurrentUser, JreFolderHelper._regsoftPath);
                JreFolderHelper.UnionJreFolder(list, curFolders);
                string[] local32Folders = JreFolderHelper.RetrieveRegistryHive(RegistryHive.LocalMachine, JreFolderHelper._regsoft32Path);
                JreFolderHelper.UnionJreFolder(list, local32Folders);
                string[] localFolders = JreFolderHelper.RetrieveRegistryHive(RegistryHive.LocalMachine, JreFolderHelper._regsoftPath);
                JreFolderHelper.UnionJreFolder(list, localFolders);
                result = list.ToArray();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                //DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(45, 1);
                //defaultInterpolatedStringHandler.AppendLiteral("Failed to GetJreFoldersFromRegistry, message：");
                //defaultInterpolatedStringHandler.AppendFormatted<Exception>(ex);
                //Logging.Info(defaultInterpolatedStringHandler.ToStringAndClear());
                //result = null;
            }
            return result;
        }

        // Token: 0x0600003B RID: 59 RVA: 0x00002F6C File Offset: 0x0000116C
        public static void UnionJreFolder(List<string> source, string[] target)
        {
            if (target == null)
            {
                return;
            }
            foreach (string item in target)
            {
                if (!source.Contains(item))
                {
                    source.Add(item);
                }
            }
        }

        // Token: 0x0600003C RID: 60 RVA: 0x00002FA1 File Offset: 0x000011A1
        public static void UnionJreFolder(List<string> source, string target)
        {
            if (!source.Contains(target))
            {
                source.Add(target);
            }
        }

        // Token: 0x0600003D RID: 61
        [DllImport("ShadowBot.NativeHelper.dll", CharSet = CharSet.Unicode)]
        private static extern bool GetProcessPebString(IntPtr ProcessHandle, JreFolderHelper.PH_PEB_OFFSET Offset, [MarshalAs(UnmanagedType.BStr)] out string pbstrResult);

        // Token: 0x0600003E RID: 62 RVA: 0x00002FB4 File Offset: 0x000011B4
        public static void CheckForLoadShadowBotNativeHelperDll()
        {
            if (!JreFolderHelper.checkedForLoadShadowBotNativeHelperDll)
            {
                JreFolderHelper.checkedForLoadShadowBotNativeHelperDll = true;
                string myDllPath = AppContext.BaseDirectory;
                foreach (string testDllPath in new string[]
                {
                    AppContext.BaseDirectory + "..\\..\\",
                    AppContext.BaseDirectory + "..\\..\\..\\bin\\shadowbot-1.0.0\\"
                })
                {
                    if (File.Exists(testDllPath + "ShadowBot.NativeHelper.dll"))
                    {
                        myDllPath = testDllPath;
                        break;
                    }
                }
                string envValue_PATH_ALL = Environment.GetEnvironmentVariable("Path");
                if (envValue_PATH_ALL.Split(';').ToList().FirstOrDefault(p => string.Compare(p, myDllPath, true) == 0) == null)
                {
                    string newPaths = envValue_PATH_ALL;
                    if (!string.IsNullOrEmpty(newPaths) && !newPaths.EndsWith(";"))
                    {
                        newPaths += ";";
                    }
                    newPaths += myDllPath;
                    Environment.SetEnvironmentVariable("Path", newPaths, EnvironmentVariableTarget.Process);
                }
            }
        }

        // Token: 0x0600003F RID: 63 RVA: 0x000030A8 File Offset: 0x000012A8

        public static string CallHelper__GetProcessPebString(int pid, JreFolderHelper.PH_PEB_OFFSET Offset)
        {
            JreFolderHelper.CheckForLoadShadowBotNativeHelperDll();
            bool bCurProcessIsWow64;
            Kernel32.IsWow64Process(Process.GetCurrentProcess().MainWindowHandle, out bCurProcessIsWow64);
            string strResult = null;
            IntPtr ProcessHandle = Kernel32.OpenProcess((int)PROCESS_ACCESS_RIGHTS.PROCESS_VM_READ | (int)PROCESS_ACCESS_RIGHTS.PROCESS_QUERY_INFORMATION, false, pid);
            if (ProcessHandle != IntPtr.Zero)
            {
                JreFolderHelper.GetProcessPebString(ProcessHandle, Offset | (bCurProcessIsWow64 ? JreFolderHelper.PH_PEB_OFFSET.Phpo64From32 : JreFolderHelper.PH_PEB_OFFSET.PhpoCurrentDirectory), out strResult);
                Kernel32.CloseHandle(ProcessHandle);
            }
            return strResult;
        }

        // Token: 0x06000040 RID: 64 RVA: 0x00003118 File Offset: 0x00001318
        public static string GetProcessFileName(Process process, bool fullFileName)
        {
            string strResult = JreFolderHelper.CallHelper__GetProcessPebString(process.Id, JreFolderHelper.PH_PEB_OFFSET.PhpoImagePathName);
            if (strResult == null)
            {
                return null;
            }
            if (!fullFileName)
            {
                return Path.GetFileName(strResult);
            }
            return strResult;
        }

        // Token: 0x06000041 RID: 65 RVA: 0x00003144 File Offset: 0x00001344
        private static string GetBinDirPathById(int processID)
        {
            string strResult = JreFolderHelper.CallHelper__GetProcessPebString(processID, JreFolderHelper.PH_PEB_OFFSET.PhpoImagePathName);
            if (strResult != null)
            {
                return Path.GetDirectoryName(strResult);
            }
            return null;
        }

        // Token: 0x06000042 RID: 66 RVA: 0x00003164 File Offset: 0x00001364
        private static string[] RetrieveRegistryHive(RegistryHive registryHive, string keyPathStr)
        {
            List<string> folders = new List<string>();
            using (RegistryKey RootReg = RegistryKey.OpenBaseKey(registryHive, RegistryView.Registry64))
            {
                using (RegistryKey JavaSoftReg = RootReg.OpenSubKey(keyPathStr))
                {
                    if (JavaSoftReg != null)
                    {
                        string[] paths = JreFolderHelper.RetrieveJavaHomeInRegistryKey(JavaSoftReg);
                        if (paths != null && paths.Length != 0)
                        {
                            JreFolderHelper.UnionJreFolder(folders, paths);
                        }
                    }
                }
            }
            return folders.ToArray();
        }

        // Token: 0x06000043 RID: 67 RVA: 0x000031DC File Offset: 0x000013DC
        private static string[] RetrieveJavaHomeInRegistryKey(RegistryKey javaSoftReg)
        {
            if (javaSoftReg == null)
            {
                return null;
            }
            List<string> folders = new List<string>();
            foreach (string subKeyName in javaSoftReg.GetSubKeyNames())
            {
                if (subKeyName == "Java Runtime Environment" || subKeyName == "Java Development Kit")
                {
                    using (RegistryKey settingsReg = javaSoftReg.OpenSubKey(subKeyName))
                    {
                        if (settingsReg != null)
                        {
                            foreach (string versionName in settingsReg.GetSubKeyNames())
                            {
                                using (RegistryKey versionReg = settingsReg.OpenSubKey(versionName))
                                {
                                    if (versionReg != null)
                                    {
                                        string javaHome = (string)versionReg.GetValue("JavaHome");
                                        if (!string.IsNullOrEmpty(javaHome) && Directory.Exists(javaHome))
                                        {
                                            string jreFolder = Directory.EnumerateDirectories(javaHome).FirstOrDefault((string f) => Path.GetFileName(f).StartsWith("jre"));
                                            if (!string.IsNullOrEmpty(jreFolder))
                                            {
                                                JreFolderHelper.UnionJreFolder(folders, jreFolder);
                                            }
                                            if (Path.GetFileName(javaHome).StartsWith("jre"))
                                            {
                                                JreFolderHelper.UnionJreFolder(folders, javaHome);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (subKeyName == "Prefs")
                {
                    using (RegistryKey targetReg = javaSoftReg.OpenSubKey("Prefs\\kingdee\\eas"))
                    {
                        if (targetReg != null)
                        {
                            string javaHome2 = (string)targetReg.GetValue("java_home");
                            if (!string.IsNullOrEmpty(javaHome2))
                            {
                                try
                                {
                                    string jreFolder2 = Directory.EnumerateDirectories(javaHome2).FirstOrDefault((string f) => Path.GetFileName(f).StartsWith("jre"));
                                    if (!string.IsNullOrEmpty(jreFolder2))
                                    {
                                        JreFolderHelper.UnionJreFolder(folders, jreFolder2);
                                    }
                                }
                                catch (Exception e)
                                {
                                    Log.Warn("EnumerateDirectories error: " + e.Message);
                                }
                            }
                        }
                    }
                }
            }
            return folders.ToArray();
        }

        // Token: 0x04000021 RID: 33
        private static readonly string _regsoft32Path = "SOFTWARE\\WOW6432Node\\JavaSoft";

        // Token: 0x04000022 RID: 34
        private static readonly string _regsoftPath = "SOFTWARE\\JavaSoft";

        // Token: 0x04000023 RID: 35
        private const string ShadowBotNativeHelperDllPath = "ShadowBot.NativeHelper.dll";

        // Token: 0x04000024 RID: 36
        private static bool checkedForLoadShadowBotNativeHelperDll;

        // Token: 0x02000008 RID: 8
        [Flags]
        internal enum PROCESS_ACCESS_RIGHTS : uint
        {
            // Token: 0x04000007 RID: 7
            PROCESS_TERMINATE = 1u,
            // Token: 0x04000008 RID: 8
            PROCESS_CREATE_THREAD = 2u,
            // Token: 0x04000009 RID: 9
            PROCESS_SET_SESSIONID = 4u,
            // Token: 0x0400000A RID: 10
            PROCESS_VM_OPERATION = 8u,
            // Token: 0x0400000B RID: 11
            PROCESS_VM_READ = 16u,
            // Token: 0x0400000C RID: 12
            PROCESS_VM_WRITE = 32u,
            // Token: 0x0400000D RID: 13
            PROCESS_DUP_HANDLE = 64u,
            // Token: 0x0400000E RID: 14
            PROCESS_CREATE_PROCESS = 128u,
            // Token: 0x0400000F RID: 15
            PROCESS_SET_QUOTA = 256u,
            // Token: 0x04000010 RID: 16
            PROCESS_SET_INFORMATION = 512u,
            // Token: 0x04000011 RID: 17
            PROCESS_QUERY_INFORMATION = 1024u,
            // Token: 0x04000012 RID: 18
            PROCESS_SUSPEND_RESUME = 2048u,
            // Token: 0x04000013 RID: 19
            PROCESS_QUERY_LIMITED_INFORMATION = 4096u,
            // Token: 0x04000014 RID: 20
            PROCESS_SET_LIMITED_INFORMATION = 8192u,
            // Token: 0x04000015 RID: 21
            PROCESS_ALL_ACCESS = 2097151u,
            // Token: 0x04000016 RID: 22
            PROCESS_DELETE = 65536u,
            // Token: 0x04000017 RID: 23
            PROCESS_READ_CONTROL = 131072u,
            // Token: 0x04000018 RID: 24
            PROCESS_WRITE_DAC = 262144u,
            // Token: 0x04000019 RID: 25
            PROCESS_WRITE_OWNER = 524288u,
            // Token: 0x0400001A RID: 26
            PROCESS_SYNCHRONIZE = 1048576u,
            // Token: 0x0400001B RID: 27
            PROCESS_STANDARD_RIGHTS_REQUIRED = 983040u
        }
        // Token: 0x0200001A RID: 26
        public enum PH_PEB_OFFSET
        {
            // Token: 0x0400003A RID: 58
            PhpoCurrentDirectory,
            // Token: 0x0400003B RID: 59
            PhpoDllPath,
            // Token: 0x0400003C RID: 60
            PhpoImagePathName,
            // Token: 0x0400003D RID: 61
            PhpoCommandLine,
            // Token: 0x0400003E RID: 62
            PhpoWindowTitle,
            // Token: 0x0400003F RID: 63
            PhpoDesktopInfo,
            // Token: 0x04000040 RID: 64
            PhpoShellInfo,
            // Token: 0x04000041 RID: 65
            PhpoRuntimeData,
            // Token: 0x04000042 RID: 66
            PhpoTypeMask = 65535,
            // Token: 0x04000043 RID: 67
            PhpoWow64,
            // Token: 0x04000044 RID: 68
            Phpo64From32 = 131072
        }
    }
}
