using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using Marv.Common;
using Marv.Common.Graph;

namespace Marv.Controls.Graph
{
    public class ToolbarButtonClickBehavior : Behavior<Button>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Click += this.AssociatedObject_Click;
        }

        private void AssociatedObject_Click(object sender, RoutedEventArgs e)
        {
            var vertexCommand = this.AssociatedObject.DataContext as IVertexCommand;
            var vertex = this.AssociatedObject.FindParent<ItemsControl>().DataContext as Vertex;
            vertexCommand.RaiseExecuted(vertex);
        }
    }
}