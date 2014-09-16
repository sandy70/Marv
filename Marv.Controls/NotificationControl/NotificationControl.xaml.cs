using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using Marv;

namespace Marv.Controls
{
    public partial class NotificationControl
    {
        public static readonly DependencyProperty NotificationsProperty =
            DependencyProperty.Register("Notifications", typeof (ObservableCollection<INotification>), typeof (NotificationControl), new PropertyMetadata(null, ChangedNotifications));

        public NotificationControl()
        {
            InitializeComponent();
        }

        public ObservableCollection<INotification> Notifications
        {
            get
            {
                return (ObservableCollection<INotification>) GetValue(NotificationsProperty);
            }
            set
            {
                SetValue(NotificationsProperty, value);
            }
        }

        private static void ChangedNotifications(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as NotificationControl;
            control.Notifications.CollectionChanged += control.Notifications_CollectionChanged;
        }

        private void Notifications_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.Notifications.Count == 0)
            {
                this.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.Visibility = Visibility.Visible;
            }

            if (e.NewItems != null)
            {
                foreach (var newItem in e.NewItems)
                {
                    var notification = newItem as INotification;
                    notification.Closed += notification_Closed;
                    notification.Open();
                }
            }
        }

        private void notification_Closed(object sender, EventArgs e)
        {
            var notification = sender as INotification;
            notification.Closed -= notification_Closed;
            this.Notifications.Remove(notification);
        }
    }
}