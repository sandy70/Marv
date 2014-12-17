using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Marv.Controls.Converters
{
    [ValueConversion(typeof (int), typeof (Visibility))]
    public class IntToVisibilityConverter : DependencyObject, IValueConverter
    {
        public static readonly DependencyProperty CutoffProperty =
            DependencyProperty.Register("Cutoff", typeof (int), typeof (IntToVisibilityConverter), new PropertyMetadata(0));

        public static readonly DependencyProperty IsReversedProperty =
            DependencyProperty.Register("IsReversed", typeof (bool), typeof (IntToVisibilityConverter), new PropertyMetadata(false));

        public int Cutoff
        {
            get { return (int) GetValue(CutoffProperty); }
            set { SetValue(CutoffProperty, value); }
        }

        public bool IsReversed
        {
            get { return (bool) this.GetValue(IsReversedProperty); }
            set { this.SetValue(IsReversedProperty, value); }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var original = (int) value;

            if (original > this.Cutoff ^ this.IsReversed)
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