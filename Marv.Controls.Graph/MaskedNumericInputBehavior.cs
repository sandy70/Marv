using System.Windows.Input;
using System.Windows.Interactivity;
using Telerik.Windows.Controls;

namespace Marv.Controls.Graph
{
    public class MaskedNumericInputBehavior : Behavior<RadMaskedNumericInput>
    {
        private void AssociatedObject_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var vertexControl = this.AssociatedObject.FindParent<VertexControl>();

            if (vertexControl == null)
            {
                return;
            }

            vertexControl.RaiseEvidenceEntered();
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.LostKeyboardFocus += AssociatedObject_LostKeyboardFocus;
        }
    }
}