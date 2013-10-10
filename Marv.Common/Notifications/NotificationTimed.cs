using NLog;
using System;
using System.Windows.Threading;

namespace Marv.Common
{
    public class NotificationTimed : NotificationBase
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private TimeSpan duration = TimeSpan.FromSeconds(5);

        public TimeSpan Duration
        {
            get
            {
                return this.duration;
            }

            set
            {
                if (value != this.duration)
                {
                    this.duration = value;
                    this.RaisePropertyChanged("Duration");
                }
            }
        }

        public override void Open()
        {
            logger.Trace("");

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