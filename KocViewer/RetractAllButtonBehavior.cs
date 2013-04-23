using LibPipeline;
using System.Windows;
using System.Windows.Interactivity;
using Telerik.Windows.Controls;

namespace KocViewer
{
    internal class RetractAllButtonBehavior : Behavior<RadButton>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Click += AssociatedObject_Click;
        }

        private void AssociatedObject_Click(object sender, RoutedEventArgs e)
        {
            var window = this.AssociatedObject.FindParent<MainWindow>();

            foreach (var vertex in window.GraphControl.SourceGraph.Vertices)
            {
                var vertexViewModel = vertex as BnVertexViewModel;
                window.RemoveInput(vertexViewModel);
            }

            window.TryUpdateNetwork();
        }
    }
}