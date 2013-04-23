using System.Windows;

namespace LibPipeline
{
    public interface IMapLayer
    {
        double DetailZoomLevel { get; set; }

        Visibility Visibility { get; set; }

        void Clear();

        void Update(BingMap bingMap, bool Force = false);
    }
}