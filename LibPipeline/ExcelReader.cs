﻿using LinqToExcel;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;

namespace LibPipeline
{
    public class ExcelReader
    {
        public static IEnumerable<ILocation> ReadLocations<TLocation>(string fileName, string sheetName = "Sheet1") where TLocation : ILocation, new()
        {
            var excel = new ExcelQueryFactory(fileName);
            var locations = new List<TLocation>();

            var colNames = excel.GetColumnNames(sheetName);

            foreach (var row in excel.Worksheet(sheetName))
            {
                if (DBNull.Value.Equals(row["Latitude"].Value) || DBNull.Value.Equals(row["Longitude"].Value))
                {
                    continue;
                }

                var location = new TLocation();

                foreach (var colName in colNames)
                {
                    if (colName.Equals("Latitude"))
                    {
                        location.Latitude = (double)row[colName].Value;
                    }
                    else if (colName.Equals("Longitude"))
                    {
                        location.Longitude = (double)row[colName].Value;
                    }
                }

                locations.Add(location);
            }

            return locations as IEnumerable<ILocation>;
        }

        public static Task<IEnumerable<ILocation>> ReadLocationsAsync<TLocation>(string fileName, string sheetName = "Sheet1") where TLocation : ILocation, new()
        {
            return Task.Run(() => ExcelReader.ReadLocations<TLocation>(fileName, sheetName));
        }

        public static IEnumerable<ILocation> ReadPropertyLocations<TLocation>(string fileName, string sheetName = "Sheet1") where TLocation : PropertyLocation, IDynamicMetaObjectProvider, new()
        {
            var excel = new ExcelQueryFactory(fileName);
            var id = 0;
            var locations = new List<TLocation>();

            var colNames = excel.GetColumnNames(sheetName);

            foreach (var row in excel.Worksheet(sheetName))
            {
                if (DBNull.Value.Equals(row["Latitude"].Value) || DBNull.Value.Equals(row["Longitude"].Value))
                {
                    continue;
                }

                var location = new TLocation();
                location.Id = id++;

                foreach (var colName in colNames)
                {
                    if (colName.Equals("Latitude"))
                    {
                        location.Latitude = (double)row[colName].Value;
                    }
                    else if (colName.Equals("Longitude"))
                    {
                        location.Longitude = (double)row[colName].Value;
                    }
                    else
                    {
                        (location as dynamic)[colName] = row[colName].Value;
                    }
                }

                locations.Add(location);
            }

            return locations as IEnumerable<ILocation>;
        }

        public static Task<IEnumerable<ILocation>> ReadPropertyLocationsAsync<TLocation>(string fileName, string sheetName = "Sheet1") where TLocation : PropertyLocation, IDynamicMetaObjectProvider, new()
        {
            return Task.Run(() => ExcelReader.ReadPropertyLocations<TLocation>(fileName, sheetName));
        }
    }
}