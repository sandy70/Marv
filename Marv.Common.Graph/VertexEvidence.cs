using System.Collections.Generic;

namespace Marv.Common.Graph
{
    public class VertexEvidence
    {
        public Dictionary<string, double> Evidence { get; set; }
        public string String { get; set; }

        public VertexEvidence(Dictionary<string, double> evidence, string str)
        {
            this.Evidence = evidence;
            this.String = str;
        }

        public override string ToString()
        {
            return this.String;
        }
    }
}