using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Marv.Common
{
    [ValueConversion(typeof (object), typeof (bool))]
    public class NullToBooleanConverter : DependencyObject, IValueConverter
    {
        public static readonly DependencyProperty IsReversedProperty =
            DependencyProperty.Register("IsReversed", typeof (bool), typeof (NullToBooleanConverter), new PropertyMetadata(false));

        public bool IsReversed
        {
            get
            {
                return (bool) this.GetValue(IsReversedProperty);
            }
            set
            {
                this.SetValue(IsReversedProperty, value);
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value == null) ^ this.IsReversed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var original = (bool) value;
            return original ? null : new object();
        }
    }
}