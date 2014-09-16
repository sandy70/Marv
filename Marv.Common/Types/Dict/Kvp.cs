namespace Marv
{
    public class Kvp<TKey, TValue> : NotifyPropertyChanged
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
                this.key = value;
                this.RaisePropertyChanged();
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
                this._value = value;
                this.RaisePropertyChanged();
            }
        }
    }
}