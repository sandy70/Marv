using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;

namespace Marv.Controls
{
    /// <summary>
    ///     Interaction logic for StatusControl.xaml
    /// </summary>
    public partial class StatusControl
    {
        public static readonly DependencyProperty IsItemVisibleProperty =
            DependencyProperty.Register("IsItemVisible", typeof (bool), typeof (StatusControl), new PropertyMetadata(true));

        public static readonly DependencyProperty NotificationsProperty =
            DependencyProperty.Register("Notifications", typeof (NotificationCollection), typeof (StatusControl), new PropertyMetadata(null, ChangedNotifications));

        public static readonly DependencyProperty SelectedNotficationIndexProperty =
            DependencyProperty.Register("SelectedNotificationIndex", typeof (int), typeof (StatusControl), new PropertyMetadata(0));

        public static readonly DependencyProperty SelectedNotificationProperty =
            DependencyProperty.Register("SelectedNotification", typeof (Notification), typeof (StatusControl), new PropertyMetadata(null));

        public bool IsItemVisible
        {
            get
            {
                return (bool) this.GetValue(IsItemVisibleProperty);
            }
            set
            {
                this.SetValue(IsItemVisibleProperty, value);
            }
        }

        public NotificationCollection Notifications
        {
            get
            {
                return this.GetValue(NotificationsProperty) as NotificationCollection;
            }
            set
            {
                this.SetValue(NotificationsProperty, value);
            }
        }

        public Notification SelectedNotification
        {
            get
            {
                return this.GetValue(SelectedNotificationProperty) as Notification;
            }
            set
            {
                this.SetValue(SelectedNotificationProperty, value);
            }
        }

        public int SelectedNotificationIndex
        {
            get
            {
                return (int) this.GetValue(SelectedNotficationIndexProperty);
            }
            set
            {
                this.SetValue(SelectedNotficationIndexProperty, value);
            }
        }

        public StatusControl()
        {
            InitializeComponent();
            this.Loaded += this.StatusControl_Loaded;
        }

        private static void ChangedNotifications(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as StatusControl;

            if (control == null) return;

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
                this.IsItemVisible = false;
                this.SelectedNotification = null;
                this.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.SelectedNotification = this.Notifications.Last();
                this.IsItemVisible = true;
                this.SelectedNotificationIndex = this.Notifications.IndexOf(this.SelectedNotification) + 1;
                this.Visibility = Visibility.Visible;
            }

            if (!this.Notifications.Contains(this.SelectedNotification)) this.SelectedNotification = null;
        }

        private void StatusControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.BackButton.Click -= this.BackButton_Click;
            this.BackButton.Click += this.BackButton_Click;

            this.ForwardButton.Click -= this.ForwardButton_Click;
            this.ForwardButton.Click += this.ForwardButton_Click;

            if (this.Notifications != null)
            {
                this.Notifications.CollectionChanged -= this.Notifications_CollectionChanged;
                this.Notifications.CollectionChanged += this.Notifications_CollectionChanged;
            }
        }
    }
}