using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;

namespace LibPipeline
{
    [ValueConversion(typeof(IEnumerable<Location>), typeof(IEnumerable<MapControl.Location>))]
    public class IEnumerableILocationToIEnumerableLocationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is IEnumerable<Location>)
            {
                var locations = value as IEnumerable<Location>;
                return locations.Select<Location, MapControl.Location>(x => new MapControl.Location { Latitude = x.Latitude, Longitude = x.Longitude });
            }
            else
            {
                return Binding.DoNothing;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is IEnumerable<MapControl.Location>)
            {
                var locations = value as IEnumerable<MapControl.Location>;
                return locations.Select<MapControl.Location, LibPipeline.Location>(x => new LibPipeline.Location { Latitude = x.Latitude, Longitude = x.Longitude });
            }
            else
            {
                return Binding.DoNothing;
            }
        }
    }
}