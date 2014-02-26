using System;
using System.Windows.Data;

namespace Marv.Common
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
            if (string.IsNullOrEmpty(value.ToString()))
            {
                return 0;
            }

            var trimmedValue = value.ToString().TrimEnd(new char[] { '%' });

            if (targetType == typeof(double))
            {
                double result;

                if (double.TryParse(trimmedValue, out result))
                {
                    return result;
                }
                else
                {
                    return value;
                }
            }

            if (targetType == typeof(decimal))
            {
                decimal result;
                if (decimal.TryParse(trimmedValue, out result))
                {
                    return result;
                }
                else
                {
                    return value;
                }
            }

            return value;
        }
    }
}