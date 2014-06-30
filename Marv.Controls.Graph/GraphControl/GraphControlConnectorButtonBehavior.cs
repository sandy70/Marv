using System.Windows;
using System.Windows.Interactivity;

namespace Marv.Controls.Graph
{
    public class GraphControlConnectorButtonBehavior : Behavior<GraphControl>
    {
        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            // I don't know why Loaded is being called twice!
            var connectorButton = this.AssociatedObject.ConnectorButton;

            connectorButton.Checked -= ConnectorButton_Checked;
            connectorButton.Checked += ConnectorButton_Checked;

            connectorButton.Unchecked -= ConnectorButton_Unchecked;
            connectorButton.Unchecked += ConnectorButton_Unchecked;
        }

        private void ConnectorButton_Checked(object sender, RoutedEventArgs e)
        {
            this.AssociatedObject.EnableConnectorEditing();
        }

        private void ConnectorButton_Unchecked(object sender, RoutedEventArgs e)
        {
            this.AssociatedObject.DisableConnectorEditing();
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Loaded += AssociatedObject_Loaded;
        }
    }
}