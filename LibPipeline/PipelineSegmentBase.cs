using Microsoft.Maps.MapControl.WPF;
using System.ComponentModel;

namespace LibPipeline
{
    public class PipelineSegmentBase : IPipelineSegment, INotifyPropertyChanged
    {
        private ILocation endLocation;
        private ILocation startLocation;

        public event PropertyChangedEventHandler PropertyChanged;

        public ILocation EndLocation
        {
            get
            {
                return this.endLocation;
            }

            set
            {
                if (value != this.endLocation)
                {
                    this.endLocation = value;
                    this.OnPropertyChanged("EndLocation");
                }
            }
        }

        public LocationCollection Locations
        {
            get
            {
                LocationCollection locations = new LocationCollection();
                locations.Add(this.StartLocation.AsLocation());
                locations.Add(this.EndLocation.AsLocation());

                return locations;
            }
        }

        public ILocation StartLocation
        {
            get
            {
                return this.startLocation;
            }

            set
            {
                if (value != this.startLocation)
                {
                    this.startLocation = value;
                    this.OnPropertyChanged("StartLocation");
                }
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}