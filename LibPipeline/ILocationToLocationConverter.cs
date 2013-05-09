using ImpromptuInterface;
using MapControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;

namespace LibPipeline
{
    [ValueConversion(typeof(ILocation), typeof(Location))]
    public class ILocationToLocationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is ILocation)
            {
                var location = value as ILocation;
                return new Location { Latitude = location.Latitude, Longitude = location.Longitude };
            }
            else
            {
                return Binding.DoNothing;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Location)
            {
                var location = value as Location;
                return location.ActLike<ILocation>();
            }
            else
            {
                return Binding.DoNothing;
            }
        }

        
    }
}