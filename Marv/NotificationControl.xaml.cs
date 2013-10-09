using NLog;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace Marv
{
    public partial class NotificationControl : UserControl
    {
        public static readonly DependencyProperty NotificationsProperty =
        DependencyProperty.Register("Notifications", typeof(ObservableCollection<INotification>), typeof(NotificationControl), new PropertyMetadata(null, ChangedNotifications));

        private static Logger logger = LogManager.GetCurrentClassLogger();

        public NotificationControl()
        {
            InitializeComponent();

            this.CloseButton.Click += CloseButton_Click;
        }

        public ObservableCollection<INotification> Notifications
        {
            get { return (ObservableCollection<INotification>)GetValue(NotificationsProperty); }
            set { SetValue(NotificationsProperty, value); }
        }

        private static void ChangedNotifications(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            logger.Trace("");

            var control = d as NotificationControl;

            control.Notifications.CollectionChanged += control.Notifications_CollectionChanged;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Notifications.Clear();
        }

        private void notification_Stopped(object sender, EventArgs e)
        {
            var notification = sender as INotification;
            notification.Stopped -= notification_Stopped;
            this.Notifications.Remove(notification);
        }

        private void Notifications_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var newItem in e.NewItems)
                {
                    var notification = newItem as INotification;

                    notification.Stopped += notification_Stopped;
                    notification.Start();
                }
            }

            if (this.Notifications.Count == 0)
            {
                this.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.Visibility = Visibility.Visible;
            }
        }
    }
}