using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Marv.Input
{
    public class LinearInterpolator
    {
        private readonly List<Point> points = new List<Point>();

        public LinearInterpolator(IEnumerable<double> xCoords, IEnumerable<double> yCoords)
        {
            var x = xCoords as IList<double> ?? xCoords.ToList();
            var y = yCoords as IList<double> ?? yCoords.ToList();

            if (x.Count < 2 || y.Count < 2 || x.Count != y.Count)
            {
                throw new InvalidValueException("Number of X and Y coordinates should be equal and > 2");
            }

            for (var i = 0; i < x.Count(); i++)
            {
                this.points.Add(new Point(x[i], y[i]));
            }

            this.points = this.points.OrderBy(point => point.X).ToList();
        }

        public double Eval(double x)
        {
            int xMinIndex;
            int xMaxIndex;

            if (x < this.points.Min(point => point.X))
            {
                xMaxIndex = 1;
                xMinIndex = 0;
            }

            else if (this.points.Max(point => point.X) < x)
            {
                xMaxIndex = this.points.Count - 1;
                xMinIndex = this.points.Count - 2;
            }

            else
            {
                xMaxIndex = this.points.IndexOf(point => point.X > x);
                xMinIndex = xMaxIndex - 1;
            }

            var max = this.points[xMaxIndex];
            var min = this.points[xMinIndex];

            return (x - min.X) * (max.Y - min.Y) / (max.X - min.X) + min.Y;
        }
    }
}