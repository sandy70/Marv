using LibPipeline;
using NDatabase;
using NDatabase.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Marv
{
    public static class ObjectDataBase
    {
        public static IEnumerable<T> ReadValues<T>(string fileName, Func<T, bool> predicate) where T : class
        {
            IEnumerable<T> locationValues = null;

            try
            {
                using (var odb = OdbFactory.Open(fileName))
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

        public static void Write<T>(string fileName, T anObject, bool isOverWritten = true) where T : class
        {
            if (isOverWritten)
            {
                // If the file already exists, then delete it
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
            }

            using (var odb = OdbFactory.Open(fileName))
            {
                Console.WriteLine("Storing: " + anObject);
                odb.Store<T>(anObject);
                Console.WriteLine("Stored: " + anObject);
            }
        }
    }
}