using System;
using System.Windows.Interactivity;

namespace Marv.Controls.Graph
{
    internal class SliderProgressBarBehavior : Behavior<SliderProgressBar>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.ValueEntered += AssociatedObject_ValueEntered;
        }

        private void AssociatedObject_ValueEntered(object sender, double e)
        {
            var vertexControl = this.AssociatedObject.FindParent<VertexControl>();

            if (Math.Abs(e - 100) < Utils.Epsilon)
            {
                vertexControl.Vertex.SetEvidence(this.AssociatedObject.DataContext as State);
            }

            vertexControl.Vertex.Normalize();
            vertexControl.Vertex.UpdateEvidenceString();

            vertexControl.RaiseEvidenceEntered();
        }
    }
}