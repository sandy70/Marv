using Marv.Common;
using NLog;
using System;
using System.Collections.Generic;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Threading;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Diagrams;
using Telerik.Windows.Diagrams.Core;
using System.Linq;

namespace Marv.Controls
{
    internal class DiagramPartBehavior : Behavior<RadDiagram>
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private Edge oldEdge = null;
        private Vertex oldVertex = null;
        private Edge newEdge = null;
        private Vertex newVertex = null;

        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.CommandExecuted += AssociatedObject_CommandExecuted;
            // this.AssociatedObject.ItemsChanged += AssociatedObject_ItemsChanged;
            this.AssociatedObject.ConnectionManipulationCompleted += AssociatedObject_ConnectionManipulationCompleted;
            this.AssociatedObject.ConnectionManipulationStarted += AssociatedObject_ConnectionManipulationStarted;
            this.AssociatedObject.GraphSourceChanged += AssociatedObject_GraphSourceChanged;
            this.AssociatedObject.ShapeClicked += AssociatedObject_ShapeClicked;
        }

        private void AssociatedObject_CommandExecuted(object sender, CommandRoutedEventArgs e)
        {
            if (e.Command.Name == "Add Connection")
            {
                this.AssociatedObject.Undo();
            }
            else if (e.Command.Name == "Change Target")
            {
                if (oldVertex.Key != newVertex.Key)
                {
                    this.AssociatedObject.Undo();
                }
            }
        }

        private void AssociatedObject_ItemsChanged(object sender, DiagramItemsChangedEventArgs e)
        {
            logger.Trace("");

            foreach (var command in this.AssociatedObject.UndoRedoService.UndoStack)
            {
                logger.Info(command.Name);
            }
        }

        private void AssociatedObject_ConnectionManipulationStarted(object sender, ManipulationRoutedEventArgs e)
        {
            if (e.Connection != null)
            {
                oldEdge = (e.Connection as RadDiagramConnection).DataContext as Edge;
                logger.Info(oldEdge);
            }
            else
            {
                oldEdge = null;
            }

            if (e.Shape != null)
            {
                oldVertex = (e.Shape as RadDiagramShape).DataContext as Vertex;
                logger.Info(oldVertex.Key);
            }
            else
            {
                oldVertex = null;
            }
        }

        private void AssociatedObject_ConnectionManipulationCompleted(object sender, ManipulationRoutedEventArgs e)
        {
            var diagram = this.AssociatedObject;

            if (e.Connection != null)
            {
                newEdge = (e.Connection as RadDiagramConnection).DataContext as Edge;
                logger.Info(newEdge);
            }
            else
            {
                newEdge = null;
            }

            if (e.Shape != null)
            {
                newVertex = (e.Shape as RadDiagramShape).DataContext as Vertex;
                logger.Info(newVertex.Key);
            }
            else
            {
                newVertex = null;
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

        private void AssociatedObject_ShapeClicked(object sender, ShapeRoutedEventArgs e)
        {
            // Add the clicked shape to the list of shapes to bring to front
            List<IDiagramItem> shapeList = new List<IDiagramItem>();
            shapeList.Add(e.Shape);

            // Change color of connections
            GraphControl graphControl = this.AssociatedObject.FindParent<GraphControl>();

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
                        var offset = this.AssociatedObject.Viewport.GetOffset(e.Shape.Bounds, pad: 20);

                        // Extension OffsetRect is part of Telerik.Windows.Diagrams.Core
                        this.AssociatedObject.BringIntoView(this.AssociatedObject.Viewport.OffsetRect(offset.X, offset.Y));
                    }

                    timer.Stop();
                };

            timer.Start();
        }
    }
}