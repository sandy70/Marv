using Marv.Common;
using System.Windows.Interactivity;
using Telerik.Windows.Controls;

namespace LibPipeline
{
    internal class RetractButtonBehavior : Behavior<RadButton>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Click += AssociatedObject_Click;
        }

        private void AssociatedObject_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var parentGraphControl = this.AssociatedObject.FindParent<BnGraphControl>();
            parentGraphControl.RaiseEvent(new ValueEventArgs<BnVertexViewModel>
            {
                RoutedEvent = BnGraphControl.RetractButtonClickedEvent,
                Value = this.AssociatedObject.DataContext as BnVertexViewModel
            });
        }
    }
}