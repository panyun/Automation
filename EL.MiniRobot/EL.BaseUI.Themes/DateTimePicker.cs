using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using Calendar = System.Windows.Controls.Calendar;

namespace EL.BaseUI.Themes
{
    public enum DateTimePickerFormat { Long, Short, Time, Custom }

    [DefaultBindingProperty("Value")]
    public class DateTimePicker : Control
    {
        #region Variables
        private Border _border;
        private CheckBox _checkBox;
        internal TextBox TextBox;
        private Image _Img;
        private Popup _popUp;
        private Calendar _calendar;
        private BlockManager _blockManager;
        private string _defaultFormat = "yyyy-MM-dd hh:mm:ss tt";
        #endregion


        #region Constructor
        public DateTimePicker()
        {
            Debug.WriteLine("DateTimePicker");
            Initializ();
            _blockManager = new BlockManager(this, FormatString);
            Language = System.Windows.Markup.XmlLanguage.GetLanguage("zh-CN");
            Format = DateTimePickerFormat.Custom;
            CustomFormat = "yyyy-MM-dd";
        }
        #endregion


        #region Properties

        [Category("DateTimePicker")]
        public Calendar Calendar
        {
            get { return _calendar; }
            set { _calendar = value; }
        }
        [Category("DateTimePicker")]
        public bool ShowCheckBox
        {
            get { return _checkBox.Visibility == Visibility.Visible; }
            set
            {
                if (value)
                    _checkBox.Visibility = Visibility.Visible;
                else
                {
                    _checkBox.Visibility = Visibility.Collapsed;
                    Checked = true;
                }
            }
        }
        [Category("DateTimePicker")]
        public bool ShowDropDown
        {
            get { return _Img.Visibility == Visibility.Visible; }
            set
            {
                if (value)
                    _Img.Visibility = Visibility.Visible;
                else
                    _Img.Visibility = Visibility.Collapsed;
            }
        }
        [Category("DateTimePicker")]
        public bool Checked
        {
            get { return _checkBox.IsChecked.HasValue && _checkBox.IsChecked.Value; }
            set { _checkBox.IsChecked = value; }
        }
        [Category("DateTimePicker")]
        private string FormatString
        {
            get
            {
                switch (Format)
                {
                    case DateTimePickerFormat.Long:
                        return "dddd, MMMM dd, yyyy";
                    case DateTimePickerFormat.Short:
                        return "M/d/yyyy";
                    case DateTimePickerFormat.Time:
                        return "h:mm:ss tt";
                    case DateTimePickerFormat.Custom:
                        if (string.IsNullOrEmpty(CustomFormat))
                            return _defaultFormat;
                        else
                            return CustomFormat;
                    default:
                        return _defaultFormat;
                }
            }
        }
        private string _customFormat;
        [Category("DateTimePicker")]
        public string CustomFormat
        {
            get { return _customFormat; }
            set
            {
                _customFormat = value;
                _blockManager = new BlockManager(this, FormatString);
            }
        }
        private DateTimePickerFormat _format;
        [Category("DateTimePicker")]
        public DateTimePickerFormat Format
        {
            get { return _format; }
            set
            {
                _format = value;
                _blockManager = new BlockManager(this, FormatString);
            }
        }
        [Category("DateTimePicker")]

        public bool ReadOnly
        {
            get
            {
                return _checkBox.Visibility == Visibility.Visible;
            }
            set
            {
                if (value)
                {
                    _checkBox.Visibility = Visibility.Collapsed;
                    _popUp.Visibility = Visibility.Collapsed;
                    _Img.IsEnabled = false;
                    TextBox.IsEnabled = false;

                }
                else
                {
                    _popUp.Visibility = Visibility.Visible;
                    _checkBox.Visibility = Visibility.Collapsed;
                    _Img.IsEnabled = true;
                    TextBox.IsEnabled = true;
                }
            }
        }
        [Category("DateTimePicker")]
        public DateTime? Value
        {
            get
            {
                if (!Checked) return null;
                return (DateTime?)GetValue(ValueProperty);
            }
            set { SetValue(ValueProperty, value); }
        }

        public string Text
        {
            get
            {
                return TextBox.Text;
            }
            set
            {
                TextBox.Text = value;
            }
        }
        internal DateTime InternalValue
        {
            set { Value = value; }
            get
            {
                DateTime? value = Value;

                return value ?? DateTime.MinValue;
            }
        }






        #endregion


        #region 依赖属性

        // Using a DependencyProperty as the backing store for TheDate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(DateTime?), typeof(DateTimePicker),
            new FrameworkPropertyMetadata(DateTime.Now, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnValueChanged,
                CoerceValue, true, UpdateSourceTrigger.PropertyChanged));

        #endregion


        #region Event
        /// <summary>
        /// 日期选中事件
        /// </summary>
        public event EventHandler<NotificationEventArgs> SelectedDateChanged;

        #endregion


        #region PrivateMethods
        private void Initializ()
        {
            Debug.WriteLine("Initializ");
            Template = FindResource("datePickerTemplate") as ControlTemplate;
            ApplyTemplate();
            _border = (Border)Template.FindName("border", this);

            _checkBox = (CheckBox)Template.FindName("checkBox", this);
            TextBox = (TextBox)Template.FindName("textBox", this);
            _Img = (Image)Template.FindName("textBlock", this);
            _calendar = new Calendar();


            _calendar.Style = FindResource("CalendarStyle") as Style;
            _calendar.CalendarItemStyle = FindResource("CalendarItemStyle") as Style;
            _calendar.CalendarDayButtonStyle = FindResource("CalendarDayButtonStyle") as Style;
            _calendar.CalendarButtonStyle = FindResource("CalendarButtonStyle") as Style;
            //_calendar.Width = 242;
            //_calendar.Height = 300;

            //_calendar.BorderThickness = new Thickness(0, 0, 0, 0);
            //_calendar.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#217A98"));
            //_calendar.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#217A98"));
            //_calendar.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#217A98"));

            _popUp = new Popup()
            {
                PlacementTarget = _border,
                Placement = PlacementMode.Bottom,
                Child = _calendar,
                StaysOpen = false,
                AllowsTransparency = true
            };

            _checkBox.Checked += _checkBox_Checked;
            _checkBox.Unchecked += _checkBox_Checked;
            MouseWheel += Dameer_MouseWheel;
            TextBox.Cursor = Cursors.Arrow;
            TextBox.AllowDrop = false;
            TextBox.PreviewMouseUp += _textBox_PreviewMouseUp;
            TextBox.PreviewKeyDown += _textBox_PreviewKeyDown;
            TextBox.ContextMenu = null;
            TextBox.IsEnabled = Checked;
            TextBox.IsReadOnly = true;
            TextBox.IsReadOnlyCaretVisible = false;
            _Img.MouseLeftButtonUp += _textBlock_MouseLeftButtonDown;
            _calendar.SelectedDatesChanged += calendar_SelectedDatesChanged;
        }
        ///样式相关统一放在Intelligent.Themes里面，此方法暂时不用 by ozm
        private ControlTemplate GetTemplate()
        {
            Debug.WriteLine("GetTemplate");
            return (ControlTemplate)XamlReader.Parse(@"
        <ControlTemplate  xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
                          xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
            <Border BorderBrush=""#C7C7C7"" BorderThickness=""1"" CornerRadius=""1"" Background=""#FFFFFF"" Name=""border"">
                <StackPanel Orientation=""Horizontal"" VerticalAlignment=""Center""   HorizontalAlignment=""Center"" Background=""#FFFFFF"">
                    <CheckBox Name=""checkBox"" VerticalAlignment=""Center"" Width=""0"" />
                    <TextBox Name=""textBox"" BorderThickness=""0"" FontSize=""18"" Padding=""10 14"" Height=""48"" Width=""205"" Foreground=""#989EA6"" Background=""#FFFFFF"" VerticalAlignment=""Center"" HorizontalAlignment=""Center""/>
                    <Image Name=""textBlock"" Source=""../images/日历icon.png"" Height=""24"" Width=""29""/>
                </StackPanel>
            </Border>
        </ControlTemplate>");
        }

        #endregion


        #region Public/ProtectedMethods
        public override string ToString()
        {
            return InternalValue.ToString(CultureInfo.InvariantCulture);
        }
        #endregion


        #region EventHandlers

        static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                var eTime = (DateTime)e.NewValue;
                var dTime = d as DateTimePicker;
                if (dTime != null)
                {
                    dTime._calendar.SelectedDate = eTime.Date;
                    dTime.InternalValue = eTime;
                    dTime._blockManager.Render();
                }
            }
        }
        static object CoerceValue(DependencyObject d, object value)
        {
            return value;
        }
        void _textBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("_button_Click");
            _popUp.IsOpen = true;
        }

        void _checkBox_Checked(object sender, RoutedEventArgs e)
        {
            TextBox.IsEnabled = Checked;
        }

        void Dameer_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Debug.WriteLine("Dameer_MouseWheel");
            _blockManager.Change(((e.Delta < 0) ? -1 : 1), true);
        }

        void _textBox_GotFocus(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("_textBox_GotFocus");
            _blockManager.ReSelect();
        }

        void _textBox_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("_textBox_PreviewMouseUp");
            _blockManager.ReSelect();
        }

        void _textBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var uie = e.OriginalSource as UIElement;
            Debug.WriteLine("_textBox_PreviewKeyDown");
            byte b = (byte)e.Key;

            if (e.Key == Key.Left)
                _blockManager.Left();
            else if (e.Key == Key.Right)
                _blockManager.Right();
            else if (e.Key == Key.Up)
                _blockManager.Change(1, true);
            else if (e.Key == Key.Down)
                _blockManager.Change(-1, true);
            else if (e.Key == Key.Enter)
            {
                e.Handled = true;
                uie.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                if (_popUp.IsOpen)
                {
                    _popUp.IsOpen = false;
                }
            }
            if (b >= 34 && b <= 43)
                _blockManager.ChangeValue(b - 34);
            if (b >= 74 && b <= 83)
                _blockManager.ChangeValue(b - 74);
            if (e.Key != Key.Tab)
                e.Handled = true;
        }

        void calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedDateChanged?.Invoke(SelectedDateChanged, new NotificationEventArgs(""));

            Checked = true;
            var calendar = sender as Calendar;
            //calendar.Width = 260;
            if (calendar != null)
            {
                var popup = calendar.Parent as Popup;
                //popup.Width = 260;
                if (popup != null) popup.IsOpen = false;
            }
            var selectedDate = (DateTime)e.AddedItems[0];
            var item = selectedDate.Year + "-" + selectedDate.Month + "-" + selectedDate.Day + " " + InternalValue.Hour +
                       ":" + InternalValue.Minute + ":" + InternalValue.Second;
            var item1 = Convert.ToDateTime(item);
            Value = new DateTime(selectedDate.Year, selectedDate.Month, selectedDate.Day, InternalValue.Hour, InternalValue.Minute, InternalValue.Second);
            _blockManager.Render();
        }
        #endregion
    }
    public class DatePickerBase : DatePicker
    {
        #region Variables


        #endregion


        #region Constructor
        public DatePickerBase()
        {
            SelectedDate = DateTime.Now;
            VerticalContentAlignment = VerticalAlignment.Center;
        }
        #endregion


        #region Properties


        #endregion


        #region 依赖属性


        #endregion


        #region Event


        #endregion


        #region PrivateMethods


        #endregion


        #region Public/ProtectedMethods


        #endregion


        #region EventHandlers

        #region 快捷键操作
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            var uie = e.OriginalSource as UIElement;
            if (e.Key == Key.Space)
            {
                Keyboard.Focus(uie);
                this.IsDropDownOpen = true;
                e.Handled = true;
            }
            else if (e.Key == Key.Return)
            {
                uie.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                e.Handled = true;
            }
            else if ((e.Key == Key.Up) && (!IsDropDownOpen))
            {
                uie.MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
                e.Handled = true;
            }
            else if ((e.Key == Key.Down) && (!IsDropDownOpen))
            {
                uie.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                e.Handled = true;
            }
            else
            {
                base.OnKeyDown(e);
            }
        }

        #endregion

        #endregion


    }
    public class BlockManager
    {
        #region Variables
        internal DateTimePicker Dameer;
        private List<Block> _blocks;
        private string _format;
        private Block _selectedBlock;
        private int _selectedIndex;
        public event EventHandler NeglectProposed;
        private readonly string[] _supportedFormats = {
                "yyyy", "MMMM", "dddd",
                "yyy", "MMM", "ddd",
                "yy", "MM", "dd",
                "y", "M", "d",
                "HH", "H", "hh", "h",
                "mm", "m",
                "ss", "s",
                "tt", "t",
                "fff", "ff", "f",
                "K", "g"};

        #endregion


        #region Constructor
        public BlockManager(DateTimePicker dameer, string format)
        {
            Debug.WriteLine("BlockManager");
            Dameer = dameer;
            _format = format;
            Dameer.LostFocus += _dameer_LostFocus;
            _blocks = new List<Block>();
            InitBlocks();
        }
        #endregion


        #region Properties


        #endregion


        #region 依赖属性


        #endregion


        #region Event


        #endregion


        #region PrivateMethods
        private void InitBlocks()
        {
            Debug.WriteLine("InitBlocks");
            foreach (string f in _supportedFormats)
                _blocks.AddRange(GetBlocks(f));
            _blocks = _blocks.OrderBy(a => a.Index).ToList();
            _selectedBlock = _blocks[0];
            Render();
        }
        private void Select(int blockIndex)
        {
            if (_blocks.Count > blockIndex)
                Select(_blocks[blockIndex]);
        }
        private void Select(Block block)
        {
            if (_selectedBlock != block)
                OnNeglectProposed();
            _selectedIndex = _blocks.IndexOf(block);
            _selectedBlock = block;
            Dameer.TextBox.Select(block.Index, block.Length);
        }

        #endregion


        #region Public/ProtectedMethods
        public void Render()
        {
            int accum = 0;
            var sb = new StringBuilder(_format);
            foreach (var b in _blocks)
                b.Render(ref accum, sb);
            Dameer.TextBox.Text = _format = sb.ToString();
            Select(_selectedBlock);
        }

        private List<Block> GetBlocks(string pattern)
        {
            Debug.WriteLine("GetBlocks");
            var bCol = new List<Block>();

            var index = -1;
            while ((index = _format.IndexOf(pattern, ++index, StringComparison.Ordinal)) > -1)
                bCol.Add(new Block(this, pattern, index));
            _format = _format.Replace(pattern, (0).ToString().PadRight(pattern.Length, '0'));
            return bCol;
        }

        public void ChangeValue(int p)
        {
            _selectedBlock.Proposed = p;
            Change(_selectedBlock.Proposed, false);
        }
        public void Change(int value, bool upDown)
        {
            Dameer.Value = _selectedBlock.Change(Dameer.InternalValue, value, upDown);
            if (upDown)
                OnNeglectProposed();
            Render();
        }
        public void Right()
        {
            Debug.WriteLine("Right");
            if (_selectedIndex + 1 < _blocks.Count)
                Select(_selectedIndex + 1);
        }

        public void Left()
        {
            Debug.WriteLine("Left");
            if (_selectedIndex > 0)
                Select(_selectedIndex - 1);
        }

        public void _dameer_LostFocus(object sender, RoutedEventArgs e)
        {
            OnNeglectProposed();
        }

        protected virtual void OnNeglectProposed()
        {
            EventHandler temp = NeglectProposed;
            if (temp != null)
            {
                temp(this, EventArgs.Empty);
            }
        }
        internal void ReSelect()
        {
            Debug.WriteLine("ReSelect");
            foreach (var b in _blocks)
                if ((b.Index <= Dameer.TextBox.SelectionStart) && ((b.Index + b.Length) >= Dameer.TextBox.SelectionStart))
                { Select(b); return; }
            Block bb = _blocks.LastOrDefault(a => a.Index < Dameer.TextBox.SelectionStart);
            if (bb == null) Select(0);
            else Select(bb);
        }

        #endregion


        #region EventHandlers


        #endregion
    }
    public class Block
    {
        #region Variables
        private readonly BlockManager _blockManager;
        internal string Pattern { get; set; }
        internal int Index { get; set; }
        private int _length;

        private int _maxLength;
        private string _proposed;
        #endregion


        #region Constructor
        public Block(BlockManager blockManager, string pattern, int index)
        {
            Debug.WriteLine("Block");
            _blockManager = blockManager;
            _blockManager.NeglectProposed += _blockManager_NeglectProposed;
            Pattern = pattern;
            Index = index;
            Length = Pattern.Length;
            _maxLength = GetMaxLength(Pattern);
        }

        #endregion


        #region Properties
        internal int Length
        {
            get
            {
                return _length;
            }
            set
            {
                _length = value;
            }
        }

        #endregion


        #region 依赖属性


        #endregion


        #region Event


        #endregion


        #region PrivateMethods
        private int GetMaxLength(string p)
        {
            switch (p)
            {
                case "y":
                case "M":
                case "d":
                case "h":
                case "m":
                case "s":
                case "H":
                    return 2;
                case "yyy":
                    return 4;
                default:
                    return p.Length;
            }
        }

        private bool CanChange()
        {
            switch (Pattern)
            {
                case "MMMM":
                case "dddd":
                case "MMM":
                case "ddd":
                case "g":
                    return false;
                default:
                    return true;
            }
        }
        #endregion


        #region Public/ProtectedMethods
        internal int Proposed
        {
            get
            {
                Debug.WriteLine("Proposed Get, {0}, {1}", _proposed, Length);
                string p = _proposed;
                return int.Parse(p.PadLeft(Length, '0'));
            }
            set
            {
                Debug.WriteLine("Proposed Set, {0}, {1}", _proposed, Length);
                if (_proposed != null && _proposed.Length >= _maxLength)
                    _proposed = value.ToString();
                else
                    _proposed = string.Format("{0}{1}", _proposed, value);
            }
        }


        internal DateTime Change(DateTime dateTime, int value, bool upDown)
        {
            Debug.WriteLine("Change(DateTime dateTime, int value, bool upDown)");
            if (!upDown && !CanChange()) return dateTime;
            int y, m, d, h, n, s;
            y = dateTime.Year;
            m = dateTime.Month;
            d = dateTime.Day;
            h = dateTime.Hour;
            n = dateTime.Minute;
            s = dateTime.Second;

            if (Pattern.Contains("y"))
                y = ((upDown) ? dateTime.Year + value : value);
            else if (Pattern.Contains("M"))
                m = ((upDown) ? dateTime.Month + value : value);
            else if (Pattern.Contains("d"))
                d = ((upDown) ? dateTime.Day + value : value);
            else if (Pattern.Contains("h") || Pattern.Contains("H"))
                h = ((upDown) ? dateTime.Hour + value : value);
            else if (Pattern.Contains("m"))
                n = ((upDown) ? dateTime.Minute + value : value);
            else if (Pattern.Contains("s"))
                s = ((upDown) ? dateTime.Second + value : value);
            else if (Pattern.Contains("t"))
                h = ((h < 12) ? (h + 12) : (h - 12));

            if (y > 9999) y = 1;
            if (y < 1) y = 9999;
            if (m > 12) m = 1;
            if (m < 1) m = 12;
            if (d > DateTime.DaysInMonth(y, m)) d = 1;
            if (d < 1) d = DateTime.DaysInMonth(y, m);
            if (h > 23) h = 0;
            if (h < 0) h = 23;
            if (n > 59) n = 0;
            if (n < 0) n = 59;
            if (s > 59) s = 0;
            if (s < 0) s = 59;
            return new DateTime(y, m, d, h, n, s);
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}", Pattern, Index);
        }

        internal void Render(ref int accum, StringBuilder sb)
        {
            Index += accum;
            string f = _blockManager.Dameer.InternalValue.ToString(Pattern + ",").TrimEnd(',');
            sb.Remove(Index, Length);
            sb.Insert(Index, f);
            accum += f.Length - Length;
            Length = f.Length;
        }
        #endregion


        #region EventHandlers
        private void _blockManager_NeglectProposed(object sender, EventArgs e)
        {
            Debug.WriteLine("_blockManager_NeglectProposed");
            _proposed = null;
        }

        #endregion
    }
    public class NotificationEventArgs : EventArgs
    {
        #region Initialization

        public NotificationEventArgs() { }
        public NotificationEventArgs(string message)
        {
            Message = message;
        }

        #endregion

        // Message
        public string Message { get; protected set; }
    }
}
