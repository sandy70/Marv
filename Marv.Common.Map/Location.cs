namespace Marv.Map
{
    public class Location : NotifyPropertyChanged, IKeyed
    {
        private string key;
        private double latitude;
        private double longitude;
        private double value;

        public double Latitude
        {
            get
            {
                return this.latitude;
            }

            set
            {
                this.latitude = value;

                this.RaisePropertyChanged();
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
                this.RaisePropertyChanged();
            }
        }

        public double Value
        {
            get
            {
                return this.value;
            }

            set
            {
                if (value.Equals(this.value))
                {
                    return;
                }

                this.value = value;
                this.RaisePropertyChanged();
            }
        }

        public string Key
        {
            get
            {
                return this.key;
            }

            set
            {
                if (value.Equals(this.key))
                {
                    return;
                }

                this.key = value;
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
                Latitude = mLocation.Latitude,
                Longitude = mLocation.Longitude
            };
        }
    }
}