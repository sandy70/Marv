using LinqToExcel;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;

namespace LibPipeline
{
    public class ExcelReader
    {
        public static List<Location> ReadLocations(string fileName, string sheetName = "Sheet1")
        {
            var excel = new ExcelQueryFactory(fileName);
            var locations = new List<Location>();

            var colNames = excel.GetColumnNames(sheetName);

            foreach (var row in excel.Worksheet(sheetName))
            {
                if (DBNull.Value.Equals(row["Latitude"].Value) || DBNull.Value.Equals(row["Longitude"].Value))
                {
                    continue;
                }

                var location = new Location();

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

            return locations;
        }

        public static Task<List<Location>> ReadLocationsAsync<TLocation>(string fileName, string sheetName = "Sheet1")
        {
            return Task.Run(() => ExcelReader.ReadLocations(fileName, sheetName));
        }

        public static List<Location> ReadLocationsWithProperties(string fileName, string sheetName = "Sheet1")
        {
            var excel = new ExcelQueryFactory(fileName);
            var id = 0L;
            var locations = new List<Location>();

            var colNames = excel.GetColumnNames(sheetName);

            foreach (var row in excel.Worksheet(sheetName))
            {
                if (DBNull.Value.Equals(row["Latitude"].Value) || DBNull.Value.Equals(row["Longitude"].Value))
                {
                    continue;
                }

                var location = new Location();
                location.Guid = id.ToGuid();

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

            return locations;
        }

        public static Task<List<Location>> ReadLocationsWithPropertiesAsync(string fileName, string sheetName = "Sheet1")
        {
            return Task.Run(() => ExcelReader.ReadLocationsWithProperties(fileName, sheetName));
        }
    }
}