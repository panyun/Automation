using System.Runtime.InteropServices;
using System.Text;

namespace EL.WindowsAPI
{
    public static class Oleacc
    {
        [DllImport("oleacc.dll")]
        public static extern uint GetRoleText(AccessibilityRole dwRole, [Out] StringBuilder lpszRole, uint cchRoleMax);

        [DllImport("oleacc.dll")]
        public static extern uint GetStateText(AccessibilityState dwStateBit, [Out] StringBuilder lpszStateBit, uint cchStateBitMax);
        [DllImport("oleacc.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ObjectFromLresult(
           [In] int lResult,
           [In] ref Guid riid,
           [In] int wParam,
           [Out, MarshalAs(UnmanagedType.IUnknown)] out object ppvObject
           );
    }
}
