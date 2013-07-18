using LibBn;
using LibPipeline;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Marv
{
    public class LocationValueStore : ViewModel
    {
        public async Task<LocationValue> GetLocationValueAsync(Location location)
        {
            Console.WriteLine("LocationValueStore: getting location value with id: " + location.Guid.ToInt64());

            var filesPerFolder = 1000;
            var extension = ".db";

            var dataBase = new ObjectDataBase<LocationValue>()
            {
                FileNamePredicate = () =>
                    {
                        string folderName = (location.Guid.ToInt64() / filesPerFolder).ToString();
                        string fileName = (location.Guid.ToInt64() % filesPerFolder).ToString() + extension;

                        return Path.Combine(folderName, fileName);
                    }
            };

            var locationValues = await dataBase.ReadValuesAsync(x => x.Id == location.Guid.ToInt64());

            LocationValue locationValue = null;

            if (locationValues.Count() > 0)
            {
                Console.WriteLine("LocationValueStore: location value for id: " + location.Guid.ToInt64() + " found in database.");
                locationValue = locationValues.First();
            }
            else
            {
                Console.WriteLine("LocationValueStore: location value for id: " + location.Guid.ToInt64() + " NOT found in database.");
            }

            return locationValue;
        }
    }
}