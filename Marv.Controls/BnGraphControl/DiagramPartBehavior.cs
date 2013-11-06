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

namespace Marv.Controls
{
    internal class DiagramPartBehavior : Behavior<RadDiagram>
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.GraphSourceChanged += AssociatedObject_GraphSourceChanged;
            this.AssociatedObject.ShapeClicked += AssociatedObject_ShapeClicked;
            this.AssociatedObject.ConnectionManipulationStarted += AssociatedObject_ConnectionManipulationStarted;
        }

        private void AssociatedObject_ConnectionManipulationStarted(object sender, ManipulationRoutedEventArgs e)
        {
            logger.Trace("");
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

                        // Extension OffsetRect is part of Telerik.Windows.Diagrams.Core
                        this.AssociatedObject.BringIntoView(this.AssociatedObject.Viewport.OffsetRect(offset.X, offset.Y));
                    }

                    timer.Stop();
                };

            timer.Start();
        }
    }
}