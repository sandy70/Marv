using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using Marv.Common;

namespace Marv.Controls
{
    public partial class StatusControl : INotifyPropertyChanged
    {
        public static readonly DependencyProperty NotificationsProperty =
            DependencyProperty.Register("Notifications", typeof (NotificationCollection), typeof (StatusControl), new PropertyMetadata(null, ChangedNotifications));

        private Notification selectedNotification;
        private int selectedNotificationIndex;

        public NotificationCollection Notifications
        {
            get { return this.GetValue(NotificationsProperty) as NotificationCollection; }
            set { this.SetValue(NotificationsProperty, value); }
        }

        public Notification SelectedNotification
        {
            get { return this.selectedNotification; }

            set
            {
                if (value != null && value.Equals(this.selectedNotification))
                {
                    return;
                }

                this.selectedNotification = value;
                this.RaisePropertyChanged();
            }
        }

        public int SelectedNotificationIndex
        {
            get { return this.selectedNotificationIndex; }

            set
            {
                if (value.Equals(this.selectedNotificationIndex))
                {
                    return;
                }

                this.selectedNotificationIndex = value;
                this.RaisePropertyChanged();
            }
        }

        public StatusControl()
        {
            InitializeComponent();
        }

        private static void ChangedNotifications(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as StatusControl;

            if (control == null)
            {
                return;
            }

            control.Notifications.CollectionChanged -= control.Notifications_CollectionChanged;
            control.Notifications.CollectionChanged += control.Notifications_CollectionChanged;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.SelectedNotification == this.Notifications.First())
            {
                return;
            }
            this.SelectedNotification = this.Notifications[this.Notifications.IndexOf(this.SelectedNotification) - 1];
            this.SelectedNotificationIndex = this.Notifications.IndexOf(this.SelectedNotification) + 1;
        }

        private void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.SelectedNotification == this.Notifications.Last())
            {
                return;
            }
            this.SelectedNotification = this.Notifications[this.Notifications.IndexOf(this.SelectedNotification) + 1];
            this.SelectedNotificationIndex = this.Notifications.IndexOf(this.SelectedNotification) + 1;
        }

        private void Notifications_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.Notifications.Count == 0)
            {
                this.SelectedNotification = null;
                this.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.SelectedNotification = this.Notifications.Last();
                this.SelectedNotificationIndex = this.Notifications.IndexOf(this.SelectedNotification) + 1;
                this.Visibility = Visibility.Visible;
            }

            if (!this.Notifications.Contains(this.SelectedNotification))
            {
                this.SelectedNotification = null;
            }
        }

        private void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void StatusControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.Notifications != null)
            {
                this.Notifications.CollectionChanged -= this.Notifications_CollectionChanged;
                this.Notifications.CollectionChanged += this.Notifications_CollectionChanged;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}