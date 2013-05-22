using LibBn;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System.Collections.Generic;
using System.IO;

namespace LibPipeline
{
    public class InputStore
    {
        private ILocation defaultLocation = new Location();

        private Dictionary<ILocation, Dictionary<int, Dictionary<string, BnVertexInput>>> pipelineInput =
            new Dictionary<ILocation, Dictionary<int, Dictionary<string, BnVertexInput>>>();

        public static Dictionary<string, BnVertexInput> ReadGraphInput(string fileName)
        {
            var jsonSerializer = new JsonSerializer();
            jsonSerializer.NullValueHandling = NullValueHandling.Ignore;

            using (var streamReader = new StreamReader(fileName))
            {
                using (var jsonTextReader = new JsonTextReader(streamReader))
                {
                    return jsonSerializer.Deserialize<Dictionary<string, BnVertexInput>>(jsonTextReader);
                }
            }
        }

        public Dictionary<string, BnVertexInput> GetGraphInput(int year, ILocation location = null)
        {
            if (location == null) location = this.defaultLocation;

            var locationValue = this.GetLocationInput(location);

            if (locationValue.ContainsKey(year))
            {
                return locationValue[year];
            }
            else
            {
                return locationValue[year] = new Dictionary<string, BnVertexInput>();
            }
        }

        public Dictionary<int, Dictionary<string, BnVertexInput>> GetLocationInput(ILocation location)
        {
            if (this.pipelineInput.ContainsKey(location))
            {
                return this.pipelineInput[location];
            }
            else
            {
                return this.pipelineInput[location] = new Dictionary<int, Dictionary<string, BnVertexInput>>();
            }
        }

        public void ReadGraphInput(string fileName, int year, ILocation location = null)
        {
            if (location == null) location = this.defaultLocation;

            var locationInput = this.GetLocationInput(location);

            locationInput[year] = InputStore.ReadGraphInput(fileName);
        }

        public void WriteGraphInput(string fileName, int year, ILocation location = null)
        {
            var graphInput = this.GetGraphInput(year, location);

            var serializer = new JsonSerializer();
            serializer.NullValueHandling = NullValueHandling.Ignore;

            var extension = Path.GetExtension(fileName);

            if (Path.GetExtension(fileName).Equals(".tgi"))
            {
                using (var streamWriter = new StreamWriter(fileName))
                {
                    using (var jsonTextWriter = new JsonTextWriter(streamWriter))
                    {
                        serializer.Serialize(jsonTextWriter, graphInput);
                    }
                }
            }
            else if (Path.GetExtension(fileName).Equals(".bgi"))
            {
                using (var binaryWriter = new BinaryWriter(new FileStream(fileName, FileMode.Create)))
                {
                    using (var bsonWriter = new BsonWriter(binaryWriter))
                    {
                        serializer.Serialize(bsonWriter, graphInput);
                    }
                }
            }
        }

        public void WriteLocationInput(string fileName, ILocation location)
        {
            var locationInput = this.GetLocationInput(location);

            var serializer = new JsonSerializer();
            serializer.NullValueHandling = NullValueHandling.Ignore;

            var extension = Path.GetExtension(fileName);

            if (Path.GetExtension(fileName).Equals(".tli"))
            {
                using (var streamWriter = new StreamWriter(fileName))
                {
                    using (var jsonTextWriter = new JsonTextWriter(streamWriter))
                    {
                        serializer.Serialize(jsonTextWriter, locationInput);
                    }
                }
            }
            else if (Path.GetExtension(fileName).Equals(".bli"))
            {
                using (var binaryWriter = new BinaryWriter(new FileStream(fileName, FileMode.Create)))
                {
                    using (var bsonWriter = new BsonWriter(binaryWriter))
                    {
                        serializer.Serialize(bsonWriter, locationInput);
                    }
                }
            }
        }
    }
}