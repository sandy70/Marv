using Marv.Common;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Marv.Controls
{
    internal class SliderProgressBarBehavior : Behavior<SliderProgressBar>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.MouseDoubleClick += AssociatedObject_MouseDoubleClick;
            this.AssociatedObject.ValueEntered += AssociatedObject_ValueEntered;
        }

        private void AssociatedObject_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var statesControl = this.AssociatedObject.FindParent<StatesControl>();

            statesControl.RaiseEvent(new ValueEventArgs<State>
            {
                RoutedEvent = StatesControl.StateDoubleClickedEvent,
                Value = this.AssociatedObject.DataContext as State
            });
        }

        private void AssociatedObject_ValueEntered(object sender, ValueEventArgs<double> e)
        {
            var statesControl = this.AssociatedObject.FindParent<StatesControl>();
            var selectedState = this.AssociatedObject.DataContext as State;

            statesControl.RaiseEvent(new ValueEventArgs<State>
            {
                RoutedEvent = StatesControl.ValueEnteredEvent,
                Value = selectedState
            });
        }
    }
}