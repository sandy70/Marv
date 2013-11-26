using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;
using System.Linq;

namespace Marv.Common
{
    public class Dict<T1, T2, TValue> : Dict<T1, Dict<T2, TValue>> where TValue : new()
    {
        public TValue this[T1 key1, T2 key2]
        {
            get
            {
                if (this.ContainsKey(key1))
                {
                    if (this[key1].ContainsKey(key2))
                    {
                        return this[key1][key2];
                    }
                    else
                    {
                        return this[key1][key2] = new TValue();
                    }
                }
                else
                {
                    this[key1] = new Dict<T2, TValue>();
                    return this[key1][key2] = new TValue();
                }
            }

            set
            {
                if (this.ContainsKey(key1))
                {
                    this[key1][key2] = value;
                }
                else
                {
                    this[key1] = new Dict<T2, TValue>();
                    this[key1][key2] = value;
                }
            }
        }
    }

    public class Dict<T1, T2, T3, TValue> : Dict<T1, Dict<T2, T3, TValue>> where TValue : new() { }

    public class Dict<T1, T2, T3, T4, TValue> : Dict<T1, Dict<T2, T3, T4, TValue>> where TValue : new() { }

    public class Dict<TKey, TValue> : Dictionary<TKey, TValue> { }

    public class Dict1<TKey, TValue> : Dictionary<TKey, TValue>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private Dispatcher dispatcher;

        #region Constructor

        public Dict1(Dispatcher dispatcher = null)
            : base()
        {
            this.dispatcher = dispatcher ??
                              (Application.Current != null
                                   ? Application.Current.Dispatcher
                                   : Dispatcher.CurrentDispatcher);
        }

        public Dict1(IDictionary<TKey, TValue> dictionary)
            : base(dictionary)
        {
        }

        public Dict1(int capacity)
            : base(capacity)
        {
        }

        #endregion Constructor

        public new TValue this[TKey key]
        {
            get
            {
                return base[key];
            }

            set
            {
                if (this.ContainsKey(key))
                {
                    var oldKvp = this.Single(kvp => kvp.Key.Equals(key));

                    this.RemoveNotifyEvents(this[key]);
                    base[key] = value;
                    this.AddNotifyEvents(this[key]);

                    var newKvp = this.Single(kvp => kvp.Key.Equals(key));

                    this.OnCollectionChanged(NotifyCollectionChangedAction.Replace, newKvp, oldKvp);
                }
                else
                {
                    base[key] = value;
                    this.AddNotifyEvents(this[key]);

                    var pair = this.Single(kvp => kvp.Key.Equals(key));
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

        public new void Add(TKey key, TValue value)
        {
            if (dispatcher.CheckAccess())
            {
                var kvp = new KeyValuePair<TKey, TValue>(key, value);
                base.Add(key, value);
                OnCollectionChanged(NotifyCollectionChangedAction.Add, kvp);
                AddNotifyEvents(value);
            }
            else
            {
                dispatcher.Invoke(new Action(() => Add(key, value)));
            }
        }

        public new void Clear()
        {
            if (dispatcher.CheckAccess())
            {
                foreach (var value in Values)
                {
                    RemoveNotifyEvents(value);
                }

                base.Clear();
                OnCollectionChanged(NotifyCollectionChangedAction.Reset);
            }
            else
            {
                dispatcher.Invoke(new Action(Clear));
            }
        }

        public new bool Remove(TKey key)
        {
            if (dispatcher.CheckAccess())
            {
                if (this.ContainsKey(key))
                {
                    var pair = this.Single(kvp => kvp.Key.Equals(key));
                    TValue value = this[key];
                    this.Remove(key);
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