using MapControl;
using SharpKml.Dom;
using System.Windows;

namespace LibPipeline
{
    public partial class GroundOverlayControl : MapPanel
    {
        public static readonly DependencyProperty GroundOverlayProperty =
        DependencyProperty.Register("GroundOverlay", typeof(GroundOverlay), typeof(GroundOverlayControl), new PropertyMetadata(null, ChangedGroundOverlay));

        public GroundOverlayControl()
        {
            InitializeComponent();
        }

        public GroundOverlay GroundOverlay
        {
            get { return (GroundOverlay)GetValue(GroundOverlayProperty); }
            set { SetValue(GroundOverlayProperty, value); }
        }

        private static void ChangedGroundOverlay(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var groundOverlayControl = d as GroundOverlayControl;
            var opacity = (double)groundOverlayControl.GroundOverlay.Color.Value.Alpha / 255.0;
            groundOverlayControl.Opacity = opacity;
        }
    }
}