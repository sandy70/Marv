﻿using NDatabase;
using NDatabase.Exceptions;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Marv.Common
{
    public static class Odb
    {
        public static Logger Logger = LogManager.GetCurrentClassLogger();

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
            catch (OdbRuntimeException)
            {
                // We are having these problems when the file is corrupt.
                // So let's delete the file and try again
                // TODO
            }

            return locationValues;
        }

        public static T ReadValueSingle<T>(string fileName, Func<T, bool> predicate) where T : class
        {
            try
            {
                using (var odb = OdbFactory.Open(fileName))
                {
                    try
                    {
                        return odb.AsQueryable<T>().Where(predicate).Single();
                    }
                    catch (InvalidOperationException exp)
                    {
                        Logger.Error("The file " + fileName + " did not contain any data of type " + typeof(T));

                        throw new OdbDataNotFoundException("The file " + fileName + " did not contain any data of type " + typeof(T), exp);
                    }
                }
            }
            catch (OdbRuntimeException exp)
            {
                Logger.Error("Something wrong with file {0}. Try deleting and rerunning.", fileName);

                throw new OdbDataNotFoundException("The file " + fileName + " may be corrupted. Try deleting and re-running.", exp);
            }
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
                Logger.Info("Storing: {0}", anObject);
                odb.Store<T>(anObject);
                Logger.Info("Stored: {0}", anObject);
            }
        }
    }
}