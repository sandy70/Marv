using LinqToExcel;
using MapControl;
using System;
using System.Collections.Generic;

namespace LibPipeline
{
    public class MultiLocationReader
    {
        public static List<Location> ReadExcel(string fileName, string sheetName = "Sheet1")
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

                var location = new MapControl.Location();

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
    }
}