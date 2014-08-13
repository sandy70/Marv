using System.Collections.Generic;
using Newtonsoft.Json;

namespace Marv.Common.Graph
{
    public class YearEvidence : IKey<int>
    {
        public readonly Dictionary<string, VertexEvidence> VertexEvidences = new Dictionary<string, VertexEvidence>();
        public int Year { get; set; }

        [JsonIgnore]
        public int Key
        {
            get { return this.Year; }

            set { this.Year = value; }
        }
    }
}