using System.ComponentModel;

namespace LibPipeline
{
    [TypeConverter(typeof(LocationRectConverter))]
    public class LocationRect : ViewModel
    {
        private double east;
        private double north;
        private Location northEast = new Location();
        private Location northWest = new Location();
        private double south;
        private Location southEast = new Location();
        private Location southWest = new Location();
        private double west;

        public double East
        {
            get
            {
                return this.east;
            }

            set
            {
                if (value != this.east)
                {
                    this.east = value;
                    this.OnPropertyChanged("East");

                    this.NorthEast.Longitude = this.East;
                    this.SouthEast.Longitude = this.East;
                }
            }
        }

        public double North
        {
            get
            {
                return this.north;
            }

            set
            {
                if (value != this.north)
                {
                    this.north = value;
                    this.OnPropertyChanged("North");

                    this.NorthWest.Latitude = this.North;
                    this.NorthEast.Latitude = this.North;
                }
            }
        }

        public Location NorthEast
        {
            get
            {
                return this.northEast;
            }

            set
            {
                if (value != this.northEast)
                {
                    this.northEast = value;
                    this.OnPropertyChanged("NorthEast");

                    this.North = this.NorthEast.Latitude;
                    this.East = this.NorthEast.Longitude;
                }
            }
        }

        public Location NorthWest
        {
            get
            {
                return this.northWest;
            }

            set
            {
                if (value != this.northWest)
                {
                    this.northWest = value;
                    this.OnPropertyChanged("NorthWest");

                    this.North = this.NorthWest.Latitude;
                    this.West = this.NorthWest.Longitude;
                }
            }
        }

        public double South
        {
            get
            {
                return this.south;
            }

            set
            {
                if (value != this.south)
                {
                    this.south = value;
                    this.OnPropertyChanged("South");

                    this.SouthEast.Latitude = this.South;
                    this.SouthWest.Latitude = this.South;
                }
            }
        }

        public Location SouthEast
        {
            get
            {
                return this.southEast;
            }

            set
            {
                if (value != this.southEast)
                {
                    this.southEast = value;
                    this.OnPropertyChanged("SouthEast");

                    this.South = this.SouthEast.Latitude;
                    this.East = this.SouthEast.Longitude;
                }
            }
        }

        public Location SouthWest
        {
            get
            {
                return this.southWest;
            }

            set
            {
                if (value != this.southWest)
                {
                    this.southWest = value;
                    this.OnPropertyChanged("SouthWest");

                    this.South = this.SouthWest.Latitude;
                    this.West = this.SouthWest.Longitude;
                }
            }
        }

        public double West
        {
            get
            {
                return this.west;
            }

            set
            {
                if (value != this.west)
                {
                    this.west = value;
                    this.OnPropertyChanged("West");

                    this.NorthWest.Longitude = this.West;
                    this.SouthWest.Longitude = this.West;
                }
            }
        }

        public override string ToString()
        {
            string str = "N:" + this.North + " E:" + this.East + " S:" + this.South + " W:" + this.West;
            return base.ToString() + ": " + str;
        }
    }
}