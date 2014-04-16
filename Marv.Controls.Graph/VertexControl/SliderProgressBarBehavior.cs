using System.Windows.Input;
using System.Windows.Interactivity;
using Marv.Common;
using Marv.Common.Graph;

namespace Marv.Controls.Graph
{
    internal class SliderProgressBarBehavior : Behavior<SliderProgressBar>
    {
        private void AssociatedObject_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var vertexControl = this.AssociatedObject.FindParent<VertexControl>();
            vertexControl.Vertex.SelectState(this.AssociatedObject.DataContext as State);
            vertexControl.RaiseEvidenceEntered();
        }

        private void AssociatedObject_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var vertexControl = this.AssociatedObject.FindParent<VertexControl>();

            if (vertexControl.IsEditable)
            {
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.MouseDoubleClick += AssociatedObject_MouseDoubleClick;
            this.AssociatedObject.MouseDown += AssociatedObject_MouseDown;
        }
    }
}