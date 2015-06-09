using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Marv.Common;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Diagrams;

namespace Marv.Controls
{
    public partial class GraphControl
    {
        private Vertex newVertex;
        private Vertex oldVertex;

        private void Diagram_CommandExecuted(object sender, CommandRoutedEventArgs e)
        {
            if (e.Command.Name == "Add Connection")
            {
                this.Diagram.Undo();
            }
            else if ((e.Command.Name == "Change Target" || e.Command.Name == "Change Source"))
            {
                if ((this.oldVertex == null || this.newVertex == null))
                {
                    this.Diagram.Undo();
                }
                else if (this.oldVertex.Key != this.newVertex.Key)
                {
                    this.Diagram.Undo();
                }
            }
        }

        private void Diagram_ConnectionManipulationCompleted(object sender, ManipulationRoutedEventArgs e)
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

        private void Diagram_ConnectionManipulationStarted(object sender, ManipulationRoutedEventArgs e)
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

        private void Diagram_DiagramLayoutComplete(object sender, RoutedEventArgs e)
        {
            this.Diagram.DiagramLayoutComplete -= this.Diagram_DiagramLayoutComplete;
            this.Diagram.AutoFitAsync(new Thickness(10));
        }

        private void Diagram_GraphSourceChanged(object sender, EventArgs e)
        {
            if (this.IsAutoLayoutEnabled)
            {
                Common.Utils.Schedule(TimeSpan.FromMilliseconds(100), () => this.UpdateLayout(true));
            }
            else
            {
                Common.Utils.Schedule(TimeSpan.FromMilliseconds(100), () => this.Diagram.AutoFit());
            }
        }

        private void Diagram_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedShape = this.Diagram.Shapes.Cast<RadDiagramShape>().FirstOrDefault(shape => shape.DataContext == this.Graph.SelectedVertex);

            if (selectedShape != null && !this.Diagram.IsInViewport(selectedShape))
            {
                this.BringIntoView(selectedShape);
            }

            this.RaiseSelectionChanged(this.Graph.SelectedVertex);
        }
    }
}