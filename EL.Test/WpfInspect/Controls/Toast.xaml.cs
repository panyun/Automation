using MahApps.Metro.IconPacks;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;
/*
 * https://blog.csdn.net/weixin_44448313/article/details/107469089
 * https://gitee.com/DLGCY_Clone/WpfToast
 */
namespace WpfInspect.Controls
{
    public class ToastOptions
    {
        public double ToastWidth { get; set; }
        public double ToastHeight { get; set; }
        public double TextWidth { get; set; }
        public int Time { get; set; } = 2000;
        public ToastIcons Icon { get; set; } = ToastIcons.None;
        public ToastLocation Location { get; set; } = ToastLocation.Default;
        public Brush Foreground { get; set; } = (Brush)new BrushConverter().ConvertFromString("#031D38");
        public Brush IconForeground { get; set; } = (Brush)new BrushConverter().ConvertFromString("#00D91A");
        public FontStyle FontStyle { get; set; } = SystemFonts.MessageFontStyle;
        public FontStretch FontStretch { get; set; } = FontStretches.Normal;
        public double FontSize { get; set; } = SystemFonts.MessageFontSize;
        public FontFamily FontFamily { get; set; } = SystemFonts.MessageFontFamily;
        public FontWeight FontWeight { get; set; } = SystemFonts.MenuFontWeight;
        public double IconSize { get; set; } = 26;
        public CornerRadius CornerRadius { get; set; } = new CornerRadius(5);
        public Brush BorderBrush { get; set; } = (Brush)new BrushConverter().ConvertFromString("#CECECE");
        public Thickness BorderThickness { get; set; } = new Thickness(0);
        public Brush Background { get; set; } = (Brush)new BrushConverter().ConvertFromString("#FFFFFF");
        public HorizontalAlignment HorizontalContentAlignment { get; set; } = HorizontalAlignment.Center;
        public VerticalAlignment VerticalContentAlignment { get; set; } = VerticalAlignment.Center;
        public EventHandler<EventArgs> Closed { get; internal set; }
        public EventHandler<EventArgs> Click { get; internal set; }
        public Thickness ToastMargin { get; set; } = new Thickness(2);
    }

    public enum ToastIcons
    {
        None,
        Information,//CheckSolid
        Error,//TimesSolid
        Warning,//ExclamationSolid
        Busy//ClockSolid
    }

    public enum ToastLocation
    {
        OwnerCenter,
        OwnerLeft,
        OwnerRight,
        OwnerTopLeft,
        OwnerTopCenter,
        OwnerTopRight,
        OwnerBottomLeft,
        OwnerBottomCenter,
        OwnerBottomRight,
        ScreenCenter,
        ScreenLeft,
        ScreenRight,
        ScreenTopLeft,
        ScreenTopCenter,
        ScreenTopRight,
        ScreenBottomLeft,
        ScreenBottomCenter,
        ScreenBottomRight,
        Default//OwnerCenter
    }

    public class ToastIconConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            object value = values[0];
            object grid = values[1];
            object txt = values[2];
            Grid _grid = grid as Grid;
            TextBlock _txt = txt as TextBlock;

            void WithoutIcon()
            {
                if (_grid != null)
                {
                    _grid.ColumnDefinitions.RemoveAt(0);
                }

                if (_txt != null)
                {
                    _txt.HorizontalAlignment = HorizontalAlignment.Center;
                }
            }

            if (value == null)
            {
                WithoutIcon();
                return PackIconFontAwesomeKind.None;
            }

            ToastIcons _value;
            try
            {
                _value = (ToastIcons)value;
            }
            catch
            {
                WithoutIcon();
                return PackIconFontAwesomeKind.None;
            }

            switch (_value)
            {
                case ToastIcons.Information:
                    return PackIconFontAwesomeKind.CheckCircleSolid;
                case ToastIcons.Error:
                    return PackIconFontAwesomeKind.TimesSolid;
                case ToastIcons.Warning:
                    return PackIconFontAwesomeKind.ExclamationSolid;
                case ToastIcons.Busy:
                    return PackIconFontAwesomeKind.ClockSolid;
            }

            WithoutIcon();
            return PackIconFontAwesomeKind.None;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Toast.xaml 的交互逻辑
    /// </summary>
    public partial class Toast : UserControl
    {
        private Window owner = null;
        private Popup popup = null;
        private DispatcherTimer timer = null;

        private Toast()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private Toast(Window owner, string message, ToastOptions options = null)
        {
            Message = message;
            InitializeComponent();
            if (options != null)
            {
                if (options.ToastWidth != 0) ToastWidth = options.ToastWidth;
                if (options.ToastHeight != 0) ToastHeight = options.ToastHeight;
                if (options.TextWidth != 0) TextWidth = options.TextWidth;

                Icon = options.Icon;
                Location = options.Location;
                Time = options.Time;
                Closed += options.Closed;
                Click += options.Click;
                Background = options.Background;
                Foreground = options.Foreground;
                FontStyle = options.FontStyle;
                FontStretch = options.FontStretch;
                FontSize = options.FontSize;
                FontFamily = options.FontFamily;
                FontWeight = options.FontWeight;
                IconSize = options.IconSize;
                BorderBrush = options.BorderBrush;
                BorderThickness = options.BorderThickness;
                HorizontalContentAlignment = options.HorizontalContentAlignment;
                VerticalContentAlignment = options.VerticalContentAlignment;
                CornerRadius = options.CornerRadius;
                ToastMargin = options.ToastMargin;
                IconForeground = options.IconForeground;
            }
            this.DataContext = this;
            if (owner == null)
            {
                this.owner = Application.Current.MainWindow;
            }
            else
            {
                this.owner = owner;
            }
            this.owner.Closed += Owner_Closed;
        }

        private void Owner_Closed(object sender, EventArgs e)
        {
            this.Close();
        }

        public static void Show(string msg, ToastOptions options = null, Action action = null)
        {
            var toast = new Toast(null, msg, options);
            int time = toast.Time;
            ShowToast(toast, time, action);
        }

        public static void Show(Window owner, string msg, ToastOptions options = null, Action action = null)
        {
            var toast = new Toast(owner, msg, options);
            int time = toast.Time;
            ShowToast(toast, time, action);
        }

        private static void ShowToast(Toast toast, int time, Action action)
        {
            toast.popup = null;
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                toast.popup = new Popup
                {
                    PopupAnimation = PopupAnimation.Fade,
                    AllowsTransparency = true,
                    StaysOpen = true,
                    Placement = PlacementMode.Left,
                    IsOpen = false,
                    Child = toast,
                    MinWidth = toast.MinWidth,
                    MaxWidth = toast.MaxWidth,
                    MinHeight = toast.MinHeight,
                    MaxHeight = toast.MaxHeight,
                };

                if (toast.ToastWidth != 0)
                {
                    toast.popup.Width = toast.ToastWidth;
                }

                if (toast.ToastHeight != 0)
                {
                    toast.popup.Height = toast.ToastHeight;
                }

                toast.popup.PlacementTarget = GetPopupPlacementTarget(toast); //为 null 则 Popup 定位相对于屏幕的左上角;
                toast.owner.LocationChanged += toast.UpdatePosition;
                toast.owner.SizeChanged += toast.UpdatePosition;
                toast.popup.Closed += Popup_Closed;

                //SetPopupOffset(toast.popup, toast);
                //toast.UpdatePosition(toast, null);
                toast.popup.IsOpen = true;  //先显示出来以确定宽高；
                SetPopupOffset(toast.popup, toast);
                //toast.UpdatePosition(toast, null);
                toast.popup.IsOpen = false; //先关闭再打开来刷新定位；
                toast.popup.IsOpen = true;
            }));
            toast.timer = new DispatcherTimer();
            toast.timer.Tick += (sender, e) =>
            {
                toast.popup.IsOpen = false;
                toast.owner.LocationChanged -= toast.UpdatePosition;
                toast.owner.SizeChanged -= toast.UpdatePosition;
                if (action != null)
                    action();
            };
            toast.timer.Interval = new TimeSpan(0, 0, 0, 0, time);
            toast.timer.Start();
        }

        private void UpdatePosition(object sender, EventArgs e)
        {
            var up = typeof(Popup).GetMethod("UpdatePosition", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (up == null || popup == null)
            {
                return;
            }
            SetPopupOffset(popup, this);
            up.Invoke(popup, null);
        }

        private static void Popup_Closed(object sender, EventArgs e)
        {
            Popup popup = sender as Popup;
            if (popup == null)
            {
                return;
            }
            Toast toast = popup.Child as Toast;
            if (toast == null)
            {
                return;
            }
            toast.RaiseClosed(e);
        }

        private void UserControl_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ClickCount == 1)
            {
                RaiseClick(e);
            }
        }

        /// <summary>
        /// 获取定位目标
        /// </summary>
        /// <param name="toast">Toast 对象</param>
        /// <returns>容器或null</returns>
        private static UIElement GetPopupPlacementTarget(Toast toast)
        {
            switch (toast.Location)
            {
                case ToastLocation.ScreenCenter:
                case ToastLocation.ScreenLeft:
                case ToastLocation.ScreenRight:
                case ToastLocation.ScreenTopLeft:
                case ToastLocation.ScreenTopCenter:
                case ToastLocation.ScreenTopRight:
                case ToastLocation.ScreenBottomLeft:
                case ToastLocation.ScreenBottomCenter:
                case ToastLocation.ScreenBottomRight:
                    return null;
            }
            return toast.owner;
        }

        private static void SetPopupOffset(Popup popup, Toast toast)
        {
            double winTitleHeight = SystemParameters.CaptionHeight; //标题高度为22；
            double owner_width = toast.owner.ActualWidth;
            double owner_height = toast.owner.ActualHeight - winTitleHeight;
            if (popup.PlacementTarget == null)
            {
                owner_width = SystemParameters.WorkArea.Size.Width;
                owner_height = SystemParameters.WorkArea.Size.Height;
            }

            double popupWidth = (popup.Child as FrameworkElement)?.ActualWidth ?? 0; //Popup 宽高为其 Child 的宽高；
            double popupHeight = (popup.Child as FrameworkElement)?.ActualHeight ?? 0;
            double x = SystemParameters.WorkArea.X;
            double y = SystemParameters.WorkArea.Y;
            Thickness margin = toast.ToastMargin;

            /*[dlgcy] 38 和 16 两个数字的猜测：
             * PlacementTarget 为 Window 时，当 Placement 为 Bottom 时，Popup 上边缘与 Window 的下边缘的距离为 38；
             * 当 Placement 为 Right 时，Popup 左边缘与 Window 的右边缘的距离为 16。
             */
            double bottomDistance = 38;
            double rightDistance = 16;

            //上面创建时 Popup 的 Placement 为 PlacementMode.Left；
            switch (toast.Location)
            {
                case ToastLocation.OwnerLeft: //容器左中间
                    popup.HorizontalOffset = popupWidth + margin.Left;
                    popup.VerticalOffset = (owner_height - popupHeight - winTitleHeight) / 2;
                    break;
                case ToastLocation.ScreenLeft: //屏幕左中间
                    popup.HorizontalOffset = popupWidth + x + margin.Left;
                    popup.VerticalOffset = (owner_height - popupHeight) / 2 + y;
                    break;
                case ToastLocation.OwnerRight: //容器右中间
                    popup.HorizontalOffset = owner_width - rightDistance - margin.Right;
                    popup.VerticalOffset = (owner_height - popupHeight - winTitleHeight) / 2;
                    break;
                case ToastLocation.ScreenRight: //屏幕右中间
                    popup.HorizontalOffset = owner_width + x - margin.Right;
                    popup.VerticalOffset = (owner_height - popupHeight) / 2 + y;
                    break;
                case ToastLocation.OwnerTopLeft: //容器左上角
                    popup.HorizontalOffset = popupWidth + margin.Left;
                    popup.VerticalOffset = margin.Top;
                    break;
                case ToastLocation.ScreenTopLeft: //屏幕左上角
                    popup.HorizontalOffset = popupWidth + x + margin.Left;
                    popup.VerticalOffset = margin.Top;
                    break;
                case ToastLocation.OwnerTopCenter: //容器上中间
                    popup.HorizontalOffset = popupWidth + (owner_width - popupWidth - rightDistance) / 2;
                    popup.VerticalOffset = margin.Top;
                    break;
                case ToastLocation.ScreenTopCenter: //屏幕上中间
                    popup.HorizontalOffset = popupWidth + (owner_width - popupWidth) / 2 + x;
                    popup.VerticalOffset = y + margin.Top;
                    break;
                case ToastLocation.OwnerTopRight: //容器右上角
                    popup.HorizontalOffset = owner_width - rightDistance - margin.Right;
                    popup.VerticalOffset = margin.Top;
                    break;
                case ToastLocation.ScreenTopRight: //屏幕右上角
                    popup.HorizontalOffset = owner_width + x - margin.Right;
                    popup.VerticalOffset = y + margin.Top;
                    break;
                case ToastLocation.OwnerBottomLeft: //容器左下角
                    //popup.HorizontalOffset = popupWidth;
                    //popup.VerticalOffset = owner_height - popupHeight - winTitleHeight;
                    popup.Placement = PlacementMode.Bottom;
                    popup.HorizontalOffset = margin.Left;
                    popup.VerticalOffset = -(bottomDistance + popupHeight + margin.Bottom);
                    break;
                case ToastLocation.ScreenBottomLeft: //屏幕左下角
                    popup.HorizontalOffset = popupWidth + x + margin.Left;
                    popup.VerticalOffset = owner_height - popupHeight + y - margin.Bottom;
                    break;
                case ToastLocation.OwnerBottomCenter: //容器下中间
                    //popup.HorizontalOffset = popupWidth + (owner_width - popupWidth - rightDistance) / 2;
                    //popup.VerticalOffset = owner_height - popupHeight - winTitleHeight;
                    popup.Placement = PlacementMode.Bottom;
                    popup.HorizontalOffset = (owner_width - popupWidth - rightDistance) / 2;
                    popup.VerticalOffset = -(bottomDistance + popupHeight + margin.Bottom);
                    break;
                case ToastLocation.ScreenBottomCenter: //屏幕下中间
                    popup.HorizontalOffset = popupWidth + (owner_width - popupWidth) / 2 + x;
                    popup.VerticalOffset = owner_height - popupHeight + y - margin.Bottom;
                    break;
                case ToastLocation.OwnerBottomRight: //容器右下角
                    //popup.HorizontalOffset = popupWidth + (owner_width - popupWidth - rightDistance);
                    //popup.VerticalOffset = owner_height - popupHeight - winTitleHeight;
                    popup.Placement = PlacementMode.Bottom;
                    popup.HorizontalOffset = owner_width - popupWidth - rightDistance - margin.Right;
                    popup.VerticalOffset = -(bottomDistance + popupHeight + margin.Bottom);
                    break;
                case ToastLocation.ScreenBottomRight: //屏幕右下角
                    popup.HorizontalOffset = owner_width + x - margin.Right;
                    popup.VerticalOffset = owner_height - popupHeight + y - margin.Bottom;
                    break;
                case ToastLocation.ScreenCenter: //屏幕正中间
                    popup.HorizontalOffset = popupWidth + (owner_width - popupWidth) / 2 + x;
                    popup.VerticalOffset = (owner_height - popupHeight) / 2 + y;
                    break;
                case ToastLocation.OwnerCenter: //容器正中间
                case ToastLocation.Default:
                    //popup.HorizontalOffset = popupWidth + (owner_width - popupWidth - rightDistance) / 2;
                    //popup.VerticalOffset = (owner_height - popupHeight - winTitleHeight) / 2;
                    popup.Placement = PlacementMode.Center;
                    popup.HorizontalOffset = -rightDistance / 2;
                    popup.VerticalOffset = -bottomDistance / 2;
                    break;
            }
        }

        public void Close()
        {
            if (timer != null)
            {
                timer.Stop();
                timer = null;
            }
            popup.IsOpen = false;
            owner.LocationChanged -= UpdatePosition;
            owner.SizeChanged -= UpdatePosition;
        }

        private event EventHandler<EventArgs> Closed;
        private void RaiseClosed(EventArgs e)
        {
            Closed?.Invoke(this, e);
        }

        private event EventHandler<EventArgs> Click;
        private void RaiseClick(EventArgs e)
        {
            Click?.Invoke(this, e);
        }

        #region 依赖属性

        private string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }
        private static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(Toast), new PropertyMetadata(string.Empty));

        private CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }
        private static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(Toast), new PropertyMetadata(new CornerRadius(5)));

        private double IconSize
        {
            get { return (double)GetValue(IconSizeProperty); }
            set { SetValue(IconSizeProperty, value); }
        }
        private static readonly DependencyProperty IconSizeProperty =
            DependencyProperty.Register("IconSize", typeof(double), typeof(Toast), new PropertyMetadata(26.0));

        private new Brush BorderBrush
        {
            get { return (Brush)GetValue(BorderBrushProperty); }
            set { SetValue(BorderBrushProperty, value); }
        }
        private static new readonly DependencyProperty BorderBrushProperty =
            DependencyProperty.Register("BorderBrush", typeof(Brush), typeof(Toast), new PropertyMetadata((Brush)new BrushConverter().ConvertFromString("#FFFFFF")));

        private new Thickness BorderThickness
        {
            get { return (Thickness)GetValue(BorderThicknessProperty); }
            set { SetValue(BorderThicknessProperty, value); }
        }
        private static new readonly DependencyProperty BorderThicknessProperty =
            DependencyProperty.Register("BorderThickness", typeof(Thickness), typeof(Toast), new PropertyMetadata(new Thickness(0)));

        private new Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }
        private static new readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register("Background", typeof(Brush), typeof(Toast), new PropertyMetadata((Brush)new BrushConverter().ConvertFromString("#2E2929")));

        private new HorizontalAlignment HorizontalContentAlignment
        {
            get { return (HorizontalAlignment)GetValue(HorizontalContentAlignmentProperty); }
            set { SetValue(HorizontalContentAlignmentProperty, value); }
        }
        private static new readonly DependencyProperty HorizontalContentAlignmentProperty =
            DependencyProperty.Register("HorizontalContentAlignment", typeof(HorizontalAlignment), typeof(Toast), new PropertyMetadata(HorizontalAlignment.Left));

        private new VerticalAlignment VerticalContentAlignment
        {
            get { return (VerticalAlignment)GetValue(VerticalContentAlignmentProperty); }
            set { SetValue(VerticalContentAlignmentProperty, value); }
        }
        private static new readonly DependencyProperty VerticalContentAlignmentProperty =
            DependencyProperty.Register("VerticalContentAlignment", typeof(VerticalAlignment), typeof(Toast), new PropertyMetadata(VerticalAlignment.Center));

        private double ToastWidth
        {
            get { return (double)GetValue(ToastWidthProperty); }
            set { SetValue(ToastWidthProperty, value); }
        }
        private static readonly DependencyProperty ToastWidthProperty =
            DependencyProperty.Register("ToastWidth", typeof(double), typeof(Toast), new PropertyMetadata(0.0));

        private double ToastHeight
        {
            get { return (double)GetValue(ToastHeightProperty); }
            set { SetValue(ToastHeightProperty, value); }
        }
        private static readonly DependencyProperty ToastHeightProperty =
            DependencyProperty.Register("ToastHeight", typeof(double), typeof(Toast), new PropertyMetadata(0.0));

        private ToastIcons Icon
        {
            get { return (ToastIcons)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }
        private static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(ToastIcons), typeof(Toast), new PropertyMetadata(ToastIcons.None));

        private int Time
        {
            get { return (int)GetValue(TimeProperty); }
            set { SetValue(TimeProperty, value); }
        }
        private static readonly DependencyProperty TimeProperty =
            DependencyProperty.Register("Time", typeof(int), typeof(Toast), new PropertyMetadata(2000));

        private ToastLocation Location
        {
            get { return (ToastLocation)GetValue(LocationProperty); }
            set { SetValue(LocationProperty, value); }
        }
        private static readonly DependencyProperty LocationProperty =
            DependencyProperty.Register("Location", typeof(ToastLocation), typeof(Toast), new PropertyMetadata(ToastLocation.Default));

        public double TextWidth
        {
            get { return (double)GetValue(TextWidthProperty); }
            set { SetValue(TextWidthProperty, value); }
        }
        public static readonly DependencyProperty TextWidthProperty =
            DependencyProperty.Register("TextWidth", typeof(double), typeof(Toast), new PropertyMetadata(double.NaN));

        public Thickness ToastMargin
        {
            get { return (Thickness)GetValue(ToastMarginProperty); }
            set { SetValue(ToastMarginProperty, value); }
        }
        public static readonly DependencyProperty ToastMarginProperty =
            DependencyProperty.Register("ToastMargin", typeof(Thickness), typeof(Toast), new PropertyMetadata(new Thickness(0)));

        private Brush IconForeground
        {
            get { return (Brush)GetValue(IconForegroundProperty); }
            set { SetValue(IconForegroundProperty, value); }
        }
        private static readonly DependencyProperty IconForegroundProperty =
            DependencyProperty.Register("IconForeground", typeof(Brush), typeof(Toast), new PropertyMetadata((Brush)new BrushConverter().ConvertFromString("#00D91A")));

        #endregion
    }
}
