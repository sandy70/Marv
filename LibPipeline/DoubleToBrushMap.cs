using System;
using System.Windows.Media;

namespace LibPipeline
{
    public class DoubleToBrushMap : IDoubleToBrushMap
    {
        public Brush Map(double d)
        {
            return new SolidColorBrush(Colors.Blue);
        }

        public double MapBack(Brush b)
        {
            return 0;
        }
    }
}