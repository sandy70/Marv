using System.Windows.Media;

namespace Marv.Controls.Map
{
    public interface IDoubleToBrushMap
    {
        Brush Map(double d);

        double MapBack(Brush b);
    }
}