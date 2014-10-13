using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using Caching;
using MapControl;
using Marv.Map;
using Location = Marv.Map.Location;

namespace Marv.Controls.Map
{
    public class MapView : MapControl.Map
    {
        public static readonly DependencyProperty StartBoundsProperty =
            DependencyProperty.Register("StartBounds", typeof (LocationRect), typeof (MapView), new PropertyMetadata(null));

        public LocationRect Bounds
        {
            get
            {
                return new LocationRect
                {
                    NorthWest = this.ViewportPointToLocation(new Point
                    {
                        X = 0,
                        Y = 0
                    }),
                    SouthEast = this.ViewportPointToLocation(new Point
                    {
                        X = this.RenderSize.Width,
                        Y = this.RenderSize.Height
                    })
                };
            }

            set { this.ZoomToBounds(value.SouthWest.ToMapControlLocation(), value.NorthEast.ToMapControlLocation()); }
        }

        public LocationRect StartBounds
        {
            get { return (LocationRect) this.GetValue(StartBoundsProperty); }
            set { this.SetValue(StartBoundsProperty, value); }
        }

        public MapView()
        {
            TileImageLoader.Cache = new ImageFileCache(TileImageLoader.DefaultCacheName, "./");
            var behaviors = Interaction.GetBehaviors(this);
            behaviors.Add(new MapViewBehavior());
        }

        public void RaiseViewportMoved(Location location)
        {
            if (this.ViewportMoved != null)
            {
                this.ViewportMoved(this, location);
            }
        }

        public void RaiseZoomLevelChanged(int zoom)
        {
            if (this.ZoomLevelChanged != null)
            {
                this.ZoomLevelChanged(this, zoom);
            }
        }

        protected override void OnManipulationInertiaStarting(ManipulationInertiaStartingEventArgs e)
        {
            base.OnManipulationInertiaStarting(e);
            e.TranslationBehavior.DesiredDeceleration = 0.001;
        }

        public event EventHandler<Location> ViewportMoved;

        public event EventHandler<int> ZoomLevelChanged;
    }
}