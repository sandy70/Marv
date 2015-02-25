using System.Windows.Input;

namespace Marv.Controls
{
    public partial class PolylineControl
    {
        private void Ellipse_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Ellipse.CaptureMouse();

            e.Handled = true;
        }

        private void Ellipse_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.CursorLocation = this.GetNearestLocation(e.GetPosition(this));
            }
        }

        private void Ellipse_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var nearestLocation = this.GetNearestLocation(e.GetPosition(this));

            this.CursorLocation = nearestLocation;
            this.SelectedLocation = nearestLocation;

            this.Ellipse.ReleaseMouseCapture();

            e.Handled = true;
        }

        private void Ellipse_TouchDown(object sender, TouchEventArgs e)
        {
            this.Ellipse.CaptureTouch(e.TouchDevice);

            e.Handled = true;
        }

        private void Ellipse_TouchMove(object sender, TouchEventArgs e)
        {
            this.CursorLocation = this.GetNearestLocation(e.GetTouchPoint(this).Position);
        }

        private void Ellipse_TouchUp(object sender, TouchEventArgs e)
        {
            var nearestLocation = this.GetNearestLocation(e.GetTouchPoint(this).Position);

            this.CursorLocation = nearestLocation;
            this.SelectedLocation = nearestLocation;

            this.Ellipse.ReleaseTouchCapture(e.TouchDevice);

            e.Handled = true;
        }

        private void MapPolyline_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var nearestLocation = this.GetNearestLocation(e.GetPosition(this));

            this.CursorLocation = nearestLocation;
            this.SelectedLocation = nearestLocation;

            this.Ellipse.CaptureMouse();

            e.Handled = true;
        }

        private void MapPolyline_TouchDown(object sender, TouchEventArgs e)
        {
            var nearestLocation = this.GetNearestLocation(e.GetTouchPoint(this).Position);

            this.CursorLocation = nearestLocation;
            this.SelectedLocation = nearestLocation;

            this.Ellipse.CaptureTouch(e.TouchDevice);

            e.Handled = true;
        }
    }
}