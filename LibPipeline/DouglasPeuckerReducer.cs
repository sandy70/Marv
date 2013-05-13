using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace LibPipeline
{
    public class DouglasPeuckerReducer
    {
        public IEnumerable<Point> Points;

        public DouglasPeuckerReducer(IEnumerable<Point> points)
        {
            this.Points = points;
            this.Tolerance = 10;
        }

        public double Tolerance { get; set; }

        public IEnumerable<Point> Reduce()
        {
            // If points are null or too few then return
            if (this.Points == null || this.Points.Count() < 3)
            {
                return this.Points;
            }

            int firstPoint = 0;
            int lastPoint = Points.Count() - 1;
            List<int> pointIndexsToKeep = new List<int>();

            //Add the first and last index to the keepers
            pointIndexsToKeep.Add(firstPoint);
            pointIndexsToKeep.Add(lastPoint);

            //The first and the last point cannot be the same
            while (this.Points.ElementAt(firstPoint).Equals(this.Points.ElementAt(lastPoint)))
            {
                lastPoint--;
            }

            this.DouglasPeuckerReduction(Points, firstPoint, lastPoint, Tolerance, ref pointIndexsToKeep);

            // Make new List<Point> to return
            List<Point> returnPoints = new List<Point>();

            pointIndexsToKeep.Sort();
            foreach (int index in pointIndexsToKeep)
            {
                returnPoints.Add(this.Points.ElementAt(index));
            }

            // If the simplification results in zero points, just add the endpoints.
            if (this.Points.Count() == 0)
            {
                returnPoints.Add(this.Points.First());
                returnPoints.Add(this.Points.Last());
            }

            return returnPoints;
        }

        private void DouglasPeuckerReduction(IEnumerable<Point> points, int firstPoint, int lastPoint, double tolerance, ref List<int> pointIndexsToKeep)
        {
            double maxDistance = 0;
            int indexFarthest = 0;

            for (int index = firstPoint; index < lastPoint; index++)
            {
                double distance = PerpendicularDistance(points.ElementAt(firstPoint), points.ElementAt(lastPoint), points.ElementAt(index));

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