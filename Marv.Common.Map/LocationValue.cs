using Marv.Map;

namespace Marv
{
    public class LocationValue : NotifyPropertyChanged
    {
        private Location location;
        private double value;

        public Location Location
        {
            get
            {
                return this.location;
            }

            set
            {
                if (value.Equals(this.location))
                {
                    return;
                }

                this.location = value;
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
    }
}