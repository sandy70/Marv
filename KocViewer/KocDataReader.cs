using LibBn;
using LibPipeline;
using LinqToExcel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KocViewer
{
    internal class KocDataReader
    {
        public Pipeline ReadProfile(string fileName)
        {
            Console.WriteLine(fileName);
            CsvReader csvReader = new CsvReader(fileName);
            string[,] data = csvReader.Read();
            Console.WriteLine(data.GetLength(0) + " " + data.GetLength(1));

            Pipeline pipeline = new Pipeline();

            int nRows = data.GetLength(0);
            for (int r = 0; r < nRows; r++)
            {
                pipeline.Segments.Add(new PipelineSegment
                {
                    DistanceFromOrigin = (double)Utils.TryParseDouble(data[r, 1]),
                    Elevation = Utils.TryParseDouble(data[r, 4]),
                    Latitude = Utils.TryParseDouble(data[r, 2]),
                    Longitude = Utils.TryParseDouble(data[r, 3])
                });
            }

            return pipeline;
        }

        public Pipeline ReadTally(string fileName)
        {
            CsvReader csvReader = new CsvReader(fileName);
            string[,] data = csvReader.Read();

            Pipeline pipeline = new Pipeline();
            int nRows = data.GetLength(0);
            for (int r = 0; r < nRows; r++)
            {
                string longSeamOrientation = data[r, 4];
                string description = data[r, 5];

                pipeline.Segments.Add(new PipelineSegment
                {
                    Latitude = Utils.TryParseDouble(data[r, 6]),
                    Longitude = Utils.TryParseDouble(data[r, 7]),
                    Elevation = Utils.TryParseDouble(data[r, 8]),
                    JointLength = Utils.TryParseDouble(data[r, 2]),
                    WallThickness = Utils.TryParseDouble(data[r, 3]),
                    LongSeamOrientation = data[r, 4],
                    Description = data[r, 5],
                    Slope = Utils.TryParseDouble(data[r, 10])
                });
            }
            
            return pipeline;
        }

        public Pipeline ReadTallyNew(string fileName)
        {
            var pipeline = new Pipeline();

            var excel = new ExcelQueryFactory(fileName);

            var colNames = excel.GetColumnNames("PipeTally");

            var locations = new List<MapControl.Location>();

            foreach (var row in excel.Worksheet("PipeTally"))
            {
                var location = new MapControl.Location();

                if (DBNull.Value.Equals(row["Latitude"].Value) || DBNull.Value.Equals(row["Longitude"].Value))
                {
                    continue;
                }

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
                        location.Properties[colName] = row[colName].Value;
                    }
                }

                locations.Add(location);
            }

            pipeline.Locations = locations;

            return pipeline;
        }

        public void ReadVertexInputsForAllYears(BnInputStore inputManager)
        {
            var vertexInputReader = new BnVertexInputReader();

            for (int year = Config.StartYear; year <= Config.EndYear; year++)
            {
                var graphInput = inputManager.GetGraphInput((int)BnInputType.Default, year);
                vertexInputReader.Read(graphInput, "Resources/Inputs/Input" + year + ".csv");
            }
        }

        public List<BnVertexValue> ReadVertexValues(string kocDir, int year)
        {
            string fileName = Path.Combine("Resources/Outputs", "Output" + year + ".vertices");

            List<BnVertexValue> vertexValues = new List<BnVertexValue>();

            foreach (var line in File.ReadLines(fileName))
            {
                string[] parts = line.Split(new char[] { ',' });

                string key = parts[0];

                int nParts = parts.Count();

                BnVertexValue vertexValue = new BnVertexValue();
                vertexValue.Key = key;

                for (int p = 1; p < nParts; p++)
                {
                    vertexValue.States.Add(Double.Parse(parts[p]));
                }

                vertexValues.Add(vertexValue);
            }

            return vertexValues;
        }

        public Dictionary<int, List<BnVertexValue>> ReadVertexValuesForAllYears()
        {
            Dictionary<int, List<BnVertexValue>> vertexValuesByYear = new Dictionary<int, List<BnVertexValue>>();

            for (int year = Config.StartYear; year <= Config.EndYear; year++)
            {
                vertexValuesByYear.Add(year, this.ReadVertexValues(Config.KocDir, year));
            }

            return vertexValuesByYear;
        }
    }
}