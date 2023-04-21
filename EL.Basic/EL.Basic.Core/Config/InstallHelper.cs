using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace EL.JavaBridge.Installer
{
    public static class InstallHelper
    {
        // Token: 0x06000029 RID: 41 RVA: 0x00002320 File Offset: 0x00000520
        public static void KillJuschedProcesses()
        {
            foreach (Process process in Process.GetProcessesByName("jusched"))
            {
                try
                {
                    process.Kill();
                }
                catch
                {
                }
                finally
                {
                    if (process != null)
                    {
                        process.Dispose();
                    }
                }
            }
        }

        // Token: 0x0600002A RID: 42 RVA: 0x0000237C File Offset: 0x0000057C
        public static bool HasInstallerProcess()
        {
            Process[] installerProcess = Process.GetProcessesByName("ShadowBot.JavaBridge.Installer");
            return installerProcess != null && installerProcess.Length > 1;
        }

        // Token: 0x0600002B RID: 43 RVA: 0x000023A0 File Offset: 0x000005A0
        public static string GetJavaVersion(string javaFileName)
        {
            Process process = new Process();
            string value;
            try
            {
                process.StartInfo.FileName = javaFileName;
                process.StartInfo.Arguments = "-version";
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                string text = process.StandardError.ReadToEnd();
                if (!text.Contains("java version"))
                {
                    throw new Exception("java env error.");
                }
                value = Regex.Match(text, "java version\\s*\"*(\\d+\\.\\d+).*").Groups[1].Value;
            }
            catch (Exception e)
            {
                throw new JavaBridgeInstallException("known java version：" + e.Message);
            }
            return value;
        }

        // Token: 0x0600002C RID: 44 RVA: 0x00002464 File Offset: 0x00000664
        public static bool IsJava64Bit(string javaFileName)
        {
            bool result;
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = javaFileName;
                process.StartInfo.Arguments = "-version";
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                if (process.StandardError.ReadToEnd().Contains("64-Bit"))
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            catch (Exception e)
            {
                Log.Warn("known java version：" + e.Message);
                result = false;
            }
            return result;
        }

        // Token: 0x0600002D RID: 45 RVA: 0x00002508 File Offset: 0x00000708
        public static void CopyToSystemRoot(string bridgeLibFolder)
        {
            Log.Info("CopyToSystemRoot bridgeLibFolder:" + bridgeLibFolder);
            string systemRoot = Environment.GetEnvironmentVariable("SYSTEMROOT");
            if (Environment.Is64BitOperatingSystem)
            {
                Log.Info("Is64BitOperatingSystem");
                if (Environment.Is64BitProcess)
                {
                    InstallHelper.JavaBridgeDllCopy(Path.Combine(bridgeLibFolder, "WindowsAccessBridge-64.dll"), systemRoot + "\\System32\\WindowsAccessBridge-64.dll", true);
                }
                else
                {
                    InstallHelper.JavaBridgeDllCopy(Path.Combine(bridgeLibFolder, "WindowsAccessBridge-64.dll"), systemRoot + "\\System32\\WindowsAccessBridge-64.dll", true);
                    //InstallHelper.JavaBridgeDllCopy(Path.Combine(bridgeLibFolder, "WindowsAccessBridge-64.dll"), systemRoot + "\\sysnative\\WindowsAccessBridge-64.dll", true);
                }
                InstallHelper.JavaBridgeDllCopy(Path.Combine(bridgeLibFolder, "WindowsAccessBridge-32.dll"), systemRoot + "\\SysWOW64\\WindowsAccessBridge-32.dll", true);
                return;
            }
            InstallHelper.JavaBridgeDllCopy(Path.Combine(bridgeLibFolder, "WindowsAccessBridge.dll"), systemRoot + "\\System32\\WindowsAccessBridge.dll", true);
        }

        // Token: 0x0600002E RID: 46 RVA: 0x000025BC File Offset: 0x000007BC
        public static void CopyToJre(string bridgeLibFolder, string javaHomeJreFolder)
        {
            string jreBinFolder = Path.Combine(javaHomeJreFolder, "bin");
            bool is64BitJava = InstallHelper.IsJava64Bit(jreBinFolder + "\\java.exe");
            string jreLibExtFolder = Path.Combine(javaHomeJreFolder, "lib\\ext");
            Environment.GetEnvironmentVariable("SystemRoot");
            if (Directory.Exists(jreLibExtFolder))
            {
                try
                {
                    Directory.CreateDirectory(jreLibExtFolder);
                }
                catch (UnauthorizedAccessException e)
                {
                    Log.Error("UnauthorizedAccessException,Path: " + jreLibExtFolder + ",Message: " + e.Message);
                    throw new JavaBridgeInstallException(jreLibExtFolder);
                }
            }
            if (Directory.Exists(jreBinFolder))
            {
                string abPath = Path.Combine(jreLibExtFolder, "access-bridge.jar");
                string ab32Path = Path.Combine(jreLibExtFolder, "access-bridge-32.jar");
                string ab64Path = Path.Combine(jreLibExtFolder, "access-bridge-64.jar");
                string jabPath = Path.Combine(jreBinFolder, "JavaAccessBridge.dll");
                string jab64Paht = Path.Combine(jreBinFolder, "JavaAccessBridge-64.dll");
                string jawtabPath = Path.Combine(jreBinFolder, "JAWTAccessBridge.dll");
                string jawtab64Path = Path.Combine(jreBinFolder, "JAWTAccessBridge-64.dll");
                string winabPath = Path.Combine(jreBinFolder, "WindowsAccessBridge.dll");
                string winab32Path = Path.Combine(jreBinFolder, "WindowsAccessBridge-32.dll");
                string winab64Path = Path.Combine(jreBinFolder, "WindowsAccessBridge-64.dll");
                string jacPath = Path.Combine(jreLibExtFolder, "jaccess.jar");
                if (Environment.Is64BitOperatingSystem)
                {
                    if (is64BitJava)
                    {
                        InstallHelper.JavaBridgeDllCopy(Path.Combine(bridgeLibFolder, "access-bridge.jar"), ab64Path, true);
                        InstallHelper.JavaBridgeDllCopy(Path.Combine(bridgeLibFolder, "JavaAccessBridge-64.dll"), jab64Paht, true);
                        InstallHelper.JavaBridgeDllCopy(Path.Combine(bridgeLibFolder, "JAWTAccessBridge-64.dll"), jawtab64Path, true);
                        InstallHelper.JavaBridgeDllCopy(Path.Combine(bridgeLibFolder, "WindowsAccessBridge-64.dll"), winab64Path, true);
                    }
                    else
                    {
                        InstallHelper.JavaBridgeDllCopy(Path.Combine(bridgeLibFolder, "access-bridge.jar"), ab32Path, true);
                        InstallHelper.JavaBridgeDllCopy(Path.Combine(bridgeLibFolder, "JavaAccessBridge.dll"), jabPath, true);
                        InstallHelper.JavaBridgeDllCopy(Path.Combine(bridgeLibFolder, "JAWTAccessBridge.dll"), jawtabPath, true);
                        InstallHelper.JavaBridgeDllCopy(Path.Combine(bridgeLibFolder, "WindowsAccessBridge-32.dll"), winab32Path, true);
                    }
                }
                else
                {
                    InstallHelper.JavaBridgeDllCopy(Path.Combine(bridgeLibFolder, "access-bridge.jar"), abPath, true);
                    InstallHelper.JavaBridgeDllCopy(Path.Combine(bridgeLibFolder, "JavaAccessBridge.dll"), jabPath, true);
                    InstallHelper.JavaBridgeDllCopy(Path.Combine(bridgeLibFolder, "JAWTAccessBridge.dll"), jawtabPath, true);
                    InstallHelper.JavaBridgeDllCopy(Path.Combine(bridgeLibFolder, "WindowsAccessBridge.dll"), winabPath, true);
                }
                InstallHelper.JavaBridgeDllCopy(Path.Combine(bridgeLibFolder, "jaccess.jar"), jacPath, true);
            }
            string jreLibFolder = Path.Combine(javaHomeJreFolder, "lib");
            if (Directory.Exists(jreLibFolder))
            {
                try
                {
                    string propPath = Path.Combine(jreLibFolder, "accessibility.properties");
                    if (File.Exists(propPath))
                    {
                        bool needOverWrite = true;
                        bool containsAssistiveLine = false;
                        string accStr = "com.sun.java.accessibility.AccessBridge";
                        List<string> finalLineList = new List<string>();
                        using (StreamReader reader = new StreamReader(propPath, Encoding.UTF8))
                        {
                            string curLine;
                            while ((curLine = reader.ReadLine()) != null)
                            {
                                if (Regex.IsMatch(curLine, "^assistive_technologies=.*$"))
                                {
                                    if (curLine.Contains(accStr))
                                    {
                                        needOverWrite = false;
                                        break;
                                    }
                                    if (curLine.IndexOf(accStr) < 0)
                                    {
                                        finalLineList.Add(curLine + "," + accStr);
                                    }
                                    else
                                    {
                                        finalLineList.Add(curLine);
                                    }
                                    containsAssistiveLine = true;
                                }
                                else
                                {
                                    finalLineList.Add(curLine);
                                }
                            }
                        }
                        if (needOverWrite)
                        {
                            if (!containsAssistiveLine)
                            {
                                finalLineList.Add("assistive_technologies=" + accStr);
                            }
                            FileStream fileStream = new FileStream(propPath, FileMode.OpenOrCreate, FileAccess.Write);
                            StreamWriter streamWriter = new StreamWriter(fileStream);
                            foreach (string line in finalLineList)
                            {
                                streamWriter.WriteLine(line);
                            }
                            streamWriter.Flush();
                            streamWriter.Close();
                            fileStream.Close();
                        }
                    }
                    else
                    {
                        InstallHelper.JavaBridgeDllCopy(Path.Combine(bridgeLibFolder, "accessibility.properties"), Path.Combine(jreLibFolder, "accessibility.properties"), true);
                    }
                }
                catch (IOException e2)
                {
                    throw new JavaBridgeInstallException(e2.Message);
                }
                catch (UnauthorizedAccessException e3)
                {
                    throw new JavaBridgeInstallException(e3.Message);
                }
            }
        }

        // Token: 0x0600002F RID: 47 RVA: 0x00002A00 File Offset: 0x00000C00
        public static void EnableJabSwitch(string[] jrefolders, string BridgeLibFolder)
        {
            try
            {
                bool enable_finish = false;
                try
                {
                    for (int i = 0; i < jrefolders.Length; i++)
                    {
                        string jreBinFolder = Path.Combine(jrefolders[i], "bin");
                        if (Directory.Exists(jreBinFolder) && File.Exists(Path.Combine(jreBinFolder, "jabswitch.exe")))
                        {
                            InstallHelper.StartJabSwitchExe(jreBinFolder);
                            enable_finish = true;
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex.StackTrace);
                }
                if (!enable_finish)
                {
                    if (Environment.Is64BitOperatingSystem)
                    {
                        InstallHelper.StartJabSwitchExe(BridgeLibFolder);
                    }
                    else
                    {
                        InstallHelper.StartJabSwitchExe(Path.Combine(BridgeLibFolder, "jabswitch32"));
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        // Token: 0x06000030 RID: 48 RVA: 0x00002AB8 File Offset: 0x00000CB8
        public static string[] GetJreFolders()
        {
            List<string> list = new List<string>();
            //string[] javaFs = JreFolderHelper.GetJreFoldersByProcessName("java");
            //JreFolderHelper.UnionJreFolder(list, javaFs);
            //string[] javawFs = JreFolderHelper.GetJreFoldersByProcessName("javaw");
            //JreFolderHelper.UnionJreFolder(list, javawFs);
            //string[] javawsFs = JreFolderHelper.GetJreFoldersByProcessName("javaws");
            //JreFolderHelper.UnionJreFolder(list, javawsFs);
            //string[] jp2lanncherFs = JreFolderHelper.GetJreFoldersByProcessName("jp2launcher");
            //JreFolderHelper.UnionJreFolder(list, jp2lanncherFs);
            string[] registryFs = JreFolderHelper.GetJreFoldersFromRegistry();
            JreFolderHelper.UnionJreFolder(list, registryFs);
            return list.ToArray();
        }

        // Token: 0x06000031 RID: 49 RVA: 0x00002B28 File Offset: 0x00000D28
        private static void JavaBridgeDllDel(string sourceFileName)
        {
            try
            {
                File.Delete(sourceFileName);
            }
            catch (IOException e)
            {
                throw new JavaBridgeInstallException(e.Message);
            }
            catch (UnauthorizedAccessException e2)
            {
                throw new JavaBridgeInstallException(e2.Message);
            }
        }

        // Token: 0x06000032 RID: 50 RVA: 0x00002B94 File Offset: 0x00000D94
        private static void JavaBridgeDllCopy(string sourceFileName, string destFileName, bool overwrite = false)
        {
            if (!overwrite && File.Exists(destFileName))
            {
                return;
            }
            try
            {
                if (File.Exists(destFileName) && !File.Exists(destFileName + "_backup"))
                {
                    File.Move(destFileName, destFileName + "_backup");
                }
                File.Copy(sourceFileName, destFileName, true);
            }
            catch (IOException e)
            {
                if (!e.Message.EndsWith("because it is being used by another process."))
                {
                    throw new JavaBridgeInstallException(e.Message);
                }
            }
            catch (UnauthorizedAccessException e2)
            {
                throw new JavaBridgeInstallException(e2.Message);
            }
        }

        // Token: 0x06000033 RID: 51 RVA: 0x00002C4C File Offset: 0x00000E4C
        private static void StartJabSwitchExe(string workDirectory)
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                WorkingDirectory = workDirectory,
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = "jabswitch.exe",
                Arguments = "-enable",
                UseShellExecute = true
            };
            process.StartInfo = startInfo;
            process.Start();
        }
    }
}
