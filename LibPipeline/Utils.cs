using MapControl;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LibPipeline
{
    public static class Utils
    {
        public static double[,] MatrixStringToDouble(string[,] str)
        {
            int nRows = str.GetLength(0);
            int nCols = str.GetLength(1);

            double[,] dbl = new double[nRows, nCols];

            for (int r = 0; r < nRows; r++)
            {
                for (int c = 0; c < nCols; c++)
                {
                    Double.TryParse(str[r, c], out dbl[r, c]);
                }
            }

            return dbl;
        }

        public static double Distance(ILocation l1, ILocation l2)
        {
            double distance = 0;

            double dLat = (l2.Latitude - l1.Latitude) / 180 * Math.PI;
            double dLong = (l2.Longitude - l1.Longitude) / 180 * Math.PI;

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
                        + Math.Cos(l1.Latitude / 180 * Math.PI) * Math.Cos(l2.Latitude / 180 * Math.PI) * Math.Sin(dLong / 2) * Math.Sin(dLong / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            //Calculate radius of earth
            // For this you can assume any of the two points.
            double radiusE = 6378135; // Equatorial radius, in metres
            double radiusP = 6356750; // Polar Radius

            //Numerator part of function
            double nr = Math.Pow(radiusE * radiusP * Math.Cos(l1.Latitude / 180 * Math.PI), 2);

            //Denominator part of the function
            double dr = Math.Pow(radiusE * Math.Cos(l1.Latitude / 180 * Math.PI), 2)
                            + Math.Pow(radiusP * Math.Sin(l1.Latitude / 180 * Math.PI), 2);

            double radius = Math.Sqrt(nr / dr);

            //Calaculate distance in metres.
            distance = radius * c;
            return distance;
        }
    }
}