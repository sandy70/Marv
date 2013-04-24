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