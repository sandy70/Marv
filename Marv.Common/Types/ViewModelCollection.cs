using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Marv.Common
{
    public class ViewModelCollection<T> : ObservableCollection<T>, IViewModel where T : INotifyPropertyChanged
    {
        private string key;
        private string name;
        private T selectedItem = default(T);

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