using System;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Media.Animation;
using Telerik.Windows.Controls;

namespace LibPipeline
{
    internal class GroupButtonBehavior : Behavior<RadButton>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Click += AssociatedObject_Click;
        }

        private void AssociatedObject_Click(object sender, RoutedEventArgs e)
        {
            var graphControl = this.AssociatedObject.FindParent<BnGraphControl>();
            var vertexViewModel = this.AssociatedObject.DataContext as BnVertexViewModel;

            var fadeOutAnimation = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = new Duration(TimeSpan.FromMilliseconds(300))
            };

            var fadeInAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                BeginTime = TimeSpan.FromSeconds(0.5),
                Duration = new Duration(TimeSpan.FromMilliseconds(300))
            };

            fadeOutAnimation.Completed += (o, args) =>
            {
                graphControl.SelectedGroup = vertexViewModel.HeaderOfGroup;
                // graphControl.BeginAnimation(BnGraphControl.ShapeOpacityProperty, fadeInAnimation);

                foreach (var shape in graphControl.DiagramPart.Shapes)
                {
                    (shape as RadDiagramShape).BeginAnimation(RadDiagramShape.OpacityProperty, fadeInAnimation);
                }

                foreach (var connection in graphControl.DiagramPart.Connections)
                {
                    (connection as FrameworkElement).BeginAnimation(RadDiagramShape.OpacityProperty, fadeInAnimation);
                }
            };

            //graphControl.BeginAnimation(BnGraphControl.ShapeOpacityProperty, fadeOutAnimation);

            foreach (var shape in graphControl.DiagramPart.Shapes)
            {
                (shape as RadDiagramShape).BeginAnimation(RadDiagramShape.OpacityProperty, fadeOutAnimation);
            }

            foreach (var connection in graphControl.DiagramPart.Connections)
            {
                (connection as FrameworkElement).BeginAnimation(RadDiagramShape.OpacityProperty, fadeOutAnimation);
            }

            graphControl.RaiseEvent(new ValueEventArgs<BnVertexViewModel>
            {
                RoutedEvent = BnGraphControl.GroupButtonClickedEvent,
                Value = vertexViewModel
            });
        }
    }
}