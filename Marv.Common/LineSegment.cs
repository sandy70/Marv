using System;
using System.Windows;

namespace Marv.Common
{
    public class LineSegment
    {
        public Point P1;
        public Point P2;

        public LineSegment() {}

        public LineSegment(Point p1, Point p2)
        {
            this.P1 = p1;
            this.P2 = p2;
        }

        public Point[] getBoundingBox()
        {
            var result = new Point[2];
            result[0] = new Point(Math.Min(P1.X, P2.X), Math.Min(P1.Y,
                P2.Y));
            result[1] = new Point(Math.Max(P1.X, P2.X), Math.Max(P1.Y,
                P2.Y));
            return result;
        }
    }
}