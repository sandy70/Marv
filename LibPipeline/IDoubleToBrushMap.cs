using System.Windows.Media;

namespace LibPipeline
{
    public interface IDoubleToBrushMap
    {
        Brush Map(double d);

        double MapBack(Brush b);
    }
}