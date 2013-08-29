using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibNetwork
{
    public class SoftEvidence : IEvidence
    {
        public double[] Evidence { get; set; }

        public void Set(BnGraph graph, string vertexKey)
        {
            graph.SetVertexEvidence(vertexKey, this.Evidence);
        }
    }
}
