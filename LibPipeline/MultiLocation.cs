using MapControl;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace LibPipeline
{
    public class MultiLocation : Dynamic, INotifyPropertyChanged
    {
        private IEnumerable<Location> locations;

        private Location selectedLocation;

        public event PropertyChangedEventHandler PropertyChanged;

        public IEnumerable<Location> Locations
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
                    this.OnPropertyChanged("Locations");

                    this.SelectedLocation = locations.FirstOrDefault();
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