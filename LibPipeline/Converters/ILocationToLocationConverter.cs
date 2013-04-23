using Microsoft.Maps.MapControl.WPF;
using System;
using System.Windows.Data;

namespace LibPipeline
{
    [ValueConversion(typeof(ILocation), typeof(Location))]
    public class ILocationToLocationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((value as ILocation) != null)
            {
                return (value as ILocation).AsLocation();
            }
            else
            {
                return Binding.DoNothing;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((value as ILocation) != null)
            {
                return value as ILocation;
            }
            else
            {
                return Binding.DoNothing;
            }
        }
    }
}