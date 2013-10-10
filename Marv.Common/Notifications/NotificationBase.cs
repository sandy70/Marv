using NLog;
using System;

namespace Marv.Common
{
    public abstract class NotificationBase : ViewModel, INotification
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private double _value = 100;
        private string description = "";
        private bool isIndeterminate = false;
        private string name = "";

        public event EventHandler Closed;

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

        public bool IsIndeterminate
        {
            get
            {
                return this.isIndeterminate;
            }

            protected set
            {
                if (value != this.isIndeterminate)
                {
                    this.isIndeterminate = value;
                    this.RaisePropertyChanged("IsIndeterminate");
                }
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

            protected set
            {
                if (value != this._value)
                {
                    this._value = value;
                    this.RaisePropertyChanged("Value");
                }
            }
        }

        public void Close()
        {
            if (this.Closed != null)
            {
                this.Closed(this, new EventArgs());
            }
        }

        public abstract void Open();
    }
}