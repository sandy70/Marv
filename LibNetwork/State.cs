using Marv.Common;
using System.ComponentModel;

namespace LibNetwork
{
    public class State : INotifyPropertyChanged
    {
        private double _value;
        private string key;
        private SortedSequence<double> range = new SortedSequence<double>();

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

        public SortedSequence<double> Range
        {
            get
            {
                return this.range;
            }

            set
            {
                if (value != this.range)
                {
                    this.range = value;
                    this.OnPropertyChanged("Range");
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