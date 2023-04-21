using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Robot.Converters
{
    public class MsgColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int type)
            {
                if (type == 0)
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3C94F2"));
                if (type == 1)
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EE9A30"));
                if (type == 3)
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#50A24B"));
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
