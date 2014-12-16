using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Marv.Controls.Converters
{
    [ValueConversion(typeof(double), typeof(LinearGradientBrush))]
    public class DoubleToJetGradientConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var original = (double)value;

            var gradientStopCollection = new GradientStopCollection();

            if (original == 1)
            {
                gradientStopCollection.Add(new GradientStop(Colors.Red, 0));
                gradientStopCollection.Add(new GradientStop(Colors.Red, 1));
            }
            else
            {
                for (double v = 0; v < original; v += 0.01)
                {
                    gradientStopCollection.Add(new GradientStop(Utils.DoubleToColor(v), v));
                }
            }

            var brush = new LinearGradientBrush { StartPoint = new Point(0, 0), EndPoint = new Point(1, 0) };
            brush.GradientStops = gradientStopCollection;
            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var original = (Color)value;
            return (double)original.R;
        }
    }
}