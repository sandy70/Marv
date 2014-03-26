using System.Windows.Interactivity;
using Marv.Common;
using Marv.Common.Graph;

namespace Marv.Controls.Graph
{
    internal class GraphControlVertexControlBehavior : Behavior<VertexControl>
    {
        private void AssociatedObject_CommandExecuted(object sender, Command<Vertex> command)
        {
            var vertexControl = this.AssociatedObject;
            var vertex = vertexControl.Vertex;

            if (command == VertexCommands.VertexLockCommand && vertex.IsLocked)
            {
                var graphControl = vertexControl.FindParent<GraphControl>();
                graphControl.RaiseEvidenceEntered(vertexControl.Vertex);
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.CommandExecuted += AssociatedObject_CommandExecuted;
        }
    }
}