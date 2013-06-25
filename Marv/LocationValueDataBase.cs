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
        private object _lock = new object();
        private string extension = ".db";
        private int filesPerFolder = 1000;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Extension
        {
            get
            {
                return this.extension;
            }

            set
            {
                if (value != this.extension)
                {
                    this.extension = value;

                    this.OnPropertyChanged("Extension");
                }
            }
        }

        public int FilesPerFolder
        {
            get
            {
                return this.filesPerFolder;
            }

            set
            {
                if (value != this.filesPerFolder)
                {
                    this.filesPerFolder = value;

                    this.OnPropertyChanged("FilesPerFolder");
                }
            }
        }

        public string GetFileName(int id)
        {
            string folderName = (id / this.FilesPerFolder).ToString();
            string fileName = (id % this.FilesPerFolder).ToString() + this.Extension;

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