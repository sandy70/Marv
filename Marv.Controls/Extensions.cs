using Marv.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Marv.Controls
{
    public static class Extensions
    {
        public static IEnumerable<Point> ToPoints(this IEnumerable<Location> locations, MapView mapView)
        {
            return locations.Select(location =>
                {
                    return mapView.LocationToViewportPoint(location.ToMapControlLocation());
                });
        }

        public static IEnumerable<Location> ToLocations(this IEnumerable<Point> points, MapView mapView)
        {
            return points.Select<Point, Location>(point =>
                {
                    return mapView.ViewportPointToLocation(point);
                });
        }
    }
}
