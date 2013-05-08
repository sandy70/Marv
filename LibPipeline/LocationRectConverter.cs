using SharpKml.Dom;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace LibPipeline
{
    public class LocationRectConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var locationRect = new LocationRect();

            var str = value as string;

            var parts = str.Split(",".ToCharArray());

            if (parts.Count() == 4)
            {
                locationRect.South = Double.Parse(parts[0]);
                locationRect.West = Double.Parse(parts[1]);
                locationRect.North = Double.Parse(parts[2]);
                locationRect.East = Double.Parse(parts[3]);
            }

            return locationRect;
        }
    }
}