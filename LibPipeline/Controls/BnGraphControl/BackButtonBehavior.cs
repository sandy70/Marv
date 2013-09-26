using Marv.Common;
using System.Windows;
using System.Windows.Interactivity;
using Telerik.Windows.Controls;

namespace LibPipeline
{
    internal class BackButtonBehavior : Behavior<RadButton>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Click += AssociatedObject_Click;
        }

        private void AssociatedObject_Click(object sender, RoutedEventArgs e)
        {
            var graphControl = this.AssociatedObject.FindParent<BnGraphControl>();
            var vertexViewModel = this.AssociatedObject.DataContext as BnVertexViewModel;

            graphControl.RaiseEvent(new ValueEventArgs<BnVertexViewModel>
            {
                RoutedEvent = BnGraphControl.BackButtonClickedEvent,
                Value = vertexViewModel
            });
        }
    }
}