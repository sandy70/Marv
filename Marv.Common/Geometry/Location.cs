using System;
using System.Windows;

namespace Marv.Common
{
    public class Location : ViewModel
    {
        private double _value;
        private double latitude;
        private double longitude;

        public double Latitude
        {
            get { return this.latitude; }

            set
            {
                if (value != this.latitude)
                {
                    this.latitude = value;

                    this.RaisePropertyChanged("Latitude");
                }
            }
        }

        public double Longitude
        {
            get { return this.longitude; }

            set
            {
                if (value != this.longitude)
                {
                    this.longitude = value;

                    this.RaisePropertyChanged("Longitude");
                }
            }
        }

        public double Value
        {
            get { return this._value; }

            set
            {
                if (value != this._value)
                {
                    this._value = value;
                    this.RaisePropertyChanged("Value");
                }
            }
        }

        public static implicit operator Location(MapControl.Location mLocation)
        {
            return new Location
            {
                Latitude = mLocation.Latitude,
                Longitude = mLocation.Longitude
            };
        }

        public static Location Parse(string locationString)
        {
            string[] parts = locationString.Split(" ,".ToCharArray());

            return new Location
            {
                Latitude = double.Parse(parts[0]),
                Longitude = double.Parse(parts[1])
            };
        }

        public bool IsWithin(LocationRect rect)
        {
            return this.Latitude > rect.South && this.Latitude < rect.North && this.Longitude > rect.West && this.Longitude < rect.East;
        }

        public override string ToString()
        {
            return this.Latitude + ", " + this.Longitude;
        }
    }
}