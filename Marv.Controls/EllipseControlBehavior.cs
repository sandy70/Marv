using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Marv.Controls
{
    public class EllipseControlBehavior : Behavior<EllipseControl>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Loaded += this.AssociatedObject_Loaded;
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            var control = this.AssociatedObject;
            var mapView = control.ParentMap as MapView;

            mapView.PreviewMouseDown += this.mapView_PreviewMouseDown;
        }

        private void mapView_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var control = this.AssociatedObject;
            control.SelectedLocationEllipse = null;
        }
    }
}