using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace LibPipeline
{
    public class SelectableCollection<T> : ObservableCollection<T>
    {
        private Dictionary<string, object> dictionary = new Dictionary<string, object>();
        private T selectedItem = default(T);
        private ValueEventHandler<T> selectionChanged;

        public event ValueEventHandler<T> SelectionChanged
        {
            add
            {
                if (this.selectionChanged == null || !this.selectionChanged.GetInvocationList().Contains(value))
                {
                    this.selectionChanged += value;
                }
            }
            remove
            {
                this.selectionChanged -= value;
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
                this.selectedItem = value;
                this.OnPropertyChanged(new PropertyChangedEventArgs("SelectedItem"));

                this.RaiseSelectionChanged();
            }
        }

        public object this[string name]
        {
            get
            {
                if (this.GetType().GetProperties().Where(info => info.Name.Equals(name)).Count() == 0)
                {
                    if (dictionary.ContainsKey(name))
                    {
                        return dictionary[name];
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return this.GetType().GetProperty(name).GetValue(this);
                }
            }

            set
            {
                if (this.GetType().GetProperties().Where(info => info.Name.Equals(name)).Count() == 0)
                {
                    dictionary[name] = value;
                }
                else
                {
                    this.GetType().GetProperty(name).SetValue(this, value);
                }

                this.OnPropertyChanged(new PropertyChangedEventArgs(name));
            }
        }

        public void RaisePropertyChanged(string propertyName)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);

            if (e.NewItems != null && this.Count == 1)
            {
                this.SelectedItem = this.First();
            }

            else if (e.OldItems != null)
            {
                bool selectedRemoved = false;

                foreach (var item in e.OldItems)
                {
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

        private void RaiseSelectionChanged()
        {
            if (this.selectionChanged != null)
            {
                this.selectionChanged(this, new ValueEventArgs<T> { Value = this.SelectedItem });
            }
        }
    }
}