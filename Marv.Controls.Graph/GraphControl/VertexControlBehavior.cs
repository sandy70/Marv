using System.Windows.Interactivity;
using Marv.Common;
using Marv.Common.Graph;

namespace Marv.Controls.Graph
{
    internal class VertexControlBehavior : Behavior<VertexControl>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.EvidenceEntered += AssociatedObject_EvidenceEntered;
        }

        private void AssociatedObject_EvidenceEntered(object sender, Vertex e)
        {
            var vertexControl = this.AssociatedObject;
            var graphControl = vertexControl.FindParent<GraphControl>();

            graphControl.RaiseEvidenceEntered(vertexControl.Vertex);
        }
    }
}