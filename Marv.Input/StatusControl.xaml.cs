using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Marv.Common; 
using Marv.Input;

namespace Marv.Input
{
    /// <summary>
    /// Interaction logic for StatusControl.xaml
    /// </summary>
    public partial class StatusControl
    {
        public static readonly DependencyProperty IsItemVisibleProperty =
            DependencyProperty.Register("IsItemVisible", typeof (bool), typeof (StatusControl), new PropertyMetadata(false));

        public static readonly DependencyProperty NotificationsProperty =
            DependencyProperty.Register("Notifications", typeof (ObservableCollection<Notification>), typeof (StatusControl), new PropertyMetadata(new ObservableCollection<Notification>()));

        public static readonly DependencyProperty SelectedNotificationProperty =
            DependencyProperty.Register("SelectedNotification", typeof (Notification), typeof (StatusControl), new PropertyMetadata(null));

        public bool IsItemVisible
        {
            get { return (bool)GetValue(IsItemVisibleProperty); }
            set { SetValue(IsItemVisibleProperty, value); }
        }

        public ObservableCollection<Notification> Notifications
        {
            get { return GetValue(NotificationsProperty) as ObservableCollection<Notification>; }
            set { SetValue(NotificationsProperty, value); }
        }

        public Notification SelectedNotification
        {
            get { return GetValue(SelectedNotificationProperty) as Notification; }
            set { SetValue(SelectedNotificationProperty, value); }
        }

        public StatusControl()
        {
            InitializeComponent();
            this.Loaded += StatusControl_Loaded;
        }

        private async void StatusControl_Loaded(object sender, RoutedEventArgs e)
        {  
            this.Notifications.CollectionChanged += Notifications_CollectionChanged;
            this.ForwardButton.Click += ForwardButton_Click;
            this.BackButton.Click += BackButton_Click;
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
                this.SelectedNotification = Notifications.Last();
                this.IsItemVisible = true;
                CurrentNoteBlock.Text = this.Notifications.IndexOf(this.SelectedNotification) + "/" + this.Notifications.Count;
            }
        }

        private void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.SelectedNotification == this.Notifications.Last()) { return; }
            this.SelectedNotification = this.Notifications[this.Notifications.IndexOf(this.SelectedNotification) + 1];
            CurrentNoteBlock.Text = this.Notifications.IndexOf(this.SelectedNotification) + "/" + this.Notifications.Count;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedNotification == Notifications.First()) { return; }
            this.SelectedNotification = this.Notifications[this.Notifications.IndexOf(this.SelectedNotification) - 1];
            CurrentNoteBlock.Text = this.Notifications.IndexOf(this.SelectedNotification) + "/" + this.Notifications.Count;
        }
        
    }
}
