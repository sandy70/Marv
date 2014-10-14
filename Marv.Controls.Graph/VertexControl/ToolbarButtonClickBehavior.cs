using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Marv.Controls.Graph
{
    internal class ToolbarButtonClickBehavior : Behavior<Button>
    {
        public VertexControl VertexControl;

        protected override void OnAttached()
        {
            base.OnAttached();

            this.VertexControl = this.AssociatedObject.FindParent<VertexControl>();

            this.AssociatedObject.Click += this.AssociatedObject_Click;
        }

        private void AssociatedObject_Click(object sender, RoutedEventArgs e)
        {
            var command = this.AssociatedObject.DataContext as Command<Vertex>;
            var vertex = this.VertexControl.Vertex;

            command.Excecute(vertex);

            this.VertexControl.RaiseCommandExecuted(command);

            if (command == VertexCommands.Lock && vertex.IsLocked)
            {
                this.VertexControl.RaiseEvidenceEntered();
            }
            else if (command == VertexCommands.Clear)
            {
                this.VertexControl.RaiseEvidenceEntered();
            }
        }
    }
}