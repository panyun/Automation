using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AduSkin.Controls.Metro
{
    public class AduIndexRadionButton : RadioButton
    {
        static AduIndexRadionButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AduIndexRadionButton), new FrameworkPropertyMetadata(typeof(AduIndexRadionButton)));


        }
        public static readonly DependencyProperty PathDataProperty = DependencyProperty.RegisterAttached(
           "PathData", typeof(Geometry), typeof(AduIndexRadionButton), new PropertyMetadata(default(Geometry)));

        public static void SetPathData(DependencyObject element, Geometry value)
            => element.SetValue(PathDataProperty, value);

        public static Geometry GetPathData(DependencyObject element)
            => (Geometry)element.GetValue(PathDataProperty);
    }
}
