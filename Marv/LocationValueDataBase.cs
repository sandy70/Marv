using LibBn;
using NDatabase;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Marv
{
    public class LocationValueDataBase : INotifyPropertyChanged
    {
        private static string Extension = ".db";
        private static int FilesPerFolder = 1000;
        private object _lock = new object();
        private string fileName = null;

        public event PropertyChangedEventHandler PropertyChanged;

        public string FileName
        {
            get
            {
                return this.fileName;
            }

            set
            {
                if (value != this.fileName)
                {
                    this.fileName = value;
                    this.OnPropertyChanged("FileName");
                }
            }
        }

        public string GetFileName(int id)
        {
            string folderName = (id / LocationValueDataBase.FilesPerFolder).ToString();
            string fileName = (id % LocationValueDataBase.FilesPerFolder).ToString() + LocationValueDataBase.Extension;

            return Path.Combine(folderName, fileName);
        }

        public IEnumerable<LocationValue> GetLocationValues(int id)
        {
            lock (this._lock)
            {
                IEnumerable<LocationValue> locationValues;

                using (var odb = OdbFactory.Open(this.GetFileName(id)))
                {
                    Console.WriteLine("Getting LocationValue with Id: " + id);
                    locationValues = odb.AsQueryable<LocationValue>().Where(x => x.Id == id).ToList();
                    Console.WriteLine("Got " + locationValues.Count() + " LocationValues with Id: " + id);
                }

                return locationValues;
            }
        }

        public Task<IEnumerable<LocationValue>> GetLocationValuesAsync(int id)
        {
            return Task.Run(() => this.GetLocationValues(id));
        }

        public void Store(LocationValue locationValue)
        {
            lock (this._lock)
            {
                using (var odb = OdbFactory.Open(this.GetFileName(locationValue.Id)))
                {
                    Console.WriteLine("Storing LocationValue with Id: " + locationValue.Id);
                    odb.Store<LocationValue>(locationValue);
                    Console.WriteLine("Stored LocationValue with Id: " + locationValue.Id);
                }
            }
        }

        public Task StoreAsync(LocationValue locationValue)
        {
            return Task.Run(() => this.Store(locationValue));
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}