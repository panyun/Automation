using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace EL.BaseUI.Themes
{
    public class ObjectIsNullOrEmptyToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility result = Visibility.Visible;
            if (value == null)
            {
                result = Visibility.Collapsed;

            }
            else if (value is Guid && (Guid)value == Guid.Empty)
            {
                result = Visibility.Collapsed;
            }
            else if (value is string && value.ToString() == "")
            {
                result = Visibility.Collapsed;
            }
            else if (value is ICollection && (value as ICollection).Count == 0)
            {
                result = Visibility.Collapsed;
            }
            return result.VisibilityReverse(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool model = (bool)value;
            Visibility result;
            if (model)
            {
                result = Visibility.Visible;

            }
            else
            {
                result = Visibility.Collapsed;
            }

            if (parameter != null && parameter.ToString() == "1")
            {
                result = result.VisibilityReverse();
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public static class ExtendHelper
    {
        public static Visibility VisibilityReverse(this Visibility model)
        {
            Visibility result;
            if (model == Visibility.Visible)
            {
                result = Visibility.Collapsed;
            }
            else
            {
                result = Visibility.Visible;
            }
            return result;
        }
        public static Visibility VisibilityReverse(this Visibility visibility, object parameter)
        {
            var result = visibility;
            if (parameter != null && Convert.ToInt32(parameter) == 1)
            {
                if (result == Visibility.Visible)
                {
                    result = Visibility.Collapsed;
                }
                else
                {
                    result = Visibility.Visible;
                }
            }
            return result;
        }

        public static bool EqualsIgnoreCase(this string s, string s2)
        {
            if (s == null)
            {
                return s == s2;
            }
            else
            {
                return s.Equals(s2, StringComparison.OrdinalIgnoreCase);
            }

        }
    }
}
