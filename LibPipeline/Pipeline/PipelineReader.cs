using LibConfig;
using LinqToExcel;
using MapControl;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LibPipeline
{
    public class PipelineReader
    {
        public static MultiLocation ReadExcel(string fileName, string sheetName)
        {
            var excel = new ExcelQueryFactory(fileName);
            var locations = new List<MapControl.Location>();
            var pipeline = new MultiLocation();

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
                    else
                    {
                        location[colName] = row[colName].Value;
                    }
                }

                locations.Add(location);
            }

            // pipeline.Locations = locations;

            return pipeline;
        }
    }
}