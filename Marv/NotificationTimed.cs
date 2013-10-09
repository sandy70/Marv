using Marv.Common;

namespace Marv
{
    public class NotificationTimed : ViewModel, INotification
    {
        private double _value = 100;
        private string description = "";
        private bool isIndeterminate = false;
        private string name = "";

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

        public void OnAdded()
        {
            // do nothing
        }
    }
}