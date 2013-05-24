using System;
using System.ComponentModel;

namespace LibBn
{
    [Serializable]
    public class BnState : INotifyPropertyChanged
    {
        private double _value;
        private string key;

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

        public double Value
        {
            get { return _value; }
            set
            {
                _value = value;
                OnPropertyChanged("Values");
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}