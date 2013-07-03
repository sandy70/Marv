using System.ComponentModel;

namespace LibPipeline
{
    public class Location : Dynamic
    {
        private double latitude;
        private double longitude;

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
    }
}