using System.Windows;
using Marv.Common;
using Telerik.Windows.Controls;

namespace Marv.NetworkViewer
{
    public partial class MainWindow
    {
        public static readonly DependencyProperty NotificationsProperty =
            DependencyProperty.Register("Notifications", typeof (NotificationCollection), typeof (MainWindow), new PropertyMetadata(new NotificationCollection()));

        public NotificationCollection Notifications
        {
            get { return (NotificationCollection) GetValue(NotificationsProperty); }
            set { SetValue(NotificationsProperty, value); }
        }

        public MainWindow()
        {
            StyleManager.ApplicationTheme = new Windows8Theme();
            InitializeComponent();
        }
    }
}