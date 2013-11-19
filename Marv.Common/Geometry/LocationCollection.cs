using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Media;

namespace Marv.Common
{
    public class LocationCollection : ViewModelCollection<Location>
    {
        private Dict<string, double> _value = new Dict<string, double>();
        private LocationRect bounds;
        private bool isEnabled = true;
        private bool isSelected = false;
        private Brush stroke = new SolidColorBrush(Colors.LightBlue);

        public LocationCollection()
            : base()
        {
        }

        public LocationCollection(IEnumerable<Location> locations)
            : base(locations)
        {
        }

        public event EventHandler ValueChanged;

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

        public bool IsEnabled
        {
            get
            {
                return this.isEnabled;
            }

            set
            {
                if (value != this.isEnabled)
                {
                    this.isEnabled = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("IsEnabled"));

                    if (!this.IsEnabled)
                    {
                        this.IsSelected = false;
                    }
                }
            }
        }

        public bool IsSelected
        {
            get
            {
                return this.isSelected;
            }

            set
            {
                if (this.IsEnabled)
                {
                    this.isSelected = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("IsSelected"));
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

        public Dict<string, double> Value
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
                if (location.IsWithin(this.Bounds))
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