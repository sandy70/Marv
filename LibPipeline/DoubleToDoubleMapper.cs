using System;
using System.Windows;
using System.Windows.Data;

namespace LibPipeline
{
    [ValueConversion(typeof(double), typeof(double))]
    internal class DoubleToDoubleMapper : DependencyObject, IValueConverter
    {
        public static readonly DependencyProperty MapperProperty =
        DependencyProperty.Register("Mapper", typeof(IMapDoubleToDouble), typeof(DoubleToDoubleMapper), new PropertyMetadata(new MapDoubleToDouble()));

        public IMapDoubleToDouble Mapper
        {
            get { return (IMapDoubleToDouble)GetValue(MapperProperty); }
            set { SetValue(MapperProperty, value); }
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var original = (double)value;

            return Mapper.Map(original);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var original = (double)value;

            return Mapper.MapBack(original);
        }
    }
}