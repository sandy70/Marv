using System;
using System.Collections.Generic;
using System.Linq;

namespace Marv.Common.Graph
{
    public class VertexEvidence : IVertexEvidence
    {
        public Dictionary<string, double> Evidence { get; set; }

        public VertexEvidence(Dictionary<string, double> evidence)
        {
            this.Evidence = evidence;
        }

        public bool Set(Vertex vertex)
        {
            vertex.Evidence = this.Evidence;
            return true;
        }

        public override string ToString()
        {
            return this.Evidence
                .Select(kvp => String.Format("{0:F2}", kvp.Value))
                .String();
        }
    }
}