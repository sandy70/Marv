using System.Windows.Media;

namespace Marv.Controls
{
    public interface IDoubleToBrushMap
    {
        Brush Map(double d);

        double MapBack(Brush b);
    }
}