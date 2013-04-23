using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace LibPipeline
{
    public class DouglasPeuckerReducer
    {
        public List<Point> Points;

        public DouglasPeuckerReducer(List<Point> points)
        {
            this.Points = points;
            this.Tolerance = 10;
        }

        public double Tolerance { get; set; }

        public IEnumerable<ILocation> Reduce(IEnumerable<ILocation> originalLocations, BingMap bingMap)
        {
            // If points are null or too few then return
            if (originalLocations == null || originalLocations.Count() < 3)
            {
                return originalLocations;
            }

            int firstPoint = 0;
            int lastPoint = originalLocations.Count() - 1;
            List<int> pointIndexsToKeep = new List<int>();

            //Add the first and last index to the keepers
            pointIndexsToKeep.Add(firstPoint);
            pointIndexsToKeep.Add(lastPoint);

            //The first and the last point cannot be the same
            while (lastPoint >= 0 && originalLocations.ElementAt(firstPoint).Equals(originalLocations.ElementAt(lastPoint)))
            {
                lastPoint--;
            }

            this.DouglasPeuckerReduction(originalLocations, firstPoint, lastPoint, Tolerance, ref pointIndexsToKeep, bingMap);

            // Make new List<Point> to return
            List<ILocation> reducedLocations = new List<ILocation>();

            pointIndexsToKeep.Sort();
            foreach (int index in pointIndexsToKeep)
            {
                reducedLocations.Add(originalLocations.ElementAt(index));
            }

            // If the simplification results in zero points, just add the endpoints.
            if (this.Points.Count == 0)
            {
                reducedLocations.Add(originalLocations.First());
                reducedLocations.Add(originalLocations.Last());
            }

            return reducedLocations;
        }

        public List<Point> Reduce()
        {
            // If points are null or too few then return
            if (this.Points == null || this.Points.Count < 3)
            {
                return this.Points;
            }

            int firstPoint = 0;
            int lastPoint = Points.Count - 1;
            List<int> pointIndexsToKeep = new List<int>();

            //Add the first and last index to the keepers
            pointIndexsToKeep.Add(firstPoint);
            pointIndexsToKeep.Add(lastPoint);

            //The first and the last point cannot be the same
            while (this.Points[firstPoint].Equals(this.Points[lastPoint]))
            {
                lastPoint--;
            }

            this.DouglasPeuckerReduction(Points, firstPoint, lastPoint, Tolerance, ref pointIndexsToKeep);

            // Make new List<Point> to return
            List<Point> returnPoints = new List<Point>();

            pointIndexsToKeep.Sort();
            foreach (int index in pointIndexsToKeep)
            {
                returnPoints.Add(this.Points[index]);
            }

            // If the simplification results in zero points, just add the endpoints.
            if (this.Points.Count == 0)
            {
                returnPoints.Add(this.Points.First());
                returnPoints.Add(this.Points.Last());
            }

            return returnPoints;
        }

        public async Task<IEnumerable<ILocation>> ReduceAsync(IEnumerable<ILocation> originalLocations, BingMap bingMap)
        {
            return await Task.Run(() => this.Reduce(originalLocations, bingMap));
        }

        private void DouglasPeuckerReduction(IEnumerable<ILocation> originalLocations, int firstPoint, int lastPoint, double tolerance, ref List<int> pointIndexsToKeep, BingMap bingMap)
        {
            double maxDistance = 0;
            int indexFarthest = 0;

            // originalLocations.ElementAt(firstPoint) or originalLocations.ElementAt(lastPoint) may not have values
            while (firstPoint <= originalLocations.Count() && !originalLocations.ElementAt(firstPoint).HasValue())
            {
                firstPoint++;
            }

            while (lastPoint >= 0 && !originalLocations.ElementAt(lastPoint).HasValue())
            {
                lastPoint--;
            }

            for (int index = firstPoint; index < lastPoint; index++)
            {
                ILocation fl = originalLocations.ElementAt(firstPoint);
                ILocation ll = originalLocations.ElementAt(lastPoint);
                ILocation l = originalLocations.ElementAt(index);

                if (fl.HasValue() && ll.HasValue() && l.HasValue())
                {
                    Point fp = bingMap.LocationToViewportPoint(fl.AsLocation());
                    Point lp = bingMap.LocationToViewportPoint(ll.AsLocation());
                    Point p = bingMap.LocationToViewportPoint(l.AsLocation());

                    double distance = PerpendicularDistance(fp, lp, p);

                    if (distance > maxDistance)
                    {
                        maxDistance = distance;
                        indexFarthest = index;
                    }
                }
            }

            if (maxDistance > tolerance && indexFarthest != 0)
            {
                //Add the largest point that exceeds the tolerance
                pointIndexsToKeep.Add(indexFarthest);

                this.DouglasPeuckerReduction(originalLocations, firstPoint, indexFarthest, tolerance, ref pointIndexsToKeep, bingMap);
                this.DouglasPeuckerReduction(originalLocations, indexFarthest, lastPoint, tolerance, ref pointIndexsToKeep, bingMap);
            }
        }

        private void DouglasPeuckerReduction(List<Point> points, int firstPoint, int lastPoint, double tolerance, ref List<int> pointIndexsToKeep)
        {
            double maxDistance = 0;
            int indexFarthest = 0;

            for (int index = firstPoint; index < lastPoint; index++)
            {
                double distance = PerpendicularDistance(points[firstPoint], points[lastPoint], points[index]);

                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    indexFarthest = index;
                }
            }

            if (maxDistance > tolerance && indexFarthest != 0)
            {
                //Add the largest point that exceeds the tolerance
                pointIndexsToKeep.Add(indexFarthest);

                this.DouglasPeuckerReduction(points, firstPoint, indexFarthest, tolerance, ref pointIndexsToKeep);
                this.DouglasPeuckerReduction(points, indexFarthest, lastPoint, tolerance, ref pointIndexsToKeep);
            }
        }

        private double PerpendicularDistance(Point Point1, Point Point2, Point Point)
        {
            Double area = Math.Abs(.5 * (Point1.X * Point2.Y + Point2.X *
                Point.Y + Point.X * Point1.Y - Point2.X * Point1.Y - Point.X *
                Point2.Y - Point1.X * Point.Y));

            Double bottom = Math.Sqrt(Math.Pow(Point1.X - Point2.X, 2) +
            Math.Pow(Point1.Y - Point2.Y, 2));
            Double height = area / bottom * 2;

            return height;
        }
    }
}