using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Marv.Common
{
    public class ViewModelCollection<T> : SelectableCollection<T> where T : class, IViewModel
    {
        public ViewModelCollection()
            : base()
        {
        }

        public ViewModelCollection(IEnumerable<T> items)
            : base(items)
        {
        }

        public T this[string key]
        {
            get
            {
                return this.dictionary[key];
            }

            set
            {
                this.dictionary[key] = value;
            }
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);

            if (e.NewItems != null)
            {
                foreach (var newItem in e.NewItems)
                {
                    this.dictionary[(newItem as T).Key] = newItem as T;
                }

                if (this.Count == 1)
                {
                    this.SelectedItem = this.First();
                }
            }

            else if (e.OldItems != null)
            {
                bool selectedRemoved = false;

                foreach (var item in e.OldItems)
                {
                    this.dictionary.Remove((item as T).Key);

                    if (item.Equals(this.SelectedItem))
                    {
                        selectedRemoved = true;
                    }
                }

                if (this.Count > 0 && selectedRemoved == true)
                {
                    this.SelectedItem = this.First();
                }
                else
                {
                    this.SelectedItem = default(T);
                }
            }
        }
    }
}