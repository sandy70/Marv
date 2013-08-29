using System.Collections.Generic;
using System.ComponentModel;

namespace LibNetwork
{
    public class LocationValue : Dictionary<int, OldModelValue>, INotifyPropertyChanged
    {
        private long id;

        public event PropertyChangedEventHandler PropertyChanged;

        public long Id
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

        public OldModelValue GetModelValue(int year)
        {
            if (this.ContainsKey(year))
            {
                return this[year];
            }
            else
            {
                return this[year] = new OldModelValue();
            }
        }

        public override string ToString()
        {
            return base.ToString() + " Id: " + this.Id;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}