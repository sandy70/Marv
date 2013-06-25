using LibBn;
using LibPipeline;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Marv
{
    public class LocationValueStore : INotifyPropertyChanged
    {
        private NearNeutralPhSccModel model;

        public event PropertyChangedEventHandler PropertyChanged;

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

            var filesPerFolder = 1000;
            var extension = ".db";

            var dataBase = new ObjectDataBase<LocationValue>()
            {
                FileNamePredicate = () =>
                    {
                        string folderName = (location.Id / filesPerFolder).ToString();
                        string fileName = (location.Id % filesPerFolder).ToString() + extension;

                        return Path.Combine(folderName, fileName);
                    }
            };

            var locationValues = await dataBase.GetValuesAsync(x => x.Id == location.Id);

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

                dataBase.StoreAsync(locationValue);
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