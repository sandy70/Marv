using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Marv.Controls.Converters
{
    [ValueConversion(typeof (bool), typeof (Brush))]
    public class BooleanToBrushConverter : DependencyObject, IValueConverter
    {
        public static readonly DependencyProperty FalseBrushProperty =
            DependencyProperty.Register("FalseBrush", typeof (Brush), typeof (BooleanToBrushConverter), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        public static readonly DependencyProperty TrueBrushProperty =
            DependencyProperty.Register("TrueBrush", typeof (Brush), typeof (BooleanToBrushConverter), new PropertyMetadata(new SolidColorBrush(Colors.White)));

        public Brush FalseBrush
        {
            get
            {
                return (Brush) this.GetValue(FalseBrushProperty);
            }

            set
            {
                this.SetValue(FalseBrushProperty, value);
            }
        }

        public Brush TrueBrush
        {
            get
            {
                return (Brush) this.GetValue(TrueBrushProperty);
            }

            set
            {
                this.SetValue(TrueBrushProperty, value);
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool) value ? this.TrueBrush : this.FalseBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return false;
        }
    }
}