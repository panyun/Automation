using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using System.Windows.Data;

namespace EL.BaseUI.Themes
{
    /// <summary>
    /// 附加属性
    /// </summary>
    public class AttachedPropertyHelper
    {

        #region ImageSource,用于图片按钮
        public static ImageSource GetImageSource(DependencyObject obj)
        {
            return (ImageSource)obj.GetValue(ImageSourceProperty);
        }

        public static void SetImageSource(DependencyObject obj, ImageSource value)
        {
            obj.SetValue(ImageSourceProperty, value);
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.RegisterAttached("ImageSource", typeof(ImageSource), typeof(AttachedPropertyHelper), new PropertyMetadata(null));
        #endregion

        #region ImageSource2,用于图片按钮
        public static ImageSource GetImageSource2(DependencyObject obj)
        {
            return (ImageSource)obj.GetValue(ImageSource2Property);
        }

        public static void SetImageSource2(DependencyObject obj, ImageSource value)
        {
            obj.SetValue(ImageSource2Property, value);
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageSource2Property =
            DependencyProperty.RegisterAttached("ImageSource2", typeof(ImageSource), typeof(AttachedPropertyHelper), new PropertyMetadata(null));
        #endregion

        #region CancelButtonMargin，用于弹出框确定按钮和取消按钮中间加按钮


        public static Thickness GetCancelButtonMargin(DependencyObject obj)
        {
            return (Thickness)obj.GetValue(CancelButtonMarginProperty);
        }

        public static void SetCancelButtonMargin(DependencyObject obj, Thickness value)
        {
            obj.SetValue(CancelButtonMarginProperty, value);
        }

        // Using a DependencyProperty as the backing store for CancelButtonMargin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CancelButtonMarginProperty =
            DependencyProperty.RegisterAttached("CancelButtonMargin", typeof(Thickness), typeof(AttachedPropertyHelper), new PropertyMetadata(new Thickness(0)));


        #endregion

        #region MultiSelected,用于多选ComboBox


        public static bool GetIsMultiSelected(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsMultiSelectedProperty);
        }

        public static void SetIsMultiSelected(DependencyObject obj, bool value)
        {
            obj.SetValue(IsMultiSelectedProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsMultiSelected.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsMultiSelectedProperty =
            DependencyProperty.RegisterAttached("IsMultiSelected", typeof(bool), typeof(AttachedPropertyHelper), new PropertyMetadata(false, (o, e) =>
            {
                if ((bool)e.NewValue)
                {
                    ComboBox control = o as ComboBox;
                    control.Loaded += (oo, ee) =>
                    {
                        if (control.ItemsSource == null) return;
                        foreach (var item in control.ItemsSource)
                        {
                            var model = item as ObservableObject;
                            model.PropertyChanged += (ooo, eee) =>
                            {
                                if (eee.PropertyName == "IsChecked")
                                {
                                    SetMultiSelectedTextBox(control);
                                }
                            };
                        }
                        SetMultiSelectedTextBox(control);
                    };
                }
            }));

        private static void SetMultiSelectedTextBox(ComboBox control)
        {
            StringBuilder builder = new StringBuilder();

            foreach (var item2 in control.ItemsSource)
            {
                var model2 = item2 as ObservableObject;
                if ((bool)CommonHelper.GetPropertyValue(model2, "IsChecked"))
                {
                    builder.Append(CommonHelper.GetPropertyValue(model2, control.DisplayMemberPath) + "，");
                }
            }
            TextBox textBox = control.Template.FindName("PART_MultiSelectedTextBox", control) as TextBox;
            textBox.Text = builder.ToString().Trim('，');
        }

        #endregion

        #region IsAllowClear，用于ComboBox清除选项


        public static bool GetIsAllowClear(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsAllowClearProperty);
        }

        public static void SetIsAllowClear(DependencyObject obj, bool value)
        {
            obj.SetValue(IsAllowClearProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsAllowClear.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsAllowClearProperty =
            DependencyProperty.RegisterAttached("IsAllowClear", typeof(bool), typeof(AttachedPropertyHelper), new PropertyMetadata(false, (o, e) =>
            {
                if ((bool)e.NewValue)
                {
                    ComboBox control = o as ComboBox;
                    control.Loaded += (oo, ee) =>
                    {
                        Image image = control.Template?.FindName("PART_ClearButton", control) as Image;
                        if (image != null) image.MouseLeftButtonDown += (ooo, eee) =>
                        {
                            control.SelectedIndex = -1;
                        };
                    };

                }
            }));



        #endregion

        #region IsAllowSearch，用于ComboBox搜索选项


        public static bool GetIsAllowSearch(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsAllowSearchProperty);
        }

        public static void SetIsAllowSearch(DependencyObject obj, bool value)
        {
            obj.SetValue(IsAllowSearchProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsAllowClear.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsAllowSearchProperty =
            DependencyProperty.RegisterAttached("IsAllowSearch", typeof(bool), typeof(AttachedPropertyHelper), new PropertyMetadata(false, (o, e) =>
            {
                if ((bool)e.NewValue)
                {
                    ComboBox control = o as ComboBox;
                    control.AddHandler(System.Windows.Controls.Primitives.TextBoxBase.TextChangedEvent, new System.Windows.Controls.TextChangedEventHandler((o, e) =>
                    {
                        if (!string.IsNullOrEmpty(control.Text))
                        {
                            ICollectionView defaultView = CollectionViewSource.GetDefaultView(control.ItemsSource);
                            if (string.IsNullOrEmpty(control.DisplayMemberPath))
                            {
                                defaultView.Filter = (x => x.ToString().Contains(control.Text));
                            }
                            else
                            {
                                defaultView.Filter = (x => ReflectHelper.GetPropertyValue(x, control.DisplayMemberPath).ToString().Contains(control.Text));
                            }
                        }
                        else
                        {
                            ICollectionView defaultView = CollectionViewSource.GetDefaultView(control.ItemsSource);
                            defaultView.Filter = null;
                        }
                    }));
                    //control.KeyUp += (o, e) =>
                    //{
                    //    if (!string.IsNullOrEmpty(control.Text))
                    //    {
                    //        ICollectionView defaultView = CollectionViewSource.GetDefaultView(this.ItemsSource);
                    //        if (string.IsNullOrEmpty(this.DisplayMemberPath))
                    //        {
                    //            defaultView.Filter = (x => x.ToString().Contains(SearchText));
                    //        }
                    //        else
                    //        {
                    //            defaultView.Filter = (x => ReflectHelper.GetPropertyValue(x, this.DisplayMemberPath).ToString().Contains(SearchText));
                    //        }
                    //    }
                    //    else
                    //    {
                    //        ICollectionView defaultView = CollectionViewSource.GetDefaultView(this.ItemsSource);
                    //        defaultView.Filter = null;
                    //    }
                    //};

                }
            }));



        #endregion

        #region CornerRadius,用于控件设置圆角


        public static CornerRadius GetCornerRadius(DependencyObject obj)
        {
            return (CornerRadius)obj.GetValue(CornerRadiusProperty);
        }

        public static void SetCornerRadius(DependencyObject obj, CornerRadius value)
        {
            obj.SetValue(CornerRadiusProperty, value);
        }

        // Using a DependencyProperty as the backing store for CornerRadius.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.RegisterAttached("CornerRadius", typeof(CornerRadius), typeof(AttachedPropertyHelper), new PropertyMetadata(new CornerRadius(0)));


        #endregion

        #region IsShowToggleButton,用于Expander


        public static bool GetIsShowToggleButton(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsShowToggleButtonProperty);
        }

        public static void SetIsShowToggleButton(DependencyObject obj, bool value)
        {
            obj.SetValue(IsShowToggleButtonProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsShowToggleButton.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsShowToggleButtonProperty =
            DependencyProperty.RegisterAttached("IsShowToggleButton", typeof(bool), typeof(AttachedPropertyHelper), new PropertyMetadata(true));


        #endregion

        #region PasswordBoxMonitor 用于PasswordBox
        public static readonly DependencyProperty IsMonitoringProperty =
          DependencyProperty.RegisterAttached("IsMonitoring", typeof(bool), typeof(AttachedPropertyHelper), new UIPropertyMetadata(false, OnIsMonitoringChanged));

        public static readonly DependencyProperty PasswordContentProperty =
          DependencyProperty.RegisterAttached("PasswordContent", typeof(string), typeof(AttachedPropertyHelper), new UIPropertyMetadata(null, (d, e) =>
          {
              var pb = d as PasswordBox;

              if (pb.Password != e.NewValue?.ToString())
              {
                  pb.Password = e.NewValue?.ToString();
              }
          }));

        #region PrivateMethods
        private static void OnIsMonitoringChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pb = d as PasswordBox;
            if (pb == null)
            {
                return;
            }
            if ((bool)e.NewValue)
            {
                pb.PasswordChanged += PasswordChanged;
            }
            else
            {
                pb.PasswordChanged -= PasswordChanged;
            }
        }

        #endregion


        #region Public/ProtectedMethods
        public static bool GetIsMonitoring(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsMonitoringProperty);
        }

        public static void SetIsMonitoring(DependencyObject obj, bool value)
        {
            obj.SetValue(IsMonitoringProperty, value);
        }

        public static string GetPasswordContent(DependencyObject obj)
        {
            return (string)obj.GetValue(PasswordContentProperty);
        }

        public static void SetPasswordContent(DependencyObject obj, string value)
        {
            obj.SetValue(PasswordContentProperty, value);
        }

        static void PasswordChanged(object sender, RoutedEventArgs e)
        {
            var pb = sender as PasswordBox;
            if (pb == null)
            {
                return;
            }
            SetPasswordContent(pb, pb.Password);
        }


        #endregion
        #endregion

   

        #region ItemWidth,用于设置itemcontrol
        public static double GetItemWidth(DependencyObject obj)
        {
            return (double)obj.GetValue(ItemWidthProperty);
        }

        public static void SetItemWidth(DependencyObject obj, double value)
        {
            obj.SetValue(ItemWidthProperty, value);
        }

        // Using a DependencyProperty as the backing store for ItemWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemWidthProperty =
            DependencyProperty.RegisterAttached("ItemWidth", typeof(double), typeof(AttachedPropertyHelper), new PropertyMetadata(0.00));


        #endregion

        #region ItemHeight,用于设置itemcontrol

        public static double GetItemHeight(DependencyObject obj)
        {
            return (double)obj.GetValue(ItemHeightProperty);
        }

        public static void SetItemHeight(DependencyObject obj, double value)
        {
            obj.SetValue(ItemHeightProperty, value);
        }

        // Using a DependencyProperty as the backing store for ItemWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemHeightProperty =
            DependencyProperty.RegisterAttached("ItemHeight", typeof(double), typeof(AttachedPropertyHelper), new PropertyMetadata(0.00));


        #endregion

        #region 文本相关
        public static string GetText(DependencyObject obj)
        {
            return (string)obj.GetValue(TextProperty);
        }

        public static void SetText(DependencyObject obj, string value)
        {
            obj.SetValue(TextProperty, value);
        }
        public static readonly DependencyProperty TextProperty =
         DependencyProperty.RegisterAttached("Text", typeof(string), typeof(AttachedPropertyHelper), new UIPropertyMetadata(null));

        #endregion

        #region 二级文本
        public static string GetSecondText(DependencyObject obj)
        {
            return (string)obj.GetValue(SecondTextProperty);
        }

        public static void SetSecondText(DependencyObject obj, string value)
        {
            obj.SetValue(SecondTextProperty, value);
        }
        public static readonly DependencyProperty SecondTextProperty =
         DependencyProperty.RegisterAttached("SecondText", typeof(string), typeof(AttachedPropertyHelper), new UIPropertyMetadata(null));
        #endregion

        #region Orientation, 用于设置StackPanel
        public static Orientation GetOrientation(DependencyObject obj)
        {
            return (Orientation)obj.GetValue(OrientationProperty);
        }

        public static void SetOrientation(DependencyObject obj, Orientation value)
        {
            obj.SetValue(OrientationProperty, value);
        }

        public static readonly DependencyProperty OrientationProperty =
         DependencyProperty.RegisterAttached("Orientation", typeof(Orientation), typeof(AttachedPropertyHelper), new PropertyMetadata(Orientation.Horizontal));
        #endregion

        #region 是否显示图标 
        public static Visibility GetIsShowIcon(DependencyObject obj)
        {
            return (Visibility)obj.GetValue(IsShowIconProperty);
        }

        public static void SetIsShowIcon(DependencyObject obj, Visibility value)
        {
            obj.SetValue(IsShowIconProperty, value);
        }

        public static readonly DependencyProperty IsShowIconProperty =
         DependencyProperty.RegisterAttached("IsShowIcon", typeof(Visibility), typeof(AttachedPropertyHelper), new PropertyMetadata(Visibility.Visible));
        #endregion

        #region 设置背景
        public static Brush GetBackground(DependencyObject obj)
        {
            return (Brush)obj.GetValue(BackgroundProperty);
        }

        public static void SetBackground(DependencyObject obj, Brush value)
        {
            obj.SetValue(BackgroundProperty, value);
        }

        public static readonly DependencyProperty BackgroundProperty =
         DependencyProperty.RegisterAttached("Background", typeof(Brush), typeof(AttachedPropertyHelper), new PropertyMetadata(null));
        #endregion

        #region 设置MouseOver颜色
        public static Brush GetMouseOverBrush(DependencyObject obj)
        {
            return (Brush)obj.GetValue(MouseOverBrushProperty);
        }

        public static void SetMouseOverBrush(DependencyObject obj, Brush value)
        {
            obj.SetValue(MouseOverBrushProperty, value);
        }

        public static readonly DependencyProperty MouseOverBrushProperty =
         DependencyProperty.RegisterAttached("MouseOverBrush", typeof(Brush), typeof(AttachedPropertyHelper), new PropertyMetadata(null));
        #endregion

        #region 设置Pressed颜色
        public static Brush GetPressedBrush(DependencyObject obj)
        {
            return (Brush)obj.GetValue(PressedBrushProperty);
        }

        public static void SetPressedBrush(DependencyObject obj, Brush value)
        {
            obj.SetValue(PressedBrushProperty, value);
        }

        public static readonly DependencyProperty PressedBrushProperty =
         DependencyProperty.RegisterAttached("PressedBrush", typeof(Brush), typeof(AttachedPropertyHelper), new PropertyMetadata(null));
        #endregion

        #region 设置矩形，可用于裁剪
        public static Rect GetRect(DependencyObject obj)
        {
            return (Rect)obj.GetValue(RectProperty);
        }

        public static void SetRect(DependencyObject obj, Rect value)
        {
            obj.SetValue(RectProperty, value);
        }

        // Using a DependencyProperty as the backing store for Rect.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RectProperty =
            DependencyProperty.RegisterAttached("Rect", typeof(Rect), typeof(AttachedPropertyHelper), new PropertyMetadata(new Rect()));
        #endregion

        #region 设置控件是否可拖动


        public static bool GetIsMouseDown(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsMouseDownProperty);
        }

        public static void SetIsMouseDown(DependencyObject obj, bool value)
        {
            obj.SetValue(IsMouseDownProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsMouseDown.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsMouseDownProperty =
            DependencyProperty.RegisterAttached("IsMouseDown", typeof(bool), typeof(AttachedPropertyHelper), new PropertyMetadata(false));




        public static bool GetIsAllowMove(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsAllowMoveProperty);
        }

        public static void SetIsAllowMove(DependencyObject obj, bool value)
        {
            obj.SetValue(IsAllowMoveProperty, value);
        }

        // Using a DependencyProperty as the backing store for AllowMove.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsAllowMoveProperty =
            DependencyProperty.RegisterAttached("IsAllowMove", typeof(bool), typeof(AttachedPropertyHelper), new PropertyMetadata(false, (o, e) =>
            {
                if ((bool)e.NewValue)
                {
                    UIElement control = o as UIElement;
                    control.MouseDown += Control_MouseDown;
                    control.MouseMove += Control_MouseMove;
                    control.MouseUp += Control_MouseUp;
                    control.TouchDown += Control_TouchDown;
                    control.TouchMove += Control_TouchMove;
                    control.TouchUp += Control_TouchUp;
                }
            }));

        private static void Control_TouchUp(object? sender, System.Windows.Input.TouchEventArgs e)
        {
            Control_MouseUp(sender, null);
        }

        private static void Control_TouchMove(object? sender, System.Windows.Input.TouchEventArgs e)
        {
            e.Handled = true;
            Control_MouseMove(sender, null);
        }

        private static void Control_TouchDown(object? sender, System.Windows.Input.TouchEventArgs e)
        {
            e.Handled = true;
            Control_MouseDown(sender, null);
        }

        private static void Control_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var c = sender as FrameworkElement;
            AttachedPropertyHelper.SetIsMouseDown(c, false);
        }

        private static void Control_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var c = sender as FrameworkElement;
            if (AttachedPropertyHelper.GetIsMouseDown(c))
            {

                DataObject data = new DataObject();
                if (c.DataContext != null)
                {
                    data.SetData(c.DataContext.GetType(), c.DataContext);
                }
                data.SetData(c.GetType(), c);
                DragDrop.DoDragDrop(c, data, DragDropEffects.Copy);
                AttachedPropertyHelper.SetIsMouseDown(c, false);
            }
        }

        private static void Control_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var c = sender as FrameworkElement;
            AttachedPropertyHelper.SetIsMouseDown(c, true);
        }


        #endregion

        #region DataGrid，ListBox是否允许父级ScrollViewer跟着滚动


        public static bool GetIsAllowParentScrollViewerMouseWheel(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsAllowParentScrollViewerMouseWheelProperty);
        }

        public static void SetIsAllowParentScrollViewerMouseWheel(DependencyObject obj, bool value)
        {
            obj.SetValue(IsAllowParentScrollViewerMouseWheelProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsAllowParentScrollViewerMouseWheel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsAllowParentScrollViewerMouseWheelProperty =
            DependencyProperty.RegisterAttached("IsAllowParentScrollViewerMouseWheel", typeof(bool), typeof(AttachedPropertyHelper), new PropertyMetadata(false, (o, e) =>
            {
                if ((bool)e.NewValue)
                {
                    UIElement control = o as UIElement;
                    control.PreviewMouseWheel += (o, e) =>
                    {
                        var fElement = VisualHelper.FindVisualParent<ScrollViewer>(control);
                        if (fElement != null)
                        {
                            var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
                            eventArg.RoutedEvent = UIElement.MouseWheelEvent;
                            eventArg.Source = o;
                            fElement.RaiseEvent(eventArg);
                        }

                    };
                }
            }));


        #endregion

        #region IsShowChildWindowButton,是否显示子窗体取消，确定按钮


        public static bool GetIsShowChildWindowButton(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsShowChildWindowButtonProperty);
        }

        public static void SetIsShowChildWindowButton(DependencyObject obj, bool value)
        {
            obj.SetValue(IsShowChildWindowButtonProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsShowChildWindowButton.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsShowChildWindowButtonProperty =
            DependencyProperty.RegisterAttached("IsShowChildWindowButton", typeof(bool), typeof(AttachedPropertyHelper), new PropertyMetadata(true));

        #endregion

        #region Tag2

        public static object GetTag2(DependencyObject obj)
        {
            return (object)obj.GetValue(Tag2Property);
        }

        public static void SetTag2(DependencyObject obj, object value)
        {
            obj.SetValue(Tag2Property, value);
        }

        // Using a DependencyProperty as the backing store for Tag2.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Tag2Property =
            DependencyProperty.RegisterAttached("Tag2", typeof(object), typeof(AttachedPropertyHelper), new PropertyMetadata(null));

        #endregion

        #region 绑定bool属性触发条件

        public static object GetBoolValue(DependencyObject obj)
        {
            return (bool)obj.GetValue(Tag2Property);
        }

        public static void SetBoolValue(DependencyObject obj, bool value)
        {
            obj.SetValue(BoolValueProperty, value);
        }

        // Using a DependencyProperty as the backing store for Tag2.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BoolValueProperty =
            DependencyProperty.RegisterAttached("BoolValue", typeof(bool), typeof(AttachedPropertyHelper), new PropertyMetadata(null));

        #endregion

        #region TextBoxt 屏蔽一些按键事件

        public static readonly DependencyProperty IsShieldKeyProperty =
            DependencyProperty.RegisterAttached("IsShieldKey", typeof(bool), typeof(AttachedPropertyHelper), new PropertyMetadata(false, OnIsShieldKeyPropertyChanged));

        public static object GetIsShieldKey(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsShieldKeyProperty);
        }

        public static void SetIsShieldKey(DependencyObject obj, bool value)
        {
            obj.SetValue(IsShieldKeyProperty, value);
        }

        private static void OnIsShieldKeyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                if (d is TextBox textBox)
                {
                    textBox.KeyDown += TextBox_KeyDown;
                }
            }
        }

        private static void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            // 如果同时按下 Ctrl 和 Tab 键，则禁止默认的 Tab 键行为
            if (e.Key == Key.Tab && Keyboard.Modifiers == ModifierKeys.Control)
            {
                e.Handled = true;
            }
        }

        #endregion
    }
    public class VisualHelper
    {
        public static T FindParent<T>(DependencyObject obj, string valueName) where T : DependencyObject
        {

            while (obj != null)
            {
                string name = obj.GetType().GetProperty("Name").GetValue(obj, null)?.ToString();
                if (obj is T && name == valueName)
                    return obj as T;
                if (obj is FrameworkContentElement && (obj as FrameworkContentElement).Parent != null)
                {
                    obj = (obj as FrameworkContentElement).Parent;
                }
                else if (obj is FrameworkElement && (obj as FrameworkElement).Parent != null)
                {
                    var aaa = (obj as FrameworkElement).Parent;
                    obj = aaa;
                }
                else
                {
                    obj = VisualTreeHelper.GetParent(obj);
                }

            }
            return null;



        }
        //根据子元素查找父元素  
        public static T FindVisualParent<T>(DependencyObject obj) where T : class
        {
            while (obj != null)
            {
                if (obj is T)
                    return obj as T;
                if (obj is FrameworkContentElement && (obj as FrameworkContentElement).Parent != null)
                {
                    obj = (obj as FrameworkContentElement).Parent;
                }
                else if (obj is FrameworkElement && (obj as FrameworkElement).Parent != null)
                {
                    obj = (obj as FrameworkElement).Parent;
                }
                else
                {
                    obj = VisualTreeHelper.GetParent(obj);
                }

            }
            return null;
        }
    }
}
