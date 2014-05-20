using System.Collections.Generic;

namespace Marv.Common.Graph
{
    public class NullEvidenceString : IEvidenceStringParser
    {
        public Dictionary<string, double> Parse(IEnumerable<State> states, string str)
        {
            return null;
        }
    }
}