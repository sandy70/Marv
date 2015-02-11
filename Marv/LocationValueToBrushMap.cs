using System.Windows.Media;
using Marv.Controls.Map;

namespace Marv
{
    public class LocationValueToBrushMap : IDoubleToBrushMap
    {
        public Brush Map(double d)
        {
            if (d > 0.8)
            {
                return new SolidColorBrush(Colors.Red);
            }

            if (d > 0.2)
            {
                return new SolidColorBrush(Colors.Yellow);
            }

            return new SolidColorBrush(Colors.Green);
        }

        public double MapBack(Brush b)
        {
            return 0.0;
        }
    }
}