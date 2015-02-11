using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using Marv.Common;

namespace Marv.Controls
{
    public class NotificationCloseButtonBehavior : Behavior<Button>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Click += AssociatedObject_Click;
        }

        private void AssociatedObject_Click(object sender, RoutedEventArgs e)
        {
            var control = this.AssociatedObject.GetParent<NotificationControl>();
            var notification = this.AssociatedObject.DataContext as Notification;

            if (control != null)
            {
                control.Notifications.Remove(notification);
            }
        }
    }
}