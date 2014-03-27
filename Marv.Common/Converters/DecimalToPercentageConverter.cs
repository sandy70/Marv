using System;
using System.Windows.Data;

namespace Marv.Common
{
    [ValueConversion(typeof(double), typeof(double))]
    public class DecimalToPercentageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var original = (double)value;

            if (original < 0)
            {
                return 0;
            }
            else if (original <= 1)
            {
                return original * 100;
            }
            else
            {
                return 100;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return 0;

            var original = (double)value;

            if (original < 0)
            {
                return 0;
            }

            if (original <= 100)
            {
                return original / 100;
            }

            return 1;
        }
    }
}