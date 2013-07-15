using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace LibPipeline
{
    [ValueConversion(typeof(Location), typeof(MapControl.Location))]
    public class ILocationToLocationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Location)
            {
                var location = value as Location;
                return new MapControl.Location { Latitude = location.Latitude, Longitude = location.Longitude };
            }
            else
            {
                return Binding.DoNothing;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is MapControl.Location)
            {
                var location = value as MapControl.Location;
                return new LibPipeline.Location { Latitude = location.Latitude, Longitude = location.Longitude };
            }
            else
            {
                return Binding.DoNothing;
            }
        }
    }
}
