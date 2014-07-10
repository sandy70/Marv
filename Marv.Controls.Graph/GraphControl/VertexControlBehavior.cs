using System.Windows.Interactivity;
using Marv.Common;
using Marv.Common.Graph;

namespace Marv.Controls.Graph
{
    internal class VertexControlBehavior : Behavior<VertexControl>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.CommandExecuted += AssociatedObject_CommandExecuted;

            this.AssociatedObject.EvidenceChanged += (o, e) => this.RaiseEvidenceEntered();
            this.AssociatedObject.EvidenceEntered += (o, e) => this.RaiseEvidenceEntered();
        }

        private void AssociatedObject_CommandExecuted(object sender, Command<Vertex> command)
        {
            var vertexControl = this.AssociatedObject;
            var graphControl = vertexControl.FindParent<GraphControl>();

            if (command == VertexCommands.SubGraph)
            {
                graphControl.Graph.UpdateDisplayGraph(vertexControl.Vertex.HeaderOfGroup);
            }

            graphControl.RaiseVertexCommandExecuted(vertexControl.Vertex, command);
        }

        private void RaiseEvidenceEntered()
        {
            var vertexControl = this.AssociatedObject;
            var graphControl = vertexControl.FindParent<GraphControl>();

            graphControl.RaiseEvidenceEntered(vertexControl.Vertex);
        }
    }
}