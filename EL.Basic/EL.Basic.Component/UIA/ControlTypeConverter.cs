using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using Interop.UIAutomationClient;

namespace EL.UIA
{
    /// <summary>
    /// Converter with converts between <see cref="UIA.UIA_ControlTypeIds"/> and FlaUIs <see cref="ControlType"/>.
    /// </summary>
    public static class ControlTypeConverter
    {
        /// <summary>
        /// Converts a <see cref="UIA.UIA_ControlTypeIds"/> to a FlaUI <see cref="ControlType"/>.
        /// </summary>
        public static ControlType ToControlType(object nativeControlType)
        {
            switch ((int)nativeControlType)
            {
                case UIA_ControlTypeIds.UIA_ButtonControlTypeId:
                    return ControlType.Button;
                case UIA_ControlTypeIds.UIA_CalendarControlTypeId:
                    return ControlType.Calendar;
                case UIA_ControlTypeIds.UIA_CheckBoxControlTypeId:
                    return ControlType.CheckBox;
                case UIA_ControlTypeIds.UIA_ComboBoxControlTypeId:
                    return ControlType.ComboBox;
                case UIA_ControlTypeIds.UIA_CustomControlTypeId:
                    return ControlType.Custom;
                case UIA_ControlTypeIds.UIA_DataGridControlTypeId:
                    return ControlType.DataGrid;
                case UIA_ControlTypeIds.UIA_DataItemControlTypeId:
                    return ControlType.DataItem;
                case UIA_ControlTypeIds.UIA_DocumentControlTypeId:
                    return ControlType.Document;
                case UIA_ControlTypeIds.UIA_EditControlTypeId:
                    return ControlType.Edit;
                case UIA_ControlTypeIds.UIA_GroupControlTypeId:
                    return ControlType.Group;
                case UIA_ControlTypeIds.UIA_HeaderControlTypeId:
                    return ControlType.Header;
                case UIA_ControlTypeIds.UIA_HeaderItemControlTypeId:
                    return ControlType.HeaderItem;
                case UIA_ControlTypeIds.UIA_HyperlinkControlTypeId:
                    return ControlType.Hyperlink;
                case UIA_ControlTypeIds.UIA_ImageControlTypeId:
                    return ControlType.Image;
                case UIA_ControlTypeIds.UIA_ListControlTypeId:
                    return ControlType.List;
                case UIA_ControlTypeIds.UIA_ListItemControlTypeId:
                    return ControlType.ListItem;
                case UIA_ControlTypeIds.UIA_MenuBarControlTypeId:
                    return ControlType.MenuBar;
                case UIA_ControlTypeIds.UIA_MenuControlTypeId:
                    return ControlType.Menu;
                case UIA_ControlTypeIds.UIA_MenuItemControlTypeId:
                    return ControlType.MenuItem;
                case UIA_ControlTypeIds.UIA_PaneControlTypeId:
                    return ControlType.Pane;
                case UIA_ControlTypeIds.UIA_ProgressBarControlTypeId:
                    return ControlType.ProgressBar;
                case UIA_ControlTypeIds.UIA_RadioButtonControlTypeId:
                    return ControlType.RadioButton;
                case UIA_ControlTypeIds.UIA_ScrollBarControlTypeId:
                    return ControlType.ScrollBar;
                case UIA_ControlTypeIds.UIA_SemanticZoomControlTypeId:
                    return ControlType.SemanticZoom;
                case UIA_ControlTypeIds.UIA_SeparatorControlTypeId:
                    return ControlType.Separator;
                case UIA_ControlTypeIds.UIA_SliderControlTypeId:
                    return ControlType.Slider;
                case UIA_ControlTypeIds.UIA_SpinnerControlTypeId:
                    return ControlType.Spinner;
                case UIA_ControlTypeIds.UIA_SplitButtonControlTypeId:
                    return ControlType.SplitButton;
                case UIA_ControlTypeIds.UIA_StatusBarControlTypeId:
                    return ControlType.StatusBar;
                case UIA_ControlTypeIds.UIA_TabControlTypeId:
                    return ControlType.Tab;
                case UIA_ControlTypeIds.UIA_TabItemControlTypeId:
                    return ControlType.TabItem;
                case UIA_ControlTypeIds.UIA_TableControlTypeId:
                    return ControlType.Table;
                case UIA_ControlTypeIds.UIA_TextControlTypeId:
                    return ControlType.Text;
                case UIA_ControlTypeIds.UIA_ThumbControlTypeId:
                    return ControlType.Thumb;
                case UIA_ControlTypeIds.UIA_TitleBarControlTypeId:
                    return ControlType.TitleBar;
                case UIA_ControlTypeIds.UIA_ToolBarControlTypeId:
                    return ControlType.ToolBar;
                case UIA_ControlTypeIds.UIA_ToolTipControlTypeId:
                    return ControlType.ToolTip;
                case UIA_ControlTypeIds.UIA_TreeControlTypeId:
                    return ControlType.Tree;
                case UIA_ControlTypeIds.UIA_TreeItemControlTypeId:
                    return ControlType.TreeItem;
                case UIA_ControlTypeIds.UIA_WindowControlTypeId:
                    return ControlType.Window;
                case UIA_ControlTypeIds.UIA_AppBarControlTypeId:
                    return ControlType.AppBar;
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Converts a FlaUI <see cref="ControlType"/> to a <see cref="UIA.UIA_ControlTypeIds"/>.
        /// </summary>
        public static object ToControlTypeNative(ControlType controlType)
        {
            switch (controlType)
            {
                case ControlType.Button:
                    return UIA_ControlTypeIds.UIA_ButtonControlTypeId;
                case ControlType.Calendar:
                    return UIA_ControlTypeIds.UIA_CalendarControlTypeId;
                case ControlType.CheckBox:
                    return UIA_ControlTypeIds.UIA_CheckBoxControlTypeId;
                case ControlType.ComboBox:
                    return UIA_ControlTypeIds.UIA_ComboBoxControlTypeId;
                case ControlType.Custom:
                    return UIA_ControlTypeIds.UIA_CustomControlTypeId;
                case ControlType.DataGrid:
                    return UIA_ControlTypeIds.UIA_DataGridControlTypeId;
                case ControlType.DataItem:
                    return UIA_ControlTypeIds.UIA_DataItemControlTypeId;
                case ControlType.Document:
                    return UIA_ControlTypeIds.UIA_DocumentControlTypeId;
                case ControlType.Edit:
                    return UIA_ControlTypeIds.UIA_EditControlTypeId;
                case ControlType.Group:
                    return UIA_ControlTypeIds.UIA_GroupControlTypeId;
                case ControlType.Header:
                    return UIA_ControlTypeIds.UIA_HeaderControlTypeId;
                case ControlType.HeaderItem:
                    return UIA_ControlTypeIds.UIA_HeaderItemControlTypeId;
                case ControlType.Hyperlink:
                    return UIA_ControlTypeIds.UIA_HyperlinkControlTypeId;
                case ControlType.Image:
                    return UIA_ControlTypeIds.UIA_ImageControlTypeId;
                case ControlType.List:
                    return UIA_ControlTypeIds.UIA_ListControlTypeId;
                case ControlType.ListItem:
                    return UIA_ControlTypeIds.UIA_ListItemControlTypeId;
                case ControlType.MenuBar:
                    return UIA_ControlTypeIds.UIA_MenuBarControlTypeId;
                case ControlType.Menu:
                    return UIA_ControlTypeIds.UIA_MenuControlTypeId;
                case ControlType.MenuItem:
                    return UIA_ControlTypeIds.UIA_MenuItemControlTypeId;
                case ControlType.Pane:
                    return UIA_ControlTypeIds.UIA_PaneControlTypeId;
                case ControlType.ProgressBar:
                    return UIA_ControlTypeIds.UIA_ProgressBarControlTypeId;
                case ControlType.RadioButton:
                    return UIA_ControlTypeIds.UIA_RadioButtonControlTypeId;
                case ControlType.ScrollBar:
                    return UIA_ControlTypeIds.UIA_ScrollBarControlTypeId;
                case ControlType.SemanticZoom:
                    return UIA_ControlTypeIds.UIA_SemanticZoomControlTypeId;
                case ControlType.Separator:
                    return UIA_ControlTypeIds.UIA_SeparatorControlTypeId;
                case ControlType.Slider:
                    return UIA_ControlTypeIds.UIA_SliderControlTypeId;
                case ControlType.Spinner:
                    return UIA_ControlTypeIds.UIA_SpinnerControlTypeId;
                case ControlType.SplitButton:
                    return UIA_ControlTypeIds.UIA_SplitButtonControlTypeId;
                case ControlType.StatusBar:
                    return UIA_ControlTypeIds.UIA_StatusBarControlTypeId;
                case ControlType.Tab:
                    return UIA_ControlTypeIds.UIA_TabControlTypeId;
                case ControlType.TabItem:
                    return UIA_ControlTypeIds.UIA_TabItemControlTypeId;
                case ControlType.Table:
                    return UIA_ControlTypeIds.UIA_TableControlTypeId;
                case ControlType.Text:
                    return UIA_ControlTypeIds.UIA_TextControlTypeId;
                case ControlType.Thumb:
                    return UIA_ControlTypeIds.UIA_ThumbControlTypeId;
                case ControlType.TitleBar:
                    return UIA_ControlTypeIds.UIA_TitleBarControlTypeId;
                case ControlType.ToolBar:
                    return UIA_ControlTypeIds.UIA_ToolBarControlTypeId;
                case ControlType.ToolTip:
                    return UIA_ControlTypeIds.UIA_ToolTipControlTypeId;
                case ControlType.Tree:
                    return UIA_ControlTypeIds.UIA_TreeControlTypeId;
                case ControlType.TreeItem:
                    return UIA_ControlTypeIds.UIA_TreeItemControlTypeId;
                case ControlType.Window:
                    return UIA_ControlTypeIds.UIA_WindowControlTypeId;
                case ControlType.AppBar:
                    return UIA_ControlTypeIds.UIA_AppBarControlTypeId;
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Converts a FlaUI <see cref="ControlType"/> to a <see cref="UIA.UIA_ControlTypeIds"/>.
        /// </summary>
        public static string ConvertTypeToName(int controlType)
        {
            var type = ToControlType(controlType);

            switch (type)
            {
                case ControlType.Button:
                    return "按钮";
                case ControlType.Calendar:
                    return "日历";
                case ControlType.CheckBox:
                    return "复选框";
                case ControlType.ComboBox:
                    return "下拉列表";
                case ControlType.Custom:
                    return "控件";
                case ControlType.DataGrid:
                    return "网格列表";
                case ControlType.DataItem:
                    return "数据项";
                case ControlType.Document:
                    return "文档";
                case ControlType.Edit:
                    return "可编辑文本";
                case ControlType.Group:
                    return "组";
                case ControlType.Header:
                    return "标题控件";
                case ControlType.HeaderItem:
                    return "标题项";
                case ControlType.Hyperlink:
                    return "超文本";
                case ControlType.Image:
                    return "图片";
                case ControlType.List:
                    return "列表";
                case ControlType.ListItem:
                    return "列表子项";
                case ControlType.MenuBar:
                    return "菜单栏";
                case ControlType.Menu:
                    return "菜单";
                case ControlType.MenuItem:
                    return "菜单项";
                case ControlType.Pane:
                    return "窗格";
                case ControlType.ProgressBar:
                    return "进度条";
                case ControlType.RadioButton:
                    return "单选";
                case ControlType.ScrollBar:
                    return "滚动条";
                case ControlType.SemanticZoom:
                    return "缩放视图";
                case ControlType.Separator:
                    return "分隔符";
                case ControlType.Slider:
                    return "滑块控件";
                case ControlType.Spinner:
                    return "微调控件";
                case ControlType.SplitButton:
                    return "拆分控件";
                case ControlType.StatusBar:
                    return "状态栏";
                case ControlType.Tab:
                    return "选项";
                case ControlType.TabItem:
                    return "选项卡";
                case ControlType.Table:
                    return "表";
                case ControlType.Text:
                    return "文本";
                case ControlType.Thumb:
                    return "滚动条位置";
                case ControlType.TitleBar:
                    return "窗口标题栏";
                case ControlType.ToolBar:
                    return "工具栏";
                case ControlType.ToolTip:
                    return "工具提示";
                case ControlType.Tree:
                    return "树";
                case ControlType.TreeItem:
                    return "树子项";
                case ControlType.Window:
                    return "窗口";
                case ControlType.AppBar:
                    return "应用";
                default:
                    return ControlType.AppBar.ToString();
            }
        }

        public enum ControlType
        {

            /// <summary>
            /// Identifies an unknown control type.
            /// </summary>
            Unknown,

            /// <summary>
            /// Identifies the AppBar control type. Supported starting with Windows 8.1.
            /// </summary>
            AppBar,

            /// <summary>
            /// Identifies the Button control type.
            /// </summary>
            Button,

            /// <summary>
            /// Identifies the Calendar control type.
            /// </summary>
            Calendar,

            /// <summary>
            /// Identifies the CheckBox control type.
            /// </summary>
            CheckBox,

            /// <summary>
            /// Identifies the ComboBox control type.
            /// </summary>
            ComboBox,

            /// <summary>
            /// Identifies the Custom control type.
            /// </summary>
            Custom,

            /// <summary>
            /// Identifies the DataGrid control type.
            /// </summary>
            DataGrid,

            /// <summary>
            /// Identifies the DataItem control type.
            /// </summary>
            DataItem,

            /// <summary>
            /// Identifies the Document control type.
            /// </summary>
            Document,

            /// <summary>
            /// Identifies the Edit control type.
            /// </summary>
            Edit,

            /// <summary>
            /// Identifies the Group control type.
            /// </summary>
            Group,

            /// <summary>
            /// Identifies the Header control type.
            /// </summary>
            Header,

            /// <summary>
            /// Identifies the HeaderItem control type.
            /// </summary>
            HeaderItem,

            /// <summary>
            /// Identifies the Hyperlink control type.
            /// </summary>
            Hyperlink,

            /// <summary>
            /// Identifies the Image control type.
            /// </summary>
            Image,

            /// <summary>
            /// Identifies the List control type.
            /// </summary>
            List,

            /// <summary>
            /// Identifies the ListItem control type.
            /// </summary>
            ListItem,

            /// <summary>
            /// Identifies the MenuBar control type.
            /// </summary>
            MenuBar,

            /// <summary>
            /// Identifies the Menu control type.
            /// </summary>
            Menu,

            /// <summary>
            /// Identifies the MenuItem control type.
            /// </summary>
            MenuItem,

            /// <summary>
            /// Identifies the Pane control type.
            /// </summary>
            Pane,

            /// <summary>
            /// Identifies the ProgressBar control type.
            /// </summary>
            ProgressBar,

            /// <summary>
            /// Identifies the RadioButton control type.
            /// </summary>
            RadioButton,

            /// <summary>
            /// Identifies the ScrollBar control type.
            /// </summary>
            ScrollBar,

            /// <summary>
            /// Identifies the SemanticZoom control type. Supported starting with Windows 8.
            /// </summary>
            SemanticZoom,

            /// <summary>
            /// Identifies the Separator control type.
            /// </summary>
            Separator,

            /// <summary>
            /// Identifies the Slider control type.
            /// </summary>
            Slider,

            /// <summary>
            /// Identifies the Spinner control type.
            /// </summary>
            Spinner,

            /// <summary>
            /// Identifies the SplitButton control type.
            /// </summary>
            SplitButton,

            /// <summary>
            /// Identifies the StatusBar control type.
            /// </summary>
            StatusBar,

            /// <summary>
            /// Identifies the Tab control type.
            /// </summary>
            Tab,

            /// <summary>
            /// Identifies the TabItem control type.
            /// </summary>
            TabItem,

            /// <summary>
            /// Identifies the Table control type.
            /// </summary>
            Table,

            /// <summary>
            /// Identifies the Text control type.
            /// </summary>
            Text,

            /// <summary>
            /// Identifies the Thumb control type.
            /// </summary>
            Thumb,

            /// <summary>
            /// Identifies the TitleBar control type.
            /// </summary>
            TitleBar,

            /// <summary>
            /// Identifies the ToolBar control type.
            /// </summary>
            ToolBar,

            /// <summary>
            /// Identifies the ToolTip control type.
            /// </summary>
            ToolTip,

            /// <summary>
            /// Identifies the Tree control type.
            /// </summary>
            Tree,

            /// <summary>
            /// Identifies the TreeItem control type.
            /// </summary>
            TreeItem,

            /// <summary>
            /// Identifies the Window control type.
            /// </summary>
            Window,
            JavaNode
        }
    }
    public static class ElemenetSystem
    {
        /// <summary>
        /// 32位MD5加密
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string GetCompareTempId(IUIAutomationElement e)
        {
            var json = JsonHelper.ToJson(new
            {
                e.CurrentAcceleratorKey,
                e.CurrentAccessKey,
                e.CurrentAriaRole,
                e.CurrentControlType,
                e.CurrentFrameworkId,
                e.CurrentItemStatus,
                e.CurrentOrientation,
            });

            string cl = json;
            string pwd = "";
            MD5 md5 = MD5.Create(); //实例化一个md5对像
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
            // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
            for (int i = 0; i < s.Length; i++)
            {
                // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符 
                pwd = pwd + s[i].ToString("X");
            }

            return pwd;
        }
        /// <summary>
        /// 32位MD5加密
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string GetCompareId(IUIAutomationElement e)
        {
            var json = JsonHelper.ToJson(new
            {
                e.CurrentAcceleratorKey,
                e.CurrentAccessKey,
                e.CurrentAriaRole,
                e.CurrentAutomationId,
                e.CurrentControlType,
                e.CurrentFrameworkId,
                e.CurrentItemStatus,
                e.CurrentOrientation,
                e.CurrentClassName,
                e.CurrentName,
            });
            string cl = json;
            string pwd = "";
            MD5 md5 = MD5.Create(); //实例化一个md5对像
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
            // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
            for (int i = 0; i < s.Length; i++)
            {
                // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符 
                pwd = pwd + s[i].ToString("X");
            }
            return pwd;
        }

        public static string GetCompareId(this string json)
        {
            if (json == null) json = "";
            string cl = json;
            string pwd = "";
            MD5 md5 = MD5.Create(); //实例化一个md5对像
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
            // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
            for (int i = 0; i < s.Length; i++)
            {
                // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符 
                pwd = pwd + s[i].ToString("X");
            }
            return pwd;
        }
    }


}
