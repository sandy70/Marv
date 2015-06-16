using System.Windows;
using System.Windows.Interactivity;
using Marv.Common;

namespace Marv.Controls
{
    public class WindowNotificationBehavior : Behavior<Window>
    {
        public static readonly DependencyProperty NotificationsProperty =
            DependencyProperty.Register("Notifications", typeof (NotificationCollection), typeof (WindowNotificationBehavior), new PropertyMetadata(new NotificationCollection()));

        public NotificationCollection Notifications
        {
            get { return (NotificationCollection) this.GetValue(NotificationsProperty); }
            set { this.SetValue(NotificationsProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.Loaded -= this.AssociatedObject_Loaded;
            this.AssociatedObject.Loaded += this.AssociatedObject_Loaded;
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            var notifiers = this.AssociatedObject.GetChildren<INotifier>();

            foreach (var notifier in notifiers)
            {
                notifier.NotificationClosed -= this.notifier_NotificationClosed;
                notifier.NotificationClosed += this.notifier_NotificationClosed;

                notifier.NotificationOpened -= this.notifier_NotificationOpened;
                notifier.NotificationOpened += this.notifier_NotificationOpened;
            }
        }

        private void notifier_NotificationClosed(object sender, Notification notification)
        {
            this.Notifications.Remove(notification);
        }

        private void notifier_NotificationOpened(object sender, Notification notification)
        {
            this.Notifications.Add(notification);
        }
    }
}