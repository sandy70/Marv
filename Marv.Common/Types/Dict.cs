using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace Marv.Common
{
    public class Dict<T1, T2, TValue> : Dict<T1, Dict<T2, TValue>> { }

    public class Dict<T1, T2, T3, TValue> : Dict<T1, Dict<T2, T3, TValue>> { }

    public class Dict<T1, T2, T3, T4, TValue> : Dict<T1, Dict<T2, T3, T4, TValue>> { }

    public class Dict<TKey, TValue> : IDictionary<TKey, TValue>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private readonly IDictionary<TKey, TValue> dictionary;

        private Dispatcher dispatcher;

        #region Constructor

        public Dict(Dispatcher dispatcher = null)
        {
            dictionary = new Dictionary<TKey, TValue>();
            this.dispatcher = dispatcher ??
                              (Application.Current != null
                                   ? Application.Current.Dispatcher
                                   : Dispatcher.CurrentDispatcher);
        }

        public Dict(IDictionary<TKey, TValue> dictionary)
        {
            this.dictionary = new Dictionary<TKey, TValue>(dictionary);
        }

        public Dict(int capacity)
        {
            dictionary = new Dictionary<TKey, TValue>(capacity);
        }

        #endregion Constructor

        public int Count
        {
            get { return dictionary.Count; }
        }

        public bool IsReadOnly
        {
            get { return dictionary.IsReadOnly; }
        }

        public ICollection<TKey> Keys
        {
            get { return dictionary.Keys; }
        }

        public ICollection<TValue> Values
        {
            get { return dictionary.Values; }
        }

        public TValue this[TKey key]
        {
            get 
            { 
                return dictionary[key]; 
            }

            set
            {
                if (this.dictionary.ContainsKey(key))
                {
                    var oldKvp = this.dictionary.Single(kvp => kvp.Key.Equals(key));

                    this.RemoveNotifyEvents(this.dictionary[key]);
                    this.dictionary[key] = value;
                    this.AddNotifyEvents(this.dictionary[key]);

                    var newKvp = this.dictionary.Single(kvp => kvp.Key.Equals(key));

                    this.OnCollectionChanged(NotifyCollectionChangedAction.Replace, newKvp, oldKvp);
                }
                else
                {
                    this.dictionary[key] = value;
                    this.AddNotifyEvents(this.dictionary[key]);

                    var pair = this.dictionary.Single(kvp => kvp.Key.Equals(key));
                    this.OnCollectionChanged(NotifyCollectionChangedAction.Add, pair);
                }

                //if (dispatcher.CheckAccess())
                //{
                //    if (dictionary.ContainsKey(key))
                //    {
                //        throw new ArgumentException("Unknown key: " + key);
                //    }

                //    OnCollectionChanged(NotifyCollectionChangedAction.Replace, dictionary[key], value);
                //    RemoveNotifyEvents(dictionary[key]);
                //    AddNotifyEvents(value);

                //    dictionary[key] = value;
                //}
                //else
                //{
                //    dispatcher.Invoke(new Action(() => this[key] = value));
                //}
            }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            if (dispatcher.CheckAccess())
            {
                dictionary.Add(item);

                OnCollectionChanged(NotifyCollectionChangedAction.Add, item);
                AddNotifyEvents(item.Value);
            }
            else
            {
                dispatcher.Invoke(new Action(() => Add(item)));
            }
        }

        public void Add(TKey key, TValue value)
        {
            if (dispatcher.CheckAccess())
            {
                var kvp = new KeyValuePair<TKey, TValue>(key, value);
                dictionary.Add(kvp);
                OnCollectionChanged(NotifyCollectionChangedAction.Add, kvp);
                AddNotifyEvents(value);
            }
            else
            {
                dispatcher.Invoke(new Action(() => Add(key, value)));
            }
        }

        public void Clear()
        {
            if (dispatcher.CheckAccess())
            {
                foreach (var value in Values)
                {
                    RemoveNotifyEvents(value);
                }

                dictionary.Clear();
                OnCollectionChanged(NotifyCollectionChangedAction.Reset);
            }
            else
            {
                dispatcher.Invoke(new Action(Clear));
            }
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return dictionary.Contains(item);
        }

        public bool ContainsKey(TKey key)
        {
            return dictionary.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            dictionary.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (dispatcher.CheckAccess())
            {
                bool result = dictionary.Remove(item);
                if (result)
                {
                    RemoveNotifyEvents(item.Value);
                    OnCollectionChanged(NotifyCollectionChangedAction.Remove, item);
                }

                return result;
            }
            else
            {
                return (bool)dispatcher.Invoke(() => Remove(item));
            }
        }

        public bool Remove(TKey key)
        {
            if (dispatcher.CheckAccess())
            {
                if (dictionary.ContainsKey(key))
                {
                    var pair = dictionary.Single(kvp => kvp.Key.Equals(key));
                    TValue value = dictionary[key];
                    dictionary.Remove(key);
                    RemoveNotifyEvents(value);
                    OnCollectionChanged(NotifyCollectionChangedAction.Remove, pair);

                    return true;
                }

                return false;
            }
            else
            {
                return (bool)dispatcher.Invoke(() => Remove(key));
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return dictionary.TryGetValue(key, out value);
        }

        #region Enumerator

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion Enumerator

        #region Notify

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, e);
            }
        }

        protected void OnCollectionChanged(NotifyCollectionChangedAction action)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(action));
            }
        }

        protected void OnCollectionChanged(NotifyCollectionChangedAction action, KeyValuePair<TKey, TValue> kvp)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, kvp));
            }
        }

        protected void OnCollectionChanged(NotifyCollectionChangedAction action, KeyValuePair<TKey, TValue> newKvp, KeyValuePair<TKey, TValue> oldKvp)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, newKvp, oldKvp));
            }
        }

        protected void OnPropertyChanged(String property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }

        #endregion Notify

        #region Private

        private void AddNotifyEvents(TValue item)
        {
            if (item is INotifyCollectionChanged)
            {
                (item as INotifyCollectionChanged).CollectionChanged += OnCollectionChangedEventHandler;
            }
            if (item is INotifyPropertyChanged)
            {
                (item as INotifyPropertyChanged).PropertyChanged += OnPropertyChangedEventHandler;
            }
        }

        private void OnCollectionChangedEventHandler(object s, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("value");
        }

        private void OnPropertyChangedEventHandler(object s, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e);
        }

        private void RemoveNotifyEvents(TValue item)
        {
            if (item is INotifyCollectionChanged)
            {
                (item as INotifyCollectionChanged).CollectionChanged -= OnCollectionChangedEventHandler;
            }
            if (item is INotifyPropertyChanged)
            {
                (item as INotifyPropertyChanged).PropertyChanged -= OnPropertyChangedEventHandler;
            }
        }

        #endregion Private
    }
}