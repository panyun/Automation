using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security;

namespace EL.WindowsAPI
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public static class Kernel32
    {
        public const string KERNEL32 = "kernel32.dll";

        /// <summary>
        /// 主线程唯一标识符
        /// </summary>
        /// <returns></returns>
        [DllImport(KERNEL32)]
        public static extern uint GetCurrentThreadId();
        [DllImport(KERNEL32)]
        public static extern IntPtr OpenProcess(int  dwDesiredAccess, bool bInheritHandle, int dwProcessId);
        [DllImport("kernel32.dll", SetLastError = true)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);
        /// <summary>
        /// 获取一个应用程序或动态链接库的模块句柄 
        /// </summary>
        /// <param name="moduleName"></param>
        /// <returns></returns>
        [DllImport(KERNEL32, CharSet = CharSet.Auto, BestFitMapping = false, SetLastError = true)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        [ResourceExposure(ResourceScope.Process)]  // Is your module side-by-side?
        public static extern IntPtr GetModuleHandle(String moduleName);

        /// <summary>
        /// GetProcAddress函数检索指定的动态链接库(DLL)中的输出库函数地址
        /// </summary>
        /// <param name="hModule"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        [DllImport(KERNEL32, CharSet = CharSet.Ansi, BestFitMapping = false, SetLastError = true, ExactSpelling = true)]
        [ResourceExposure(ResourceScope.None)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, String methodName);

        [DllImport(KERNEL32, SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        [ResourceExposure(ResourceScope.Machine)]
        public static extern bool IsWow64Process([In] IntPtr hSourceProcessHandle, [Out, MarshalAs(UnmanagedType.Bool)] out bool isWow64);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr LoadLibrary(string lpFileName);
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
