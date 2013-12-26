using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Marv.Controls
{
    [ValueConversion(typeof(object), typeof(Visibility))]
    public class NullToVisibilityConverter : DependencyObject, IValueConverter
    {
        public static readonly DependencyProperty IsReversedProperty =
        DependencyProperty.Register("IsReversed", typeof(bool), typeof(NullToVisibilityConverter), new PropertyMetadata(false));

        public bool IsReversed
        {
            get { return (bool)GetValue(IsReversedProperty); }
            set { SetValue(IsReversedProperty, value); }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value == null) ^ this.IsReversed)
            {
                return Visibility.Collapsed;
            }
            else
            {
                return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility original = (Visibility)value;

            if ((original == Visibility.Visible) ^ this.IsReversed)
            {
                return new object();
            }
            else
            {
                return null;
            }
        }
    }
}