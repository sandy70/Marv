using System.Windows.Interactivity;
using Marv.Common;
using Marv.Common.Graph;

namespace Marv.Controls.Graph
{
    internal class VertexControlBehavior : Behavior<VertexControl>
    {
        private void AssociatedObject_CommandExecuted(object sender, Command<Vertex> command)
        {
            var vertexControl = this.AssociatedObject;
            var graphControl = vertexControl.FindParent<GraphControl>();

            if (command == VertexCommands.SubGraph)
            {
                graphControl.DisplayGraph = graphControl.Graph.GetSubGraph(vertexControl.Vertex.HeaderOfGroup);
            }

            graphControl.RaiseVertexCommandExecuted(vertexControl.Vertex, command);
        }

        private void AssociatedObject_EvidenceEntered(object sender, Vertex e)
        {
            var vertexControl = this.AssociatedObject;
            var graphControl = vertexControl.FindParent<GraphControl>();

            graphControl.RaiseEvidenceEntered(vertexControl.Vertex);
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.CommandExecuted += AssociatedObject_CommandExecuted;
            this.AssociatedObject.EvidenceEntered += AssociatedObject_EvidenceEntered;
        }
    }
}