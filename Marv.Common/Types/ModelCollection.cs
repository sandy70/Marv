using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace Marv.Common
{
    public class ModelCollection<T> : ObservableCollection<T>, IModel where T : IModel
    {
        private bool isEnabled;
        private bool isSelected;
        private string key;
        private string name;
        private Dictionary<string, object> properties = new Dictionary<string, object>();

        public ModelCollection()
        {
        }

        public ModelCollection(IEnumerable<T> items)
            : base(items)
        {
        }

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
                return this.FirstOrDefault(item => item.IsSelected);
            }
        }

        public IEnumerable<T> SelectedItems
        {
            get
            {
                return this.Where(item => item.IsSelected);
            }
        }

        public T this[string aKey]
        {
            get
            {
                return this.First(item => item.Key == aKey);
            }
        }

        public IEnumerable<T> this[IEnumerable<string> keys]
        {
            get
            {
                return keys.Select(aKey => this[aKey]);
            }
        }

        public bool Contains(string aKey)
        {
            return this.Count(item => item.Key == aKey) > 0;
        }

        public int IndexOf(string aKey)
        {
            return this.IndexOf(this[aKey]);
        }

        public void Select(string theKey)
        {
            if (this.Contains(theKey))
            {
                this.UnselectAll();
                this[theKey].IsSelected = true;
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
    }
}