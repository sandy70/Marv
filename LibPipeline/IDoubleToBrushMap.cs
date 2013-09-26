using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LibPipeline
{
    public interface IDoubleToBrushMap
    {
        Brush Map(double d);
        double MapBack(Brush b);
    }
}
