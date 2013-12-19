using Marv.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Marv.Controls
{
    public partial class EllipseControl : MapControl.MapPanel
    {
        public static readonly DependencyProperty LocationEllipsesProperty =
        DependencyProperty.Register("LocationEllipses", typeof(IEnumerable<LocationEllipse>), typeof(EllipseControl), new PropertyMetadata(null));

        public static readonly DependencyProperty LocationsProperty =
        DependencyProperty.Register("Locations", typeof(IEnumerable<Location>), typeof(EllipseControl), new PropertyMetadata(null, OnLocationsChanged));

        public static readonly DependencyProperty ScalingFuncProperty =
        DependencyProperty.Register("ScalingFunc", typeof(Func<double, double>), typeof(EllipseControl), new PropertyMetadata((Func<double, double>)(x => x), OnScalingFuncChanged));

        public static readonly DependencyProperty SelectedLocationEllipseProperty =
        DependencyProperty.Register("SelectedLocationEllipse", typeof(LocationEllipse), typeof(EllipseControl), new PropertyMetadata(null, OnSelectedLocationEllipseChanged));

        public static readonly DependencyProperty SelectedLocationProperty =
        DependencyProperty.Register("SelectedLocation", typeof(Location), typeof(EllipseControl), new PropertyMetadata(null, OnSelectedLocationChanged));

        public EllipseControl()
        {
            InitializeComponent();

            this.Loaded += EllipseControl_Loaded;
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

        public Func<double, double> ScalingFunc
        {
            get { return (Func<double, double>)GetValue(ScalingFuncProperty); }
            set { SetValue(ScalingFuncProperty, value); }
        }

        public Location SelectedLocation
        {
            get { return (Location)GetValue(SelectedLocationProperty); }
            set { SetValue(SelectedLocationProperty, value); }
        }

        public LocationEllipse SelectedLocationEllipse
        {
            get { return (LocationEllipse)GetValue(SelectedLocationEllipseProperty); }
            set { SetValue(SelectedLocationEllipseProperty, value); }
        }

        private static void OnLocationsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as EllipseControl;
            UpdateLocationEllipses(control);
        }

        private static void OnScalingFuncChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as EllipseControl;
            UpdateLocationEllipses(control);
        }

        private static void OnSelectedLocationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as EllipseControl;

            if (control.Locations is SelectableCollection<Location>)
            {
                (control.Locations as SelectableCollection<Location>).SelectedItem = control.SelectedLocation;
            }
        }

        private static void OnSelectedLocationEllipseChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as EllipseControl;

            if (control.SelectedLocationEllipse == null)
            {
                control.SelectedLocation = null;
            }
            else
            {
                control.SelectedLocation = control.SelectedLocationEllipse.Center;
            }
        }

        private static void UpdateLocationEllipses(EllipseControl control)
        {
            if (control.Locations != null && control.ScalingFunc != null)
            {
                control.LocationEllipses = control.Locations.Select(location =>
                {
                    return new LocationEllipse
                    {
                        Center = location,
                        Radius = control.ScalingFunc(location.Value)
                    };
                });
            }
        }

        private void EllipseControl_Loaded(object sender, RoutedEventArgs e)
        {
            (this.ParentMap as MapView).PreviewMouseDown += ParentMap_MouseDown;
        }

        private void ParentMap_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.SelectedLocationEllipse = null;
        }
    }
}