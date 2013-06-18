using System.ComponentModel;

namespace LibPipeline
{
    public class PropertyLocation : Dynamic, ILocation, INotifyPropertyChanged
    {
        private int id;
        private double latitude;
        private double longitude;

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