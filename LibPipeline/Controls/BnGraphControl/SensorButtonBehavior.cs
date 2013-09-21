using Marv.Common;
using System.Windows.Controls.Primitives;
using System.Windows.Interactivity;

namespace LibPipeline
{
    internal class SensorButtonBehavior : Behavior<ToggleButton>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.Checked += (o, e) =>
                {
                    var parentGraphControl = this.AssociatedObject.FindParent<BnGraphControl>();

                    parentGraphControl.RaiseEvent(new ValueEventArgs<BnVertexViewModel>
                    {
                        RoutedEvent = BnGraphControl.SensorButtonCheckedEvent,
                        Value = this.AssociatedObject.DataContext as BnVertexViewModel
                    });
                };

            this.AssociatedObject.Unchecked += (o, e) =>
                {
                    var parentGraphControl = this.AssociatedObject.FindParent<BnGraphControl>();

                    parentGraphControl.RaiseEvent(new ValueEventArgs<BnVertexViewModel>
                    {
                        RoutedEvent = BnGraphControl.SensorButtonUncheckedEvent,
                        Value = this.AssociatedObject.DataContext as BnVertexViewModel
                    });
                };
        }
    }
}