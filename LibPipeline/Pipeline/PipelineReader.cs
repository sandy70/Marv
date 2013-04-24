using LibConfig;
using LinqToExcel;
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
        public PipelineReader(MongoInfo info)
        {
            this.mongodbInfo = info;
        }

        public MongoInfo mongodbInfo { get; set; }

        public Pipeline Read(string filename)
        {
            StreamReader streamReader = new StreamReader(filename);
            string collection = streamReader.ReadLine();
            streamReader.Close();

            this.mongodbInfo.Collection = "Segments" + collection;
            MongoInstance mongodbInstance = new MongoInstance(this.mongodbInfo);

            Pipeline pipeline = new Pipeline();
            pipeline.Collection = collection;

            this.mongodbInfo.Collection = "Properties" + pipeline.Collection;
            mongodbInstance = new MongoInstance(this.mongodbInfo);
            MongoCollection<PipelineProperties> propertiesCollection = mongodbInstance.GetCollection<PipelineProperties>();
            pipeline.Properties = propertiesCollection.AsQueryable<PipelineProperties>().Select(e => e).First();

            return pipeline;
        }

        public async Task<Pipeline> ReadAsync(string fileName)
        {
            return await Task.Run(async () =>
                {
                    StreamReader streamReader = new StreamReader(fileName);
                    string collection = await streamReader.ReadLineAsync();
                    streamReader.Close();

                    this.mongodbInfo.Collection = "Segments" + collection;
                    MongoInstance mongodbInstance = new MongoInstance(this.mongodbInfo);

                    Pipeline pipeline = new Pipeline();
                    pipeline.Collection = collection;

                    this.mongodbInfo.Collection = "Properties" + pipeline.Collection;
                    mongodbInstance = new MongoInstance(this.mongodbInfo);
                    MongoCollection<PipelineProperties> propertiesCollection = mongodbInstance.GetCollection<PipelineProperties>();
                    pipeline.Properties = propertiesCollection.AsQueryable<PipelineProperties>().Select(e => e).First();

                    return pipeline;
                });
        }

        public static Pipeline ReadExcel(string fileName, string sheetName)
        {
            var excel = new ExcelQueryFactory(fileName);
            var locations = new List<MapControl.Location>();
            var pipeline = new Pipeline();

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
                        location.Properties[colName] = row[colName].Value;
                    }
                }

                locations.Add(location);
            }

            pipeline.Locations = locations;

            return pipeline;
        }
    }
}