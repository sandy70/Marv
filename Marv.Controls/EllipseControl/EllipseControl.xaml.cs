using Marv.Common;
using System.Collections.Generic;
using System.Windows;
using System.Linq;

namespace Marv.Controls
{
    public partial class EllipseControl : MapControl.MapPanel
    {
        public static readonly DependencyProperty LocationEllipsesProperty =
        DependencyProperty.Register("LocationEllipses", typeof(IEnumerable<LocationEllipse>), typeof(EllipseControl), new PropertyMetadata(null));

        public static readonly DependencyProperty LocationsProperty =
        DependencyProperty.Register("Locations", typeof(IEnumerable<Location>), typeof(EllipseControl), new PropertyMetadata(null, OnLocationsChanged));

        private static void OnLocationsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as EllipseControl;

            if(control.Locations != null)
            {
                control.LocationEllipses = control.Locations.Select(location =>
                    {
                        return new LocationEllipse
                        {
                            Center = location,
                            Radius = location.Value * 10
                        };
                    });
            }
        }

        public EllipseControl()
        {
            InitializeComponent();
        }

        public IEnumerable<LocationEllipse> LocationEllipses
        {
            get { return (IEnumerable<LocationEllipse>)GetValue(LocationEllipsesProperty); }
            set { SetValue(LocationEllipsesProperty, value); }
        }

        public IEnumerable<Location> Locations
        {
            get { return (IEnumerable<Location>)GetValue(LocationsProperty); }
            set { SetValue(LocationsProperty, value); }
        }
    }
}