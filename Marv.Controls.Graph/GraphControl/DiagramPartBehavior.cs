using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Threading;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Diagrams;
using Telerik.Windows.Diagrams.Core;

namespace Marv.Controls.Graph
{
    internal class DiagramPartBehavior : Behavior<RadDiagram>
    {
        private RadDiagram diagram;
        private Vertex newVertex;
        private Vertex oldVertex;

        protected override void OnAttached()
        {
            base.OnAttached();

            diagram = this.AssociatedObject;

            this.diagram.CommandExecuted += this.AssociatedObject_CommandExecuted;
            this.diagram.ConnectionManipulationCompleted += this.AssociatedObject_ConnectionManipulationCompleted;
            this.diagram.ConnectionManipulationStarted += this.AssociatedObject_ConnectionManipulationStarted;
            this.diagram.ShapeClicked += this.AssociatedObject_ShapeClicked;
        }

        private void AssociatedObject_CommandExecuted(object sender, CommandRoutedEventArgs e)
        {
            if (e.Command.Name == "Add Connection")
            {
                this.diagram.Undo();
            }
            else if ((e.Command.Name == "Change Target" || e.Command.Name == "Change Source"))
            {
                if ((this.oldVertex == null || this.newVertex == null))
                {
                    base.AssociatedObject.Undo();
                }
                else if (this.oldVertex.Key != this.newVertex.Key)
                {
                    base.AssociatedObject.Undo();
                }
            }
        }

        private void AssociatedObject_ConnectionManipulationCompleted(object sender, ManipulationRoutedEventArgs e)
        {
            if (e.Shape == null)
            {
                this.newVertex = null;
            }
            else
            {
                this.newVertex = (e.Shape as RadDiagramShape).DataContext as Vertex;
            }
        }

        private void AssociatedObject_ConnectionManipulationStarted(object sender, ManipulationRoutedEventArgs e)
        {
            if (e.Shape != null)
            {
                this.oldVertex = (e.Shape as RadDiagramShape).DataContext as Vertex;
            }
            else
            {
                this.oldVertex = null;
            }
        }

        private void AssociatedObject_ShapeClicked(object sender, ShapeRoutedEventArgs e)
        {
            // Add the clicked shape to the list of shapes to bring to front
            var shapeList = new List<IDiagramItem>
            {
                e.Shape
            };

            // Change color of connections
            var graphControl = this.diagram.GetParent<GraphControl>();

            foreach (var conn in this.diagram.Connections)
            {
                (conn as RadDiagramConnection).Stroke = new SolidColorBrush(graphControl.ConnectionColor);
            }

            foreach (var conn in e.Shape.IncomingLinks)
            {
                (conn as RadDiagramConnection).Stroke = new SolidColorBrush(graphControl.IncomingConnectionHighlightColor);
            }

            foreach (var conn in e.Shape.OutgoingLinks)
            {
                (conn as RadDiagramConnection).Stroke = new SolidColorBrush(graphControl.OutgoingConnectionHighlightColor);
            }

            this.diagram.BringToFront(shapeList);

            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(150)
            };

            timer.Tick += (o, args) =>
            {
                if (!e.Shape.Bounds.IsInBounds(this.diagram.Viewport))
                {
                    var offset = this.diagram.Viewport.GetOffset(e.Shape.Bounds, 20);

                    // Extension OffsetRect is part of Telerik.Windows.Diagrams.Core
                    this.diagram.BringIntoView(this.diagram.Viewport.OffsetRect(offset.X, offset.Y));
                }

                timer.Stop();
            };

            timer.Start();
        }
    }
}