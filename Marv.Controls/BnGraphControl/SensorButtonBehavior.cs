using LibNetwork;
using Marv.Common;
using System.Windows.Controls.Primitives;
using System.Windows.Interactivity;

namespace Marv.Controls
{
    internal class SensorButtonBehavior : Behavior<ToggleButton>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.Checked += (o, e) =>
                {
                    var parentGraphControl = this.AssociatedObject.FindParent<BnGraphControl>();

                    parentGraphControl.RaiseEvent(new ValueEventArgs<VertexViewModel>
                    {
                        RoutedEvent = BnGraphControl.SensorButtonCheckedEvent,
                        Value = this.AssociatedObject.DataContext as VertexViewModel
                    });
                };

            this.AssociatedObject.Unchecked += (o, e) =>
                {
                    var parentGraphControl = this.AssociatedObject.FindParent<BnGraphControl>();

                    parentGraphControl.RaiseEvent(new ValueEventArgs<VertexViewModel>
                    {
                        RoutedEvent = BnGraphControl.SensorButtonUncheckedEvent,
                        Value = this.AssociatedObject.DataContext as VertexViewModel
                    });
                };
        }
    }
}