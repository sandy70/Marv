using Marv.Common;
using NLog;
using System;
using System.Windows.Threading;

namespace Marv.Controls
{
    public class NotificationTimed : ViewModel, INotification
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private double _value = 100;
        private string description = "";
        private TimeSpan duration = TimeSpan.FromSeconds(5);
        private bool isIndeterminate = false;
        private string name = "";

        public event EventHandler Stopped;

        public string Description
        {
            get
            {
                return this.description;
            }

            set
            {
                if (value != this.description)
                {
                    this.description = value;
                    this.RaisePropertyChanged("Description");
                }
            }
        }

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

        public bool IsIndeterminate
        {
            get
            {
                return this.isIndeterminate;
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                if (value != this.name)
                {
                    this.name = value;
                    this.RaisePropertyChanged("Name");
                }
            }
        }

        public double Value
        {
            get
            {
                return this._value;
            }

            private set
            {
                if (value != this._value)
                {
                    this._value = value;
                    this.RaisePropertyChanged("Value");
                }
            }
        }

        public void Start()
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
                        this.Stop();
                    }
                };

            timer.Start();
        }

        public void Stop()
        {
            if (this.Stopped != null)
            {
                this.Stopped(this, new EventArgs());
            }
        }
    }
}