using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Marv
{
    public class KeyedCollection<T> : ObservableCollection<T>, IKeyed
    {
        private readonly Dictionary<string, T> dictionary = new Dictionary<string, T>();
        private string key;

        public T this[string aKey]
        {
            get
            {
                return this.dictionary[aKey];
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
                this.key = value;
                this.OnPropertyChanged(new PropertyChangedEventArgs("Key"));
            }
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);

            if (e.OldItems != null)
            {
                foreach (var keyedItem in e.OldItems.OfType<IKeyed>())
                {
                    this.dictionary.Remove(keyedItem.Key);
                }
            }

            if (e.NewItems != null)
            {
                foreach (var keyedItem in e.NewItems.OfType<IKeyed>())
                {
                    this.dictionary[keyedItem.Key] = (T) keyedItem;
                }
            }
        }
    }
}