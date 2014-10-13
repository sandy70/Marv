using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Marv.Controls
{
    [ValueConversion(typeof (double), typeof (Visibility))]
    public class DoubleToVisibilityConverter : DependencyObject, IValueConverter
    {
        public static readonly DependencyProperty IsReversedProperty =
            DependencyProperty.Register("IsReversed", typeof (bool), typeof (DoubleToVisibilityConverter), new PropertyMetadata(false));

        public bool IsReversed
        {
            get { return (bool) this.GetValue(IsReversedProperty); }
            set { this.SetValue(IsReversedProperty, value); }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var doubleValue = (double) value;

            if ((doubleValue == 0) ^ this.IsReversed)
            {
                return Visibility.Collapsed;
            }

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var original = (Visibility) value;

            if ((original == Visibility.Visible) ^ this.IsReversed)
            {
                return 1.0;
            }

            return 0.0;
        }
    }
}