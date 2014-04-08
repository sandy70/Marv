using System.Collections.Generic;

namespace Marv.Common.Graph
{
    public abstract class EvidenceStringParser
    {
        public abstract Dictionary<string, double> Parse(Vertex vertex, string str);
    }
}