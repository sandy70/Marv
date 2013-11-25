﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Marv.Common
{
    public class SelectableCollection<T> : ObservableCollection<T>, IViewModel where T : class
    {
        protected Dictionary<string, T> dictionary = new Dictionary<string, T>();
        private string key = "";
        private IEnumerable<string> keys;
        private string name = "";
        private Dynamic properties = new Dynamic();
        private T selectedItem = default(T);

        public SelectableCollection()
            : base()
        {
        }

        public SelectableCollection(IEnumerable<T> items)
            : base(items)
        {
        }

        public event EventHandler<T> SelectionChanged;

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

        public IEnumerable<string> Keys
        {
            get
            {
                return this.dictionary.Keys;
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
                if (value == null || !value.Equals(this.selectedItem))
                {
                    this.selectedItem = value;
                    this.RaisePropertyChanged("SelectedItem");
                    this.RaiseSelectionChanged();
                }

                if (!this.Contains(value))
                {
                    this.Add(value);
                }
            }
        }

        public bool ContainsKey(string key)
        {
            return this.dictionary.ContainsKey(key);
        }

        protected void RaisePropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        protected void RaiseSelectionChanged()
        {
            if (this.SelectionChanged != null)
            {
                this.SelectionChanged(this, this.SelectedItem);
            }
        }
    }
}