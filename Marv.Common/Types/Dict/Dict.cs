using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Marv
{
    public partial class Dict<TKey, TValue> : ObservableCollection<Kvp<TKey, TValue>>
    {
        private readonly Dictionary<TKey, Kvp<TKey, TValue>> dictionary = new Dictionary<TKey, Kvp<TKey, TValue>>();

        public TValue this[TKey key]
        {
            get
            {
                if (!this.ContainsKey(key))
                {
                    this[key] = Utils.Create<TValue>();
                }

                return this.dictionary[key].Value;
            }

            set
            {
                if (value == null)
                {
                    this.Remove(key);
                }
                else
                {
                    this.Add(key, value);
                }
            }
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                    var kvp = item as Kvp<TKey, TValue>;
                    this.dictionary.Remove(kvp.Key);
                }
            }

            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                {
                    var kvp = item as Kvp<TKey, TValue>;
                    this.dictionary[kvp.Key] = kvp;
                }
            }

            base.OnCollectionChanged(e);
        }

        public void ChangeKey(TKey oldKey, TKey newKey)
        {
            var oldKvp = this.dictionary[oldKey];
            this.dictionary.Remove(oldKey);
            oldKvp.Key = newKey;
            this.dictionary[newKey] = oldKvp;
        }
    }

    public class Dict<T2, T1, TValue> : Dict<T2, Dict<T1, TValue>> {}

    public class Dict<T3, T2, T1, TValue> : Dict<T3, Dict<T2, T1, TValue>> {}

    public class Dict<T4, T3, T2, T1, TValue> : Dict<T4, Dict<T3, T2, T1, TValue>> {}
}