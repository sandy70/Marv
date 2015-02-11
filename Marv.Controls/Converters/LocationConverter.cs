using System;
using System.Globalization;
using System.Windows.Data;
using Marv.Common;

namespace Marv.Controls.Converters
{
    public class LocationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Location)
            {
                return (MapControl.Location) (value as Location);
            }

            if (value is MapControl.Location)
            {
                return (Location) (value as MapControl.Location);
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Location)
            {
                return (MapControl.Location) (value as Location);
            }

            if (value is MapControl.Location)
            {
                return (Location) (value as MapControl.Location);
            }

            return Binding.DoNothing;
        }
    }
}