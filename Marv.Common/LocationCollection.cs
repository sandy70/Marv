using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;

namespace Marv.Common
{
    public class LocationCollection : KeyedCollection<Location>
    {
        private LocationRect bounds;
        private string name;

        /// <summary>
        ///     The geographic bounds of this collection of Locations. Bounds is updated everytime the collection changes.
        /// </summary>
        public LocationRect Bounds
        {
            get { return this.bounds; }

            private set
            {
                if (value != this.bounds)
                {
                    this.bounds = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public string Name
        {
            get { return this.name; }

            set
            {
                if (value.Equals(this.name))
                {
                    return;
                }

                this.name = value;
                this.RaisePropertyChanged();
            }
        }

        public Dict<string, double> Value
        {
            set
            {
                foreach (var key in value.Keys)
                {
                    this[key].Value = value[key];
                }

                this.RaisePropertyChanged();
                this.RaiseValueChanged();
            }
        }

        public LocationCollection() {}

        public LocationCollection(IEnumerable<Location> locations)
        {
            foreach (var location in locations)
            {
                this.Add(location);
            }
        }

        /// <summary>
        ///     Reads a LocationCollection from a CSV file. The file is expected to have 3 columns - Section ID, Lat, Lon. The file
        ///     is assumed to have a header.
        /// </summary>
        /// <param name="path">Full path of the file to read.</param>
        /// <returns>Contents to the file as LocationCollection.</returns>
        public static LocationCollection ReadCsv(string path)
        {
            var locationCollection = new LocationCollection();

            var lines = File.ReadAllLines(path);

            var headers = lines.First()
                               .Trim()
                               .Split(",".ToArray());

            foreach (var line in lines.Skip(1))
            {
                var parts = line.Trim().Split(",".ToCharArray());

                if (parts.Length != headers.Count())
                {
                    continue;
                }

                var location = new Location();

                for (var i = 0; i < headers.Length; i++)
                {
                    if (headers[i].ToLower() == "latitude")
                    {
                        location.Latitude = Convert.ToDouble(parts[i]);
                    }
                    else if (headers[i].ToLower() == "longitude")
                    {
                        location.Longitude = Convert.ToDouble(parts[i]);
                    }
                    else if (headers[i].ToLower() == "section id")
                    {
                        location.Key = parts[i];
                    }
                    else
                    {
                        location[headers[i]] = parts[i].Parse();
                    }
                }

                location["Null"] = "Unknown";
                locationCollection.Add(location);
            }

            return locationCollection;
        }

        public Location GetLocationNearestTo(Location queryLocation)
        {
            return this.MinBy(location => Utils.Distance(location, queryLocation));
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);

            if (e.NewItems != null)
            {
                foreach (var newItem in e.NewItems)
                {
                    this.UpdateBounds(newItem as Location);
                }
            }

            if (e.OldItems != null)
            {
                this.UpdateBounds();
            }
        }

        protected void RaiseValueChanged()
        {
            if (this.ValueChanged != null)
            {
                this.ValueChanged(this, new EventArgs());
            }
        }

        // Everytime the collection is changed, the bounds are updated.
        private void UpdateBounds()
        {
            this.Bounds = null;

            foreach (var location in this)
            {
                this.UpdateBounds(location);
            }
        }

        private void UpdateBounds(Location location)
        {
            if (this.Bounds == null)
            {
                this.Bounds = new LocationRect
                {
                    North = location.Latitude,
                    East = location.Longitude,
                    South = location.Latitude,
                    West = location.Longitude
                };
            }
            else
            {
                if (this.Bounds.Contains(location))
                {
                    // There is nothing to be done
                }
                else
                {
                    this.Bounds.North = Math.Max(this.Bounds.North, location.Latitude);
                    this.Bounds.East = Math.Max(this.Bounds.East, location.Longitude);
                    this.Bounds.South = Math.Min(this.Bounds.South, location.Latitude);
                    this.Bounds.West = Math.Min(this.Bounds.West, location.Longitude);
                }
            }
        }

        public static implicit operator MapControl.LocationCollection(LocationCollection locations)
        {
            return new MapControl.LocationCollection(locations.Select(location => (MapControl.Location) location));
        }

        public event EventHandler ValueChanged;
    }
}