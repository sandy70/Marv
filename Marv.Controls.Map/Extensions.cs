using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Marv.Map;

namespace Marv.Controls.Map
{
    public static class Extensions
    {
        public static IEnumerable<Point> ToPoints(this IEnumerable<Location> locations, MapView mapView)
        {
            return locations.Select(location => mapView.LocationToViewportPoint(Marv.Map.Extensions.ToMapControlLocation(location)));
        }

        public static IEnumerable<Location> ToLocations(this IEnumerable<Point> points, MapView mapView)
        {
            return points.Select<Point, Location>(point => mapView.ViewportPointToLocation(point));
        }
    }
}
