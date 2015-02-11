using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Threading;
using Marv.Common;

namespace Marv.Controls.Map
{
    public class SegmentedPolylineControlBehavior : Behavior<SegmentedPolylineControl>
    {
        private readonly Stack<Location> locationStack = new Stack<Location>();
        private readonly DispatcherTimer timer = new DispatcherTimer();
        private bool isDragging;
        private MapView mapView;

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Loaded += AssociatedObject_Loaded;

            this.timer.Interval = TimeSpan.FromMilliseconds(200);
            this.timer.Tick += this.timer_Tick;
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            this.AssociatedObject.MapPolyline.MouseDown += this.MapPolyline_MouseDown;
            this.AssociatedObject.MapPolyline.MouseUp += this.MapPolyline_MouseUp;
            this.AssociatedObject.MapPolyline.TouchDown += this.MapPolyline_TouchDown;
            this.AssociatedObject.MapPolyline.TouchUp += MapPolyline_TouchUp;
            this.AssociatedObject.Ellipse.MouseDown += this.Ellipse_MouseDown;
            this.AssociatedObject.Ellipse.MouseUp += this.Ellipse_MouseUp;
            this.AssociatedObject.Ellipse.MouseMove += this.Ellipse_MouseMove;
            this.AssociatedObject.Ellipse.TouchDown += this.Ellipse_TouchDown;
            this.AssociatedObject.Ellipse.TouchMove += this.Ellipse_TouchMove;
            this.AssociatedObject.Ellipse.TouchUp += this.Ellipse_TouchUp;

            this.mapView = this.AssociatedObject.GetParent<MapView>();

            if (this.mapView != null)
            {
                // this.mapView.ViewportMoved += this.mapView_ViewportMoved;
                this.mapView.ZoomLevelChanged += this.mapView_ZoomLevelChanged;
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

            // this.OnDown();
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

            // this.OnDown();
        }

        private void MapPolyline_TouchUp(object sender, TouchEventArgs e)
        {
            this.OnUp();
            e.Handled = true;
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
            var map = this.AssociatedObject.GetParent<MapControl.Map>();
            var mLocation = map.ViewportPointToLocation(position);
            var location = new Location
            {
                Latitude = mLocation.Latitude,
                Longitude = mLocation.Longitude
            };
            var nearestLocation = this.AssociatedObject.Locations.GetLocationNearestTo(location);

            this.AssociatedObject.CursorLocation = nearestLocation;

            this.locationStack.Push(nearestLocation);

            if (!this.timer.IsEnabled)
            {
                this.timer.Start();
            }
        }

        private void mapView_ViewportMoved(object sender, Location location)
        {
            this.AssociatedObject.UpdateSimplifiedPolylineParts();
        }

        private async void mapView_ZoomLevelChanged(object sender, int zoom)
        {
            await this.AssociatedObject.UpdateSimplifiedPolylinePartsAsync();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (this.locationStack.Count > 0)
            {
                var location = this.locationStack.Pop();

                this.AssociatedObject.SelectedLocation = location;
                this.AssociatedObject.RaiseSelectionChanged(location);

                this.locationStack.Clear();
                this.timer.Stop();
            }
        }
    }
}