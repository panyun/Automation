﻿using EL;
using EL.WindowsAPI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace EL.Input
{
    /// <summary>
    /// Class with various helper tools used in various places
    /// </summary>
    public static class Wait
    {
        private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(1);

        /// <summary>
        /// Waits a little to allow inputs (mouse, keyboard, ...) to be processed.
        /// </summary>
        /// <param name="waitTimeout">An optional timeout. If no value or null is passed, the timeout is 100ms.</param>
        public static void UntilInputIsProcessed(TimeSpan? waitTimeout = null)
        {
            // Let the thread some time to process the system's hardware input queue.
            // For details see this post: http://blogs.msdn.com/b/oldnewthing/archive/2014/02/13/10499047.aspx
            var waitTime = (waitTimeout ?? TimeSpan.FromMilliseconds(20)).TotalMilliseconds;
            Thread.Sleep((int)waitTime);
        }

        /// <summary>
        /// Waits until the given hwnd is responsive.
        /// See: https://blogs.msdn.microsoft.com/oldnewthing/20161118-00/?p=94745
        /// </summary>
        /// <param name="hWnd">The hwnd that should be waited for.</param>
        /// <returns>True if the hwnd was responsive, false otherwise.</returns>
        public static bool UntilResponsive(IntPtr hWnd)
        {
            return UntilResponsive(hWnd, DefaultTimeout);
        }

        /// <summary>
        /// Waits until the given hwnd is responsive.
        /// See: https://blogs.msdn.microsoft.com/oldnewthing/20161118-00/?p=94745
        /// </summary>
        /// <param name="hWnd">The hwnd that should be waited for.</param>
        /// <param name="timeout">The timeout of the waiting.</param>
        /// <returns>True if the hwnd was responsive, false otherwise.</returns>
        public static bool UntilResponsive(IntPtr hWnd, TimeSpan timeout)
        {
            var ret = User32.SendMessageTimeout(hWnd, WindowsMessages.WM_NULL,
                UIntPtr.Zero, IntPtr.Zero, SendMessageTimeoutFlags.SMTO_NORMAL, (uint)timeout.TotalMilliseconds, out _);
            // There might be other things going on so do a small sleep anyway...
            // Other sources: http://blogs.msdn.com/b/oldnewthing/archive/2014/02/13/10499047.aspx
            Thread.Sleep(20);
            return ret != IntPtr.Zero;
        }
    }
    
  
}
