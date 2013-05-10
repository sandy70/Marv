using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibPipeline
{
    public class PropertyLocation : Dynamic, ILocation, INotifyPropertyChanged
    {
        private double latitude;
        private double longitude;

        public event PropertyChangedEventHandler PropertyChanged;

        public double Latitude
        {
            get
            {
                return this.latitude;
            }

            set
            {
                if (value != this.latitude)
                {
                    this.latitude = value;

                    this.OnPropertyChanged("Latitude");
                }
            }
        }

        public double Longitude
        {
            get
            {
                return this.longitude;
            }

            set
            {
                if (value != this.longitude)
                {
                    this.longitude = value;

                    this.OnPropertyChanged("Longitude");
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
