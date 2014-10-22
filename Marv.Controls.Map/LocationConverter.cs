using System.Windows;
using System.Windows.Media;
using MapControl;
using Location = Marv.Map.Location;

namespace Marv.Controls.Map
{
    public class LocationConverter
    {
        private readonly MapTransform mapTransform;
        private readonly Transform viewportTransform;

        public LocationConverter(MapView mapView)
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