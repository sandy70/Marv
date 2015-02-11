using System;

namespace Marv.Common
{
    public interface INotifier
    {
        event EventHandler<Notification> NotificationOpened;
        event EventHandler<Notification> NotificationClosed;
    }
}