using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using Marv.Common;
using Marv.Common.Graph;

namespace Marv.Controls.Graph
{
    internal class VertexControlToolbarButtonClickBehavior : Behavior<Button>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Click += this.AssociatedObject_Click;
        }

        private void AssociatedObject_Click(object sender, RoutedEventArgs e)
        {
            var command = this.AssociatedObject.DataContext as Command<Vertex>;
            
            var vertexControl = this.AssociatedObject.FindParent<VertexControl>();
            
            command.Excecute(vertexControl.Vertex);

            vertexControl.RaiseCommandExecuted(command);
        }
    }
}