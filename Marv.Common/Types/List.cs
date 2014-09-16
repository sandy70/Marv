using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Marv
{
    public class List<TValue, TKey> : ObservableCollection<TValue> where TValue : class, IKey<TKey>
    {
        private readonly Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();

        public TValue this[TKey key]
        {
            get
            {
                if (!this.ContainsKey(key))
                {
                    var newValue = Utils.Create<TValue>();

                    if (newValue != null)
                    {
                        newValue.Key = key;
                    }

                    this.Add(newValue);
                }

                return this.dictionary[key];
            }
        }

        public void Add(TKey key)
        {
            var newValue = Utils.Create<TValue>();

            if (newValue != null) newValue.Key = key;
            else throw new InvalidValueException(String.Format("Cannot create objects of type {0}", typeof (TValue)));

            this.Add(newValue);
        }

        public bool ContainsKey(TKey key)
        {
            return this.dictionary.ContainsKey(key);
        }

        public void ReplaceKey(TKey oldKey, TKey newKey)
        {
            var oldValue = this[oldKey];
            this.Remove(oldValue);
            
            oldValue.Key = newKey;
            this.Add(oldValue);
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);

            if (e.OldItems != null)
            {
                foreach (var oldItem in e.OldItems)
                {
                    this.dictionary.Remove((oldItem as TValue).Key);
                }
            }

            if (e.NewItems != null)
            {
                foreach (var newItem in e.NewItems)
                {
                    var newValue = newItem as TValue;
                    this.dictionary[newValue.Key] = newValue;
                }
            }
        }
    }
}