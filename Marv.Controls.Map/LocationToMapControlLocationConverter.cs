using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Marv.Common;

namespace Marv.Controls.Map
{
    [ValueConversion(typeof (Location), typeof (MapControl.Location))]
    [ValueConversion(typeof (IEnumerable<Location>), typeof (IEnumerable<MapControl.Location>))]
    public class LocationToMapControlLocationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Location)
            {
                var location = value as Location;
                return new MapControl.Location { Latitude = location.Latitude, Longitude = location.Longitude };
            }

            if (value is IEnumerable<Location>)
            {
                var locations = value as IEnumerable<Location>;
                return locations.Select(x => x);
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is MapControl.Location)
            {
                var location = value as MapControl.Location;
                return new Location { Latitude = location.Latitude, Longitude = location.Longitude };
            }
            if (value is IEnumerable<MapControl.Location>)
            {
                var locations = value as IEnumerable<MapControl.Location>;
                return locations.Select<MapControl.Location, Location>(x => x);
            }
            return Binding.DoNothing;
        }
    }
}