using System;

namespace Marv.Common
{
    public class Notification : NotifyPropertyChanged
    {
        private string description;
        private TimeSpan duration = TimeSpan.FromSeconds(3);
        private bool isIndeterminate;
        private bool isMuteable;
        private bool isMuted;
        private bool isTimed;
        private string name;
        private double value;

        public string Description
        {
            get { return this.description; }

            set
            {
                if (value != this.description)
                {
                    this.description = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public TimeSpan Duration
        {
            get { return this.duration; }

            set
            {
                if (value != this.duration)
                {
                    this.duration = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public bool IsIndeterminate
        {
            get { return this.isIndeterminate; }

            set
            {
                if (value != this.isIndeterminate)
                {
                    this.isIndeterminate = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public bool IsMuteable
        {
            get { return this.isMuteable; }

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
            get { return this.isMuted; }

            set
            {
                if (value != this.isMuted)
                {
                    this.isMuted = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public bool IsTimed
        {
            get { return this.isTimed; }

            set
            {
                if (value != this.isTimed)
                {
                    this.isTimed = value;
                    this.RaisePropertyChanged();

                    if (this.IsTimed)
                    {
                        this.IsIndeterminate = true;
                    }
                }
            }
        }

        public string Name
        {
            get { return this.name; }

            set
            {
                if (value.Equals(this.name))
                {
                    return;
                }

                this.name = value;
                this.RaisePropertyChanged();
            }
        }

        public double Value
        {
            get { return this.value; }

            set
            {
                if (value != this.value)
                {
                    this.value = value;
                    this.RaisePropertyChanged();
                }
            }
        }
    }
}