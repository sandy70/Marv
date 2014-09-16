using System.Collections.Generic;
using Newtonsoft.Json;

namespace Marv.Graph
{
    public class YearEvidence : IKey<int>
    {
        public readonly Dictionary<string, VertexData> VertexEvidences = new Dictionary<string, VertexData>();
        public int Year { get; set; }

        [JsonIgnore]
        public int Key
        {
            get { return this.Year; }

            set { this.Year = value; }
        }
    }
}