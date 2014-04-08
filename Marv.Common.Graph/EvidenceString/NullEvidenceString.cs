using System.Collections.Generic;

namespace Marv.Common.Graph
{
    public class NullEvidenceString : EvidenceStringParser
    {
        public override Dictionary<string, double> Parse(Vertex vertex, string str)
        {
            return null;
        }
    }
}