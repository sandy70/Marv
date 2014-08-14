using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace Marv.Common
{
    public class NotificationCollection : ObservableCollection<Notification>
    {
        public new void Add(Notification notification)
        {
            if (this.Contains(notification)) return;

            base.Add(notification);

            if (notification.IsTimed)
            {
                var timer = new DispatcherTimer
                {
                    Interval = notification.Duration,
                };

                timer.Tick += (o, e) =>
                {
                    this.Remove(notification);
                    timer.Stop();
                };

                timer.Start();
            }
        }
    }
}