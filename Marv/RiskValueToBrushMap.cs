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
            if (d > 0.9986)
            {
                return new SolidColorBrush(Colors.Green);
            }
            else if (d > 0.9983)
            {
                return new SolidColorBrush(Colors.Yellow);
            }
            else
            {
                return new SolidColorBrush(Colors.Red);
            }
        }

        public double MapBack(Brush b)
        {
            return 0;
        }
    }
}
