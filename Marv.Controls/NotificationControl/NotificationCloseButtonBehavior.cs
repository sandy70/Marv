﻿using Marv.Common;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Marv.Controls
{
    public class NotificationCloseButtonBehavior : Behavior<Button>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Click += AssociatedObject_Click;
        }

        private void AssociatedObject_Click(object sender, RoutedEventArgs e)
        {
            var notification = this.AssociatedObject.DataContext as INotification;
            notification.Close();
        }
    }
}