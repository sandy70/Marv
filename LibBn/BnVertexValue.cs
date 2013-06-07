using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace LibBn
{
    public class BnVertexValue : IEnumerable<double>, INotifyPropertyChanged
    {
        private Dictionary<string, double> stateValueByKey = new Dictionary<string, double>();
        private bool isEvidenceEntered;
        private string key;

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

        public double GetStateValue(string stateKey)
        {
            return this.stateValueByKey[stateKey];
        }

        public void SetStateValue(string stateKey, double value)
        {
            this.stateValueByKey[stateKey] = value;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public IEnumerator GetEnumerator()
        {
            return this.stateValueByKey.Values.GetEnumerator();
        }

        IEnumerator<double> IEnumerable<double>.GetEnumerator()
        {
            return this.stateValueByKey.Values.GetEnumerator();
        }
    }
}