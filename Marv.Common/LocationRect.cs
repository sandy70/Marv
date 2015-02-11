using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Marv.Common
{
    [TypeConverter(typeof (LocationRectConverter))]
    public class LocationRect : NotifyPropertyChanged, IEnumerable<Location>
    {
        private double east;
        private double north;
        private Location northEast = new Location();
        private Location northWest = new Location();
        private double south;
        private Location southEast = new Location();
        private Location southWest = new Location();
        private double west;

        public Location Center
        {
            get { return Utils.Mid(this.NorthEast, this.SouthWest); }
        }

        public double East
        {
            get { return this.east; }

            set
            {
                if (value != this.east)
                {
                    this.east = value;
                    this.RaisePropertyChanged();

                    this.NorthEast.Longitude = this.East;
                    this.SouthEast.Longitude = this.East;
                }
            }
        }

        public double MaxDimension
        {
            get { return Math.Max(this.North - this.South, this.East - this.West); }
        }

        public double North
        {
            get { return this.north; }

            set
            {
                if (value != this.north)
                {
                    this.north = value;
                    this.RaisePropertyChanged();

                    this.NorthWest.Latitude = this.North;
                    this.NorthEast.Latitude = this.North;
                }
            }
        }

        public Location NorthEast
        {
            get { return this.northEast; }

            set
            {
                if (value != this.northEast)
                {
                    this.northEast = value;
                    this.RaisePropertyChanged();

                    this.North = this.NorthEast.Latitude;
                    this.East = this.NorthEast.Longitude;
                }
            }
        }

        public Location NorthWest
        {
            get { return this.northWest; }

            set
            {
                if (value != this.northWest)
                {
                    this.northWest = value;
                    this.RaisePropertyChanged();

                    this.North = this.NorthWest.Latitude;
                    this.West = this.NorthWest.Longitude;
                }
            }
        }

        public double South
        {
            get { return this.south; }

            set
            {
                if (value != this.south)
                {
                    this.south = value;
                    this.RaisePropertyChanged();

                    this.SouthEast.Latitude = this.South;
                    this.SouthWest.Latitude = this.South;
                }
            }
        }

        public Location SouthEast
        {
            get { return this.southEast; }

            set
            {
                if (value != this.southEast)
                {
                    this.southEast = value;
                    this.RaisePropertyChanged();

                    this.South = this.SouthEast.Latitude;
                    this.East = this.SouthEast.Longitude;
                }
            }
        }

        public Location SouthWest
        {
            get { return this.southWest; }

            set
            {
                if (value != this.southWest)
                {
                    this.southWest = value;
                    this.RaisePropertyChanged();

                    this.South = this.SouthWest.Latitude;
                    this.West = this.SouthWest.Longitude;
                }
            }
        }

        public double West
        {
            get { return this.west; }

            set
            {
                if (value != this.west)
                {
                    this.west = value;
                    this.RaisePropertyChanged();

                    this.NorthWest.Longitude = this.West;
                    this.SouthWest.Longitude = this.West;
                }
            }
        }

        public static LocationRect Union(LocationRect rect1, LocationRect rect2)
        {
            return new LocationRect
            {
                North = Math.Max(rect1.North, rect2.North),
                East = Math.Max(rect1.East, rect2.East),
                South = Math.Min(rect1.South, rect2.South),
                West = Math.Min(rect1.West, rect2.West)
            };
        }

        public bool Contains(Location location)
        {
            if (location.Latitude < this.South ||
                location.Latitude > this.North ||
                location.Longitude < this.West ||
                location.Longitude > this.East)
            {
                return false;
            }

            return location.Latitude > this.South && location.Latitude < this.North && location.Longitude > this.West && location.Longitude < this.East;
        }

        public IEnumerator<Location> GetEnumerator()
        {
            return (new List<Location>
            {
                this.SouthWest,
                this.NorthWest,
                this.NorthEast,
                this.SouthEast
            }).GetEnumerator();
        }

        public LocationRect GetPadded(double pad)
        {
            return new LocationRect
            {
                North = this.North + pad,
                West = this.West - pad,
                South = this.South - pad,
                East = this.East + pad
            };
        }

        public LocationRect Scale(double scale)
        {
            return new LocationRect
            {
                North = this.Center.Latitude + (this.North - this.Center.Latitude) * scale,
                South = this.Center.Latitude + (this.South - this.Center.Latitude) * scale,
                East = this.Center.Longitude + (this.East - this.Center.Longitude) * scale,
                West = this.Center.Longitude + (this.West - this.Center.Longitude) * scale,
            };
        }

        public override string ToString()
        {
            var str = String.Format("N:{0,9:F4} E:{1,9:F4} S:{2,9:F4} W:{3,9:F4}", this.North, this.East, this.South, this.West);
            return base.ToString() + ": " + str;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (new List<Location>
            {
                this.SouthWest,
                this.NorthWest,
                this.NorthEast,
                this.SouthEast
            }).GetEnumerator();
        }
    }
}