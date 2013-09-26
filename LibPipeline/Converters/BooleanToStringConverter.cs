using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace LibPipeline 
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
            get { return (string)GetValue(FalseStringProperty); }
            set { SetValue(FalseStringProperty, value); }
        }

        public string TrueString
        {
            get { return (string)GetValue(TrueStringProperty); }
            set { SetValue(TrueStringProperty, value); }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool original = (bool)value;

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