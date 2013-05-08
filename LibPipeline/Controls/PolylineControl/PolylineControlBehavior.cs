﻿using MapControl;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Threading;

namespace LibPipeline
{
    internal class PolylineControlBehavior : Behavior<PolylineControl>
    {
        private bool isDragging = false;
        private Stack<Location> locationStack = new Stack<Location>();
        private DispatcherTimer timer = new DispatcherTimer();

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Loaded += AssociatedObject_Loaded;

            timer.Interval = TimeSpan.FromMilliseconds(200);
            timer.Tick += (o, e) =>
                {
                    if (this.locationStack.Count > 0)
                    {
                        this.AssociatedObject.SelectedLocation = this.locationStack.Pop();
                        this.locationStack.Clear();
                        timer.Stop();
                    }
                };
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            this.AssociatedObject.MapPolyline.MouseDown += MapPolyline_MouseDown;
            this.AssociatedObject.MapPolyline.MouseUp += MapPolyline_MouseUp;
            this.AssociatedObject.MapPolyline.TouchDown += MapPolyline_TouchDown;
            this.AssociatedObject.Ellipse.MouseDown += Ellipse_MouseDown;
            this.AssociatedObject.Ellipse.MouseUp += Ellipse_MouseUp;
            this.AssociatedObject.Ellipse.MouseMove += Ellipse_MouseMove;
            this.AssociatedObject.Ellipse.TouchDown += Ellipse_TouchDown;
            this.AssociatedObject.Ellipse.TouchMove += Ellipse_TouchMove;
            this.AssociatedObject.Ellipse.TouchUp += Ellipse_TouchUp;
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
            this.OnDown();
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
            this.OnDown();
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
            var map = this.AssociatedObject.FindParent<Map>();
            var location = map.ViewportPointToLocation(position);
            var nearestLocation = this.AssociatedObject.Locations.NearestTo(location);

            this.AssociatedObject.CursorLocation = nearestLocation;
            this.locationStack.Push(nearestLocation);

            if (!timer.IsEnabled)
            {
                timer.Start();
            }
        }
    }
}