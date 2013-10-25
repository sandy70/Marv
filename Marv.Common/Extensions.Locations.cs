using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using System.Windows;

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

        public static IEnumerable<Location> ToViewportPoints(this IEnumerable<Location> locations, Func<Location, Point> locationToPoint, string valueMemberPath = "Value")
        {
            return locations.Select(location =>
                                        {
                                            var point = locationToPoint(location.ToMapControlLocation());

                                            return new Location
                                            {
                                                Value = (double)location[valueMemberPath],
                                                X = point.X,
                                                Y = point.Y
                                            };
                                        }
                                    )
                            .ToList();
        }

        public static IEnumerable<Location> Within(this IEnumerable<Location> locations, LocationRect rect)
        {
            return locations.Where(x => x.Latitude > rect.South && x.Latitude < rect.North &&
                                        x.Longitude > rect.West && x.Longitude < rect.East);
        }

        public static IEnumerable<Location> Reduce(this IEnumerable<Location> locations, Func<Location, Point> locationToPoint, double tolerance = 10, Dictionary<Location, Point> viewportPointCache = null, int level = 0)
        {
            if (locations.Count() <= 2)
            {
                return locations;
            }
            else
            {
                var nLocations = locations.Count();

                if (viewportPointCache == null)
                {
                    viewportPointCache = new Dictionary<Location, Point>();

                    // we need to build the viewportPointCache
                    foreach (var location in locations)
                    {
                        viewportPointCache[location] = locationToPoint(location.ToMapControlLocation());
                    }
                }

                var first = locations.First();
                var last = locations.Last();

                var maxDistance = double.MinValue;
                var maxDistanceLocation = first;

                foreach (var location in locations)
                {
                    var distance = Utils.Distance(viewportPointCache[first],
                                                  viewportPointCache[last],
                                                  viewportPointCache[location]);

                    if (distance > maxDistance)
                    {
                        maxDistance = distance;
                        maxDistanceLocation = location;
                    }
                }

                logger.Debug("level: {0}, nLocations: {1}, maxDistance {2}", level, nLocations, maxDistance);

                if (maxDistance > tolerance)
                {
                    return locations.TakeUntil(x => x == maxDistanceLocation)
                                    .Reduce(locationToPoint, tolerance, viewportPointCache, level + 1)
                                    .Concat(locations.SkipWhile(x => x != maxDistanceLocation)
                                                     .Reduce(locationToPoint, tolerance, viewportPointCache, level + 1)
                                                     .Skip(1));
                }
                else
                {
                    return locations.Take(1)
                                    .Concat(last);
                }
            }
        }

        public static MapControl.Location ToMapControlLocation(this Location location)
        {
            return new MapControl.Location { Latitude = location.Latitude, Longitude = location.Longitude };
        }


    }
}