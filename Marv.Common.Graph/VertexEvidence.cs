using System.Collections.Generic;
using System.Windows.Interop;

namespace Marv.Common.Graph
{
    public interface IVertexEvidence
    {
        bool Set(Vertex vertex);
    }

    public class VertexEvidence : IVertexEvidence
    {
        private readonly Dictionary<string, double> evidence;

        public VertexEvidence(Dictionary<string, double> evidence)
        {
            this.evidence = evidence;
        }

        public bool Set(Vertex vertex)
        {
            vertex.Evidence = this.evidence;
            return true;
        }
    }
}