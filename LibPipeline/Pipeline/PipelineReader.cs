using LibConfig;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
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
            MongoCollection<PipelineSegment> segmentCollection = mongodbInstance.GetCollection<PipelineSegment>();

            Pipeline pipeline = new Pipeline();
            pipeline.Collection = collection;

            foreach (var segment in segmentCollection.AsQueryable<PipelineSegment>().Select(e => e))
            {
                pipeline.Segments.Add(segment);
            }

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
                    MongoCollection<PipelineSegment> segmentCollection = mongodbInstance.GetCollection<PipelineSegment>();

                    Pipeline pipeline = new Pipeline();
                    pipeline.Collection = collection;

                    foreach (var segment in segmentCollection.AsQueryable<PipelineSegment>().Select(e => e))
                    {
                        pipeline.Segments.Add(segment);
                    }

                    this.mongodbInfo.Collection = "Properties" + pipeline.Collection;
                    mongodbInstance = new MongoInstance(this.mongodbInfo);
                    MongoCollection<PipelineProperties> propertiesCollection = mongodbInstance.GetCollection<PipelineProperties>();
                    pipeline.Properties = propertiesCollection.AsQueryable<PipelineProperties>().Select(e => e).First();

                    return pipeline;
                });
        }
    }
}