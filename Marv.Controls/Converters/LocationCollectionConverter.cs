using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Marv.Common;

namespace Marv.Controls.Converters
{
    [ValueConversion(typeof (IEnumerable<Location>), typeof (IEnumerable<MapControl.Location>))]
    [ValueConversion(typeof (IEnumerable<MapControl.Location>), typeof (IEnumerable<Location>))]
    internal class LocationCollectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable<Location>)
            {
                return (value as IEnumerable<Location>).Select(location => (MapControl.Location) location);
            }

            if (value is IEnumerable<MapControl.Location>)
            {
                return (value as IEnumerable<MapControl.Location>).Select(location => (Location) location);
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable<Location>)
            {
                return (value as IEnumerable<Location>).Select(location => (MapControl.Location) location);
            }

            if (value is IEnumerable<MapControl.Location>)
            {
                return (value as IEnumerable<MapControl.Location>).Select(location => (Location) location);
            }

            return Binding.DoNothing;
        }
    }
}