namespace Marv.Common
{
    public class Kvp<TKey, TValue> : NotifyPropertyChanged, IKeyed<TKey>
    {
        private TKey key;
        private TValue value;

        public TValue Value
        {
            get
            {
                return this.value;
            }

            set
            {
                this.value = value;
                this.RaisePropertyChanged();
            }
        }

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
    }
}