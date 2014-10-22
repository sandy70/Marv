using System;
using System.Windows.Interactivity;

namespace Marv.Controls.Graph
{
    internal class SliderProgressBarBehavior : Behavior<SliderProgressBar>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.ValueEntered -= AssociatedObject_ValueEntered;
            this.AssociatedObject.ValueEntered += AssociatedObject_ValueEntered;
        }

        private void AssociatedObject_ValueEntered(object sender, double e)
        {
            var vertexControl = this.AssociatedObject.GetParent<VertexControl>();

            if (Math.Abs(e - 100) < Marv.Utils.Epsilon)
            {
                vertexControl.Vertex.SetEvidence(this.AssociatedObject.DataContext as State);
            }

            vertexControl.Vertex.Normalize();
            vertexControl.Vertex.UpdateEvidenceString();

            vertexControl.RaiseEvidenceEntered();
        }
    }
}