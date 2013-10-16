using LibNetwork;
using Marv.Common;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Marv.Controls
{
    public class ToolbarButtonClickBehavior : Behavior<Button>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Click += AssociatedObject_Click;
        }

        private void AssociatedObject_Click(object sender, RoutedEventArgs e)
        {
            var vertexObserver = this.AssociatedObject.DataContext as IVertexCommand;
            // vertexObserver.Notify(this.AssociatedObject.FindParent<It
        }
    }
}