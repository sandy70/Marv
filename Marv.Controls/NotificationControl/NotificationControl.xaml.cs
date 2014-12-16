using System.Windows;
using Marv.Common;

namespace Marv.Controls
{
    public partial class NotificationControl
    {
        public static readonly DependencyProperty NotificationsProperty =
            DependencyProperty.Register("Notifications", typeof (NotificationCollection), typeof (NotificationControl), new PropertyMetadata(null));

        public NotificationCollection Notifications
        {
            get
            {
                return (NotificationCollection) GetValue(NotificationsProperty);
            }
            set
            {
                SetValue(NotificationsProperty, value);
            }
        }

        public NotificationControl()
        {
            InitializeComponent();
        }
    }
}