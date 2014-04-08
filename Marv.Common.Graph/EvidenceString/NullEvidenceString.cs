using System.Collections.Generic;

namespace Marv.Common.Graph
{
    public class NullEvidenceString : EvidenceString
    {
        public NullEvidenceString(string aString) : base(aString)
        {
        }

        public override Dictionary<string, double> Parse(Vertex vertex)
        {
            return null;
        }
    }
}