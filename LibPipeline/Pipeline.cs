using MapControl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace LibPipeline
{
    [Serializable]
    public class Pipeline : INotifyPropertyChanged
    {
        private List<Location> locations = new List<Location>();
        private Location selectedLocation;

        public event PropertyChangedEventHandler PropertyChanged;

        public List<Location> Locations
        {
            get
            {
                return this.locations;
            }

            set
            {
                if (value != this.locations)
                {
                    this.locations = value;

                    if (locations != null && locations.Count > 0)
                    {
                        this.SelectedLocation = locations.First();
                    }
                    else
                    {
                        this.SelectedLocation = null;
                    }

                    this.OnPropertyChanged("Locations");
                }
            }
        }

        public Location SelectedLocation
        {
            get
            {
                return this.selectedLocation;
            }

            set
            {
                if (value != this.selectedLocation)
                {
                    this.selectedLocation = value;
                    this.OnPropertyChanged("SelectedLocation");
                }
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}