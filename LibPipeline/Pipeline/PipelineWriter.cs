using LibConfig;
using MongoDB.Driver;
using System.IO;

namespace LibPipeline
{
    public class PipelineWriter
    {
        public PipelineWriter(MongoInfo info)
        {
            this.mongodbInfo = info;
        }

        public MongoInfo mongodbInfo { get; set; }

        public void Write(string filename, Pipeline pipeline)
        {
            this.mongodbInfo.Collection = "Segments" + pipeline.Collection;

            MongoInstance mongodbInstance = new MongoInstance(this.mongodbInfo);
            MongoCollection<PipelineSegment> collection = mongodbInstance.GetCollection<PipelineSegment>();

            foreach (var segment in pipeline.Segments)
            {
                collection.Save(segment);
            }

            this.mongodbInfo.Collection = "Properties" + pipeline.Collection;
            mongodbInstance = new MongoInstance(this.mongodbInfo);
            MongoCollection<PipelineProperties> propertiesCollection = mongodbInstance.GetCollection<PipelineProperties>();
            propertiesCollection.Save(pipeline.Properties);

            StreamWriter streamWriter = new StreamWriter(filename);
            streamWriter.Write(pipeline.Collection);
            streamWriter.Close();
        }
    }
}