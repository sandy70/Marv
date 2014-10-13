﻿using System.Windows.Interactivity;

namespace Marv.Controls.Graph
{
    internal class VertexControlBehavior : Behavior<VertexControl>
    {
        public GraphControl GraphControl;
        public VertexControl VertexControl;

        protected override void OnAttached()
        {
            base.OnAttached();

            this.VertexControl = this.AssociatedObject;
            this.GraphControl = this.VertexControl.FindParent<GraphControl>();

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
            else if (command == VertexCommands.Expand)
            {
                this.GraphControl.AutoLayout();
            }
        }
    }
}