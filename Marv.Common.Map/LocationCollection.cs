﻿using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;

namespace Marv.Map
{
    public class LocationCollection : KeyedCollection<Location>
    {
        private LocationRect bounds;

        /// <summary>
        ///     The geographic bounds of this collection of Locations. Bounds is updated everytime the collection changes.
        /// </summary>
        public LocationRect Bounds
        {
            get
            {
                return this.bounds;
            }

            private set
            {
                if (value != this.bounds)
                {
                    this.bounds = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        ///     Reads a LocationCollection from a CSV file. The file is expected to have 3 columns - Section ID, Lat, Lon. The file
        ///     is not expected to have a header.
        /// </summary>
        /// <param name="path">Full path of the file to read.</param>
        /// <returns>Contents to the file as LocationCollection.</returns>
        public static LocationCollection ReadCsv(string path)
        {
            var locationCollection = new LocationCollection();

            var lines = File.ReadAllLines(path);

            foreach (var line in lines)
            {
                var parts = line.Trim().Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length != 3)
                {
                    continue;
                }

                locationCollection.Add(new Location
                {
                    Key = parts[0],
                    Latitude = Convert.ToDouble(parts[1]),
                    Longitude = Convert.ToDouble(parts[2])
                });
            }

            return locationCollection;
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
    }
}