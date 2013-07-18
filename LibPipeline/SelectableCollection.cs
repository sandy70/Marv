using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace LibPipeline
{
    public class SelectableCollection<T> : ObservableCollection<T>, INotifyPropertyChanged
    {
        private T selectedItem = default(T);

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
    }
}