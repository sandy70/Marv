using System.Windows.Input;
using System.Windows.Interactivity;
using Telerik.Windows.Controls;

namespace Marv.Controls.Graph
{
    public class MaskedNumericInputBehavior : Behavior<RadMaskedNumericInput>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.LostKeyboardFocus += AssociatedObject_LostKeyboardFocus;
        }

        private void AssociatedObject_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var vertexControl = this.AssociatedObject.GetParent<VertexControl>();

            if (vertexControl == null)
            {
                return;
            }

            vertexControl.Vertex.Normalize();
            vertexControl.RaiseEvidenceEntered();
        }
    }
}