using MapControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using ImpromptuInterface;

namespace LibPipeline
{
    [ValueConversion(typeof(IEnumerable<ILocation>), typeof(IEnumerable<Location>))]
    public class IEnumerableILocationToIEnumerableLocationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is IEnumerable<ILocation>)
            {
                var locations = value as IEnumerable<ILocation>;
                return locations.Select<ILocation, Location>(x => new Location { Latitude = x.Latitude, Longitude = x.Longitude });
            }
            else
            {
                return Binding.DoNothing;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is IEnumerable<Location>)
            {
                var locations = value as IEnumerable<Location>;
                return locations.AllActLike<ILocation>();
            }
            else
            {
                return Binding.DoNothing;
            }
        }
    }
}
