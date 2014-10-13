using System;
using System.Windows.Data;

namespace Marv.Controls
{
    public class DecimalPercentalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (string.IsNullOrEmpty(value.ToString()))
            {
                return 0;
            }

            if (value.GetType() == typeof(double))
            {
                return (int)((double)value * 100);
            }

            if (value.GetType() == typeof(decimal))
            {
                return (int)((double)value * 100);
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var str = value.ToString();

            if (string.IsNullOrEmpty(str)) return 0;

            var trimmedValue = str.TrimEnd("%".ToCharArray());

            if (targetType != typeof (decimal) && targetType != typeof(double)) return value;

            var result = 0.0;
            return double.TryParse(trimmedValue, out result) ? result : value;
        }
    }
}