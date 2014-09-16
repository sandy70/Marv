using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Marv
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BooleanToVisibilityConverter : DependencyObject, IValueConverter
    {
        public static readonly DependencyProperty IsReversedProperty =
        DependencyProperty.Register("IsReversed", typeof(bool), typeof(BooleanToVisibilityConverter), new PropertyMetadata(false));

        public bool IsReversed
        {
            get { return (bool)this.GetValue(IsReversedProperty); }
            set { this.SetValue(IsReversedProperty, value); }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var original = (bool)value;
            
            if (original ^ this.IsReversed)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var original = (Visibility)value;

            if (original == Visibility.Visible)
            {
                return !this.IsReversed;
            }
            else
            {
                return this.IsReversed;
            }
        }
    }
}