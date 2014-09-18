using NLog;
using System;
using System.Windows.Threading;

namespace Marv
{
    public class NotificationTimed : Notification
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public void Open()
        {
            Logger.Trace("");

            this.Value = 100;

            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(this.Duration.TotalMilliseconds / 100)
            };

            timer.Tick += (o, e) =>
                {
                    if (this.Value > 0)
                    {
                        this.Value -= 1;
                    }
                    else
                    {
                        timer.Stop();
                        this.Close();
                    }
                };

            timer.Start();
        }
    }
}