using System;
using System.Windows.Interactivity;
using Marv;
using Marv.Graph;

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
                vertexControl.Vertex.States.SetEvidence(this.AssociatedObject.DataContext as State);
            }

            vertexControl.Vertex.States.SetEvidence(vertexControl.Vertex.States.GetEvidence().Normalized());
            vertexControl.Vertex.UpdateEvidenceString();
            vertexControl.RaiseEvidenceEntered();
        }
    }
}