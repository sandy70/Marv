using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using WpfMap;

namespace WpfMapDemo
{
    internal class MainWindowBehavior : Behavior<MainWindow>
    {
        private bool IsDragging = false;

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            //this.AssociatedObject.MapPolyline.MouseDown += MapPolyline_MouseDown;
            //this.AssociatedObject.MapPolyline.MouseUp += MapPolyline_MouseUp;
            //this.AssociatedObject.MapPolyline.TouchDown += MapPolyline_TouchDown;
            //this.AssociatedObject.Ellipse.MouseDown += Ellipse_MouseDown;
            //this.AssociatedObject.Ellipse.MouseUp += Ellipse_MouseUp;
            //this.AssociatedObject.Ellipse.MouseMove += Ellipse_MouseMove;
            //this.AssociatedObject.Ellipse.TouchDown += Ellipse_TouchDown;
            //this.AssociatedObject.Ellipse.TouchMove += Ellipse_TouchMove;
            //this.AssociatedObject.Ellipse.TouchUp += Ellipse_TouchUp;
        }

        private void Ellipse_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.OnDown();
            e.Handled = true;
        }

        private void Ellipse_MouseMove(object sender, MouseEventArgs e)
        {
            var position = e.GetPosition(this.AssociatedObject);
            var location = this.AssociatedObject.Map.ViewportPointToLocation(position);

            if (this.IsDragging)
            {
                this.AssociatedObject.SelectedLocation = this.AssociatedObject.Locations.NearestTo(location);
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
            var position = e.GetTouchPoint(this.AssociatedObject).Position;
            var location = this.AssociatedObject.Map.ViewportPointToLocation(position);

            if (this.IsDragging)
            {
                this.AssociatedObject.SelectedLocation = this.AssociatedObject.Locations.NearestTo(location);
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
            this.SetSelectedLocation(position);
            this.OnDown();
            e.Handled = true;
        }

        private void MapPolyline_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.OnUp();
            e.Handled = true;
        }

        private void MapPolyline_TouchDown(object sender, TouchEventArgs e)
        {
            var position = e.GetTouchPoint(this.AssociatedObject).Position;
            this.SetSelectedLocation(position);
            e.Handled = true;
        }

        private void OnDown()
        {
            //this.AssociatedObject.Ellipse.CaptureMouse();
            this.IsDragging = true;
        }

        private void OnUp()
        {
            //this.AssociatedObject.Ellipse.ReleaseMouseCapture();
            this.IsDragging = false;
        }

        private void SetSelectedLocation(Point position)
        {
            var location = this.AssociatedObject.Map.ViewportPointToLocation(position);
            this.AssociatedObject.SelectedLocation = this.AssociatedObject.Locations.NearestTo(location);
        }
    }
}