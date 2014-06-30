namespace Marv.Common.Map
{
    public class GeoPoint : Model
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
                this.latitude = value;
                this.RaisePropertyChanged("Latitude");
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
                this.longitude = value;
                this.RaisePropertyChanged("Longitude");
            }
        }
    }
}