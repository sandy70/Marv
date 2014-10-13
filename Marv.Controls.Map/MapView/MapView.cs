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
        public static readonly DependencyProperty BoundsProperty =
            DependencyProperty.Register("Bounds", typeof (LocationRect), typeof (MapView), new PropertyMetadata(null));

        public static readonly DependencyProperty StartExtentProperty =
            DependencyProperty.Register("StartExtent", typeof (LocationRect), typeof (MapView), new PropertyMetadata(null));

        public static readonly RoutedEvent ViewportMovedEvent =
            EventManager.RegisterRoutedEvent("ViewportMoved", RoutingStrategy.Bubble, typeof (RoutedEventHandler<ValueEventArgs<Location>>), typeof (MapView));

        public static readonly RoutedEvent ZoomLevelChangedEvent =
            EventManager.RegisterRoutedEvent("ZoomLevelChanged", RoutingStrategy.Bubble, typeof (RoutedEventHandler<ValueEventArgs<int>>), typeof (MapView));

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

        public LocationRect StartExtent
        {
            get { return (LocationRect) this.GetValue(StartExtentProperty); }
            set { this.SetValue(StartExtentProperty, value); }
        }

        public MapView()
        {
            TileImageLoader.Cache = new ImageFileCache(TileImageLoader.DefaultCacheName, "./");
            var behaviors = Interaction.GetBehaviors(this);
            behaviors.Add(new MapViewBehavior());
        }

        protected override void OnManipulationInertiaStarting(ManipulationInertiaStartingEventArgs e)
        {
            base.OnManipulationInertiaStarting(e);
            e.TranslationBehavior.DesiredDeceleration = 0.001;
        }

        public event RoutedEventHandler<ValueEventArgs<Location>> ViewportMoved
        {
            add { this.AddHandler(ViewportMovedEvent, value); }
            remove { this.RemoveHandler(ViewportMovedEvent, value); }
        }

        public event RoutedEventHandler<ValueEventArgs<int>> ZoomLevelChanged
        {
            add { this.AddHandler(ZoomLevelChangedEvent, value); }
            remove { this.RemoveHandler(ZoomLevelChangedEvent, value); }
        }
    }
}