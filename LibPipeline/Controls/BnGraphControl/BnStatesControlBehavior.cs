using LibBn;
using System.Windows.Interactivity;

namespace LibPipeline
{
    internal class BnStatesControlBehavior : Behavior<BnStatesControl>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.StateSelected += AssociatedObject_StateSelected;
            this.AssociatedObject.ValueEntered += AssociatedObject_ValueEntered;
        }

        private void AssociatedObject_StateSelected(object sender, ValueEventArgs<State> e)
        {
            var parentGraphControl = this.AssociatedObject.FindParent<BnGraphControl>();
            var vertexViewModel = this.AssociatedObject.DataContext as BnVertexViewModel;
            vertexViewModel.IsEvidenceEntered = true;

            parentGraphControl.RaiseEvent(new ValueEventArgs<BnVertexViewModel>
            {
                RoutedEvent = BnGraphControl.NewEvidenceAvailableEvent,
                Value = vertexViewModel
            });
        }

        private void AssociatedObject_ValueEntered(object sender, ValueEventArgs<State> e)
        {
            var vertexViewModel = this.AssociatedObject.DataContext as BnVertexViewModel;
            vertexViewModel.IsEvidenceEntered = true;
        }
    }
}