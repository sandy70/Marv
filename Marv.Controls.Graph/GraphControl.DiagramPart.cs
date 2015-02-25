using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Marv.Common;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Diagrams;
using Telerik.Windows.Diagrams.Core;

namespace Marv.Controls
{
    public partial class GraphControl
    {
        private Vertex newVertex;
        private Vertex oldVertex;

        private void DiagramPart_CommandExecuted(object sender, CommandRoutedEventArgs e)
        {
            if (e.Command.Name == "Add Connection")
            {
                this.DiagramPart.Undo();
            }
            else if ((e.Command.Name == "Change Target" || e.Command.Name == "Change Source"))
            {
                if ((this.oldVertex == null || this.newVertex == null))
                {
                    this.DiagramPart.Undo();
                }
                else if (this.oldVertex.Key != this.newVertex.Key)
                {
                    this.DiagramPart.Undo();
                }
            }
        }

        private void DiagramPart_ConnectionManipulationCompleted(object sender, ManipulationRoutedEventArgs e)
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

        private void DiagramPart_ConnectionManipulationStarted(object sender, ManipulationRoutedEventArgs e)
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

        private void DiagramPart_DiagramLayoutComplete(object sender, RoutedEventArgs e)
        {
            this.DiagramPart.DiagramLayoutComplete -= this.DiagramPart_DiagramLayoutComplete;
            this.DiagramPart.AutoFitAsync(new Thickness(10));
        }

        private void DiagramPart_GraphSourceChanged(object sender, EventArgs e)
        {
            if (this.IsAutoLayoutEnabled)
            {
                Common.Utils.Schedule(TimeSpan.FromMilliseconds(300), () => this.UpdateLayout(true));
            }
            else
            {
                Common.Utils.Schedule(TimeSpan.FromMilliseconds(300), () => this.DiagramPart.AutoFit());
            }
        }

        private void DiagramPart_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.RaiseSelectionChanged(this.Graph.SelectedVertex);
        }

        private void DiagramPart_ShapeClicked(object sender, ShapeRoutedEventArgs e)
        {
            // Add the clicked shape to the list of shapes to bring to front
            var shapeList = new List<IDiagramItem>
            {
                e.Shape
            };

            // Change color of connections
            var graphControl = this.DiagramPart.GetParent<GraphControl>();

            foreach (var conn in this.DiagramPart.Connections)
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

            this.DiagramPart.BringToFront(shapeList);

            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(150)
            };

            timer.Tick += (o, args) =>
            {
                if (!e.Shape.Bounds.IsInBounds(this.DiagramPart.Viewport))
                {
                    var offset = this.DiagramPart.Viewport.GetOffset(e.Shape.Bounds, 20);

                    // Extension OffsetRect is part of Telerik.Windows.Diagrams.Core
                    this.DiagramPart.BringIntoView(this.DiagramPart.Viewport.OffsetRect(offset.X, offset.Y));
                }

                timer.Stop();
            };

            timer.Start();
        }

        private void GraphControl_Loaded_DiagramPart(object sender, RoutedEventArgs e)
        {
            this.DiagramPart.CommandExecuted -= this.DiagramPart_CommandExecuted;
            this.DiagramPart.CommandExecuted += this.DiagramPart_CommandExecuted;

            this.DiagramPart.ConnectionManipulationCompleted -= this.DiagramPart_ConnectionManipulationCompleted;
            this.DiagramPart.ConnectionManipulationCompleted += this.DiagramPart_ConnectionManipulationCompleted;

            this.DiagramPart.ConnectionManipulationStarted -= this.DiagramPart_ConnectionManipulationStarted;
            this.DiagramPart.ConnectionManipulationStarted += this.DiagramPart_ConnectionManipulationStarted;

            this.DiagramPart.GraphSourceChanged -= this.DiagramPart_GraphSourceChanged;
            this.DiagramPart.GraphSourceChanged += this.DiagramPart_GraphSourceChanged;

            this.DiagramPart.SelectionChanged -= this.DiagramPart_SelectionChanged;
            this.DiagramPart.SelectionChanged += this.DiagramPart_SelectionChanged;

            this.DiagramPart.ShapeClicked -= this.DiagramPart_ShapeClicked;
            this.DiagramPart.ShapeClicked += this.DiagramPart_ShapeClicked;
        }
    }
}