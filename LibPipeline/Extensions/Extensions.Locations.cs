using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibPipeline
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

        public static ObservableCollection<MultiLocationSegment> ToSegments(this IEnumerable<Location> locations)
        {
            Location start = null;
            Location middle = null;
            Location end = null;

            var index = -1;
            var random = new Random();

            var segments = new ObservableCollection<MultiLocationSegment>();

            foreach (var location in locations)
            {
                index++;

                start = middle;
                middle = end;
                end = location;

                if (index == 0)
                {
                    continue;
                }
                else if (index == 1)
                {
                    segments.Add(new MultiLocationSegment
                    {
                        Middle = middle,
                        End = Utils.Mid(middle, end),
                    });
                }
                else
                {
                    segments.Add(new MultiLocationSegment
                    {
                        Start = Utils.Mid(start, middle),
                        Middle = middle,
                        End = Utils.Mid(middle, end),
                    });

                    if (index == locations.Count() - 1)
                    {
                        segments.Add(new MultiLocationSegment
                        {
                            Start = Utils.Mid(middle, end),
                            Middle = end,
                        });
                    }
                }
            }

            Console.WriteLine("nSegments: " + segments.Count);

            return segments;
        }

        public static IEnumerable<Location> ToViewportPoints(this IEnumerable<Location> locations, MapView mapView, string valueMemberPath = "Value")
        {
            return locations.Select(location =>
            {
                var point = mapView.LocationToViewportPoint(location.ToMapControlLocation());

                return new Location
                {
                    Value = (double)location[valueMemberPath],
                    X = point.X,
                    Y = point.Y
                };
            }).ToList();
        }

        public static IEnumerable<Location> Within(this IEnumerable<Location> locations, LocationRect rect)
        {
            return locations.Where(x => x.Latitude > rect.South && x.Latitude < rect.North &&
                                        x.Longitude > rect.West && x.Longitude < rect.East);
        }
    }
}
