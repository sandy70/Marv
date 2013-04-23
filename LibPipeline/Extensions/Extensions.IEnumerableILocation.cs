using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace LibPipeline
{
    public partial class Extensions
    {
        public static LocationCollection AsLocationCollection(this IEnumerable<ILocation> locations)
        {
            LocationCollection locationCollection = new LocationCollection();

            foreach (var location in locations)
            {
                if (location.HasValue())
                {
                    locationCollection.Add(location.AsLocation());
                }
            }

            return locationCollection;
        }

        public static List<Point> AsViewportPoints(this IEnumerable<ILocation> locations, BingMap bingMap)
        {
            List<Point> points = new List<Point>();

            if (locations == null)
                return points;

            foreach (var location in locations)
            {
                Point viewportPoint = bingMap.LocationToViewportPoint(location.AsLocation());
                points.Add(viewportPoint);
            }

            return points;
        }

        public static LocationRect GetBoundingBox(this IEnumerable<ILocation2D> locations, double pad = 0)
        {
            LocationRect locationRect = new LocationRect();

            locationRect.North = (double)locations.Max(l => l.Latitude) + pad;
            locationRect.South = (double)locations.Min(l => l.Latitude) - pad;
            locationRect.East = (double)locations.Max(l => l.Longitude) + pad;
            locationRect.West = (double)locations.Min(l => l.Longitude) - pad;

            return locationRect;
        }

        public static ILocation GetNearest(this IEnumerable<ILocation> locations, Location queryLocation)
        {
            double nearestDistance = Double.MaxValue;
            ILocation nearestLocation = new PipelineSegment();

            foreach (var location in locations)
            {
                double distance = Utils.DistanceBetweenLocation(location, queryLocation);

                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestLocation = location;
                }
            }

            return nearestLocation;
        }

        public static IEnumerable<ILocation> WithinViewport(this IEnumerable<ILocation> locations, BingMap bingMap)
        {
            List<ILocation> insideLocations = new List<ILocation>();
            LocationRect boundingRect = bingMap.BoundingRectangle;

            // Calculate padding. Since distances are small, using lat, lon values works
            double pad = Math.Max(Math.Abs(boundingRect.East - boundingRect.West),
                                  Math.Abs(boundingRect.North - boundingRect.South));

            foreach (var location in locations)
            {
                if (boundingRect.South - pad <= location.Latitude && location.Latitude <= boundingRect.North + pad &&
                   boundingRect.West - pad <= location.Longitude && location.Longitude <= boundingRect.East + pad)
                {
                    insideLocations.Add(location);
                }
            }

            return insideLocations;
        }
    }
}