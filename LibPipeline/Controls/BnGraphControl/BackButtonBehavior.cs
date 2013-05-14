using LibBn;
using System.Windows;
using System.Windows.Interactivity;
using Telerik.Windows.Controls;

namespace LibPipeline
{
    internal class BackButtonBehavior : Behavior<RadButton>
    {
        public void AssociatedObject_Click(object sender, RoutedEventArgs e)
        {
            var graphControl = this.AssociatedObject.FindParent<BnGraphControl>();
            graphControl.Back();
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Click += AssociatedObject_Click;
        }
    }
}