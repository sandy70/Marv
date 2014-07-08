using System.Collections;
using System.Collections.Generic;

namespace Marv.Common.Graph
{
    public class VertexEvidence : IVertexEvidence
    {
        public VertexEvidence(Dictionary<string, double> evidence)
        {
            this.Evidence = evidence;
        }

        public Dictionary<string, double> Evidence { get; set; }

        public bool Set(Vertex vertex)
        {
            vertex.Evidence = this.Evidence;
            return true;
        }
    }
}