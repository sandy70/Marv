namespace Marv.Common
{
    public class ObservableKeyValuePair<TKey, TValue> : ViewModel
    {
        private TValue _value;
        private TKey key;

        public TKey Key
        {
            get
            {
                return this.key;
            }

            set
            {
                if (!value.Equals(this.key))
                {
                    this.key = value;
                    this.RaisePropertyChanged("Key");
                }
            }
        }

        public TValue Value
        {
            get
            {
                return this._value;
            }

            set
            {
                if (!value.Equals(this.key))
                {
                    this._value = value;
                    this.RaisePropertyChanged("Value");
                }
            }
        }
    }
}