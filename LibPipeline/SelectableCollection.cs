using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace LibPipeline
{
    public class SelectableCollection<T> : ObservableCollection<T>
    {
        private bool isFirstSelectedOnAdd = true;
        private T selectedItem = default(T);

        public bool IsFirstSelectedOnAdd
        {
            get
            {
                return this.isFirstSelectedOnAdd;
            }

            set
            {
                if (value != this.isFirstSelectedOnAdd)
                {
                    this.isFirstSelectedOnAdd = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("IsFirstSelectedOnAdd"));
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
                this.selectedItem = value;
                this.OnPropertyChanged(new PropertyChangedEventArgs("SelectedItem"));
            }
        }

        public void RemoveSelected()
        {
            int removedIndex = -1;

            if (this.SelectedItem != null)
            {
                removedIndex = this.IndexOf(this.SelectedItem);
                this.Remove(this.SelectedItem);
            }
            else
            {
                if (this.Count > 0)
                {
                    removedIndex = 0;
                    this.RemoveAt(0);
                }
            }

            if (removedIndex >= 0)
            {
                if (this.Count > removedIndex)
                {
                    this.SelectedItem = this[removedIndex];
                }
                else
                {
                    if (this.Count > 0)
                    {
                        this.SelectedItem = this.Last();
                    }
                    else
                    {
                        this.SelectedItem = default(T);
                    }
                }
            }
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);

            if (!this.IsFirstSelectedOnAdd)
            {
                return;
            }

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
    }
}