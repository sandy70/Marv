using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using Marv.Common;

namespace Marv.Input
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
                return (bool) GetValue(IsItemVisibleProperty);
            }
            set
            {
                SetValue(IsItemVisibleProperty, value);
            }
        }

        public ObservableCollection<Notification> Notifications
        {
            get
            {
                return GetValue(NotificationsProperty) as ObservableCollection<Notification>;
            }
            set
            {
                SetValue(NotificationsProperty, value);
            }
        }

        public Notification SelectedNotification
        {
            get
            {
                return GetValue(SelectedNotificationProperty) as Notification;
            }
            set
            {
                SetValue(SelectedNotificationProperty, value);
            }
        }

        public int SelectedNotificationIndex
        {
            get
            {
                return (int) GetValue(SelectedNotficationIndexProperty);
            }
            set
            {
                SetValue(SelectedNotficationIndexProperty, value);
            }
        }

        public StatusControl()
        {
            InitializeComponent();
            this.Loaded += StatusControl_Loaded;
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
            if (SelectedNotification == Notifications.First())
            {
                return;
            }
            this.SelectedNotification = this.Notifications[this.Notifications.IndexOf(this.SelectedNotification) - 1];
            this.SelectedNotificationIndex = Notifications.IndexOf(this.SelectedNotification) + 1;
        }

        private void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.SelectedNotification == this.Notifications.Last())
            {
                return;
            }
            this.SelectedNotification = this.Notifications[this.Notifications.IndexOf(this.SelectedNotification) + 1];
            this.SelectedNotificationIndex = Notifications.IndexOf(this.SelectedNotification) + 1;
        }

        private void Notifications_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (Notifications.Count == 0)
            {
                this.IsItemVisible = false;
                this.SelectedNotification = null;
            }
            else
            {
                this.SelectedNotification = this.Notifications.Last();
                this.IsItemVisible = true;
                this.SelectedNotificationIndex = this.Notifications.IndexOf(this.SelectedNotification) + 1;
            }

            if (!this.Notifications.Contains(this.SelectedNotification)) this.SelectedNotification = null;
        }

        private void StatusControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.BackButton.Click -= BackButton_Click;
            this.BackButton.Click += BackButton_Click;

            this.ForwardButton.Click -= ForwardButton_Click;
            this.ForwardButton.Click += ForwardButton_Click;

            if (this.Notifications != null)
            {
                this.Notifications.CollectionChanged -= Notifications_CollectionChanged;
                this.Notifications.CollectionChanged += Notifications_CollectionChanged;
            }
        }
    }
}