using NLog;
using System;

namespace Marv.Common
{
    public interface INotification
    {
        event EventHandler Closed;

        string Description { get; set; }

        bool IsIndeterminate { get; }

        string Name { get; set; }

        double Value { get; }

        void Close();

        void Open();
    }

    public abstract class Notification : ViewModel, INotification
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private double _value = 100;
        private string description = "";
        private bool isIndeterminate = false;

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