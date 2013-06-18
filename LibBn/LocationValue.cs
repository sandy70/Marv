using System.Collections.Generic;
using System.ComponentModel;

namespace LibBn
{
    public class LocationValue : Dictionary<int, ModelValue>, INotifyPropertyChanged
    {
        private int id;

        public event PropertyChangedEventHandler PropertyChanged;

        public int Id
        {
            get
            {
                return this.id;
            }

            set
            {
                if (value != this.id)
                {
                    this.id = value;

                    this.OnPropertyChanged("Id");
                }
            }
        }

        public ModelValue GetModelValue(int year)
        {
            if (this.ContainsKey(year))
            {
                return this[year];
            }
            else
            {
                return this[year] = new ModelValue();
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}