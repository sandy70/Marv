using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Marv.Controls.Graph
{
    internal class ToolbarButtonClickBehavior : Behavior<Button>
    {
        public VertexControl VertexControl;

        protected override void OnAttached()
        {
            base.OnAttached();

            this.VertexControl = this.AssociatedObject.FindParent<VertexControl>();

            this.AssociatedObject.Click += this.AssociatedObject_Click;
        }

        private void AssociatedObject_Click(object sender, RoutedEventArgs e)
        {
            (this.AssociatedObject.DataContext as Command<VertexControl>).Excecute(this.VertexControl);
        }
    }
}