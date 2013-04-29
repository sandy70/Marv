using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace LibMarv
{
    [ValueConversion(typeof(bool), typeof(Brush))]
    public class BooleanToColorConverter : DependencyObject, IValueConverter
    {
        public static readonly DependencyProperty FalseColorProperty =
        DependencyProperty.Register("FalseColor", typeof(Color), typeof(BooleanToColorConverter), new PropertyMetadata(Colors.Black));

        public static readonly DependencyProperty TrueColorProperty =
        DependencyProperty.Register("TrueColor", typeof(Color), typeof(BooleanToColorConverter), new PropertyMetadata(Colors.White));

        public Color FalseColor
        {
            get { return (Color)GetValue(FalseColorProperty); }
            set { SetValue(FalseColorProperty, value); }
        }

        public Color TrueColor
        {
            get { return (Color)this.GetValue(TrueColorProperty); }
            set { SetValue(TrueColorProperty, value); }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool original = (bool)value;

            if (original == true)
            {
                return this.TrueColor;
            }
            else
            {
                return this.FalseColor;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return false;
        }
    }
}