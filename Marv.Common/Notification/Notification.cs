using System;

namespace Marv.Common
{
    public interface INotification
    {
        string Description { get; set; }
        bool IsIndeterminate { get; }
        bool IsMuteable { get; set; }
        bool IsMuted { get; set; }
        string Name { get; set; }
        double Value { get; }

        void Close();

        void Open();

        event EventHandler Closed;
    }

    public class Notification : Model, INotification
    {
        private double _value = 100;
        private string description;
        private TimeSpan duration = TimeSpan.FromSeconds(3);
        private bool isIndeterminate;
        private bool isMuteable;
        private bool isMuted;
        private bool isTimed;

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
                    this.RaisePropertyChanged();
                }
            }
        }

        public bool IsTimed
        {
            get
            {
                return this.isTimed;
            }

            set
            {
                if (value != this.isTimed)
                {
                    this.isTimed = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public bool IsMuteable
        {
            get
            {
                return this.isMuteable;
            }

            set
            {
                if (value != this.isMuteable)
                {
                    this.isMuteable = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public bool IsMuted
        {
            get
            {
                return this.isMuted;
            }

            set
            {
                if (value != this.isMuted)
                {
                    this.isMuted = value;
                    this.RaisePropertyChanged();
                }
            }
        }

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
                    this.RaisePropertyChanged();
                }
            }
        }

        public bool IsIndeterminate
        {
            get
            {
                return this.isIndeterminate;
            }

            set
            {
                if (value != this.isIndeterminate)
                {
                    this.isIndeterminate = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public double Value
        {
            get
            {
                return this._value;
            }

            set
            {
                if (value != this._value)
                {
                    this._value = value;
                    this.RaisePropertyChanged();
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

        public void Open() {}
    }
}