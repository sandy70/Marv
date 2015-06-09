using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Data;

namespace Marv.Controls.Converters
{
    public class PathCombineConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return Path.Combine(values.Cast<string>().ToArray());
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}