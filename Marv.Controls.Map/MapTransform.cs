using System.Windows;
using System.Windows.Media;
using Marv.Common;

namespace Marv.Controls.Map
{
    public class MapTransform
    {
        private readonly MapControl.MapTransform mapTransform;
        private readonly Transform viewportTransform;

        public MapTransform(MapView mapView)
        {
            this.mapTransform = mapView.MapTransform;
            this.viewportTransform = mapView.ViewportTransform;
        }

        public Location ToLocation(Point point)
        {
            return this.mapTransform.Transform(this.viewportTransform.Inverse.Transform(point));
        }

        public Point ToPoint(Location location)
        {
            return this.viewportTransform.Transform(this.mapTransform.Transform(location));
        }
    }
}