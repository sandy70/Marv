using System.Windows.Input;
using System.Windows.Interactivity;
using Marv.Common;
using Marv.Common.Graph;

namespace Marv.Controls.Graph
{
    internal class SliderProgressBarBehavior : Behavior<SliderProgressBar>
    {
        private VertexControl vertexControl;

        private void AssociatedObject_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.vertexControl.Vertex.SetEvidence(this.AssociatedObject.DataContext as State);
            this.vertexControl.RaiseEvidenceEntered();
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            this.vertexControl = this.AssociatedObject.FindParent<VertexControl>();

            this.AssociatedObject.MouseDoubleClick += AssociatedObject_MouseDoubleClick;
            this.AssociatedObject.MouseDown += AssociatedObject_MouseDown;
            this.AssociatedObject.MouseUp += AssociatedObject_MouseUp;
        }

        private void AssociatedObject_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!this.vertexControl.IsValueVisible)
            {
                this.vertexControl.RaiseEvidenceEntered();
            }
        }

        private void AssociatedObject_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.vertexControl.Vertex.EvidenceString = null;
        }
    }
}