using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Marv
{
    [ValueConversion(typeof (int), typeof (Visibility))]
    public class IntToVisibilityConverter : DependencyObject, IValueConverter
    {
        public static readonly DependencyProperty IsReversedProperty =
            DependencyProperty.Register("IsReversed", typeof (bool), typeof (IntToVisibilityConverter), new PropertyMetadata(false));

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
            var original = (int) value;

            if (original != 0 ^ this.IsReversed)
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}