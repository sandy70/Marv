using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Marv
{
    public class KeyedCollection<T> : ObservableCollection<T>, IKeyed where T : IKeyed
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

        public IEnumerable<T> this[IEnumerable<string> keys]
        {
            get
            {
                return keys.Select(aKey => this[aKey]);
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

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }
    }
}