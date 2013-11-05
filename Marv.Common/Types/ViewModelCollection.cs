using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Linq;

namespace Marv.Common
{
    public class ViewModelCollection<T> : ObservableCollection<T>, IViewModel where T : class, IViewModel
    {
        private string key = "";
        private string name = "";
        private Dictionary<string, T> dictionary = new Dictionary<string, T>();
        private T selectedItem = default(T);
        private Dynamic properties = new Dynamic();

        public Dynamic Properties
        {
            get
            {
                return this.properties;
            }

            set
            {
                if (value != this.properties)
                {
                    this.properties = value;
                    this.RaisePropertyChanged("Properties");
                }
            }
        }

        public ViewModelCollection()
            : base()
        {
        }

        public ViewModelCollection(IEnumerable<T> items)
            : base(items)
        {
            
        }

        public event ValueEventHandler<T> SelectionChanged;

        public string Key
        {
            get
            {
                return this.key;
            }

            set
            {
                if (value != this.key)
                {
                    this.key = value;
                    this.RaisePropertyChanged("Key");
                }
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                if (value != this.name)
                {
                    this.name = value;
                    this.RaisePropertyChanged("Name");
                }
            }
        }

        public T SelectedItem
        {
            get
            {
                return this.selectedItem;
            }

            set
            {
                if (this.Contains(value))
                {
                    if (!value.Equals(this.selectedItem))
                    {
                        this.selectedItem = value;
                        this.RaisePropertyChanged("SelectedItem");
                        this.RaiseSelectionChanged();
                    }
                }
                else
                {
                    throw new ArgumentException("The value provided for SelectedItem does not exist in the collection.");
                }
            }
        }

        public object this[string key]
        {
            get
            {
                return this.dictionary[key];
            }

            set
            {
                this.dictionary[key] = value as T;
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

        protected void RaisePropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        protected void RaiseSelectionChanged()
        {
            if (this.SelectionChanged != null)
            {
                this.SelectionChanged(this, new ValueEventArgs<T> { Value = this.SelectedItem });
            }
        }
    }
}