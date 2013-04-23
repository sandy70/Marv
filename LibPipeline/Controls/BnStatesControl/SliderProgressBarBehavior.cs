using LibBn;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace LibPipeline
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
            var statesControl = this.AssociatedObject.FindParent<BnStatesControl>();
            var vertexViewModel = statesControl.DataContext as BnVertexViewModel;
            var selectedState = this.AssociatedObject.Tag as BnState;

            vertexViewModel.SelectState(selectedState);

            statesControl.RaiseEvent(new ValueEventArgs<BnState>
            {
                RoutedEvent = BnStatesControl.StateSelectedEvent,
                Value = selectedState
            });
        }

        private void AssociatedObject_ValueEntered(object sender, ValueEventArgs<double> e)
        {
            var statesControl = this.AssociatedObject.FindParent<BnStatesControl>();
            var selectedState = this.AssociatedObject.DataContext as BnState;

            statesControl.RaiseEvent(new ValueEventArgs<BnState>
            {
                RoutedEvent = BnStatesControl.ValueEnteredEvent,
                Value = selectedState
            });
        }
    }
}