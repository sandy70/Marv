using System;
using System.Collections.Generic;
using System.Linq;

namespace Marv.Common
{
    public static partial class Extensions
    {
        public static LocationRect Bounds(this IEnumerable<Location> locations)
        {
            var locationRect = new LocationRect();

            locationRect.South = locations.Min(x => x.Latitude);
            locationRect.West = locations.Min(x => x.Longitude);
            locationRect.North = locations.Max(x => x.Latitude);
            locationRect.East = locations.Max(x => x.Longitude);

            return locationRect;
        }

        public static LocationRect GetBounds(this IEnumerable<LocationCollection> locationCollections)
        {
            var bounds = locationCollections.First().Bounds;

            foreach (var locationCollection in locationCollections)
            {
                bounds = LocationRect.Union(bounds, locationCollection.Bounds);
            }

            return bounds;
        }

        public static Location NearestTo(this IEnumerable<Location> locations, Location queryLocation)
        {
            if (locations == null || queryLocation == null)
            {
                return null;
            }

            double nearestDistance = Double.MaxValue;
            var nearestLocation = locations.FirstOrDefault();

            foreach (var location in locations)
            {
                double distance = Utils.Distance(location, queryLocation);

                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestLocation = location;
                }
            }

            return nearestLocation;
        }

        public static MapControl.Location ToMapControlLocation(this Location location)
        {
            return new MapControl.Location { Latitude = location.Latitude, Longitude = location.Longitude };
        }

        public static IEnumerable<Location> Within(this IEnumerable<Location> locations, LocationRect rect)
        {
            return locations.Where(x => x.Latitude > rect.South && x.Latitude < rect.North &&
                                        x.Longitude > rect.West && x.Longitude < rect.East);
        }
    }
}