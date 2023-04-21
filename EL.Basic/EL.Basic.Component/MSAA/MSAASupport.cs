
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Interop.UIAutomationClient;

namespace EL.MSAA
{
    public class MSAAComponent
    {
        public const uint OBJID_WINDOW = 0x00000000;
        public const uint OBJID_SYSMENU = 0xFFFFFFFF;
        public const uint OBJID_TITLEBAR = 0xFFFFFFFE;
        public const uint OBJID_MENU = 0xFFFFFFFD;
        public const uint OBJID_CLIENT = 0xFFFFFFFC;
        public const uint OBJID_VSCROLL = 0xFFFFFFFB;
        public const uint OBJID_HSCROLL = 0xFFFFFFFA;
        public const uint OBJID_SIZEGRIP = 0xFFFFFFF9;
        public const uint OBJID_CARET = 0xFFFFFFF8;
        public const uint OBJID_CURSOR = 0xFFFFFFF7;
        public const uint OBJID_ALERT = 0xFFFFFFF6;
        public const uint OBJID_SOUND = 0xFFFFFFF5;

        private IAccessible accessible;
        private MSAAProperties properties;
        //构造函数
        public MSAAComponent(IntPtr handle, uint OBJID)
        {
            object ppvObject = null;
            Guid guidCOM = new Guid("618736E0-3C3D-11CF-810C-00AA00389B71");
            AccessibleObjectFromWindow(handle, OBJID, ref guidCOM, ref ppvObject);
            this.accessible = ppvObject as IAccessible;
            if (this.accessible == null)
                throw new ArgumentException("当前句柄无效");
            this.properties = new MSAAProperties(this.accessible);
        }
        //构造函数
        public MSAAComponent(IntPtr handle) : this(handle, OBJID_CLIENT) { }
        //构造函数
        public MSAAComponent(IAccessible accessible)
        {
            this.accessible = accessible;
            if (this.accessible == null)
                throw new ArgumentException("当前句柄无效");
            this.properties = new MSAAProperties(this.accessible);
        }
        //构造函数
        public MSAAComponent(IAccessible accessible, int childId)
        {
            if (accessible == null)
                throw new ArgumentException("当前句柄无效");
            if (childId < 0)
                throw new ArgumentException("子代元素ID不能为负值");
            if (childId == 0)
                this.accessible = accessible;
            this.properties = new MSAAProperties(accessible, childId);
        }
        //Accessible 接口
        public IAccessible Accessible { get { return this.accessible; } }
        //获取 MSAA 属性
        public MSAAProperties Properties { get { return this.properties; } }
        public static List<MSAAComponent> GetAccessibleChildren(IAccessible accessible)
        {
            if (accessible == null) return default;
            int childCount = 0;
            try { childCount = accessible.accChildCount; }
            catch { }

            object[] array = new object[childCount];
            int pcObtained = 0;
            if (childCount != 0)
                AccessibleChildren(accessible, 0, childCount, array, ref pcObtained);
            List<MSAAComponent> result = new();
            for (int i = 0; i < pcObtained; i++)
            {
                IAccessible child = array[i] as IAccessible;
                if (child == null)
                    result.Add(new MSAAComponent(accessible, Convert.ToInt32(array[i])));
                else
                    result.Add(new MSAAComponent(child));
            }
            return result;
        }
        public List<MSAAComponent> GetAccessibleChildren()
        {
            return GetAccessibleChildren(this.accessible);
        }
        public static MSAAComponent GetAccessibleChild(IAccessible accessible, int[] indexs)
        {
            if (indexs.Length != 0)
            {
                List<MSAAComponent> children = GetAccessibleChildren(accessible);
                MSAAComponent child = children[indexs[0]];

                if (child.Properties.ChildCount == 0)
                    return null;
                return GetAccessibleChild(child.accessible, indexs.Skip(1).ToArray());
            }
            else
                return new MSAAComponent(accessible);
        }
        public MSAAComponent GetAccessibleChild(int[] indexs)
        {
            return GetAccessibleChild(this.accessible, indexs);
        }
        #region API
        [DllImport("oleacc.dll")]
        internal static extern int AccessibleObjectFromWindow(
             IntPtr hwnd,
             uint dwObjectID,
             ref Guid riid,
             [In, Out, MarshalAs(UnmanagedType.IUnknown)]
             ref object ppvObject
            );

        [DllImport("oleacc.dll")]
        public static extern int AccessibleChildren(
            IAccessible paccContainer,
            int iChildStart,
            int cChildren,
            [Out()] [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)]
            object[] rgvarChildren, ref int pcObtained
            );
        #endregion
    }

}
