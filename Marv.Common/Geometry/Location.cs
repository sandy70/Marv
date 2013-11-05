﻿using System;
using System.Windows;

namespace Marv.Common
{
    public class Location : ViewModel
    {
        private double _value;
        private Guid guid;
        private double latitude;
        private double longitude;

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
                    this.RaisePropertyChanged("Guid");
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

                    this.RaisePropertyChanged("Latitude");
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

                    this.RaisePropertyChanged("Longitude");
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
                    this.RaisePropertyChanged("Value");
                }
            }
        }

        public double X
        {
            get
            {
                return this.Longitude;
            }

            set
            {
                if (value != this.Longitude)
                {
                    this.Longitude = value;
                    this.RaisePropertyChanged("X");
                }
            }
        }

        public double Y
        {
            get
            {
                return this.Latitude;
            }

            set
            {
                if (value != this.Latitude)
                {
                    this.Latitude = value;
                    this.RaisePropertyChanged("Y");
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

        public static implicit operator Location(Point point)
        {
            return new Location
            {
                X = point.X,
                Y = point.Y
            };
        }

        public static Location Parse(string locationString)
        {
            var parts = locationString.Split(" ".ToCharArray());

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

        public Point ToPoint()
        {
            return new Point { X = this.X, Y = this.Y };
        }

        public override string ToString()
        {
            return this.Latitude + ", " + this.Longitude;
        }
    }
}