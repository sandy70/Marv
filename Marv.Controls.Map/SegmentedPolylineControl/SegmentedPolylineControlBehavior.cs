using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Threading;
using Marv;
using Marv.Map;

namespace Marv.Controls.Map
{
    public class SegmentedPolylineControlBehavior : Behavior<SegmentedPolylineControl>
    {
        private bool isDragging;
        private Stack<Location> locationStack = new Stack<Location>();
        private DispatcherTimer timer = new DispatcherTimer();

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Loaded += AssociatedObject_Loaded;

            this.timer.Interval = TimeSpan.FromMilliseconds(200);
            this.timer.Tick += this.timer_Tick;
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            this.AssociatedObject.TouchDown += AssociatedObject_TouchDown;

            this.AssociatedObject.MapPolyline.MouseDown += this.MapPolyline_MouseDown;
            this.AssociatedObject.MapPolyline.MouseUp += this.MapPolyline_MouseUp;
            this.AssociatedObject.MapPolyline.TouchDown += this.MapPolyline_TouchDown;

            this.AssociatedObject.Ellipse.MouseDown += this.Ellipse_MouseDown;
            this.AssociatedObject.Ellipse.MouseUp += this.Ellipse_MouseUp;
            this.AssociatedObject.Ellipse.MouseMove += this.Ellipse_MouseMove;
            this.AssociatedObject.Ellipse.TouchDown += this.Ellipse_TouchDown;
            this.AssociatedObject.Ellipse.TouchMove += this.Ellipse_TouchMove;
            this.AssociatedObject.Ellipse.TouchUp += this.Ellipse_TouchUp;

            var mapView = Marv.Extensions.FindParent<MapView>(this.AssociatedObject);

            if (mapView != null)
            {
                mapView.ViewportMoved += this.mapView_ViewportMoved;
                mapView.ZoomLevelChanged += this.mapView_ZoomLevelChanged;
            }
        }

        private void AssociatedObject_TouchDown(object sender, TouchEventArgs e)
        {
            var mapItemsControl = Marv.Extensions.FindParent<MapControl.MapItemsControl>(this.AssociatedObject);

            if (mapItemsControl != null)
            {
                mapItemsControl.SelectedItem = this.AssociatedObject.Locations;
            }
        }

        private void Ellipse_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.OnDown();
            e.Handled = true;
        }

        private void Ellipse_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.isDragging)
            {
                var position = e.GetPosition(this.AssociatedObject);
                this.SelectLocation(position);
            }
        }

        private void Ellipse_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.OnUp();
            e.Handled = true;
        }

        private void Ellipse_TouchDown(object sender, TouchEventArgs e)
        {
            this.OnDown();
            e.Handled = true;
        }

        private void Ellipse_TouchMove(object sender, TouchEventArgs e)
        {
            if (this.isDragging)
            {
                var position = e.GetTouchPoint(this.AssociatedObject).Position;
                this.SelectLocation(position);
            }
        }

        private void Ellipse_TouchUp(object sender, TouchEventArgs e)
        {
            this.OnUp();
            e.Handled = true;
        }

        private void MapPolyline_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var position = e.GetPosition(this.AssociatedObject);
            this.SelectLocation(position);
        }

        private void MapPolyline_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.OnUp();
            e.Handled = true;
        }

        private void MapPolyline_TouchDown(object sender, TouchEventArgs e)
        {
            var position = e.GetTouchPoint(this.AssociatedObject).Position;
            this.SelectLocation(position);
        }

        private void mapView_ViewportMoved(object sender, ValueEventArgs<Location> e)
        {
            // this.AssociatedObject.UpdateSimplifiedPolylineParts();
        }

        private void mapView_ZoomLevelChanged(object sender, ValueEventArgs<int> e)
        {
            this.AssociatedObject.UpdateSimplifiedPolylineParts();
        }

        private void OnDown()
        {
            this.AssociatedObject.Ellipse.CaptureMouse();
            this.isDragging = true;
        }

        private void OnUp()
        {
            this.AssociatedObject.Ellipse.ReleaseMouseCapture();
            this.isDragging = false;
        }

        private void SelectLocation(Point position)
        {
            var map = Marv.Extensions.FindParent<MapControl.Map>(this.AssociatedObject);
            var mLocation = map.ViewportPointToLocation(position);
            var location = new Location { Latitude = mLocation.Latitude, Longitude = mLocation.Longitude };
            var nearestLocation = Marv.Map.Extensions.NearestTo(this.AssociatedObject.Locations, location);

            this.AssociatedObject.CursorLocation = nearestLocation;

            this.locationStack.Push(nearestLocation);

            if (!this.timer.IsEnabled)
            {
                this.timer.Start();
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (this.locationStack.Count > 0)
            {
                this.AssociatedObject.Locations.Select(this.locationStack.Pop());
                this.locationStack.Clear();
                this.timer.Stop();
            }
        }
    }
}