using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Marv.Controls.Converters
{
    [ValueConversion(typeof(bool), typeof(string))]
    public class BooleanToStringConverter : DependencyObject, IValueConverter
    {
        public static readonly DependencyProperty FalseStringProperty =
        DependencyProperty.Register("FalseString", typeof(string), typeof(BooleanToStringConverter), new PropertyMetadata("false"));

        public static readonly DependencyProperty TrueStringProperty =
        DependencyProperty.Register("TrueString", typeof(string), typeof(BooleanToStringConverter), new PropertyMetadata("true"));

        public string FalseString
        {
            get { return (string)this.GetValue(FalseStringProperty); }
            set { this.SetValue(FalseStringProperty, value); }
        }

        public string TrueString
        {
            get { return (string)this.GetValue(TrueStringProperty); }
            set { this.SetValue(TrueStringProperty, value); }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var original = (bool)value;

            if (original == true)
            {
                return this.TrueString;
            }
            else
            {
                return this.FalseString;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return false;
        }
    }
}