using System;
using System.Collections.Generic;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Threading;
using Marv.Common;
using Marv.Common.Graph;
using NLog;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Diagrams;
using Telerik.Windows.Diagrams.Core;

namespace Marv.Controls.Graph
{
    internal class DiagramPartBehavior : Behavior<RadDiagram>
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private Edge newEdge;
        private Vertex newVertex;
        private Edge oldEdge;
        private Vertex oldVertex;

        private void AssociatedObject_CommandExecuted(object sender, CommandRoutedEventArgs e)
        {
            if (e.Command.Name == "Add Connection")
            {
                this.AssociatedObject.Undo();
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
            if (e.Connection == null)
            {
                this.newEdge = null;
            }
            else
            {
                this.newEdge = (e.Connection as RadDiagramConnection).DataContext as Edge;
                logger.Info(this.newEdge);
            }

            if (e.Shape == null)
            {
                this.newVertex = null;
            }
            else
            {
                this.newVertex = (e.Shape as RadDiagramShape).DataContext as Vertex;
                logger.Info(this.newVertex.Key);
            }

            foreach (var command in base.AssociatedObject.UndoRedoService.UndoStack)
            {
                logger.Info(command.Name);
            }
        }

        private void AssociatedObject_ConnectionManipulationStarted(object sender, ManipulationRoutedEventArgs e)
        {
            if (e.Connection != null)
            {
                this.oldEdge = (e.Connection as RadDiagramConnection).DataContext as Edge;
                logger.Info(this.oldEdge);
            }
            else
            {
                this.oldEdge = null;
            }

            if (e.Shape != null)
            {
                this.oldVertex = (e.Shape as RadDiagramShape).DataContext as Vertex;
                logger.Info(this.oldVertex.Key);
            }
            else
            {
                this.oldVertex = null;
            }
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

        private void AssociatedObject_ShapeClicked(object sender, ShapeRoutedEventArgs e)
        {
            // Add the clicked shape to the list of shapes to bring to front
            var shapeList = new List<IDiagramItem>();
            shapeList.Add(e.Shape);

            // Change color of connections
            var graphControl = this.AssociatedObject.FindParent<GraphControl>();
            graphControl.SelectedVertex = e.Shape.Content as Vertex;

            foreach (var conn in this.AssociatedObject.Connections)
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

        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.CommandExecuted += this.AssociatedObject_CommandExecuted;
            this.AssociatedObject.ConnectionManipulationCompleted += this.AssociatedObject_ConnectionManipulationCompleted;
            this.AssociatedObject.ConnectionManipulationStarted += this.AssociatedObject_ConnectionManipulationStarted;
            this.AssociatedObject.GraphSourceChanged += this.AssociatedObject_GraphSourceChanged;
            this.AssociatedObject.ShapeClicked += this.AssociatedObject_ShapeClicked;
        }
    }
}