using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace EL.BaseUI.Themes
{
    /// <summary>
    /// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
    ///
    /// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Rivamed.Core.UserControls.View"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Rivamed.Core.UserControls.View;assembly=Rivamed.Core.UserControls.View"
    ///
    /// 您还需要添加一个从 XAML 文件所在的项目到此项目的项目引用，
    /// 并重新生成以避免编译错误:
    ///
    ///     在解决方案资源管理器中右击目标项目，然后依次单击
    ///     “添加引用”->“项目”->[浏览查找并选择此项目]
    ///
    ///
    /// 步骤 2)
    /// 继续操作并在 XAML 文件中使用控件。
    ///
    ///     <MyNamespace:CustomComboBox/>
    ///
    /// </summary>
    public class CustomComboBox : ComboBox
    {
        static CustomComboBox()
        {
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomComboBox), new FrameworkPropertyMetadata(typeof(CustomComboBox)));
        }



        public string SearchText
        {
            get { return (string)GetValue(SearchTextProperty); }
            set { SetValue(SearchTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SearchText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SearchTextProperty =
            DependencyProperty.Register("SearchText", typeof(string), typeof(CustomComboBox), new PropertyMetadata(""));

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            //var searchButton = GetTemplateChild("PART_SearchButton") as Button;
            //if (searchButton != null)
            //{
            //    searchButton.Click += SearchButton_Click;
            //}
            var searchButton = GetTemplateChild("PART_SearchButton") as SearchTextBox;
            if (searchButton != null)
            {
                searchButton.SearchCommand = new CommunityToolkit.Mvvm.Input.RelayCommand(() => SearchButton_Click(null, null)) ;
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(SearchText))
            {
                ICollectionView defaultView = CollectionViewSource.GetDefaultView(this.ItemsSource);
                if (string.IsNullOrEmpty(this.DisplayMemberPath))
                {
                    defaultView.Filter = (x => x.ToString().Contains(SearchText));
                }
                else
                {
                    defaultView.Filter = (x => ReflectHelper.GetPropertyValue(x, this.DisplayMemberPath).ToString().Contains(SearchText));
                }
            }
            else
            {
                ICollectionView defaultView = CollectionViewSource.GetDefaultView(this.ItemsSource);
                defaultView.Filter = null;
            }
        }
    }
}
