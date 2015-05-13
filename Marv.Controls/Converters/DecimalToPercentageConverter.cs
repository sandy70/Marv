using System;
using System.Globalization;
using System.Windows.Data;

namespace Marv.Controls.Converters
{
    [ValueConversion(typeof(double), typeof(double))]
    public class DecimalToPercentageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return 0;

            var original = (double)value;

            if (original < 0)
            {
                return 0;
            }

            if (original > 1)
            {
                return 100;
            }
        
            return original * 100;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
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