using System.ComponentModel;

namespace LibBn
{
    public class BnState : INotifyPropertyChanged
    {
        private double _value;
        private string key;
        private double max = double.MinValue;
        private double min = double.MinValue;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Key
        {
            get
            {
                return this.key;
            }

            set
            {
                if (value != this.key)
                {
                    this.key = value;
                    this.OnPropertyChanged("Key");
                }
            }
        }

        public double Max
        {
            get
            {
                return this.max;
            }

            set
            {
                if (value != this.max)
                {
                    this.max = value;

                    this.OnPropertyChanged("Max");
                }
            }
        }

        public double Min
        {
            get
            {
                return this.min;
            }

            set
            {
                if (value != this.min)
                {
                    this.min = value;

                    this.OnPropertyChanged("Min");
                }
            }
        }

        public double Value
        {
            get { return _value; }
            set
            {
                _value = value;
                OnPropertyChanged("Value");
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}