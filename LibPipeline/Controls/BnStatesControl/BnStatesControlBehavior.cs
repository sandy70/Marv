using LibBn;
using System.Windows.Interactivity;

namespace LibPipeline
{
    internal class BnStatesControlBehavior : Behavior<BnStatesControl>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.StateDoubleClicked += AssociatedObject_StateDoubleClicked;
            this.AssociatedObject.ValueEntered += AssociatedObject_ValueEntered;
        }

        private void AssociatedObject_StateDoubleClicked(object sender, ValueEventArgs<BnState> e)
        {
            var parentGraphControl = this.AssociatedObject.FindParent<BnGraphControl>();

            parentGraphControl.RaiseEvent(new BnGraphControlEventArgs
            {
                RoutedEvent = BnGraphControl.StateDoubleClickedEvent,
                State = e.Value,
                Vertex = this.AssociatedObject.DataContext as BnVertexViewModel
            });

        }

        private void AssociatedObject_ValueEntered(object sender, ValueEventArgs<BnState> e)
        {
            var vertexViewModel = this.AssociatedObject.DataContext as BnVertexViewModel;
            vertexViewModel.IsEvidenceEntered = true;
        }
    }
}