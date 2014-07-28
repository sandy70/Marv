using System.Collections.Generic;
using Marv.Common;
using Marv.Common.Graph;
using Newtonsoft.Json;

namespace Marv.Input
{
    public class YearEvidence : IKey<int>
    {
        public readonly Dictionary<string, VertexEvidence> GraphEvidence = new Dictionary<string, VertexEvidence>();
        public int Year { get; set; }

        [JsonIgnore]
        public int Key
        {
            get
            {
                return this.Year;
            }

            set
            {
                this.Year = value;
            }
        }
    }
}