using System.Collections.Generic;

namespace Marv.Common.Graph
{
    public class NullEvidenceString : IEvidenceStringParser
    {
        public Dictionary<string, double> Parse(Vertex vertex, string str)
        {
            return null;
        }
    }
}