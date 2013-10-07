using LibPipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Marv
{
    public class RiskValueToBrushMap : IDoubleToBrushMap
    {
        public Brush Map(double d)
        {
            if (d > 0.75)
            {
                return new SolidColorBrush(Colors.Red);
            }
            else if (d > 0.50)
            {
                return new SolidColorBrush(Colors.Orange);
            }
            else if (d > 0.25)
            {
                return new SolidColorBrush(Colors.Yellow);
            }
            else
            {
                return new SolidColorBrush(Colors.Green);
            }
        }

        public double MapBack(Brush b)
        {
            return 0;
        }
    }
}
