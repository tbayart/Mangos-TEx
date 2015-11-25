using System;
using System.Globalization;
using System.Windows.Data;

namespace Framework.Converters
{
    [ValueConversion(typeof(object), typeof(bool))]
    public class IsEqualConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && parameter != null
                && string.Compare(value.ToString(), parameter.ToString(), true) == 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return parameter;
        }
    }
}
