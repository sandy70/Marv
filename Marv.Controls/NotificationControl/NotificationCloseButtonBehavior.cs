using Marv.Common;
using NLog;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Marv.Controls
{
    public class NotificationCloseButtonBehavior : Behavior<Button>
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.Click += AssociatedObject_Click;
        }

        private void AssociatedObject_Click(object sender, RoutedEventArgs e)
        {
            logger.Debug("DataContext: {0}", this.AssociatedObject.DataContext);

            var notification = this.AssociatedObject.DataContext as INotification;
            notification.Stop();
        }
    }
}