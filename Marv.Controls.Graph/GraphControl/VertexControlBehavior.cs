using System.Windows.Interactivity;

namespace Marv.Controls.Graph
{
    internal class VertexControlBehavior : Behavior<VertexControl>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.CommandExecuted += AssociatedObject_CommandExecuted;
        }

        private void AssociatedObject_CommandExecuted(object sender, Command<Vertex> command)
        {
            var vertexControl = this.AssociatedObject;
            var graphControl = vertexControl.FindParent<GraphControl>();

            if (command == VertexCommands.SubGraph)
            {
                graphControl.Graph.UpdateDisplayGraph(vertexControl.Vertex.HeaderOfGroup);
            }

            if (command == VertexCommands.Clear)
            {
                graphControl.Graph.ClearEvidence();
                graphControl.Graph.Run();
            }
        }
    }
}