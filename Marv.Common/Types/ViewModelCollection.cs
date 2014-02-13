using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Marv.Common
{
    public class ViewModelCollection<T> : ObservableCollection<T>, IViewModel where T : class, IViewModel
    {
        private bool isEnabled;
        private bool isSelected = false;
        private string key;
        private string name;
        private Dictionary<string, object> properties = new Dictionary<string, object>();

        public ViewModelCollection()
            : base()
        {
        }

        public ViewModelCollection(IEnumerable<T> items)
            : base(items)
        {
        }

        public event EventHandler<T> SelectionChanged;

        public bool IsEnabled
        {
            get
            {
                return this.isEnabled;
            }

            set
            {
                if (value != this.isEnabled)
                {
                    this.isEnabled = value;
                    this.RaisePropertyChanged("IsEnabled");
                }
            }
        }

        public bool IsSelected
        {
            get
            {
                return this.isSelected;
            }

            set
            {
                if (value != this.isSelected)
                {
                    this.isSelected = value;
                    this.RaisePropertyChanged("IsSelected");
                }
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

        public Dictionary<string, object> Properties
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

        public T SelectedItem
        {
            get
            {
                return this.Where(item => item.IsSelected).FirstOrDefault();
            }
        }

        public IEnumerable<T> SelectedItems
        {
            get
            {
                return this.Where(item => item.IsSelected);
            }
        }

        public T this[string key]
        {
            get
            {
                return this.Where(item => item.Key == key).First();
            }
        }

        public IEnumerable<T> this[IEnumerable<string> keys]
        {
            get
            {
                return keys.Select(key => this[key]);
            }
        }

        public bool Contains(string key)
        {
            if (this.Where(item => item.Key == key).Count() > 0) return true;
            else return false;
        }

        public int IndexOf(string key)
        {
            return this.IndexOf(this[key]);
        }

        public void Select(string key)
        {
            if (this.Contains(key))
            {
                this.UnselectAll();
                this[key].IsSelected = true;
                this.RaisePropertyChanged("SelectedItem");
            }
            else
            {
                throw new ItemNotFoundException("Given item not found in this collection.");
            }
        }

        public void Select(T item)
        {
            if (this.Contains(item))
            {
                this.UnselectAll();
                item.IsSelected = true;
                this.RaisePropertyChanged("SelectedItem");
            }
            else
            {
                throw new ItemNotFoundException("Given item not found in this collection.");
            }
        }

        public void UnselectAll()
        {
            foreach (var item in this.SelectedItems)
            {
                item.IsSelected = false;
            }
        }

        protected void RaisePropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        private void RaiseSelectionChanged(T item)
        {
            if (this.SelectionChanged != null)
            {
                this.SelectionChanged(this, item);
            }
        }
    }
}