using System;
using System.Collections.Generic;
using System.Linq;

namespace Marv.Map
{
    public static partial class Extensions
    {
        public static LocationRect GetBounds(this IEnumerable<LocationCollection> locationCollections)
        {
            var collections = locationCollections as IList<LocationCollection> ?? locationCollections.ToList();

            var bounds = collections.First().Bounds;

            return collections.Aggregate(bounds, (current, locationCollection) => LocationRect.Union(current, locationCollection.Bounds));
        }

        public static Location NearestTo(this IEnumerable<Location> locations, Location queryLocation)
        {
            if (locations == null || queryLocation == null)
            {
                return null;
            }

            var nearestDistance = Double.MaxValue;

            var locationList = locations as IList<Location> ?? locations.ToList();

            var nearestLocation = locationList.FirstOrDefault();

            foreach (var location in locationList)
            {
                var distance = Utils.Distance(location, queryLocation);

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
            return new MapControl.Location
            {
                Latitude = location.Latitude, 
                Longitude = location.Longitude
            };
        }

        public static IEnumerable<Location> Within(this IEnumerable<Location> locations, LocationRect rect)
        {
            return locations.Where(x => x.Latitude > rect.South && x.Latitude < rect.North &&
                                        x.Longitude > rect.West && x.Longitude < rect.East);
        }
    }
}