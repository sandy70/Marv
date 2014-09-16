using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Marv.Map
{
    public class LocationRectConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) ||
                   sourceType == typeof(LocationRect);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is LocationRect)
            {
                return value;
            }
            
            if (value is string)
            {
                var str = value as string;

                if (Countries.BoundsForKey.ContainsKey(str))
                {
                    return Countries.BoundsForKey[str];
                }
                
                var locationRect = new LocationRect();
                
                var parts = str.Split(",".ToCharArray());

                if (parts.Count() == 4)
                {
                    locationRect.North = Double.Parse(parts[0]);
                    locationRect.East = Double.Parse(parts[1]);
                    locationRect.South = Double.Parse(parts[2]);
                    locationRect.West = Double.Parse(parts[3]);
                }

                return locationRect;
            }

            return null;
        }
    }
}