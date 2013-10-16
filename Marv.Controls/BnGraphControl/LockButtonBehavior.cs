using LibNetwork;
using Marv.Common;
using System.Windows;
using System.Windows.Interactivity;
using Telerik.Windows.Controls;

namespace Marv.Controls
{
    internal class LockButtonBehavior : Behavior<RadButton>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Click += AssociatedObject_Click;
        }

        private void AssociatedObject_Click(object sender, RoutedEventArgs e)
        {
            var vertexViewModel = this.AssociatedObject.DataContext as VertexViewModel;
            vertexViewModel.IsLocked = !vertexViewModel.IsLocked;

            if (vertexViewModel.IsLocked && vertexViewModel.IsEvidenceEntered)
            {
                var parentGraphControl = this.AssociatedObject.FindParent<BnGraphControl>();

                parentGraphControl.RaiseEvent(new ValueEventArgs<VertexViewModel>
                {
                    RoutedEvent = BnGraphControl.NewEvidenceAvailableEvent,
                    Value = vertexViewModel
                });
            }
        }
    }
}