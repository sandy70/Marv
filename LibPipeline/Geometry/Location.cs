using System;

namespace LibPipeline
{
    public class Location : Dynamic
    {
        private double _value;
        private Guid guid;
        private double latitude;
        private double longitude;
        private string name;

        public Guid Guid
        {
            get
            {
                return this.guid;
            }

            set
            {
                if (value != this.guid)
                {
                    this.guid = value;
                    this.OnPropertyChanged("Guid");
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

        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                if (value != this.name)
                {
                    this.name = value;
                    this.OnPropertyChanged("Name");
                }
            }
        }

        public double Value
        {
            get
            {
                return this._value;
            }

            set
            {
                if (value != this._value)
                {
                    this._value = value;
                    this.OnPropertyChanged("Value");
                }
            }
        }

        public override string ToString()
        {
            return this.Latitude + ", " + this.Longitude;
        }
    }
}