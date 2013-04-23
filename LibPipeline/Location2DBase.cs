using System.ComponentModel;

namespace LibPipeline
{
    public class Location2DBase : ILocation2D
    {
        private double? latitude;
        private double? longitude;

        public event PropertyChangedEventHandler PropertyChanged;

        public double? Latitude
        {
            get { return latitude; }
            set
            {
                latitude = value;
                OnPropertyChanged("Latitude");
            }
        }

        public double? Longitude
        {
            get { return longitude; }
            set
            {
                longitude = value;
                OnPropertyChanged("Longitude");
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}