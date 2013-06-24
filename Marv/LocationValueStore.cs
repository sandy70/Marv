using LibBn;
using LibPipeline;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Marv
{
    public class LocationValueStore : INotifyPropertyChanged
    {
        private LocationValueDataBase locationValueDataBase = new LocationValueDataBase();
        private NearNeutralPhSccModel model;

        public event PropertyChangedEventHandler PropertyChanged;

        public LocationValueDataBase LocationValueDataBase
        {
            get
            {
                return this.locationValueDataBase;
            }

            set
            {
                if (value != this.locationValueDataBase)
                {
                    this.locationValueDataBase = value;

                    this.OnPropertyChanged("LocationValueDataBase");
                }
            }
        }

        public NearNeutralPhSccModel Model
        {
            get
            {
                return this.model;
            }

            set
            {
                if (value != this.model)
                {
                    this.model = value;

                    this.OnPropertyChanged("Model");
                }
            }
        }

        public async Task<LocationValue> GetLocationValueAsync(PropertyLocation location)
        {
            Console.WriteLine("LocationValueStore: getting location value with id: " + location.Id);

            var locationValues = await this.LocationValueDataBase.GetLocationValuesAsync(location.Id);
            var locationValue = new LocationValue();

            if (locationValues.Count() > 0)
            {
                Console.WriteLine("LocationValueStore: location value for id: " + location.Id + " found in database.");
                locationValue = locationValues.First();
            }
            else
            {
                Console.WriteLine("LocationValueStore: location value for id: " + location.Id + " NOT found in database.");
                locationValue = await this.Model.RunAsync(location);
                locationValue.Id = location.Id;

                this.LocationValueDataBase.StoreAsync(locationValue);
            }

            return locationValue;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}