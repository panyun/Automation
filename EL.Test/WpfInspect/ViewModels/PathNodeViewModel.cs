using System;
using WpfInspect.Core;

namespace WpfInspect.ViewModels
{
    /// <summary>
    /// 筛选特征与筛选结果 展示路径模型
    /// </summary>
    public class PathNodeViewModel : ObservableObject
    {
        public Action ActiveChangeMethod;
        /// <summary>
        /// 是否选中
        /// </summary>
        public bool IsActive
        {
            get { return GetProperty<bool>(); }
            set
            {
                SetProperty(value);
                if (ActiveChangeMethod != null)
                    ActiveChangeMethod.Invoke();
            }
        }
        /// <summary>
        /// 在父子节点中所占节点索引值 从1开始
        /// </summary>
        public int SortNumber { get { return GetProperty<int>(); } set { SetProperty(value); } }
        /// <summary>
        /// 节点名
        /// </summary>
        public string Name { get { return GetProperty<string>(); } set { SetProperty(value); } }
        /// <summary>
        /// 是否显示节点名
        /// </summary>
        public bool IsActiveName { get; set; } = true;
        /// <summary>
        /// 想要匹配索引值
        /// </summary>
        public int Index { get { return GetProperty<int>(); } set { SetProperty(value); } }
        /// <summary>
        /// 是否显示节点索引
        /// </summary>
        public bool IsActiveIndex { get; set; } = true;
        /// <summary>
        /// 节点Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 控件类型
        /// </summary>
        public string ControTypeName { get; set; }
        /// <summary>
        /// 显示的筛选结果
        /// </summary>
        public string FiltRes { get { return GetProperty<string>(); } set { SetProperty(value); } }
        /// <summary>
        /// 设置筛选结果(更新UI界面使用)
        /// </summary>
        public void SetFeiltRes()
        {
            var value = $"ctype={ControTypeName}";
            if (Index != 0 && IsActiveIndex)
                value += $";index={Index}";
            if (!string.IsNullOrEmpty(Name) && IsActiveName)
                value += $";name='{Name.ContrlStr()}'";
            FiltRes = value;
            //FiltRes = (IsActiveName ? Name : "").ContrlStr() + " " + (IsActiveIndex ? (ControTypeName + $"[{Index}]") : "");
        }
    }

    /// <summary>
    /// NODE详情
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PathNodeDetails : ObservableObject
    {
        public Action<PathNodeDetails> ChangeMethod;
        /// <summary>
        /// PathNodeViewModel Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 属性名
        /// </summary>
        public string PropertyName { get; set; }
        /// <summary>
        /// 属性值
        /// </summary>
        public object PropertyValue
        {
            get
            { return GetProperty<object>(); }
            set
            {
                SetProperty(value);
                if (ChangeMethod != null)
                    ChangeMethod.Invoke(this);
            }
        }
        /// <summary>
        /// 是否选择
        /// </summary>
        public bool IsActive
        {
            get { return GetProperty<bool>(); }
            set
            {
                SetProperty(value);
                if (ChangeMethod != null)
                    ChangeMethod.Invoke(this);
            }
        }
        /// <summary>
        /// 在父子节点中所占节点索引值 从1开始
        /// </summary>
        public int SortNumber { get; set; }
    }
}
