using Microsoft.Maps.MapControl.WPF;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LibPipeline
{
    public static class Utils
    {
        public static void AddColumnDefinitions(Grid ParentGrid, int nColumns)
        {
            for (int i = 0; i < nColumns; i++)
            {
                ParentGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }
        }

        public static void AddRowDefinitions(Grid ParentGrid, int nRows)
        {
            for (int i = 0; i < nRows; i++)
            {
                ParentGrid.RowDefinitions.Add(new RowDefinition());
            }
        }

        /// <summary>
        /// Returns the distance between two geographic locations using the Haversine formula.
        /// </summary>
        /// <param name="l1">location 1</param>
        /// <param name="l2">location 2</param>
        /// <returns>the distance between the two geographic locations</returns>
        public static double DistanceBetweenLocation(ILocation l1, Location l2)
        {
            double distance = 0;

            if (!l1.HasValue()) return Double.MaxValue;

            if (l1.Latitude.Value == l2.Latitude && l1.Longitude.Value == l2.Longitude) return 0;

            double dLat = (l2.Latitude - l1.Latitude.Value) / 180 * Math.PI;
            double dLong = (l2.Longitude - l1.Longitude.Value) / 180 * Math.PI;

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
                        + Math.Cos(l1.Latitude.Value / 180 * Math.PI) * Math.Cos(l2.Latitude / 180 * Math.PI) * Math.Sin(dLong / 2) * Math.Sin(dLong / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            //Calculate radius of earth
            // For this you can assume any of the two points.
            double radiusE = 6378135; // Equatorial radius, in metres
            double radiusP = 6356750; // Polar Radius

            //Numerator part of function
            double nr = Math.Pow(radiusE * radiusP * Math.Cos(l1.Latitude.Value / 180 * Math.PI), 2);

            //Denominator part of the function
            double dr = Math.Pow(radiusE * Math.Cos(l1.Latitude.Value / 180 * Math.PI), 2)
                            + Math.Pow(radiusP * Math.Sin(l1.Latitude.Value / 180 * Math.PI), 2);

            double radius = Math.Sqrt(nr / dr);

            //Calaculate distance in metres.
            distance = radius * c;
            return distance;
        }

        public static double? DistanceBetweenLocations(ILocation l1, ILocation l2)
        {
            if (l1.HasValue() && l2.HasValue())
            {
                return Utils.DistanceBetweenLocation(l1, l2.AsLocation());
            }
            else
            {
                return null;
            }
        }

        public static Color DoubleToColor(double value)
        {
            double fourValue = 4 * value;
            double red = Math.Min(fourValue - 1.5, -fourValue + 4.5);
            double green = Math.Min(fourValue - 0.5, -fourValue + 3.5);
            double blue = Math.Min(fourValue + 0.5, -fourValue + 2.5);

            return Color.FromScRgb(1, (float)red.Clamp(0, 1), (float)green.Clamp(0, 1), (float)blue.Clamp(0, 1));
        }

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

        public static LocationCollection MatrixStringToLocations(string[,] str)
        {
            double[,] dbl = Utils.MatrixStringToDouble(str);

            int nRows = dbl.GetLength(0);

            LocationCollection locations = new LocationCollection();

            for (int r = 0; r < nRows; r++)
            {
                locations.Add(new Location(dbl[r, 0], dbl[r, 1]));
            }

            return locations;
        }

        public static void ToggleVisibility(UIElement element)
        {
            if (element.Visibility == Visibility.Hidden) element.Visibility = Visibility.Visible;
            else element.Visibility = Visibility.Hidden;
        }

        public static Visibility ToggleVisibilityCollapsed(Visibility visibility)
        {
            if (visibility == Visibility.Collapsed) return Visibility.Visible;
            else return Visibility.Collapsed;
        }

        public static Visibility ToggleVisibilityHidden(Visibility visibility)
        {
            if (visibility == Visibility.Hidden) return Visibility.Visible;
            else return Visibility.Hidden;
        }

        public static double? TryParseDouble(string str)
        {
            double dbl;

            if (Double.TryParse(str, out dbl))
            {
                return dbl;
            }
            else
            {
                return null;
            }
        }
    }
}