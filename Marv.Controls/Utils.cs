using System;
using System.Windows.Media;
using Marv.Common;

namespace Marv.Controls
{
    public static class Utils
    {
        public static Color DoubleToColor(double value)
        {
            var fourValue = 4 * value;
            var red = Math.Min(fourValue - 1.5, -fourValue + 4.5);
            var green = Math.Min(fourValue - 0.5, -fourValue + 3.5);
            var blue = Math.Min(fourValue + 0.5, -fourValue + 2.5);

            return Color.FromScRgb(1, (float) red.Clamp(0, 1), (float) green.Clamp(0, 1), (float) blue.Clamp(0, 1));
        }
    }
}