using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Marv
{
    public class SelectableCollection<T> : ObservableCollection<T>, IModel where T : class
    {
        protected Dictionary<string, T> dictionary = new Dictionary<string, T>();
        private bool isEnabled;
        private bool isSelected;
        private string key = "";
        private string name = "";
        private Dynamic properties = new Dynamic();
        private T selectedItem;

        public SelectableCollection(IEnumerable<T> items)
            : base(items)
        {
        }

        public IEnumerable<string> Keys
        {
            get
            {
                return this.dictionary.Keys;
            }
        }

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

        public T SelectedItem
        {
            get
            {
                return this.selectedItem;
            }

            set
            {
                if (value == null)
                {
                    this.selectedItem = value;
                    this.RaisePropertyChanged("SelectedItem");
                }
                else
                {
                    if (value.Equals(this.SelectedItem))
                    {
                        // do nothing
                    }
                    else
                    {
                        if (this.Contains(value))
                        {
                            // do nothing
                        }
                        else
                        {
                            this.Add(value);
                        }

                        this.selectedItem = value;
                        this.RaisePropertyChanged("SelectedItem");
                    }
                }
            }
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

        protected void RaisePropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        public bool ContainsKey(string key)
        {
            return this.dictionary.ContainsKey(key);
        }
    }
}