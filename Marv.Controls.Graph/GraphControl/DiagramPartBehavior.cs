using System;
using System.Collections.Generic;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Threading;
using Marv.Common;
using Marv.Common.Graph;
using NLog;
using Telerik.Windows.Diagrams.Core;

namespace Marv.Controls.Graph
{
    internal class DiagramPartBehavior : Behavior<Telerik.Windows.Controls.RadDiagram>
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private Edge oldEdge;
        private Vertex oldVertex;
        private Edge newEdge;
        private Vertex newVertex;

        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.CommandExecuted += this.AssociatedObject_CommandExecuted;
            this.AssociatedObject.ConnectionManipulationCompleted += this.AssociatedObject_ConnectionManipulationCompleted;
            this.AssociatedObject.ConnectionManipulationStarted += this.AssociatedObject_ConnectionManipulationStarted;
            this.AssociatedObject.GraphSourceChanged += this.AssociatedObject_GraphSourceChanged;
            this.AssociatedObject.ShapeClicked += this.AssociatedObject_ShapeClicked;
        }

        private void AssociatedObject_CommandExecuted(object sender, Telerik.Windows.Controls.Diagrams.CommandRoutedEventArgs e)
        {
            if (e.Command.Name == "Add Connection")
            {
                this.AssociatedObject.Undo();
            }
            else if (e.Command.Name == "Change Target")
            {
                if (this.oldVertex.Key != this.newVertex.Key)
                {
                    this.AssociatedObject.Undo();
                }
            }
        }

        private void AssociatedObject_ConnectionManipulationStarted(object sender, Telerik.Windows.Controls.Diagrams.ManipulationRoutedEventArgs e)
        {
            if (e.Connection != null)
            {
                this.oldEdge = (e.Connection as Telerik.Windows.Controls.RadDiagramConnection).DataContext as Edge;
                logger.Info(this.oldEdge);
            }
            else
            {
                this.oldEdge = null;
            }

            if (e.Shape != null)
            {
                this.oldVertex = (e.Shape as Telerik.Windows.Controls.RadDiagramShape).DataContext as Vertex;
                logger.Info(this.oldVertex.Key);
            }
            else
            {
                this.oldVertex = null;
            }
        }

        private void AssociatedObject_ConnectionManipulationCompleted(object sender, Telerik.Windows.Controls.Diagrams.ManipulationRoutedEventArgs e)
        {
            var diagram = this.AssociatedObject;

            if (e.Connection != null)
            {
                this.newEdge = (e.Connection as Telerik.Windows.Controls.RadDiagramConnection).DataContext as Edge;
                logger.Info(this.newEdge);
            }
            else
            {
                this.newEdge = null;
            }

            if (e.Shape != null)
            {
                this.newVertex = (e.Shape as Telerik.Windows.Controls.RadDiagramShape).DataContext as Vertex;
                logger.Info(this.newVertex.Key);
            }
            else
            {
                this.newVertex = null;
            }

            foreach (var command in this.AssociatedObject.UndoRedoService.UndoStack)
            {
                logger.Info(command.Name);
            }

            //var graphSource = diagram.GraphSource as Graph;
            //var edgeToRemove = graphSource.Edges.Where(x => x.Source.Key == "cpon" && x.Target.Key == "coatd").First();
            //graphSource.Edges.Remove(edgeToRemove);
        }

        private void AssociatedObject_GraphSourceChanged(object sender, EventArgs e)
        {
            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(500)
            };

            timer.Tick += (o, e2) =>
            {
                this.AssociatedObject.AutoFit();
                timer.Stop();
            };

            timer.Start();
        }

        private void AssociatedObject_ShapeClicked(object sender, Telerik.Windows.Controls.Diagrams.ShapeRoutedEventArgs e)
        {
            // Add the clicked shape to the list of shapes to bring to front
            var shapeList = new List<IDiagramItem>();
            shapeList.Add(e.Shape);

            // Change color of connections
            var graphControl = this.AssociatedObject.FindParent<GraphControl>();
            graphControl.SelectedVertex = e.Shape.Content as Vertex;

            foreach (var conn in this.AssociatedObject.Connections)
            {
                (conn as Telerik.Windows.Controls.RadDiagramConnection).Stroke = new SolidColorBrush(graphControl.ConnectionColor);
            }

            foreach (var conn in e.Shape.IncomingLinks)
            {
                (conn as Telerik.Windows.Controls.RadDiagramConnection).Stroke = new SolidColorBrush(graphControl.IncomingConnectionHighlightColor);
            }

            foreach (var conn in e.Shape.OutgoingLinks)
            {
                (conn as Telerik.Windows.Controls.RadDiagramConnection).Stroke = new SolidColorBrush(graphControl.OutgoingConnectionHighlightColor);
            }

            this.AssociatedObject.BringToFront(shapeList);

            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(150)
            };

            timer.Tick += (o, args) =>
                {
                    if (!e.Shape.Bounds.IsInBounds(this.AssociatedObject.Viewport))
                    {
                        var offset = this.AssociatedObject.Viewport.GetOffset(e.Shape.Bounds, 20);

                        // Extension OffsetRect is part of Telerik.Windows.Diagrams.Core
                        this.AssociatedObject.BringIntoView(this.AssociatedObject.Viewport.OffsetRect(offset.X, offset.Y));
                    }

                    timer.Stop();
                };

            timer.Start();
        }
    }
}