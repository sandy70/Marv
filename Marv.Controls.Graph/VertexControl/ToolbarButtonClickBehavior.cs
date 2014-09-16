using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using Marv;
using Marv.Graph;

namespace Marv.Controls.Graph
{
    internal class ToolbarButtonClickBehavior : Behavior<Button>
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
            var vertex = vertexControl.Vertex;

            command.Excecute(vertex);

            vertexControl.RaiseCommandExecuted(command);

            if (command == VertexCommands.Lock && vertex.IsLocked)
            {
                vertexControl.RaiseEvidenceEntered();
            }

            if (command == VertexCommands.Clear)
            {
                vertexControl.RaiseEvidenceEntered();
            }
        }
    }
}