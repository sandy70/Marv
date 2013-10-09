using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using NLog;
using System.Collections.Specialized;

namespace Marv
{
    public partial class NotificationControl : UserControl
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static readonly DependencyProperty NotificationsProperty =
        DependencyProperty.Register("Notifications", typeof(ObservableCollection<INotification>), typeof(NotificationControl), new PropertyMetadata(null, ChangedNotifications));

        private static void ChangedNotifications(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            logger.Trace("");

            var control = d as NotificationControl;

            control.Notifications.CollectionChanged += Notifications_CollectionChanged;
        }

        private static void Notifications_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (var newItem in e.NewItems)
            {
                var notification = newItem as INotification;
                notification.OnAdded();
            }
        }

        public NotificationControl()
        {
            InitializeComponent();
        }

        public ObservableCollection<INotification> Notifications
        {
            get { return (ObservableCollection<INotification>)GetValue(NotificationsProperty); }
            set { SetValue(NotificationsProperty, value); }
        }

        public void Add(INotification notification)
        {
            this.Notifications.Add(notification);
            notification.OnAdded();
        }
    }
}