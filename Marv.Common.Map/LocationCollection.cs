using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows.Media;

namespace Marv.Common.Map
{
    public class LocationCollection : ViewModelCollection<Location>
    {
        // ReSharper disable once InconsistentNaming
        private Dictionary<string, double> _value = new Dictionary<string, double>();
        private LocationRect bounds;
        private Brush stroke = new SolidColorBrush(Colors.LightBlue);

        public LocationCollection()
        {
        }

        public LocationCollection(IEnumerable<Location> locations)
            : base(locations)
        {
        }

        /// <summary>
        ///     The geographics bounds of this collection of Locations. Bounds is updated everytime the collection changes.
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
                    this.RaisePropertyChanged("GetBounds");
                }
            }
        }

        public Brush Stroke
        {
            get
            {
                return this.stroke;
            }

            set
            {
                if (value != this.stroke)
                {
                    this.stroke = value;
                    this.RaisePropertyChanged("Stroke");
                }
            }
        }

        public Dictionary<string, double> Value
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

                    foreach (var location in this)
                    {
                        location.Value = this.Value[location.Name];
                    }

                    if (this.ValueChanged != null)
                    {
                        this.ValueChanged(this, new EventArgs());
                    }
                }
            }
        }

        public event EventHandler ValueChanged;

        // Everytime the collection is changed, the bounds are updated.
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
    }
}