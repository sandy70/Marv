using System.Windows.Input;
using System.Windows.Interactivity;
using Marv.Common;
using Marv.Common.Graph;

namespace Marv.Controls.Graph
{
    internal class SliderProgressBarBehaviorNew : Behavior<SliderProgressBar>
    {
        private void AssociatedObject_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var vertexControl = this.AssociatedObject.FindParent<VertexControl>();
            vertexControl.Vertex.SelectState(this.AssociatedObject.DataContext as State);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.MouseDoubleClick += this.AssociatedObject_MouseDoubleClick;
        }
    }
}