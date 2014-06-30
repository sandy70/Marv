using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Marv.Common;
using Marv.Common.Map;

namespace Marv.Controls.Map
{
    public partial class EllipseControl
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
        }

        public IEnumerable<LocationEllipse> LocationEllipses
        {
            get { return (IEnumerable<LocationEllipse>)this.GetValue(LocationEllipsesProperty); }
            set { this.SetValue(LocationEllipsesProperty, value); }
        }

        public IEnumerable<Location> Locations
        {
            get { return (IEnumerable<Location>)this.GetValue(LocationsProperty); }
            set { this.SetValue(LocationsProperty, value); }
        }

        public Func<double, double> ScalingFunc
        {
            get { return (Func<double, double>)this.GetValue(ScalingFuncProperty); }
            set { this.SetValue(ScalingFuncProperty, value); }
        }

        public Location SelectedLocation
        {
            get { return (Location)this.GetValue(SelectedLocationProperty); }
            set { this.SetValue(SelectedLocationProperty, value); }
        }

        public LocationEllipse SelectedLocationEllipse
        {
            get { return (LocationEllipse)this.GetValue(SelectedLocationEllipseProperty); }
            set { this.SetValue(SelectedLocationEllipseProperty, value); }
        }

        private static void OnLocationsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as EllipseControl;
            UpdateLocationEllipses(control);
        }

        private static void OnScalingFuncChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as EllipseControl;
            UpdateLocationEllipsesRadius(control);
        }

        private static void OnSelectedLocationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as EllipseControl;

            if (control != null && control.Locations is SelectableCollection<Location>)
            {
                (control.Locations as SelectableCollection<Location>).SelectedItem = control.SelectedLocation;
            }
        }

        private static void OnSelectedLocationEllipseChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as EllipseControl;

            if (control != null)
            {
                control.SelectedLocation = control.SelectedLocationEllipse == null ? null : control.SelectedLocationEllipse.Center;
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

        private static void UpdateLocationEllipsesRadius(EllipseControl control)
        {
            if (control.LocationEllipses == null)
            {
                UpdateLocationEllipses(control);
            }
            else
            {
                if (control.ScalingFunc == null)
                {
                    foreach (var locationEllipse in control.LocationEllipses)
                    {
                        locationEllipse.Radius = locationEllipse.Center.Value;
                    }
                }
                else
                {
                    foreach (var locationEllipse in control.LocationEllipses)
                    {
                        locationEllipse.Radius = control.ScalingFunc(locationEllipse.Center.Value);
                    }
                }
            }
        }
    }
}