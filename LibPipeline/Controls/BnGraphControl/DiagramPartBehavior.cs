using Marv.Common;
using NLog;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Threading;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Diagrams;
using Telerik.Windows.Diagrams.Core;

namespace LibPipeline
{
    internal class DiagramPartBehavior : Behavior<RadDiagram>
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.GraphSourceChanged += AssociatedObject_GraphSourceChanged;
            this.AssociatedObject.SelectionChanged += AssociatedObject_SelectionChanged;
            this.AssociatedObject.ShapeClicked += AssociatedObject_ShapeClicked;
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

        private void AssociatedObject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var graphControl = this.AssociatedObject.FindParent<BnGraphControl>();

            foreach (var item in e.RemovedItems)
            {
                BnVertexViewModel vertexViewModel = item as BnVertexViewModel;

                if (vertexViewModel != null)
                {
                    vertexViewModel.IsSelected = false;
                    vertexViewModel.IsSensorChecked = false;

                    if (vertexViewModel.IsEvidenceEntered)
                    {
                        var parentGraphControl = this.AssociatedObject.FindParent<BnGraphControl>();
                        parentGraphControl.RaiseEvent(new ValueEventArgs<BnVertexViewModel>
                        {
                            RoutedEvent = BnGraphControl.NewEvidenceAvailableEvent,
                            Value = vertexViewModel
                        });
                    }
                }
            }

            foreach (var item in e.AddedItems)
            {
                BnVertexViewModel vertexViewModel = item as BnVertexViewModel;

                if (vertexViewModel != null)
                {
                    vertexViewModel.IsSelected = true;
                    graphControl.SelectedVertexViewModel = vertexViewModel;
                }
            }
        }

        private void AssociatedObject_ShapeClicked(object sender, ShapeRoutedEventArgs e)
        {
            // Add the clicked shape to the list of shapes to bring to front
            List<IDiagramItem> shapeList = new List<IDiagramItem>();
            shapeList.Add(e.Shape);

            // Change color of connections
            BnGraphControl graphControl = this.AssociatedObject.FindParent<BnGraphControl>();

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
                        this.AssociatedObject.FitTo(this.AssociatedObject.Viewport.OffsetRect(offset.X, offset.Y));
                    }

                    timer.Stop();
                };

            timer.Start();
        }
    }
}