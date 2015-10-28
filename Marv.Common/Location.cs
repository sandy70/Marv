using Marv.Common.Types;

namespace Marv.Common
{
    public class Location : Dynamic, IKeyed
    {
        private string key;
        private double latitude;
        private double longitude;

        public string Key
        {
            get { return this.key; }

            set
            {
                this.key = value;
                this.RaisePropertyChanged();
            }
        }

        public double Latitude
        {
            get { return this.latitude; }

            set
            {
                this.latitude = value;

                this.RaisePropertyChanged();
            }
        }

        public double Longitude
        {
            get { return this.longitude; }

            set
            {
                this.longitude = value;
                this.RaisePropertyChanged();
            }
        }

        public static Location Parse(string locationString)
        {
            var parts = locationString.Split(" ,".ToCharArray());

            return new Location
            {
                Latitude = double.Parse(parts[0]),
                Longitude = double.Parse(parts[1])
            };
        }

        public override string ToString()
        {
            return this.Latitude + ", " + this.Longitude;
        }

        public static implicit operator Location(MapControl.Location mLocation)
        {
            return new Location
            {
                Key = mLocation.Latitude + "," + mLocation.Longitude,
                Latitude = mLocation.Latitude,
                Longitude = mLocation.Longitude
            };
        }

        public static implicit operator MapControl.Location(Location location)
        {
            return new MapControl.Location
            {
                Latitude = location.Latitude,
                Longitude = location.Longitude
            };
        }
    }
}