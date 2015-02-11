using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Marv.Common;

namespace Marv.Controls.Map
{
    public static class Extensions
    {
        public static IEnumerable<LocationCollection> Reduce(this IEnumerable<LocationCollection> locationCollections, MapView mapView, double tolerance)
        {
            return locationCollections.Select(locationCollection => locationCollection.ToPoints(mapView).Reduce(tolerance).ToLocations(mapView).ToLocationCollection());
        }

        public static LocationCollection Reduce(this IEnumerable<Location> locations, MapView mapView, double tolerance)
        {
            return locations.ToPoints(mapView).Reduce(tolerance).ToLocations(mapView).ToLocationCollection();
        }

        public static IEnumerable<Location> Reduce(this IEnumerable<Location> locations, MapTransform converter, double tolerance)
        {
            return locations.ToPoints(converter).Reduce(tolerance).ToLocations(converter);
        }

        public static Location ToLocation(this Point point, MapTransform converter)
        {
            return converter.ToLocation(point);
        }

        public static IEnumerable<Location> ToLocations(this IEnumerable<Point> points, MapView mapView)
        {
            return points.Select<Point, Location>(point => mapView.ViewportPointToLocation(point));
        }

        public static IEnumerable<Location> ToLocations(this IEnumerable<Point> points, MapTransform converter)
        {
            return points.Select(point => point.ToLocation(converter));
        }

        public static Point ToPoint(this Location location, MapTransform converter)
        {
            return converter.ToPoint(location);
        }

        public static IEnumerable<Point> ToPoints(this IEnumerable<Location> locations, MapView mapView)
        {
            return locations.Select(location => mapView.LocationToViewportPoint(location));
        }

        public static IEnumerable<Point> ToPoints(this IEnumerable<Location> locations, MapTransform converter)
        {
            return locations.Select(location => location.ToPoint(converter));
        }
    }
}