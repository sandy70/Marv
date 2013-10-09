using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Marv
{
    public partial class NotificationControl : UserControl
    {
        public static readonly DependencyProperty NotificationsProperty =
        DependencyProperty.Register("Notifications", typeof(ObservableCollection<NotificationTimed>), typeof(NotificationControl), new PropertyMetadata(new ObservableCollection<NotificationTimed>()));

        public NotificationControl()
        {
            InitializeComponent();
        }

        public ObservableCollection<NotificationTimed> Notifications
        {
            get { return (ObservableCollection<NotificationTimed>)GetValue(NotificationsProperty); }
            set { SetValue(NotificationsProperty, value); }
        }

        public void Add(NotificationTimed notification)
        {
            this.Notifications.Add(notification);
            notification.OnAdded();
        }
    }
}