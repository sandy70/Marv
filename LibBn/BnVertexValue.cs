using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace LibBn
{
    public class BnVertexValue : INotifyPropertyChanged
    {
        private bool isEvidenceEntered;
        private string key;
        private IEnumerable<BnStateValue> stateValues = new ObservableCollection<BnStateValue>();

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsEvidenceEntered
        {
            get
            {
                return this.isEvidenceEntered;
            }

            set
            {
                if (value != this.isEvidenceEntered)
                {
                    this.isEvidenceEntered = value;
                    this.OnPropertyChanged("IsEvidenceEntered");
                }
            }
        }

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

        public IEnumerable<BnStateValue> StateValues
        {
            get
            {
                return this.stateValues;
            }

            set
            {
                if (value != this.stateValues)
                {
                    this.stateValues = value;
                    this.OnPropertyChanged("StateValues");
                }
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}