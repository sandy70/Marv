using LibPipeline;
using NDatabase;
using NDatabase.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Marv
{
    public class ObjectDataBase<T> : ViewModel where T : class
    {
        protected object _lock = new object();

        private Func<string> fileNamePredicate = new Func<string>(() => "");

        public string FileName
        {
            get
            {
                return this.fileNamePredicate();
            }

            set
            {
                this.FileNamePredicate = new Func<string>(() => value);
                OnPropertyChanged("FileName");
            }
        }

        public Func<string> FileNamePredicate
        {
            get
            {
                return this.fileNamePredicate;
            }

            set
            {
                if (value != this.fileNamePredicate)
                {
                    this.fileNamePredicate = value;
                    this.OnPropertyChanged("FileNamePredicate");
                }
            }
        }

        public IEnumerable<T> ReadValues(Func<T, bool> predicate)
        {
            lock (this._lock)
            {
                IEnumerable<T> locationValues = null;

                try
                {
                    using (var odb = OdbFactory.Open(this.FileName))
                    {
                        locationValues = odb.AsQueryable<T>().Where(predicate).ToList();
                    }
                }
                catch(OdbRuntimeException exp)
                {
                    // We are having these problems when the file is corrupt.
                    // So let's delete the file and try again
                    // TODO
                }

                return locationValues;
            }
        }

        public Task<IEnumerable<T>> ReadValuesAsync(Func<T, bool> predicate)
        {
            return Task.Run(() => this.ReadValues(predicate));
        }

        public void Write(T anObject)
        {
            lock (this._lock)
            {
                using (var odb = OdbFactory.Open(this.FileName))
                {
                    Console.WriteLine("Storing: " + anObject);
                    odb.Store<T>(anObject);
                    Console.WriteLine("Stored: " + anObject);
                }
            }
        }

        public Task WriteAsync(T anObject)
        {
            return Task.Run(() => this.Write(anObject));
        }
    }
}